using UnityEngine;

public class ScreenShotter : MonoBehaviour
{

    public Texture2D CaptureScreenshot()  //语法：Texture2D 是unity中用于存储纹理数据的类。这里创建一个类，返回：纹理（图片数据）
    {
        int width = Screen.width;
        int height = Screen.height;//获取当前屏幕的宽高，Screen为当前屏幕

        RenderTexture rt = RenderTexture.GetTemporary(width, height, 24);  //语法： RenderTexture为渲染纹理类型， rt 为渲染纹理类型的变量，把它用于存储摄像机的输出。这里给它尺寸设置为屏幕的宽高，深度缓冲区位数24位。

        Camera mainCamera = Camera.main;
        if (mainCamera ==null)//判断场景中有主摄像机
        {
            Debug.LogError(Constants.CAMERA_NOT_FOUND);//没有就报错，不会陷入卡死
            return null;
        }


        //默认情况下，摄像机渲染输出到屏幕上，我们将输出存到 临时纹理变量 rt 中，展开处理。
        mainCamera.targetTexture = rt;//功能：告诉主摄像机的输出存在 rt 里。
        RenderTexture.active = rt; //将 rt 设置为活动项
        mainCamera.Render();//手动渲染当前场景，把当前场景的图片渲染并存到 rt 中。


        Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);//创建一个存储纹理数据的类，名为scrrenshot,把它用于存储屏幕的内容
        screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        // ReadPixels方法，从帧缓冲区读取像素，把它存到当前对象纹理中。
        //关键步骤：由存摄像机输出的渲染纹理 RenderTexture 传入存储纹理数据的 Texture2D 中。即由屏幕当前渲染的画面截屏后存储到存储纹理数据的数组中。
        //前一个0， 0 是从左下角的屏幕坐标0，0处开始读取像素。 后一个0， 0是将读取到的像素存入 存储纹理数据数组的开始位置
        screenshot.Apply();//调用 Apply 应用更改，将数据写入内存。


        mainCamera.targetTexture = null;//将主摄像机的输出调回默认
        RenderTexture.active = null;//清除当前屏幕的活动项纹理
        RenderTexture.ReleaseTemporary(rt);//释放临时屏幕的渲染纹理，避免内存泄露


        Texture2D resizedScreenshot = ResizeTexture(screenshot, width/6, height/6);//创建一个纹理数据变量 resizedScreenshot, 用于存储修改后的纹理。  这里调用 ResizeTexture 方法，用返回值为它赋值。  传参：截图的纹理数据，目标纹理宽度，目标纹理高度

        Destroy(screenshot);//销毁截图，释放内存

        return resizedScreenshot;
    }



    private Texture2D ResizeTexture(Texture2D original, int newWidth, int newHeight)
    {
        RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight, 24);//创建一个临时渲染纹理变量，尺寸设置为原截图尺寸的1/6。缩小截图不是为了让截图看起来变小，而是让截图变得内存更小。
        RenderTexture.active = rt;//将临时变量设置为活动项


        Graphics.Blit(original, rt);//利用GPU,将 original 中的纹理数据创建一个符合 rt 尺寸和规格的副本，存入rt。 original的纹理数据来自处理后的屏幕截图 screenshot

        Texture2D resized = new Texture2D(newWidth, newHeight,TextureFormat.RGB24, false);
        resized.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
        resized.Apply();


        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);//释放资源，避免内存泄露


        return resized;
    }
}
