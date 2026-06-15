using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class LM
{
    public static string GLV(string key) => LocalizationManager.Instance.GetLocalizedValue(key);

    public static string GetSpeakerName(ExcelData data)
    {
        string currentSpeakerName = string.Empty;
        switch (GameManager.Instance.currentLanguageIndex)
        {
            case 0:
                currentSpeakerName = data.speakerName;
                break;
            case 1:
                currentSpeakerName = data.japaneseName;
                break;
            case 2:
                currentSpeakerName = data.englishName;
                break;
        }
        return currentSpeakerName;
    }

    public static string GetSpeakingContent(ExcelData data)
    {
        string currentSpeakingContent = string.Empty;
        switch (GameManager.Instance.currentLanguageIndex)
        {
            case 0:
                currentSpeakingContent = data.speakingContent;
                break;
            case 1:
                currentSpeakingContent = data.japaneseContent;
                break;
            case 2:
                currentSpeakingContent = data.englishContent;
                break;
        }
        return currentSpeakingContent;
    }

}

public class LocalizationManager : MonoBehaviour
{
    public Dictionary<string, string> localizedText;                                            //字典用来保存当前语言的键值对                <key, value> 一个 key 对应一个 value。key 和 value 对应的内容存储在 Assets/languages/**.json文件夹下
    public string currentLanguage = Constants.DEFAULT_LANGUAGE;                                 //将当前语言赋值为默认语言

    public delegate void OnLanguageChanged();                                                                       //语法： delegate 委托， event 事件，常一起使用。 event 用于发布和订阅。未订阅event，当要求发出时，不会进行对应的操作；订阅了事件event,则会进行对应的操作
    public event OnLanguageChanged LanguageChanged;                                                                 //功能：这里如果已经订阅了 event ，那么会在语言发生切换的时候，即玩家点击了MenuPanel的语言按钮，那么event 就会通知其他已订阅本事件的组件，来更新新的文本

    public static LocalizationManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);                                                                         //功能：不销毁LocalizationManager，在任意场景中都能访问本脚本实例
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private void Start()
    {
        LoadLanguage(Constants.DEFAULT_LANGUAGE);
    }


    public void LoadLanguage(string language)
    {
        currentLanguage = language;                                                                                                                                     //功能：更新一下当前语言
        string filePath = Path.Combine(Application.streamingAssetsPath, Constants.LANGUAGE_PATH, language + Constants.JSON_FILE_EXTENSION);                             //功能：获取本地文件夹中.json文件的地址
        if (File.Exists(filePath))                                                                                                                                      //功能：如果该地址不为空
        {
            string dataAsJson = File.ReadAllText(filePath);                                                                                                             //功能：只读.json文件的所有内容，并赋值给临时变量
            localizedText = JsonConvert.DeserializeObject<Dictionary<string, string>>(dataAsJson);                                                                      //功能：把.json文件反序列化，转化成字典

            //通过判断添加的组件内容是否为空，来自动触发事件
            LanguageChanged?.Invoke();                                                                                      //语法：判断 LanguageChanged 组件是否为空？（为空即没有函数订阅）   不为空，则调用 LanguageChanged 的事件，已订阅该事件的组件将对此进行对应操作
        }
        else
        {
            Debug.LogError(Constants.LOCALIZATION_LOAD_FAILED + filePath);
        }
    }


    public string GetLocalizedValue(string key)//定义   获取本地化数据   函数
    {
        if(localizedText != null &&  localizedText.ContainsKey(key))                                                                                                    //功能：本地化字典不为空，且字典中包含key
        {
            return localizedText[key];                                                                                                                                  //功能：返回字典中 key 对应的 值
        }
        return key;                                                                                                                                                     //功能：不包含，则返回key
    }
}
