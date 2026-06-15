using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HistoryManager : MonoBehaviour
{
    public Transform historyContent;//Unity中HistoryScrollView 的 Content 对象, Transform是一个类类型，在Unity中代表一个组件
    public GameObject historyItemPrefab;//用于显示历史记录的文本预制体,用于上面的Content对象
    public GameObject historyScrollView;//ScrollView 对象
    public Button closeButton;//关闭按钮


    private LinkedList<ExcelData> historyRecords; //保存历史记录

    public static HistoryManager Instance {get; private set; }
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


    void Start()
    {
        closeButton.GetComponentInChildren<TextMeshProUGUI>().text = LM.GLV(Constants.CLOSE);                                                   //将关闭历史记录按钮的文本获取本地化， key是常量中的内容
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(CloseHistory);                                                                                               //功能：添加  关闭历史记录按钮  监听器
        ShowHistory(GameManager.Instance.historyRecords);
    }



    public void ShowHistory(LinkedList<ExcelData> records)
    {

        //清空所有历史记录
        foreach (Transform child in historyContent)             //枚举组件内的每一个孩子，就是把unity组件中每一个 Item 都清掉
        {
            Destroy(child.gameObject);                          //直接销毁,清除之前留的历史记录，方便写新的历史记录
        }

        historyRecords = records;                               //把传过来的历史记录 赋值 给HistoryManager 的历史记录
        LinkedListNode<ExcelData> currentNode = historyRecords.Last;//语法：LinkListNode 为链表节点类型，存一个链表的节点                   //功能：当前节点赋值为历史记录链表的最后一个元素    调整历史记录的读取顺序，从后往前读取

        while (currentNode != null)                                                                                                                     //功能：只要当前节点不为空
        {
            var name = LM.GetSpeakerName(currentNode.Value);
            var content = LM.GetSpeakingContent(currentNode.Value);                                                                                             //记录默认的说话人与说话内容
            AddHistoryItem(name + LM.GLV(Constants.COLON) + content);//添加历史记录                                                                       功能：将当前节点的说话人和说话内容添加到预制体中
            currentNode = currentNode.Previous;                                                                                                         //功能：当前节点往前移动                                                                                            
        }

        historyContent.GetComponent<RectTransform>().localPosition = Vector3.zero;                                                                      //功能：将滚动视图位置重置为顶部
        historyScrollView.SetActive(true);
    }


    public void CloseHistory()
    {
        GameManager.Instance.historyRecords.RemoveLast();                                                                                               //移除最后一条历史记录，因为当前行的历史记录在场景跳转的时候，会显示2次，也就是说，历史记录会记录两次当前行，需要移除
        SceneManager.LoadScene(GameManager.Instance.currentScene);                                                                                      //返回当前场景
    }


    void AddHistoryItem(string text)
    {
        GameObject historyItem = Instantiate(historyItemPrefab, historyContent); //语法：historyItemPrefab 是模板，historyContent 是父容器              //功能：以historyItemPrefab为模板，在historyContent下面创建一个 临时 预制体 变量，用来存传过来的每条历史记录
        historyItem.GetComponentInChildren<TextMeshProUGUI>().text = text;                                                                              //功能：让预制体的 text 赋值为传过来的  这条历史记录的文案
        historyItem.transform.SetAsFirstSibling();                                                                                                      //功能：将历史记录放在 顶部
    }
}
