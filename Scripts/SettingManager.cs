using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    public Toggle fullscreenToggle;//Toggle是 Unity UI 系统中的一种组件类型，属于 UnityEngine.UI.Toggle类。可以设置为 true 或 false ，控制开关。
    public Text toggleLabel;//用来控制 Toggle 改变开关之后，所显示的不同内容
    public TMP_Dropdown resolutionDropdown;//TMP_Dropdown用来控制分辨率的下拉菜单组件。


    private Resolution[] availableResolutions;                                                                                                  //分辨率数组，存玩家当前电脑可用的所有分辨率
    private Resolution defaultResolution;                                                                                                     //默认分辨率，可以在点击恢复默认和刚开始游戏时采用
    public Button defaultButton;//回复默认按钮
    public Button closeButton;//关闭设置界面按钮

    public Slider masterVolumeSlider;                                                                                                           //用3组滑块，来控制3组音频的音量
    public Slider musicVolumeSlider;
    public Slider voiceVolumeSlider;
    public AudioMixer audioMixer;


    public static SettingManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance == null)
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
        AddListener();
        Initialization();
    }

    void AddListener()
    {
        fullscreenToggle.onValueChanged.RemoveAllListeners();                                                                   //为 画面显示模式、设置分辨率、关闭设置界面、恢复默认设置  按钮  添加监听器
        fullscreenToggle.onValueChanged.AddListener(SetDisplayMode);
        resolutionDropdown.onValueChanged.RemoveAllListeners();
        resolutionDropdown.onValueChanged.AddListener(SetResolution);//TMP_Dropdown.onValueChanged 自带参数，参数是用户点击下拉列表时的索引
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(CloseSetting);
        defaultButton.onClick.RemoveAllListeners();
        defaultButton.onClick.AddListener(RestSetting);
        masterVolumeSlider.onValueChanged.RemoveAllListeners();
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        musicVolumeSlider.onValueChanged.RemoveAllListeners();
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        voiceVolumeSlider.onValueChanged.RemoveAllListeners();
        voiceVolumeSlider.onValueChanged.AddListener(SetVoiceVolume);//把当前滑块值自动传参，调用SetVoiceVulume()方法
    }

    void Initialization()
    {
        InitializeDisplayMode();            //初始化显示模式
        InitializeResolutions();            //初始化分辨率
        InitializeButtons();                //初始化按钮
        InitializeVolume();                 //初始化音频
    }

    void InitializeDisplayMode()
    {
        fullscreenToggle.isOn = Screen.fullScreenMode == FullScreenMode.FullScreenWindow;                                       //功能：判断当前屏幕是否为全屏，返回 true 或 false，把返回值赋值给fullscreenToggle，控制全屏开关显示
        UpdateToggleLabel(fullscreenToggle.isOn);                                                                               //功能：把 true 或 false 传参给  更新Toggle内容函数
    }

    void InitializeButtons()
    {
        closeButton.GetComponentInChildren<TextMeshProUGUI>().text = LM.GLV(Constants.CLOSE);
        defaultButton.GetComponentInChildren<TextMeshProUGUI>().text = LM.GLV(Constants.RESET);
    }

    void InitializeVolume()
    {
        //PlayerPrefs是unity用来存储玩家偏好的本地工具，可以存储string,int,float类型的变量，以  键值对  的形式保存。
        //GetFloat这里的功能是：从PlayerPrefs中找Constants常量里的 _VOLUME （键），找到了就用其对应的 （值）来代替PlayerPrefs的值，没找到就用后面的DEFAULT_VOLUME来代替。
        //初始时这三个常量 （键） 中都没有存对应的Float类型的数据，所以这里会初始化为默认音量0.8f
        //PlayerPrefs最终返回的是一个值，一个键对应的值，或者设置好的默认值
        masterVolumeSlider.value = PlayerPrefs.GetFloat(Constants.MASTER_VOLUME, Constants.DEFAULT_VOLUME);
        musicVolumeSlider.value = PlayerPrefs.GetFloat(Constants.MUSIC_VOLUME, Constants.DEFAULT_VOLUME);
        voiceVolumeSlider.value = PlayerPrefs.GetFloat(Constants.VOICE_VOLUME, Constants.DEFAULT_VOLUME);

        SetMasterVolume(masterVolumeSlider.value);
        SetMusicVolume(musicVolumeSlider.value);
        SetVoiceVolume(voiceVolumeSlider.value);

    }
    void InitializeResolutions()//定义  初始化分辨率  函数
    {
        availableResolutions = Screen.resolutions;          //获取所有可用分辨率，并赋值给  分辨率数组
        resolutionDropdown.ClearOptions();                  //清理掉下拉菜单中所有的内容


        var resolutionMap = new Dictionary<string, Resolution>();              //Map，映射，用于去重。原因是Screen.resolutions取得的分辨率可能会携带屏幕的刷新率，不同刷新率相同分辨率，也是不同的，会重复出现相同的分辨率。需要一个字典Dictionary来记录已经有的分辨率
        int currentResolutionIndex = 0;                                         //当前分辨率下拉列表的索引


        foreach (var res in availableResolutions)                        //枚举所有分辨率数组中的分辨率，对它们进行如下操作去重
        {
            const float aspectRatio = 16f/9f;                                   //设置一个  16/9  的宽高比
            const float epsilon = 0.01f;                                        //设置一个可容忍误差

            if (Mathf.Abs((float)res.width/res.height - aspectRatio) > epsilon)                   //(当前屏幕的宽高比 - 16/9)  >  可容忍误差
                continue;               //结束本组分辨率的枚举，开始下一组


            string option = res.width + "x" + res.height;         //临时变量  下拉列表的选项  设置为  宽 + “x” + 高
            if (!resolutionMap.ContainsKey(option))                             //之前的 resolutionMap 字典中没有存这个分辨率（宽高）
            {
                resolutionMap[option] = res;
                resolutionDropdown.options.Add(new TMP_Dropdown.OptionData(option));                                                                        //功能：为字典添加一个新的选项数据option
                if (res.width == Screen.currentResolution.width && res.height == Screen.currentResolution.height)                                           //功能：当前屏幕的宽高是否为枚举到的宽高
                {
                    currentResolutionIndex = resolutionDropdown.options.Count - 1;                                                                          //功能：把当前分辨率的索引记录下来，这个索引就是默认分辨率的索引。   当前分辨率的索引 = 分辨率下拉列表的选项的数目 -1
                    defaultResolution = res;                                                                                                                //功能：把当前分辨率设置为默认分辨率
                }
            }
        }

        resolutionDropdown.value = currentResolutionIndex;//让下拉列表的值，显示为当前分辨率的索引的值，即显示默认分辨率
        resolutionDropdown.RefreshShownValue();//更新下拉菜单的显示
    }

    void SetDisplayMode(bool isFullscreen)
    {
        Screen.fullScreenMode = isFullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;                                                  //现在窗口是全屏吗？ Y 就全屏 N  就窗口
        UpdateToggleLabel(isFullscreen);//更新一下Toggle显示的内容
    }

    void UpdateToggleLabel(bool isFullscreen)//更新标签文本
    {
        toggleLabel.text = isFullscreen ? LM.GLV(Constants.FULLSCREEN) : LM.GLV(Constants.WINDOWED);                                                                                       //根据是否全屏显示  全屏/窗口
    }

    void SetResolution(int index)//定义  设置分辨率  函数       传参：由 TMP_DropDown.onValueChanged 自带 的用户点击任意下拉列表时的 索引（从0开始计算）
    {
        string[] dimensions = resolutionDropdown.options[index].text.Split('x');//获取用户点击时下拉列表即分辨率数组时，该分辨率的索引。将该索引下的内容以 “x” 为分割，获得宽高，然后再把宽高和目前是否全屏作为参数传给系统函数 Screen.SetResolution(),实现用户点击下拉列表之后更换分辨率
        int width = int.Parse(dimensions[0].Trim());
        int height = int.Parse(dimensions[1].Trim());
        Screen.SetResolution(width, height, Screen.fullScreenMode);
    }

    void SetMasterVolume(float value)
    {
        audioMixer.SetFloat(Constants.MASTER_VOLUME, SliderValueToFenbei(value));                      //SliderValueToBel通过滚动条的值，来转换分贝。把转换后的分贝值，赋值给常量值
    }

    void SetMusicVolume(float value)
    {
        audioMixer.SetFloat(Constants.MUSIC_VOLUME, SliderValueToFenbei(value));
    }

    void SetVoiceVolume(float value)
    {
        audioMixer.SetFloat(Constants.VOICE_VOLUME, SliderValueToFenbei(value));
    }

    private float SliderValueToFenbei(float value)                                                          //value的值为0~1
    {
        return value > 0.0001f ? Mathf.Log10(value)*20f : -80f;                                             //返回value代入分贝计算公式的值
    }
    void CloseSetting()
    {
        var sceneName = GameManager.Instance.currentScene;
        if(sceneName == Constants.GAME_SCENE)
        {
            GameManager.Instance.historyRecords.RemoveLast();
        }
        PlayerPrefs.SetFloat(Constants.MASTER_VOLUME, masterVolumeSlider.value);                        //把Constants.MASTER_VOLUME的值设置为maserVolumeSlider.value的值，更新键值对
        PlayerPrefs.SetFloat(Constants.MUSIC_VOLUME, musicVolumeSlider.value);
        PlayerPrefs.SetFloat(Constants.VOICE_VOLUME, voiceVolumeSlider.value);
        PlayerPrefs.Save();                                                                             //保存PlayerPrefs的更新，写入磁盘

        SceneManager.LoadScene(sceneName);
    }

    void RestSetting()
    {
        resolutionDropdown.value = resolutionDropdown.options.FindIndex(
            option => option.text == $"{defaultResolution.width}x{defaultResolution.height}");//在resolutionDropdown.options下拉列表中找符合（默认宽x默认高）的选项，赋值给下拉列表目前的变量，即恢复默认的分辨率
        fullscreenToggle.isOn = true;                                                           //恢复全屏

        masterVolumeSlider.value = Constants.DEFAULT_VOLUME;
        musicVolumeSlider.value = Constants.DEFAULT_VOLUME;
        voiceVolumeSlider.value = Constants.DEFAULT_VOLUME;

        SetMasterVolume(masterVolumeSlider.value);
        SetMusicVolume(musicVolumeSlider.value);
        SetVoiceVolume(voiceVolumeSlider.value);
    }


}
