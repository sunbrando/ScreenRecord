/*
 * Copyright <2020> <sunbrando>
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Moments.Encoder;
using System.IO;
using ThreadPriority = System.Threading.ThreadPriority;

public class ScreenRecord : MonoBehaviour {

    //线程状态
    public enum RecorderState
    {
        Prepare,
        Finish
    }

    #region 只允许单例

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

#endregion

    #region 所有的字段

    //准备开始转为gif
    public Action OnPreProcessingDone;
    //准备开始保存gif
    public Action<int, float> OnFileSaveProgress;
    //保存gif完毕
    public Action<int, string> OnFileSaved;


    //屏幕缓冲队列
    Queue<Texture2D> bufferQueue = new Queue<Texture2D>();
    //会持续记录的时间，单位：秒（根据需要调整，注意：设置太大容易内存爆炸）
    int durationTime = 10;
    //多少秒拍照记录一次（即一秒3张，根据需要调整，注意：设置太小容易内存爆炸）
    float shotTime = 0.3f;
    //下一次拍照的时间
    float nextTime;
    //-1表示不循环，0表示循环，大于0表示循环的时间
    int repeat = 0;
    //品质1~100， 1 最好
    int quality = 15;
    //线程状态
    RecorderState State { get; set; }

    #endregion

    #region 生成gif逻辑部分
    void Start()
    {
        nextTime = 0.0f;
        State = RecorderState.Finish;
        Camera.onPostRender += RenderBuffer;
    }

    void OnDestroy()
    {
        Camera.onPostRender -= RenderBuffer;
    }

    void RenderBuffer(Camera camera)
    {
        //这里会获取最后才渲染的那个相机的帧缓冲，
        //注意：这里填写的要是最后才进行渲染的那个摄像机的名字，
        //一般是UI摄像机最后，例如使用的是fairyGUI则填：Stage Camera（但UGUI没有Camera，暂时没有找到处理办法）
        if (camera.name != "Main Camera")
        {
            return;
        }

        if (State != RecorderState.Finish)
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

    public void Capture(string path)
    {
        if (State != RecorderState.Finish)
        {
            Debug.LogWarning("当前正在生成GIF，请稍等...");
            return;
        }
        State = RecorderState.Prepare;
        string name = "案发现场" + System.DateTime.Now.ToString("MM-dd HH-mm-ss") + ".gif";
        path = Path.Combine(path, name);
        PreProcess(path);
    }

    void Enqueue(Texture2D text, Queue<Texture2D> queue)
    {
        if (queue.Count > (durationTime / shotTime))
        {
            Texture2D screenShot = queue.Dequeue();
            DestroyImmediate(screenShot);
        }

        queue.Enqueue(text);
    }

    //截取屏幕
    Texture2D TakeShot(Camera camera, int width, int height)
    {
        Texture2D screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);
        //因为从GPU里读取屏幕像素，这里执行的时候，CPU无可避免的会出现峰值，可能会有点卡顿
        screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenShot.Apply(false);

        return screenShot;
    }

    void PreProcess(string filepath)
    {
        List<GifFrame> frames = new List<GifFrame>(bufferQueue.Count);

        while (bufferQueue.Count > 0)
        {
            Texture2D screenShot = bufferQueue.Dequeue();

            GifFrame frame = new GifFrame() 
            { 
                Width = screenShot.width, 
                Height = screenShot.height, 
                Data = screenShot.GetPixels32() 
            };

            frames.Add(frame);
            Flush(screenShot);
        }

        bufferQueue.Clear();

        // Callback
        if (OnPreProcessingDone != null)
            OnPreProcessingDone();

        GifEncoder encoder = new GifEncoder(repeat, quality);
        encoder.SetDelay(Mathf.RoundToInt(shotTime * 1000f));
        Moments.Worker worker = new Moments.Worker()
        {
            m_Encoder = encoder,
            m_Frames = frames,
            m_FilePath = filepath,
            m_OnFileSaved = OnFileSaved,
            m_OnFileSaveProgress = OnFileSaveProgress,
            m_OnFinish = new Action(Finish)
        };
        worker.Start();
    }

    void Flush(UnityEngine.Object obj)
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
            Destroy(obj);
        else
            DestroyImmediate(obj);
#else
            UnityEngine.Object.Destroy(obj);
#endif
    }

    void Finish()
    {
        State = RecorderState.Finish;
    }

    #endregion

}
