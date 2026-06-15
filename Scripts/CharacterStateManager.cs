using System.Collections.Generic;
using UnityEngine;

public class CharacterStateManager : MonoBehaviour
{
    public CharacterData[] allCharacterDatas;           //角色数据数组，游戏开始前配置好所有角色数据
    private Dictionary<string, CharacterState> states;      //用字典来管理 角色ID， ID对应的角色状态类

    public static CharacterStateManager instance { get; private set; }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);      //让角色数据控制脚本跨场景保持
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        states = new Dictionary<string, CharacterState>();

        foreach ( var cd in allCharacterDatas)                              //枚举角色数据数组中的所有孩子
        {
            if (!states.ContainsKey(cd.characterID))                        //如果字典中没有存这个孩子
            {
                states[cd.characterID] = new CharacterState(cd.characterID);        //把这个角色数据数组的孩子存上，这样states就成为了包含  角色ID为key，角色数据类为Value  的字典
            }
        }
    }

    public int GetAffection(string characterID)//定义  查询好感度  函数
    {
        return states.TryGetValue(characterID, out var aff) ? aff.affection : 0;        //根据给到的ID,找对应的state类，该类取名为aff。然后返回该状态中的好感度。
    }

    public void ChangeAffection(string  characterID, int delta)//定义  修改好感度  函数          参数：角色ID，要修改的数值delta
    {
        if (!states.TryGetValue(characterID, out var aff))      //没找到角色ID对应的好感度，就报错
        {
            Debug.LogWarning($"无效的角色 ID: {characterID}");
        }
        aff.affection +=delta;      //角色好感度加上传过来的参数
    }

    public void ResetAffection()//定义  恢复默认好感度  函数       MenuManager的StartGame()函数中调用
    {
        foreach ( var kv in states)
        {
            kv.Value.affection = 0;
        }
    }

    public void LoadStates(Dictionary<string, int> dictionary)     //定义  加载数据  函数        传参：之前保存的 以角色ID，好感度为键值对的字典          在VNManager的Start()中调用，用于加载保存的数据
    {
        foreach (var kv in dictionary)                                     //遍历字典
        {
            if (states.ContainsKey(kv.Key))                         //如果角色状态类中包含了字典中的key
            {
                states[kv.Key].affection = kv.Value;                //把之前存的Value赋值给当前角色数据字典key
            }
        }
    }

    public  Dictionary<string, int> DumpStates()        //定义  转存函数      返回的是记录角色ID，好感度键值对的字典。在VNManager中作为保存好感度时调用，用于取得当前角色id以及其对应的好感度
    {
        var dictionary = new Dictionary<string, int>();
        foreach (var kv in states)                                  //遍历每一个states中的键值对
        {
            dictionary[kv.Key] = kv.Value.affection;                       //把当前角色数据字典中键对应的值的好感度，赋给dictionary中同名键对应的值，达到转存/复制效果
        }
        return dictionary;
    }
}
