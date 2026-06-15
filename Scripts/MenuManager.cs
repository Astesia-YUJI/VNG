using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour
{
    public Image backgroundImage;

    public Button startButton;
    public Button continueButton;
    public Button loadButton;
    public Button galleryButton;
    public Button settingsButton;
    public Button quitButton;
    public Button languageButton;                                                       //语言按钮


    private int currentLanguageIndex;

    public static MenuManager Instance { get; private set; }

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

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.currentScene = Constants.MENU_SCENE; 
        MenuButtonsAddListener();                                                                       //添加监听器
        currentLanguageIndex = GameManager.Instance.currentLanguageIndex;
        LocalizationManager.Instance.LoadLanguage(Constants.LANGUAGES[currentLanguageIndex]);
        UpdateLanguageButtonText();                                                                     //更新语言按钮文本
    }

    void MenuButtonsAddListener()
    {
        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(StartGame);
        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(ContinueGame);
        loadButton.onClick.RemoveAllListeners();
        loadButton.onClick.AddListener(LoadGame);
        galleryButton.onClick.RemoveAllListeners();
        galleryButton.onClick.AddListener(() => SceneManager.LoadScene(Constants.GALLERY_SCENE));
        settingsButton.onClick.RemoveAllListeners();
        settingsButton.onClick.AddListener(() => SceneManager.LoadScene(Constants.SETTING_SCENE));
        quitButton.onClick.AddListener(QuitGame);
        languageButton.onClick.RemoveAllListeners();
        languageButton.onClick.AddListener(UpdateLanguage);
    }

     void StartGame()//定义  开始游戏按钮  函数                                               //初始化所有数据
    {
        GameManager.Instance.hasStarted = true;
        GameManager.Instance.currentStoryFile = Constants.DEFAULT_STORY_FILE;
        GameManager.Instance.currentLineIndex = Constants.DEFAULT_START_LINE;
        GameManager.Instance.currentBackgroundImage = string.Empty;
        GameManager.Instance.currentBackgroundMusic = string.Empty;
        GameManager.Instance.isCharacter1Display = false;
        GameManager.Instance.isCharacter2Display = false;
        GameManager.Instance.historyRecords = new LinkedList<ExcelData>();
        CharacterStateManager.instance.ResetAffection();                                            
        SceneManager.LoadScene(Constants.GAME_SCENE);
    }

     void ContinueGame()//定义  继续游戏按钮  函数
    {
        if (GameManager.Instance.hasStarted)//已经开始游戏
        {
            GameManager.Instance.historyRecords.RemoveLast();                                                         //当前行在之后场景跳转到下一个场景的时候，又会显示一次，导致当前行记录了两次，这里提前把已经记录的当前行删除
            SceneManager.LoadScene(Constants.GAME_SCENE);
        }
    }

     void LoadGame()//定义  展示保存加载游戏界面  函数
    {
            GameManager.Instance.currentSaveLoadMode = GameManager.SaveLoadMode.Load;
            SceneManager.LoadScene(Constants.SAVE_LOAD_SCENE);
    }

    void QuitGame()
    {
        Application.Quit();
    }

     void UpdateLanguage()
    {
        currentLanguageIndex = (currentLanguageIndex + 1) % Constants.LANGUAGES.Length;                     //每次点击语言按钮，都会执行 + 1 并取余，让当前语言索引一直在0~2循环

        var language = Constants.LANGUAGES[currentLanguageIndex];
        LocalizationManager.Instance.LoadLanguage(language);                                         //调用LocalizationManager本地化脚本，根据传参当前语言标识，改变除GamePanel外界面的语言

        GameManager.Instance.currentLanguageIndex = currentLanguageIndex;
        GameManager.Instance.currentLanguage = language;
        UpdateLanguageButtonText();                                                                         //更新当前语言按钮文本
    }


     void UpdateLanguageButtonText()
    {
        var languageButtonTMP = languageButton.GetComponentInChildren<TextMeshProUGUI>();                   //从语言按钮的孩子中找一个TMP组件
        languageButtonTMP.text = LM.GLV(Constants.LANGUAGES[currentLanguageIndex]);                         //调用LocalizationManager的GetLocalizedValue(key)，从LocalizedText中获取key对应的本地化文本
    }

}
