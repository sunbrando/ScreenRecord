using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ScreenRecordCtrl
{
    public static void CreateScreenRecord()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        ScreenRecord.CreateInstance();
#endif
    }

    public static void DestroyScreenRecord()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        ScreenRecord screenRecord = ScreenRecord.GetInstance();

        if (screenRecord != null)
        {
            GameObject.Destroy(screenRecord.gameObject);
        }
#endif
    }

    public static string CallScreenRecord()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        ScreenRecord screenRecord = ScreenRecord.GetInstance();

        if (screenRecord == null)
        {
            return "调用失败，没有开启屏幕回溯功能";
        }
        else
        {
            string path = Path.Combine(Application.dataPath, "Gif");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return screenRecord.Capture(path);
        }
#else
        return "只能在编辑器模式和PC包使用";
#endif
    }
}
