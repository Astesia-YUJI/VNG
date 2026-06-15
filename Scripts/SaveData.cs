using System.Collections.Generic;


public class SaveData  //存档数据类，用于定义存档时需要保存到档位的内容
{
    public string savedStoryFileName;                                                                                                                                         //该存档读到的文件名
    public int savedLine;                                                                                                                                                     //当前行
    public byte[] savedScreenshotData;                                                                                                                                               //截屏数据
    public LinkedList<ExcelData> savedHistoryRecords;                                                                                                                              //历史记录
    public string savedBackgroundImage;
    public string savedBackgroundMusic;
    public string savedCharacter1Image;
    public string savedCharacter2Image;
    public string savedCharacter1Action;
    public string savedCharacter2Action;
    public bool savedIsCharacter1Display;
    public bool savedIsCharacter2Display;
    public Dictionary<string, int> savedAffection;                                                                                                                              //记录角色ID，好感度数值的键值对
}