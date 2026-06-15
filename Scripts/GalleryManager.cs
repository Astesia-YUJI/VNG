using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GalleryManager : MonoBehaviour
{
    public TextMeshProUGUI panelTitle;
    public Button[] galleryButtons;
    public Button prevPageButton;
    public Button nextPageButton;
    public Button backButton;
    public GameObject bigImagePanel;        //展示大图的面板
    public Image bigImage;                  //用来展示的大图


    private int currentPage = Constants.DEFAULT_START_INDEX;                                                //设置默认的展示页面为 第 0 页
    private readonly int slotsPerPage = Constants.GALLERY_SLOTS_PER_PAGE;                                   //设置一页保存多少张 背景图片
    private readonly int totalSlots = Constants.ALL_BACKGROUNDS.Length;                                     //设置背景图片总数量

    public static GalleryManager Instance { get; private set; }

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



    private void Start()
    {
        panelTitle.text = LM.GLV(Constants.GALLERY);
        prevPageButton.GetComponentInChildren<TextMeshProUGUI>().text = LM.GLV(Constants.PREV_PAGE);
        nextPageButton.GetComponentInChildren<TextMeshProUGUI>().text = LM.GLV(Constants.NEXT_PAGE);
        backButton.GetComponentInChildren<TextMeshProUGUI>().text = LM.GLV(Constants.BACK);

        prevPageButton.onClick.RemoveAllListeners();        //为 上一页、下一页、返回 按钮添加监听器
        prevPageButton.onClick.AddListener(PrevPage);
        nextPageButton.onClick.RemoveAllListeners();
        nextPageButton.onClick.AddListener(NextPage);
        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(GoBack);
        bigImagePanel.SetActive(false);

        Button bigImageButton = bigImagePanel.GetComponent<Button>();           //从大图面板找 大图按钮，并赋值给 临时变量按钮bigImageButton;
        if (bigImageButton != null)
        {
            bigImageButton.onClick.RemoveAllListeners();
            bigImageButton.onClick.AddListener(HideBigImage);                       //添加监听器，控制隐藏大图
        }

        UpdateUI();
    }



    private void UpdateUI()
    {
        for (int i = 0; i < slotsPerPage; i++)
        {
            int slotIndex = currentPage * slotsPerPage +i;
            if (slotIndex < totalSlots)
            {
                UpdateGalleryButtons(galleryButtons[i], slotIndex);
            }
            else
            {
                galleryButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void UpdateGalleryButtons(Button button, int index)                                             //传参：当前页面的某个按钮，该按钮在当前页面的索引
    {
        button.gameObject.SetActive(true);
        string bgName = Constants.ALL_BACKGROUNDS[index];                                                   //根据索引，取到当前索引对应的背景图片的文件名
        bool isUnlocked = GameManager.Instance.unlockedBackgrounds.Contains(bgName);                          //如果包含，则已经解锁了；如果没有包含，就未解锁

        string imagePath = Constants.THUMBNAIL_PATH + (isUnlocked ? bgName : Constants.GALLERY_PLACEHOLDER);        //如果已经解锁，图片名就为图片名；如果未解锁，图片名为占位图名。然后把图片地址赋值给临时变量
        Sprite sprite = Resources.Load<Sprite>(imagePath);                                                          //用自带函数 Resources.Load<>()从imagePath中读取图片
        if (sprite != null)
        {
            button.GetComponentInChildren<Image>().sprite = sprite;                                                   //把地址中读到的图片赋值给当前 button 的Image类型的孩子 
        }
        else
        {
            Debug.LogError(Constants.IMAGE_LOAD_FAILED + imagePath);                                        //没读到图片，那就报错，并提供目标地址
        }
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => OnButtonClick(button, index));                                     //为当前按钮添加新监听器
    }

    private void OnButtonClick(Button button, int index)
    {
        string bgName = Constants.ALL_BACKGROUNDS[index];
        bool isUnlocked = GameManager.Instance.unlockedBackgrounds.Contains(bgName);

        if (!isUnlocked)//哈希集中未记录背景图片，什么也不做
        {
            return;
        }

        string imagePath = Constants.BACKGROUND_PATH + bgName;
        Sprite sprite = Resources.Load<Sprite>(imagePath);
        if (sprite != null)
        {
            bigImage.sprite = sprite;
            bigImagePanel.SetActive(true);
        }
        else
        {
            Debug.LogError(Constants.BIG_IMAGE_LOAD_FAILED + imagePath);
        }
    }

    private void HideBigImage()
    {
        bigImagePanel.SetActive(false);
    }

    private void PrevPage()
    {
        if(currentPage > 0)
        {
            currentPage--;
            UpdateUI();
        }
    }

    private void NextPage()
    {
        if ((currentPage + 1) * slotsPerPage < totalSlots)
        {
            currentPage++;
            UpdateUI();
        }
    }

    private void GoBack()
    {
        SceneManager.LoadScene(Constants.MENU_SCENE);
    }


}
