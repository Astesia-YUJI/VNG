using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveLoadManager : MonoBehaviour
{
    public TextMeshProUGUI panelTitle;          //声明一个Text 命名为panelTitle，提示该界面是在加载还是保存
    public SaveSlot[] slots;            //声明一个数组，用于存储保存加载页面的存档
    public Button prevPageButton;//上一页按钮
    public Button nextPageButton;//下一页按钮
    public Button backButton;//返回按钮

    public GameObject confirmPanel; //确认窗口
    public TextMeshProUGUI confirmText; //确认文本 组件
    public Button confirmButton;    //确认按钮
    public Button cancelButton;     //取消按钮

    private int currentPage = Constants.DEFAULT_START_INDEX;            //初始化当前页为 0
    private readonly int slotsPerpage = Constants.SLOTS_PER_PAGE;       //设置好一个页面中有多少个栏位，即一个页面中有多少个存档数组元素
    private readonly int totalSlots = Constants.TOTAL_SLOTS;            //设置好一共有多少个存档栏位，即saveLoadButtons数组中一共有多少个元素

    private bool isLoad => GameManager.Instance.currentSaveLoadMode == GameManager.SaveLoadMode.Load;          

    public static SaveLoadManager Instance { get; private set; }                                               //语法：设置一个静态的Instance属性，get默认为公共的，set设置为private。外部可以只读这个Instance，只能从SaveLoadManager内部修改              功能：确保整个应用程序中SaveLoadManager只有一个实例，并提供一个全局可访问的位置，避免重复管理的麻烦与资源浪费
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



    // Start is called before the first frame update
    void Start()
    {
        prevPageButton.GetComponentInChildren<TextMeshProUGUI>().text = LM.GLV(Constants.PREV_PAGE);
        nextPageButton.GetComponentInChildren<TextMeshProUGUI>().text = LM.GLV(Constants.NEXT_PAGE);
        backButton.GetComponentInChildren<TextMeshProUGUI>().text = LM.GLV(Constants.BACK);

        prevPageButton.onClick.RemoveAllListeners();        //为三个按钮添加监听器
        prevPageButton.onClick.AddListener(PrevPage);
        nextPageButton.onClick.RemoveAllListeners();
        nextPageButton.onClick.AddListener(NextPage);
        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(GoBack);

        panelTitle.text = isLoad ? LM.GLV(Constants.LOAD_GAME) :  LM.GLV(Constants.SAVE_GAME);                                                                               //根据当前是否是加载，来显示当前页面标题文本
        confirmPanel.SetActive(false);
        RefreshPage();                                                                                                                                                      //刷新保存加载界面
    }


    private void RefreshPage()
    {
        for (int i = 0; i < slots.Length; i++)//遍历当前存档页面，其中一共8个存档栏位
        {
            int slotIndex = currentPage * slotsPerpage + i;                                                 //求当前存档栏位的索引
            if (slotIndex >= totalSlots)
            {
                slots[i].gameObject.SetActive(false);
                continue;
            }

            slots[i].gameObject.SetActive(true);                                                            //显示当前存档栏位
            slots[i].Init(this, slotIndex);                                                      //初始化
            slots[i].Refresh();                                                                                //刷新要显示的内容
        }
    }


    private void PrevPage()//定义  上一页  函数
    {
        if (currentPage>0)//若当前页>0
        {
            currentPage--;//页面减1
            RefreshPage();
        }
    }

    private void NextPage()//定义  下一页  函数
    {
        if ((currentPage + 1) * slotsPerpage < totalSlots)//若 当前页码乘每页存档数<总存档数
        {
            currentPage++;//翻到下一页
            RefreshPage();
        }
    }

    private void GoBack()//定义  返回  函数
    {
        var sceneName = GameManager.Instance.currentScene;
        if(sceneName == Constants.GAME_SCENE)                                                                                               //如果当前场景为游戏场景，那么跳转前就要移除当前行历史记录
        {
            GameManager.Instance.historyRecords.RemoveLast();
        }
        GameManager.Instance.pendingData = null;
        SceneManager.LoadScene(sceneName);
    }

    public void HandleEmptySlot(int slotIndex, SaveSlot slot)
    {
        SaveToSlot(slotIndex, slot);
    }

    public void HandleExistingSlot(int slotIndex, SaveSlot slot)
    {
        if (isLoad)
        {
            GameManager.Instance.Load(slotIndex);
            SceneManager.LoadScene(Constants.GAME_SCENE);
        }
        else
        {
            ShowConfirm(
                LM.GLV(Constants.CONFIRM_COVER_SAVE_FILE),                                          
                () => { SaveToSlot(slotIndex, slot); }                                              //传一个文本和一个保存动作
                );
        }
    }

    public void RequestDelete(int slotIndex, SaveSlot slot)//定义 询问是否删除 函数
    {
        ShowConfirm(
            LM.GLV(Constants.CONFIRM_DELETE_SAVE_FILE),                                             //传一个文本，传一个删除动作
            () => { DeleteSlot(slotIndex, slot); }
            );
    }


    private void ShowConfirm(string msg, System.Action onYes)//定义  展示弹窗  函数
    {
        confirmText.text = msg;
        confirmPanel.SetActive(true);

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() =>                                                     //确认按钮监听器：关闭当前的弹窗，onYes不为空，就执行这个动作
        {
            confirmPanel.SetActive(false);
            onYes?.Invoke();
        });

        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(() => confirmPanel.SetActive(false));                      //取消按钮监听器： 关闭当前弹窗，什么也不做。
    }

    private void SaveToSlot(int slotIndex, SaveSlot slot)//定义 保存存档  函数
    {
        GameManager.Instance.Save(slotIndex);                                                       //调用GameManager中的保存函数，将该存档存入索引的地址
        slot.Refresh();                                                                             //更新对应存档栏位
    }

    private void DeleteSlot(int slotIndex, SaveSlot slot)//定义  删除存档  函数
    {
        File.Delete(GameManager.Instance.GenerateDataPath(slotIndex));                              //调用  内置  Delete()  方法，把地址中的文件删除，实现删除存档功能
        slot.Refresh();                                                                             //刷新本存档栏位
    }
}
