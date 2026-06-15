using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public AudioMixer audioMixer;                                                                               //对应 MainMixer 混音器
    public AudioMixerGroup musicGroup;                                                                          //对应 Music 通道
    public AudioMixerGroup voiceGroup;                                                                          //对应 Voice 通道

    private AudioSource musicSource;                                                                            //背景音乐和人声 对应的 音源
    private AudioSource voiceSource;

    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);                                                                      //场景切换时，AudioManager不清除

            musicSource = gameObject.AddComponent<AudioSource>();                                               //为当前游戏对象添加一个音频源组件
            musicSource.outputAudioMixerGroup = musicGroup;                                                     //控制musicSource的输出混音通道
            musicSource.loop = true;

            voiceSource = gameObject.AddComponent<AudioSource>();
            voiceSource.outputAudioMixerGroup = voiceGroup;
            voiceSource.loop = false;

            LoadVolumeSettings();                                                                               //加载音量设置
            SceneManager.sceneLoaded += OnSceneLoaded;                                                          //为场景切换添加一个事件，会在场景切换之后再执行
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private void LoadVolumeSettings()
    {
        float m = PlayerPrefs.GetFloat(Constants.MASTER_VOLUME, Constants.DEFAULT_VOLUME);                      //返回Constants.MASTER_VOLUME（键）对应的 值 ，如果没有存这个值，那么返回Constants.DEFAULT_VOLUME。
        float mu = PlayerPrefs.GetFloat(Constants.MUSIC_VOLUME, Constants.DEFAULT_VOLUME);
        float v = PlayerPrefs.GetFloat(Constants.VOICE_VOLUME, Constants.DEFAULT_VOLUME);

        audioMixer.SetFloat(Constants.MASTER_VOLUME, SliderToFenbei(m));                                        //把上面键对应的值带入SliderToFenbei()中，用公式计算出这个值对应的分贝，再把这个分贝作为值存到暴露出来的参数键中
        audioMixer.SetFloat(Constants.MUSIC_VOLUME, SliderToFenbei(mu));
        audioMixer.SetFloat(Constants.VOICE_VOLUME, SliderToFenbei(v));
    }

    private float SliderToFenbei(float value)
    {
        return value > 0.0001f ? Mathf.Log10(value) : -80f;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == Constants.MENU_SCENE)                                                                  //如果当前是菜单场景，那就播放菜单场景的背景音乐，传参音乐名常量
        {
            PlayBackground(Constants.MENU_MUSIC_FILE_NAME);
        }
        else if(scene.name == Constants.GAME_SCENE)                                                             //如果当前是游戏场景
        {
            string lastMusic = GameManager.Instance.currentBackgroundMusic;                                     //GameManager中存的currentBackgroundMusic不为空
            if(!string.IsNullOrEmpty(lastMusic))                                                
                PlayBackground(lastMusic);                                                                      //播放GameManager中的currentBackgroundMusic
        }
    }

    public void PlayBackground(string musicFileName)
    {
        AudioClip clip = Resources.Load<AudioClip>(Constants.MUSIC_PATH + musicFileName);
        if(clip == null)
        {
            Debug.LogError(Constants.MUSIC_PATH + musicFileName);
            return;
        }
        if(clip == musicSource.clip)                                                                            //如果音乐和当前音乐相同，那么什么也不做
        {
            return;
        }
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlayVoice(string voiceFileName)
    {
        AudioClip clip = Resources.Load<AudioClip>(Constants.VOCAL_PATH + voiceFileName);
        if (clip == null)
        {
            Debug.LogError(Constants.VOCAL_PATH + voiceFileName);
            return;
        }
        voiceSource.clip = clip;
        voiceSource.Play();
    }

}
