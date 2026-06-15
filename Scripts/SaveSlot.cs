using Newtonsoft.Json;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    public Button slotButton;
    public Button deleteButton;
    public RawImage thumbnail;
    public TextMeshProUGUI topText;
    public TextMeshProUGUI bottomText;

    private int slotIndex;                                              //标记当前槽位的索引
    private SaveLoadManager owner;                                      //用于记录保存加载Manager
    private bool hasFile;                                               //标记存档栏位是否有存档

    public void Init(SaveLoadManager mgr, int index)//定义  初始化存档栏位  函数
    {
        owner = mgr;
        slotIndex = index;

        slotButton.onClick.RemoveAllListeners();
        slotButton.onClick.AddListener(OnSlotClick);                                        //点击存档栏位时的监听器
        deleteButton.onClick.RemoveAllListeners();
        deleteButton.onClick.AddListener(OnDeleteClick);                                    //点击删除按钮时的监听器
    }

    public void Refresh()//刷新  存档栏位  函数
    {
        string path = GameManager.Instance.GenerateDataPath(slotIndex);                                 //根据存档栏位的索引生成一个本地化地址
        hasFile = File.Exists(path);                                                                    //判断路径中是否保存了存档文件
        bool isLoad = GameManager.Instance.currentSaveLoadMode == GameManager.SaveLoadMode.Load;        //判断当前是否是加载

        deleteButton.gameObject.SetActive(hasFile);                                                     //删除按钮要根据当前存档栏位是否有存档文件来决定是否显示

        slotButton.interactable = hasFile || !isLoad;                                                   //栏位有存档，该栏位都可以互动；如果现在是保存界面，也可以互动。如果既没有存档，又在加载界面，那就不能互动。

        thumbnail.texture = null;                                                                       //缩略图初始为空

        if (!hasFile)                                                                                   //没有存档时的操作
        {
            topText.text = "";
            bottomText.text = (slotIndex + 1) + " " + LM.GLV(Constants.EMPTY_SLOT);
            return;
        }

        string json = File.ReadAllText(path);
        var data = JsonConvert.DeserializeObject<SaveData>(json);                           //根据文件的内容反序列化之后，传给data

        if (data.savedScreenshotData != null)                                                           //判断截屏数据不为空
        {
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(data.savedScreenshotData);
            thumbnail.texture = tex;
        }

        if (data.savedHistoryRecords?.Last != null)                                                     //存档数据不为空
        {
            topText.text = LM.GetSpeakingContent(data.savedHistoryRecords.Last.Value);                  //更新保存的最后一句话
        }
        bottomText.text = File.GetLastWriteTime(path).ToString("G");                                    //更新存档时间
    }

    private void OnSlotClick()
    {
        if (hasFile)
            owner.HandleExistingSlot(slotIndex, this);                                                  //让SaveLoadManager 处理有存档的情况
        else
            owner.HandleEmptySlot(slotIndex, this);                                                     //让SaveLoadManager 处理没有存档的情况
    }

    private void OnDeleteClick()
    {
        owner.RequestDelete(slotIndex, this);                                                           //让SaveLoadManager 处理删除存档的情况
    }
}
