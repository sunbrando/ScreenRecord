/*
 * Copyright <2020> <sunbrando>
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleScript : MonoBehaviour
{
    //保存的进度
    float progress;
    bool isSaving = false;

    void Start()
    {
        ScreenRecordCtrl.CreateScreenRecord();
    }

    void OnDestroy()
    {
        ScreenRecordCtrl.DestroyScreenRecord();
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width / 2, 0, 100, 60), "保留案发现场"))
        {
            if (!isSaving)
            {
                progress = 0f;
                ScreenRecordCtrl.CallScreenRecord(OnProcessingDone, OnFileSaveProgress, OnFileSaved);
            }
        }
    }

    void OnProcessingDone()
    {
        Debug.Log("准备转为GIF");
    }

    void OnFileSaveProgress(int id, float percent)
    {
        progress = percent * 100f;
        Debug.Log("当前进度 ： " + progress.ToString("F2") + "%");
    }

    void OnFileSaved(int id, string filePath)
    {
        isSaving = false;
        Debug.Log("生成gif：" + filePath);
    }
}
