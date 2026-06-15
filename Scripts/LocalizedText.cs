using TMPro;
using UnityEngine;

public class LocalizedText : MonoBehaviour
{
    public string key;                                                                                                                  //通过 key 来找对应的值

    private TextMeshProUGUI textComponent;                                                                                              //私有的 TextMeshPro 空组件，用来存储挂载到相同的 gameObject 下的TextMeshPro组件，方便后续对相同gameObject下TextMeshPro的文本进行修改

    void Start()
    {
        textComponent = GetComponent<TextMeshProUGUI>();                                 //在本脚本LocalizedText挂载的gameObject中查找 TextMeshProUGUI 组件，并把textComponent设置为该组件的代理。textComponent就能代为进行操作。

            LocalizationManager.Instance.LanguageChanged += UpdateText;                     //语法：+=，就是订阅该事件，当事件触发，那么就之行这个UpdateText()方法      通过单例，将当前脚本的 UpdateText 注册到 LocalizationManager 脚本中对应的事件上。    

            UpdateText();  
                                                                                                                //用于初始化，只执行一次
    }



    void OnDestroy()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.LanguageChanged -= UpdateText;                                                                 //注销之前注册的方法，避免内存泄露
        }
    }

    private void UpdateText()                                                                                                           //订阅的事件触发了，执行本函数
    {
        if (textComponent != null)                                                                                                      //如果当前的组件不为空，该组件内放置的是 key
        {
            textComponent.text = LocalizationManager.Instance.GetLocalizedValue(key);                                                   //把当前组件的文本，替换为对应的本地化的值
        }
    }
}
