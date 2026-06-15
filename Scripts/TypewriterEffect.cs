using System.Collections;
using TMPro;
using UnityEngine;

public class TypewriterEffect : MonoBehaviour
{
    public TextMeshProUGUI textDisplay;                                             //关联textDisplay组件

    private float typingSpeed;                //定义typingSpeed变量，设计打字速度
    private Coroutine typingCoroutine;                      //声明一个协程
    private bool isTyping;                                   //声明一个布尔变量，判断当前是否正在打字，仅在打字机脚本内部使用


    public void StartTyping(string text,float speed)        //协程开关
    {
        typingSpeed = speed;
        if (typingCoroutine != null)                                //如果有一个正在执行的协程
        {
            StopCoroutine(typingCoroutine);                         //停止  协程
        }
        typingCoroutine = StartCoroutine(TypeLine(text));           //启用  协程
    }

    private IEnumerator TypeLine(string text)                   //协程：  返回一个IEnumerator枚举器，返回值为多个 yield return        作用：按规定时间间隔，逐行打印文本，实现打字机效果
    {
        isTyping = true;
        textDisplay.text = text;                                            //将文案赋值给textDisplay组件中的文本
        textDisplay.maxVisibleCharacters = 0;                               //语法：maxVisibleCharacters 内置变量，为最大当前可见字符数。当其值为当前行长度时，会输出所有剩余文本。相反，就会隐藏剩余文本。  这是逐字输出打字机效果的关键功能。

        for (int i = 0;i<=text.Length;i++)
        {
            textDisplay.maxVisibleCharacters = i;
            yield return new WaitForSeconds(typingSpeed);                                //yield return 暂停协程，直到等待WaitForSeconds内置方法运行结束后，再继续协程，进入下一次循环，输出下一个字符。    括号内的参数设置为需等待的时间间隔       
        }
        isTyping = false;                                                           //当前行内容输出完毕，将isTyping赋假，结束协程
    }


    public void CompleteLine()                                                                          //完成当前行
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);                                                                 //StopCoroutine内置方法，用于停止协程
        }

        textDisplay.maxVisibleCharacters = textDisplay.text.Length;                                     //最大文本数等于文本长度，即输出该行剩余文本
        isTyping = false;                                                                                //返回打字机未在打字
    }

    public bool IsTyping() => isTyping;          //返回isTyping的值
}
