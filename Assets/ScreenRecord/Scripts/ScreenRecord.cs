#if UNITY_EDITOR || UNITY_STANDALONE_WIN

using Gif.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using UnityEngine;

public class ScreenRecord : MonoBehaviour {
    //只允许单例
    private static ScreenRecord instance = null;
    public static void CreateInstance()
    {
        if (instance == null)
        {
            var go = new GameObject();
            go.name = "ScreenRecord";
            GameObject.DontDestroyOnLoad(go);
            instance = go.AddComponent<ScreenRecord>();
        }
    }

    public static ScreenRecord GetInstance()
    {
        return instance;
    }

    //屏幕缓冲队列
    Queue<Texture2D> bufferQueue = new Queue<Texture2D>();

    //会持续记录的时间，单位：秒（根据需要调整，注意：设置太大容易内存爆炸）
    int durationTime = 10;
    //多少秒拍照记录一次（即一秒3张，根据需要调整，注意：设置太小容易内存爆炸）
    float shotTime = 0.3f;
    //下一次拍照的时间
    float nextTime;

    void Start()
    {
        Camera.onPostRender += RenderBuffer;
        nextTime = 0.0f;
    }
    void OnDestroy()
    {
        Camera.onPostRender -= RenderBuffer;
    }

    void RenderBuffer(Camera camera)
    {
        //这里会获取最后才渲染的那个相机的帧缓冲，
        //注意：这里填写的要是最后才进行渲染的那个摄像机的名字，
        //一般是UI摄像机最后，例如使用的是fairyGUI则填：Stage Camera（UGUI暂时不知道怎么处理）
        if (camera.name != "Main Camera")
        {
            return;
        }
        if (Time.fixedTime - nextTime < shotTime)
        {
            return;
        }
        nextTime = Time.fixedTime;

        Texture2D tex = TakeShot(Camera.main, Screen.width, Screen.height);
        Enqueue(tex, bufferQueue);
    }

    public string Capture(string path)
    {
        string name = "案发现场" + System.DateTime.Now.ToString("MM-dd HH-mm-ss") + ".gif";
        path = Path.Combine(path, name);
        ToGif(bufferQueue, path, (int)(shotTime * 1000));

        bufferQueue.Clear();
        GC.Collect();

        return path;
    }

    void Enqueue(Texture2D text, Queue<Texture2D> queue)
    {
        if (queue.Count > (durationTime / shotTime))
        {
            Texture2D screenShot = queue.Dequeue();
            GameObject.DestroyImmediate(screenShot);
        }

        queue.Enqueue(text);
    }

    //截取屏幕
    Texture2D TakeShot(Camera camera, int width, int height)
    {
        Texture2D screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);
        //因为从GPU里读取屏幕像素，这里执行的时候，CPU无可避免的会出现峰值，但不会造成明显卡顿
        screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenShot.Apply(false);

        return screenShot;
    }

    /// <summary>
    /// 生成为gif文件
    /// </summary>
    /// <param name="giffile">gif保存路径</param>
    /// <param name="time">每帧的时间/ms</param>
    void ToGif(Queue<Texture2D> queue, string giffile, int time)
    {
        AnimatedGifEncoder e = new AnimatedGifEncoder();
        e.Start(giffile);

        //每帧播放时间
        e.SetDelay(time);

        //-1：不重复，0：重复
        e.SetRepeat(0);

        //图像转换的质量，1~20：1最好，20最差，但生成速度快
        e.SetQuality(20);

        for (int i = 0, count = queue.Count; i < count; i++)
        {
            System.Drawing.Image img = GetImg(queue);
            if (img == null)
            {
                break;
            }
            e.AddFrame(img);
        }
        e.Finish();
    }

    System.Drawing.Image GetImg(Queue<Texture2D> queue)
    {
        Texture2D screenShot = queue.Dequeue();
        if (screenShot == null)
        {
            return null;
        }

        //0~100：0最差，100最好
        byte[] bytes = screenShot.EncodeToJPG(50);
        MemoryStream ms = new MemoryStream(bytes);
        GameObject.DestroyImmediate(screenShot);
        System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
        //转成缩略图再输出
        System.Drawing.Image.GetThumbnailImageAbort myCallback = new Image.GetThumbnailImageAbort(ThumbnailCallback);
        System.Drawing.Image imgThumbnail = img.GetThumbnailImage(img.Width / 2, img.Height / 2, myCallback, IntPtr.Zero);
        return imgThumbnail;
    }

    public bool ThumbnailCallback()
    {
        return false;
    }
}

#endif