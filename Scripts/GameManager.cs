using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static VNmanager;

public class GameManager : MonoBehaviour
{
    public string currentScene;
    public string currentStoryFile;
    public int currentLineIndex;
    public int currentLanguageIndex = Constants.DEFAULT_LANGUAGE_INDEX;
    public string currentLanguage = Constants.DEFAULT_LANGUAGE;
    public string currentBackgroundImage;
    public string currentBackgroundMusic;
    public bool isCharacter1Display;
    public bool isCharacter2Display;
    public string currentCharacter1Image;
    public string currentCharacter2Image;
    public string currentCharacter1Action;
    public string currentCharacter2Action;

    public bool hasStarted;                                                                                                                                                         //是否开始标记，通知菜单界面，开始游戏按钮是否可用
    public HashSet<string> unlockedBackgrounds = new HashSet<string>();                                     //哈希表去重保存已经解锁的背景
    public Dictionary<string, int> maxReachedLineIndices = new Dictionary<string, int>();                   //全局存储每个文件的最远索引
    public LinkedList<ExcelData> historyRecords;                                                //保存历史记录

    public enum SaveLoadMode { None, Save, Load }                                                                 //枚举 保存/加载                                                        //通知SaveLoadScene，现在是保存还是加载
    public SaveLoadMode currentSaveLoadMode { get; set; } = SaveLoadMode.None;                           //初始化 标记为 非
    public SaveData pendingData;

    public void Save(int slotIndex)//定义  保存存档  函数
    {
        string path = GenerateDataPath(slotIndex);                                                              //生成存档地址
        File.WriteAllText(path, JsonConvert.SerializeObject(pendingData, Formatting.Indented));                 //把pendingData用缩进的格式序列化，写入path中，完成存档
    }

    public void Load(int slotIndex)
    {
        string path = GenerateDataPath(slotIndex);
        pendingData = JsonConvert.DeserializeObject<SaveData>(File.ReadAllText(path));                          //读取路径中的数据，暂时存入pendingData中
    }

    public string GenerateDataPath(int index)//定义  生成保存路径方法
    {
        return Path.Combine(Application.persistentDataPath, Constants.SAVE_FILE_PATH, index + Constants.SAVE_FILE_EXTENSION);//由 持久路径，存档文件夹名， 索引 + 文件格式 组合   SAVE_FILE_PATH是在持久路径persistentDataPath下的一个文件夹，index + Constants.SAVE_FILE_EXTENSION则是该文件夹下的文件名
    }


    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);//让GameManager跨场景保持，切换场景时不会被销毁。
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
