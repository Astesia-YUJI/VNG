using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VNmanager : MonoBehaviour
{
    #region Variables
    public GameObject dialogueBox;//控制对话框
    public TextMeshProUGUI speakerName;     //声明游戏中的说话人名字,引用TextMeshProUGUI组件
    public TypewriterEffect typewriterEffect;       //引用TypewriterEffect脚本，声明一个typewriterEffect字段     依赖注入​​：通过 Unity Inspector 面板或代码动态绑定一个 TypewriterEffect对象，实现脚本间的通信。功能调用​​：通过该字段访问 TypewriterEffect的公共方法或属性（如启动逐字打印效果）。
    public ScreenShotter screenShotter;         //屏幕拍摄器组件

    public Image avatarImage;
    public Image backgroundImage;
    public Image characterImage1;
    public Image characterImage2;


    public GameObject bottomButtons;
    public Button autoButton;
    public Button skipButton;
    public Button saveButton;
    public Button loadButton;
    public Button historyButton;
    public Button settingsButton;
    public Button homeButton;
    public Button closeButton;


    public class historyData                                            //历史数据类，用于存储历史记录中不同语种的  说话人，说话内容
    {
        public string chineseName;
        public string chineseContent;
        public string japaneseName;
        public string japaneseContent;
        public string englishName;
        public string englishContent;

    }


    private string currentSpeakingContent;//保存当前对话内容
    private List<ExcelData> storyData;      //声明一个名为storyData的、以ExcelReader中的ExcelData结构体为参考的结构体列表对象
    private int currentLine;//当前行数
    private string currentStoryFileName;//当前文本表格名字
    private float currentTypingSpeed = Constants.DEFAULT_TYPING_SPEED;//当前行默认打字速度，用于传参给打字机中的 StartTyping 函数，改变打字速度。


    private bool isAutoPlay = false;    //自动播放标记
    private bool isSkip = false;        //跳过标记
    private int maxReachedLineIndex = 0;  //初始化最远行索引

    public static VNmanager Instance { get; private set; }

    #endregion


    #region Lifecycle

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
        }
    }



    void Start()
    {
        GameManager.Instance.hasStarted = true;
        GameManager.Instance.currentScene = Constants.GAME_SCENE;
        if (GameManager.Instance.pendingData != null)                               //如果GameManager的pendingData不为空，说明之前保存有角色数据
        {
            var savedData = GameManager.Instance.pendingData;
            GameManager.Instance.hasStarted = true;

            GameManager.Instance.currentStoryFile = savedData.savedStoryFileName;
            savedData.savedLine--;
            GameManager.Instance.currentLineIndex = savedData.savedLine;

            if(savedData.savedHistoryRecords.Count > 1)
            {
            savedData.savedHistoryRecords.RemoveLast();
            }
            GameManager.Instance.historyRecords = savedData.savedHistoryRecords;

            GameManager.Instance.currentBackgroundImage = savedData.savedBackgroundImage;
            GameManager.Instance.currentBackgroundMusic = savedData.savedBackgroundMusic;

            GameManager.Instance.currentCharacter1Image = savedData.savedCharacter1Image;
            GameManager.Instance.currentCharacter2Image = savedData.savedCharacter2Image;
            GameManager.Instance.currentCharacter1Action = savedData.savedCharacter1Action;
            GameManager.Instance.currentCharacter2Action = savedData.savedCharacter2Action;
            GameManager.Instance.isCharacter1Display = savedData.savedIsCharacter1Display;
            GameManager.Instance.isCharacter2Display = savedData.savedIsCharacter2Display;

            CharacterStateManager.instance.LoadStates(savedData.savedAffection);
        }
        currentLine = GameManager.Instance.currentLineIndex;                                                                                                                                      //初始化存档路径
        bottomButtonsAddListener();                                                         //调用   为按钮添加监听器 函数
        InitializeImage();
        LoadStory(GameManager.Instance.currentStoryFile);
        DisplayNextLine();
    }



    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))                                                                                 //功能：如果点击了鼠标左键|或者按了空格
        {
            if (!dialogueBox.activeSelf)                                                                                                                                 //功能：且现在对话框DialogueBox不显示，说明之前点击了关闭UI，需要打开UI
            {
                OpenUI();//展示UI
            }
            else if (!IsHittingBottomButtons() && !ChoiceManager.Instance.choicePanel.activeSelf)                                                          //功能：当前对话框DialogueBox显示，且没有点击底部按钮,且选项界面没有显示
            {
                DisplayNextLine();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))                                                                                                                    //功能：如果按下Esc键
        {
            if (!dialogueBox.activeSelf)                                                                                                                                //功能：对话框显示，就关闭UI；否则打开UI。此功能方便玩家截屏或者保存游戏内画面
            {
                CloseUI();
            }
            else
            {
                OpenUI();
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            Debug.Log("按下Control键");
            CtrlSkip();
        }
    }

    #endregion


    #region Initialization


    void bottomButtonsAddListener()//为  按钮  添加监听器
    {
        autoButton.onClick.RemoveAllListeners();
        autoButton.onClick.AddListener(OnAutoButtonClick);
        skipButton.onClick.RemoveAllListeners();
        skipButton.onClick.AddListener(OnSkipButtonClick);
        saveButton.onClick.RemoveAllListeners();
        saveButton.onClick.AddListener(OnSaveButtonClick);
        loadButton.onClick.RemoveAllListeners();
        loadButton.onClick.AddListener(OnLoadButtonClick);
        historyButton.onClick.RemoveAllListeners();
        historyButton.onClick.AddListener(OnHistoryButtonClick);
        settingsButton.onClick.RemoveAllListeners();
        settingsButton.onClick.AddListener(OnSettingButtonClick);


        homeButton.onClick.RemoveAllListeners();
        homeButton.onClick.AddListener(OnHomeButtonClick);
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(OnCloseButtonClick);
    }

    void LoadStory(string fileName)
    {
        LoadStoryFromFile(fileName);
        RecoverLastBackgroundAndCharacter();
    }

    void InitializeImage()
    {
        backgroundImage.gameObject.SetActive(false);
        avatarImage.gameObject.SetActive(false);
        characterImage1.gameObject.SetActive(false);
        characterImage2.gameObject.SetActive(false);
    }


    void LoadStoryFromFile(string fileName)//定义   读取文案 函数
    {
        currentStoryFileName = fileName;                                                                                                            //功能： 替换当前文件名
        string filePath = Path.Combine(Application.streamingAssetsPath, Constants.STORY_PATH, fileName + Constants.STORY_FILE_EXTENSION);

        storyData = ExcelReader.ReadExcel(filePath);                                                                                                   //功能：通过ExcelReader读取器利用path地址，读取文案表格并赋值给storyData

        if (storyData == null|| storyData.Count == 0)                                                                                              //功能：storyData结构体列表为空或者结构体列表中元素总数为0，则输出错误         语法：.Count为内置方法，为元素总数量
        {
            Debug.LogError(Constants.NO_DATA_FOUND);                                                                                                //功能：调用Constants脚本中的报错函数
        }

        GameManager.Instance.currentStoryFile = currentStoryFileName;

        if (GameManager.Instance.maxReachedLineIndices.ContainsKey(currentStoryFileName))                                                                          //若字典中存了该文件名的文件信息           语法：ContainsKey()为内置方法，查询()内部的文件名是否有记录
        {
            maxReachedLineIndex = GameManager.Instance.maxReachedLineIndices[currentStoryFileName];                                                                //把存过的最远到达过的行数赋值给当前的最远行
        }
        else
        {
            maxReachedLineIndex = 0;                                                                                                                //如果没有存该文件名，就把最远到达行设置为0
            GameManager.Instance.maxReachedLineIndices[currentStoryFileName] = maxReachedLineIndex;                                                 //如果没有存该文件，把该文件名的最远到达行数0存入MaxReachedLineIndices字典
        }
    }

    #endregion


    #region Display

    void DisplayNextLine()//定义  输出下一行 函数
    {
        if (currentLine > maxReachedLineIndex)                                                                                                      //若当前行大于最远到达行
        {
            maxReachedLineIndex = currentLine;                                                                                                      //更新最远到达行
            GameManager.Instance.maxReachedLineIndices[currentStoryFileName] = maxReachedLineIndex;                                                                        //更新字典中当前文案的最远到达行
        }

        if (currentLine >= storyData.Count - 1)                                                                                                     //如果当前行为最后一行。一张表的最后一行为第storyData[storyData.Count-1]行
        {
            if (isAutoPlay)                                                                                                                         //如果还在自动播放状态
            {
                isAutoPlay = false;                                                                                                                 //停止自动播放
                UpdateButtonImage(Constants.AUTO_OFF, autoButton);                                                                                  //更新自动播放图片
            }

            if (storyData[currentLine].speakerName == Constants.END_OF_STORY)                                                                       //如果故事结束
            {
                GameManager.Instance.hasStarted = false;                                                                                            //关闭游戏界面，打开菜单界面
                SceneManager.LoadScene(Constants.MENU_SCENE);
                Debug.Log("故事结束，返回菜单");
            }

            if (storyData[currentLine].speakerName == Constants.CHOICE)                                                                             //如果当前内容为  选项（这里用表的尽头跳到另一张表来显示选项后的游戏内容，以后可能有更好的解决方法）
            {
                ShowChoices();
            }

            if (storyData[currentLine].speakerName == Constants.GOTO)                                                                               //功能：如果当前内容为  goto, 跳转
            {
                LoadStory(storyData[currentLine].speakingContent);                                                   //功能：重新加载文案文件，从说话内容中的文件名所代表的文件，从初始行加载文件
                currentLine = Constants.DEFAULT_START_LINE;
                DisplayNextLine();
            }
            return;
        }
        if (typewriterEffect.IsTyping())                                                                           //语法：若 TypewriterEffect 脚本中 IsTyping() 返回的 isTyping 为真    功能：当前打字机正在打印，本次点击鼠标左键，则会调用CompleteLine()方法，提前结束播放，全部显示当前结构体的说话内容
        {
            typewriterEffect.CompleteLine();                                                                            //调用 TypewriterEffect 脚本中的 CompleteLine() 函数，停止当前协程并直接完成当前说话内容
        }
        else
        {
            DisplayThisLine();                                                                                                                      //    未在打印的话，那么就输出本行结构体的内容
        }

    }

    void DisplayThisLine()
    {
        GameManager.Instance.currentLineIndex = currentLine;                                                                                                            //功能：更新当前行索引
        var data = storyData[currentLine];                                                                                                                              //功能：将storyData列表中第currentLine行的元素赋值给临时变量data

        speakerName.text = LM.GetSpeakerName(data);
        currentSpeakingContent = LM.GetSpeakingContent(data);

        typewriterEffect.StartTyping(currentSpeakingContent, currentTypingSpeed);                                                                      // 打字机脚本入口              功能：把说话内容,打字机速度赋值给打字机参数

        GameManager.Instance.historyRecords.AddLast(data);                                                                                             //记录历史文本

        if (NotNullNorEmpty(data.avatarImageFileName))                                                                                                                  //功能：判断头像图片是否为空
        {
            UpdateAvatarImage(data.avatarImageFileName);                                                                                                                //功能：更新头像图片
        }
        else
        {
            avatarImage.gameObject.SetActive(false);                                                                                                                    //功能：头像图片为空，则让avatarImage组件 不可见
        }
        if (NotNullNorEmpty(data.vocalAudioFileName))
        {
            PlayVocalAudio(data.vocalAudioFileName);                                                                        //调用 播放人声音频函数
        }
        if (NotNullNorEmpty(data.backgroundImageFileName))
        {
            GameManager.Instance.currentBackgroundImage = data.backgroundImageFileName;                                     //记录当前背景图片
            UpdateBackgroundImage(data.backgroundImageFileName);                                                            //调用 更新背景图片函数
        }
        if (NotNullNorEmpty(data.backgroundMusicFileName))
        {
            GameManager.Instance.currentBackgroundMusic = data.backgroundMusicFileName;                                     //记录当前背景音乐
            UpdateBackgroundMusic(data.backgroundMusicFileName);                                                            //调用 更新背景音乐函数
        }
        if (NotNullNorEmpty(data.character1Action))
        {
            if (data.character1Action == Constants.DISAPPEAR)
            {
                GameManager.Instance.isCharacter1Display = false;
            }
            else
            {
                GameManager.Instance.isCharacter1Display= true;
                GameManager.Instance.currentCharacter1Image = data.character1ImageFileName;
                GameManager.Instance.currentCharacter1Action = data.character1Action;
            }
            UpdateCharacterImage(data.character1Action, data.character1ImageFileName, characterImage1);                      //调用 更新角色立绘函数            传参：角色行动、角色图片名2，角色图片1组件变量
        }
        if (NotNullNorEmpty(data.character2Action))
        {
            if (data.character2Action == Constants.DISAPPEAR)
            {
                GameManager.Instance.isCharacter2Display = false;
            }
            else
            {
                GameManager.Instance.isCharacter2Display= true;
                GameManager.Instance.currentCharacter2Image = data.character2ImageFileName;
                GameManager.Instance.currentCharacter2Action = data.character2Action;
            }
            UpdateCharacterImage(data.character2Action, data.character2ImageFileName, characterImage2);                      //调用 更新角色立绘函数            传参：角色行动、角色图片名2、角色图片2组件变量
        }
        Debug.Log(storyData.Count-1);
        currentLine++;                                                                                                                                        //功能：跳到下一行(下一个结构体）
    }




    void RecoverLastBackgroundAndCharacter()//定义  复原游戏场景内容  函数
    {
        var data = storyData[currentLine];//读取当前行的数据
        if (NotNullNorEmpty(GameManager.Instance.currentBackgroundImage))//加载存档的背景图片
        {
            UpdateBackgroundImage(GameManager.Instance.currentBackgroundImage);
        }
        if (NotNullNorEmpty(GameManager.Instance.currentBackgroundMusic))//加载存档的背景音乐
        {
            UpdateBackgroundMusic(GameManager.Instance.currentBackgroundMusic);
        }
        if (GameManager.Instance.isCharacter1Display || NotNullNorEmpty(GameManager.Instance.currentCharacter1Action))//如果角色1正在展示
        {
            if (GameManager.Instance.currentCharacter1Action.StartsWith(Constants.APPEAR_AT))
            {
                UpdateCharacterImage(GameManager.Instance.currentCharacter1Action, GameManager.Instance.currentCharacter1Image, characterImage1);
            }
            else if (GameManager.Instance.currentCharacter1Action == Constants.DISAPPEAR)//角色1的 lastAction 为 "0" 或者 disappear ，不执行任何操作
            {}
            else if (GameManager.Instance.currentCharacter1Action == Constants.MOVE_TO)                                                                           //角色1的 lastAction 为 moveto
            {
                var position = Position(GameManager.Instance.currentCharacter1Action);                                //获取 lastAction 的坐标
                string NewAction = Constants.APPEAR_AT + "(" + position[0] + "," + position[1] + ")";              //用新的临时变量来存角色的动作
                UpdateCharacterImage(NewAction, data.character1ImageFileName, characterImage1);//用一个新的临时变量来作为UpdateCharacterImage的参数1,更新角色1的图片
            }
        }
        if (GameManager.Instance.isCharacter2Display || NotNullNorEmpty(GameManager.Instance.currentCharacter2Action))
        {
            if (GameManager.Instance.currentCharacter2Action.StartsWith(Constants.APPEAR_AT))
            {
                UpdateCharacterImage(GameManager.Instance.currentCharacter2Action, GameManager.Instance.currentCharacter2Image, characterImage2);
            }
            else if (GameManager.Instance.currentCharacter2Action == Constants.DISAPPEAR)
            {}
            else if (GameManager.Instance.currentCharacter1Action == Constants.MOVE_TO)
            {
                var position = Position(GameManager.Instance.currentCharacter2Action);
                string NewAction = Constants.APPEAR_AT + "(" + position[0] + "," + position[1] + ")";
                UpdateCharacterImage(NewAction, data.character2ImageFileName, characterImage2);
            }
        }
    }

    bool NotNullNorEmpty(string str)
    {
        return !string.IsNullOrEmpty(str);
    }

    #endregion


    #region Choice


    void ShowChoices()
    {
        var choices = ParseChoices();       //返回列表，列表中包含了4个选项的文本、目标文件名、好感度变化和条件等信息，每个选项都是一个ChoiceOption对象，存储在choices列表中
        foreach (var opt in choices)
        {
            Debug.Log($"选项文本: {opt.text}, 目标文件: {opt.nextStoryFileName}, 好感度变化: {string.Join(", ", opt.changes.Select(c => $"{c.characterID}:{c.delta}"))}, 条件: {string.Join(", ", opt.conditions.Select(c => $"{c.characterID}:{c.minValue}-{c.maxValue}"))}");
        }
        ChoiceManager.Instance.ShowChoices(choices, HandleChoice);

    }

    void HandleChoice(string selectedChoice)
    {
        currentLine = Constants.DEFAULT_START_LINE;
        LoadStory(selectedChoice);
        DisplayNextLine();
    }

    List<ChoiceOption> ParseChoices()//定义  解析选项  函数             返回一个 完整的包含一整组4个选项数组的 列表
    {
        var data = storyData[currentLine];
        var choiceTexts = LM.GetSpeakingContent(data)
                        .Split(Constants.HuanHang)
                        .Select(s => s.Trim())
                        .ToArray();         //把读取到的内容以换行符为分割，分成一个个选项文本，存到字符串数组choiceTexts中

        var targetFiles = data.avatarImageFileName
                .Split(Constants.HuanHang)
                .Select(s => s.Trim())
                .ToArray();

        var changes = data.vocalAudioFileName
                .Split(Constants.HuanHang)
                .Select(s => s.Trim())
                .ToArray();

        var conditions = data.backgroundImageFileName
                .Split(Constants.HuanHang)
                .Select(s => s.Trim())
                .ToArray();

        var choices = new List<ChoiceOption>();     //新建一个ChoiceOption类型的列表，用来存储解析后的选项数据
        int choiceCount = choiceTexts.Length;       //选项数量以文本数组的长度为准，理论上四个数组的长度应该一致，如果不一致，以文本数组为准，缺失的部分用默认值代替
        for (int i = 0; i < choiceCount; i++)
        {
            var opt = new ChoiceOption
            {
                text = choiceTexts.ElementAtOrDefault(i) ?? "",         //如果有文本，就用文本；如果没有，就用空字符串
                nextStoryFileName = targetFiles.ElementAtOrDefault(i) ?? "",
                changes = ParseChanges(changes.ElementAtOrDefault(i)),          //如果有变化，就解析变化；如果没有，就用空列表
                conditions = ParseConditions(conditions.ElementAtOrDefault(i))      //如果有条件，就解析条件；如果没有，就用空列表
            };
            choices.Add(opt);       //把解析后的选项添加到列表中         每一个解析后的选项都是一个ChoiceOption类对象，包含了文本、目标文件名、好感度变化和条件
        }
        return choices;
    }

    List<AffectionChange> ParseChanges(string raw)
    {
        var list = new List<AffectionChange>();
        if (string.IsNullOrEmpty(raw)) return list;

        foreach (var part in raw.Split(';', StringSplitOptions.RemoveEmptyEntries))
        {
            var kv = part.Split(':', StringSplitOptions.RemoveEmptyEntries);
            if (kv.Length != 2) continue;               //如果分割后不是两部分(角色ID，变化值)，说明格式不对，跳过这个变化
            if (int.TryParse(kv[1], out int delta))     //kv[0]是角色ID,kv[1]是变化值，尝试把变化值转换成整数，如果成功，就创建一个AffectionChange对象，存储角色ID和变化值，并添加到列表中；如果失败，说明变化值不是一个有效的整数，跳过这个变化
            {
                list.Add(new AffectionChange
                {
                    characterID = kv[0].Trim(),
                    delta = delta
                });
            }
        }
        return list;
    }

    List<AffectionCondition> ParseConditions(string raw)
    {
        var list = new List<AffectionCondition>();
        if (string.IsNullOrEmpty(raw)) return list;

        foreach (var part in raw.Split(";", StringSplitOptions.RemoveEmptyEntries))
        {
            var kv = part.Split(":", StringSplitOptions.RemoveEmptyEntries);
            if (kv.Length != 2) continue;

            var id = kv[0].Trim();
            var nums = kv[1].Split(',', StringSplitOptions.RemoveEmptyEntries);
            if(nums.Length != 2) continue;

            if (int.TryParse(nums[0], out int min) &&
                int.TryParse(nums[1], out int max))
            {
                list.Add(new AffectionCondition
                {
                    characterID = kv[0].Trim(),
                    minValue = min,
                    maxValue = max
                });
            }
        }
        return list;
    }

    #endregion



    #region Audios

    void PlayVocalAudio(string audioFileName)//定义  人声音频播放函数
    {
        AudioManager.Instance.PlayVoice(audioFileName);//调用  播放音频函数
    }


    void UpdateBackgroundMusic(string musicFileName)//定义  更新bgm函数
    {
        AudioManager.Instance.PlayBackground(musicFileName);
    }
    #endregion


    #region Images

    void UpdateAvatarImage(string imageFileName)//定义  头像图片更换函数
    {
        string imagePath = Constants.AVATAR_PATH + imageFileName;
        UpdateImage(imagePath, avatarImage);                                                            //调用  更新图片函数
    }


    void UpdateBackgroundImage(string imageFileName)//定义  更新背景图片函数
    {
        string imagePath = Constants.BACKGROUND_PATH + imageFileName;
        UpdateImage(imagePath, backgroundImage);                                                         //调用  更新图片函数

        if (!GameManager.Instance.unlockedBackgrounds.Contains(imageFileName))                                                                                             //功能；如果该背景图片文件名未在 哈希集 中，那么将它录入哈希集中。但因为用哈希集存背景图片，自动去重，所以这个if判断可以不加，直接添加进哈希集即可
        {
            GameManager.Instance.unlockedBackgrounds.Add(imageFileName);
        }
    }


    void UpdateButtonImage(string imageFileName, Button button)                                         //定义  更新按钮图片函数
    {
        string imagePath = Constants.BUTTON_PATH + imageFileName;
        UpdateImage(imagePath, button.image);
    }



    string[] Position(string action)//定义   获取坐标函数         语法：返回字符串数组，存放了action中传过来的坐标
    {
        ReadOnlySpan<char> span = action.AsSpan();                                                                                                    //功能：将action中的字符串传给span
        int openIndex = span.IndexOf('(');                                                                                                            //功能：找span中左括号 “（” 和右括号 “）” 的位置
        int closeIndex = span.IndexOf(")");
        if (openIndex != -1 && closeIndex != -1 && openIndex < closeIndex)                                                                            //功能：判断两括号间内容不为空
        {
            ReadOnlySpan<char> coordinatesSpan = span.Slice(openIndex + 1, closeIndex - openIndex - 1);                                               //功能：切片获取两括号中间的内容，赋值给coordinatesSpan字符串数组
            string[] coordinates = coordinatesSpan.ToString().Split(',');                                                                             //功能：以逗号为分割，获取coordinatesSpan中的元素，即为xy坐标，传给新建字符串数组coordinates
            return coordinates;
        }
        else
        {
            Debug.LogError(Constants.COORDINATE_MISSING +  action);
            return null;
        }
    }


    void UpdateCharacterImage(string action, string imageFileName, Image characterImage)//定义  更新角色立绘函数
    {
        characterImage.DOKill();
        if (action.StartsWith(Constants.APPEAR_AT))                                                                                             //功能：判断action是否为appearAt(x,y)                 语法：检测 action 字符串是否以 Constants.characterActionAppearAt 中的字符为开头
        {
            string imagePath = Constants.CHARACTER_PATH + imageFileName;                                                                        //功能：传完整地址
            UpdateImage(imagePath, characterImage);                                                   //调用  更新图片函数                  传参：完成立绘地址 和 Image组件类型的 characterImage变量
            var position = Position(action);                                                                                                //功能：调用  获取坐标函数
            var newPosition = new Vector2(float.Parse(position[0]), float.Parse(position[1]));                                              //利用 Vector2 内置方法,设置立绘的坐标
            characterImage.rectTransform.anchoredPosition = newPosition;                                                                    //功能：将新的坐标传给组件位置
            characterImage.DOFade(1,  Constants.DURATION_TIME).From(0);            //当前为加载界面时,角色图片直接出现； 否则按时间展示出现动画        立绘透明度从0开始，经过Constants.DURATION_TIME给定的时间后，变成1。  实现缓慢消失/出现效果。
            }

        else if (action == Constants.DISAPPEAR)                                                                                                  //功能：判断action是否为disappear
        {
            characterImage.DOFade(0, Constants.DURATION_TIME).OnComplete(() => characterImage.gameObject.SetActive(false));                       //功能：无论目前透明度为多少，经过设定时间Constants.DURATION_TIME后，都变为0           语法：OnComplete()方法，动画之后执行的操作
        }

        else if (action.StartsWith(Constants.MOVE_TO))                                                                                           //功能：判断action是否为moveTo(x,y)
        {
            string[] position = Position(action);
            characterImage.rectTransform.DOAnchorPosX(float.Parse(position[0]), Constants.DURATION_TIME);
            characterImage.rectTransform.DOAnchorPosY(float.Parse(position[1]), Constants.DURATION_TIME);
        }
        else
        {
            Debug.LogError(Constants.ACTION_NOT_SET +  action);
        }

    }


    void UpdateImage(string imagePath, Image image)//定义  更新图像函数
    {
        Sprite sprite = Resources.Load<Sprite>(imagePath);                                                                                          //语法：Sprite为精灵图类型，用于显示2D图像。
        if (sprite != null)                                                                                                                         //功能：判断sprite是否为空
        {
            image.sprite = sprite;                                                                                                                  //语法：将读取到的sprite图片赋值给 image 组件
            image.gameObject.SetActive(true);                                                                                                       //功能：显示图片
        }
        else
        {
            Debug.LogError(Constants.IMAGE_LOAD_FAILED + imagePath);
        }
    }


    #endregion


    #region Buttons

    #region Buttom
    bool IsHittingBottomButtons()                                                                                                                                   //功能：判断否点击底部按钮，返回true 或 false
    {
        return RectTransformUtility.RectangleContainsScreenPoint(                                                                                                   //语法：Unity UI 工具方法，检测点是否在矩形内
            bottomButtons.GetComponent<RectTransform>(),                                                                                                            //功能：获取底部按钮的矩形变换组件
            Input.mousePosition,                                                                                                                                    //当前鼠标位置（屏幕坐标）
            Camera.main                                                                                                                                                    //使用主摄像机
            );
    }
    #endregion

    #region Auto
    void OnAutoButtonClick()//定义  自动播放按钮函数
    {
        isAutoPlay = !isAutoPlay;
        UpdateButtonImage((isAutoPlay ? Constants.AUTO_ON : Constants.AUTO_OFF), autoButton);                       //调用：更新按钮图片函数   传参  文件名+控件名
        if (isAutoPlay)                                                                                                                                              //功能：  如果现在正在自动播放
        {
            StartCoroutine(StartAutoPlay());                                                                        //调用： 开始自动播放 协程
        }
    }


    private IEnumerator StartAutoPlay()//定义  开始自动播放协程
    {
        while (isAutoPlay)                                                                                                                                                //功能：如果现在正在自动播放
        {
            if (!typewriterEffect.IsTyping())                                                                                                                            //功能：且现在没有在打字
            {
                DisplayNextLine();                                                                                                                                       //功能：输出下一行
            }
            yield return new WaitForSeconds(Constants.DEFAULT_AUTO_WAITING_SECONDS);
        }
    }

    //协程的作用是在规定的时间间隔内，循环完成某个任务。这里的协程作用是完成一轮打字后，间隔规定时间之后，再开始下一轮打字。输出下一行时，由于当前处在自动播放且未在打字状态，打字机输出下一行结构体的内容。
    #endregion

    #region Skip
    void OnSkipButtonClick()//定义  点击快速跳过按钮  函数
    {
        if (!isSkip && CanSkip())                                                                                                                                          //如果 当前未在跳过，且能够跳过
        {
            StartSkip();                                                                                        //调用  开始跳过 函数
        }
        else if (isSkip)                                                                                                                                                //如果 当前正在跳过
        {
            StopCoroutine(SkipToMaxReachedLine());                                                                                                                      //停止跳过至最远行协程
            EndSkip();                                                                                                                                                  //结束跳过
        }
    }


    bool CanSkip()
    {
        return currentLine <= maxReachedLineIndex;                                                                                                                       //若当前行小于等于最远到达行索引，则返回true，否则返回false
    }


    void StartSkip()//定义  开始跳过 函数
    {
        isSkip = true;
        UpdateButtonImage(Constants.SKIP_ON, skipButton);                                                                                                               //更新skipButton按钮图片
        currentTypingSpeed = Constants.SKIP_MODE_TYPING_SPEED;                                                                                               //更改打字机打字速度
        StartCoroutine(SkipToMaxReachedLine());                                                                                                                         //开始  跳过至最远到达行  协程
    }




    private IEnumerator SkipToMaxReachedLine()//定义  跳过至最远到达行  协程
    {
        while (isSkip)
        {
            if (CanSkip())
            {
                DisplayNextLine();
            }
            else
            {
                EndSkip();
            }
            yield return new WaitForSeconds(Constants.DEFAULT_SKIP_WAITING_SECONDS);                                                                                   //在规定时间后输出下一行
        }
    }



    void EndSkip()
    {
        isSkip = false;                                                                                                                                                 //将标记否定
        currentTypingSpeed = Constants.DEFAULT_TYPING_SPEED;                                                                                                  //将打字机脚本的协程速度改为原本速度
        UpdateButtonImage(Constants.SKIP_OFF, skipButton);                                                                                                              //更新按钮图片
    }


    void CtrlSkip()//定义  Control快速跳过  函数
    {
        currentTypingSpeed = Constants.SKIP_MODE_TYPING_SPEED;
        StartCoroutine(SkipWhilePressingCtrl());
    }

    private IEnumerator SkipWhilePressingCtrl()//定义  Control键快速跳过携程
    {
        while (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))                                                                         //功能：只要按住Control键，就会一直循环执行以下操作
        {
            DisplayNextLine();    //输出下一行
            yield return new WaitForSeconds(Constants.DEFAULT_SKIP_WAITING_SECONDS);       //每行输出间隔为快速跳过的时间间隔
        }
    }
    #endregion

    #region Home
    void OnHomeButtonClick()//定义  返回主页按钮
    {
        SceneManager.LoadScene(Constants.MENU_SCENE);
    }
    #endregion

    #region Close
    void OnCloseButtonClick()
    {
        CloseUI();//调用  关闭UI界面
    }

    void OpenUI()
    {
        dialogueBox.SetActive(true);//显示对话框
        bottomButtons.SetActive(true);//显示底部按钮
    }

    void CloseUI()
    {
        dialogueBox.SetActive(false);//隐藏对话框
        bottomButtons.SetActive(false);//隐藏底部按钮
    }
    #endregion

    #region Save
    void OnSaveButtonClick()
    {
        SaveData();
        GameManager.Instance.currentSaveLoadMode = GameManager.SaveLoadMode.Save;
        SceneManager.LoadScene(Constants.SAVE_LOAD_SCENE);
    }

    void SaveData()
    {
        CloseUI();
        Texture2D screenshot = screenShotter.CaptureScreenshot();       //用截图脚本截取当前无UI界面
        OpenUI();

        GameManager.Instance.pendingData = new SaveData     //为GameManager中的pendingData赋值，pendingData为用于存储游戏存档数据的临时变量，存档时就存pendingData里的数据
        {
            savedStoryFileName = currentStoryFileName,
            savedLine = currentLine,
            savedScreenshotData = screenshot.EncodeToPNG(),
            savedHistoryRecords = GameManager.Instance.historyRecords,
            savedBackgroundImage = GameManager.Instance.currentBackgroundImage,
            savedBackgroundMusic = GameManager.Instance.currentBackgroundMusic,
            savedCharacter1Image = GameManager.Instance.currentCharacter1Image,
            savedCharacter2Image = GameManager.Instance.currentCharacter2Image,
            savedCharacter1Action = GameManager.Instance.currentCharacter1Action,
            savedCharacter2Action = GameManager.Instance.currentCharacter2Action,
            savedIsCharacter1Display = GameManager.Instance.isCharacter1Display,
            savedIsCharacter2Display = GameManager.Instance.isCharacter2Display,
            savedAffection = CharacterStateManager.instance.DumpStates()                        //已保存的好感度，就是角色状态控制器中转存函数返回的值
        };
    }
    #endregion

    #region Load
    void OnLoadButtonClick()
    {
        GameManager.Instance.currentSaveLoadMode = GameManager.SaveLoadMode.Load;
        SceneManager.LoadScene(Constants.SAVE_LOAD_SCENE);
    }
    #endregion

    #region History
    void OnHistoryButtonClick()
    {
        SceneManager.LoadScene(Constants.HISTORY_SCENE);
    }
    #endregion

    #region Setting
    void OnSettingButtonClick()
    {
        SceneManager.LoadScene(Constants.SETTING_SCENE);
    }
    #endregion

    #endregion
}





