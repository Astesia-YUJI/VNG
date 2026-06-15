using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceManager : MonoBehaviour
{
    public GameObject choicePanel;
    public Button choiceButtonPrefab;                                                                                       //选项按钮
    public Transform choiceButtonContainer;                                                                                 //选项按钮容器            需要将实例化的  choiceButtonPrefab  放入  choiceButtonContainer  中

    public static ChoiceManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        choicePanel.SetActive(false);
    }

    public void ShowChoices(List<ChoiceOption> options, Action<string> onChoiceSelected)
    {
        var available = new List<ChoiceOption>();       //available列表是用来存储所有满足好感度条件的选项的列表，最终会根据这个列表来生成选项按钮并展示给玩家。
        foreach (var opt in options)
        {
            bool allConditionsMet = true;

            foreach (var c in opt.conditions)
            {
                int aff = CharacterStateManager.instance.GetAffection(c.characterID);
                if (aff < c.minValue || aff > c.maxValue)       //如果当前好感度不满足条件，就跳过这个选项
                {
                    allConditionsMet = false;
                    break;                                      //只要有一个条件不满足，就跳过这个选项
                }
            }

            if (allConditionsMet)       //如果所有条件都满足，就把这个选项加入可用选项列表
            {
                available.Add(opt);
            }
        }

        if (available.Count == 0)
        {
            return;
        }

        if (available.Count == 1)
        {
            foreach(var ch in available[0].changes)
            {
                CharacterStateManager.instance.ChangeAffection(ch.characterID, ch.delta);
            }
            onChoiceSelected?.Invoke(available[0].nextStoryFileName);
            return;
        }

        foreach (Transform child in choiceButtonContainer)      //清除之前的选项按钮
        {
            Destroy(child.gameObject);
        }
        foreach (var opt in available)      //为每个可用选项创建一个按钮
        {
            var btn = Instantiate(choiceButtonPrefab, choiceButtonContainer);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = opt.text;
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener( () =>
            {
                foreach (var ch in opt.changes)
                {
                    CharacterStateManager.instance.ChangeAffection(ch.characterID, ch.delta);           
                }
                onChoiceSelected.Invoke(opt.nextStoryFileName);             
                choicePanel.SetActive(false);
            }
                );
        }
        choicePanel.SetActive(true);                                                                                                        //展示选项界面
    }
}
