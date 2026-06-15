using System.Collections.Generic;


public class AffectionChange        //好感度类
{
    public string characterID;  //给定一个id，对应一个变化的好感度值
    public int delta;
}

public class AffectionCondition     //好感度条件类
{
    public string characterID;
    public int minValue;
    public int maxValue;
}

public class ChoiceOption       //选项类       每一个选项，都对应一个ChoiceOption类
{
    public string text;     //文本
    public string nextStoryFileName;        //要跳转到的文件名
    public List<AffectionChange> changes;       //好感度变化
    public List<AffectionCondition> conditions;     //好感度条件
}

public struct ExcelData   //表格数据结构体，包含说话人与说话内容。     struct是值类型​​，存储在栈内存（小型数据更高效），适合轻量级数据结构。class是引用类型​​，存储在堆内存（适合复杂对象）。
{
    public string speakerName;
    public string speakingContent;
    public string avatarImageFileName;
    public string vocalAudioFileName;
    public string backgroundImageFileName;
    public string backgroundMusicFileName;
    public string character1Action;
    public string character1ImageFileName;
    public string character2Action;
    public string character2ImageFileName;
    public string englishName;
    public string englishContent;
    public string japaneseName;
    public string japaneseContent;
}
