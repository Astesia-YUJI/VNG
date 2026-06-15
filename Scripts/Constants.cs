public class Constants
{
    public static string MENU_SCENE = "MenuScene";
    public static string GAME_SCENE = "GameScene";
    public static string SETTING_SCENE = "SettingScene";
    public static string SAVE_LOAD_SCENE = "SaveLoadScene";
    public static string GALLERY_SCENE = "GalleryScene";
    public static string HISTORY_SCENE = "HistoryScene";



    public static string STORY_PATH = "story";                    //读取文案地址
    public static string DEFAULT_STORY_FILE = "1-1";                                 //文件名
    public static string STORY_FILE_EXTENSION = ".xlsx";                                //文件类型
    public static int DEFAULT_START_LINE = 1;                                           //开始读取行数


    public static string AVATAR_PATH = "image/avatar/";                                 //读取图像地址
    public static string BACKGROUND_PATH = "image/background/";
    public static string BUTTON_PATH = "image/button/";
    public static string CHARACTER_PATH = "image/character/";
    public static string THUMBNAIL_PATH = "image/thumbnail/";                            //画廊界面缩略图存址
    public static string IMAGE_LOAD_FAILED = "Filed to load image: ";
    public static string BIG_IMAGE_LOAD_FAILED = "Failed to load big image: ";           //画廊大图地址没找到报错


    public static float DEFAULT_TYPING_SPEED = 0.15f;                                  //打字机默认打字时，字与字之间的输出的间隔
    public static float SKIP_MODE_TYPING_SPEED = 0.01f;                                //快速跳过时，字与字之间的输出间隔


    public static string AUTO_ON = "autoplayon";
    public static string AUTO_OFF = "autoplayoff";
    public static float DEFAULT_AUTO_WAITING_SECONDS = 1f;                          //自动播放每行之间的时间间隔


    public static string SKIP_ON = "skipon";
    public static string SKIP_OFF = "skipoff";
    public static float DEFAULT_SKIP_WAITING_SECONDS = 0.25f;                           //默认快速跳过每行时间间隔

    public static string VOCAL_PATH = "audio/vocal/";                                    //读取音频地址
    public static string MUSIC_PATH = "audio/music/";
    public static string AUDIO_LOAD_FAILED = "Failed to load audio: ";
    public static string MENU_MUSIC_FILE_NAME = "1";


    public static string NO_DATA_FOUND = "No data found";
    public static string END_OF_STORY = "End of story";
    public static string CHOICE = "choice";


    public static string APPEAR_AT = "appearAt";                                        //设置立绘动作词条
    public static string DISAPPEAR = "disappear";
    public static string MOVE_TO = "moveTo";
    public static int DURATION_TIME = 1;                                                    //持续时间
    public static string COORDINATE_MISSING = "Corrdinate missing";                         //坐标缺失报错
    public static string ACTION_NOT_SET = "Action not set : ";                              //没有设置该动作

    public static int DEFAULT_START_INDEX = 0;//保存加载界面的初始页面为第0页
    public static int SLOTS_PER_PAGE = 8;//与界面UI绘制时一致，每个页面设置有8个存档栏位
    public static int TOTAL_SLOTS = 40;//设计总存档栏位数目为40
    public static string COLON = ": ";
    public static string SAVE_GAME = "save_game";
    public static string LOAD_GAME = "load_game";
    public static string EMPTY_SLOT = "empty_slot";


    public static string CAMERA_NOT_FOUND = "Main camera not found";
    public static string SAVE_FILE_PATH = "saves";
    public static string SAVE_FILE_EXTENSION = ".json";




    public static int MAX_LENGTH = 50;
    public static int GALLERY_SLOTS_PER_PAGE = 9;
    public static string GALLERY = "gallery";
    public static string GALLERY_PLACEHOLDER = "gallery_placeholder";
    public static readonly string[] ALL_BACKGROUNDS = { "1", "2", "3", "4", "c1","c2" };    //所有会展示在画廊系统内的图片
    public static string UNLOCED = "unlocked";



    public static string GOTO = "goto";


    public static string DEFAULT_LANGUAGE = "zh";
    public static string LANGUAGE_PATH = "languages";
    public static string JSON_FILE_EXTENSION = ".json";
    public static string LOCALIZATION_LOAD_FAILED = "Failed to load localization file";
    public static int DEFAULT_LANGUAGE_INDEX = 0;

    public static string[] LANGUAGES = {"zh","ja","en"};


    public static string PREV_PAGE = "previous_page";
    public static string NEXT_PAGE = "next_page";
    public static string BACK = "back";
    public static string CLOSE = "close";


    public static string FULLSCREEN = "fullscreen";
    public static string WINDOWED = "windowed";
    public static string RESET = "reset";

    public static string MASTER_VOLUME = "MasterVolume";
    public static string MUSIC_VOLUME = "MusicVolume";
    public static string VOICE_VOLUME = "VoiceVolume";

    public static float DEFAULT_VOLUME = 0.8f;

    public static char HuanHang = '\n';

    public static string CONFIRM_DELETE_SAVE_FILE = "confirm_delete_save_file";
    public static string CONFIRM_COVER_SAVE_FILE = "confirm_cover_save_file";
}