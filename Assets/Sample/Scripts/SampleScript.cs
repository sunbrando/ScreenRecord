using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleScript : MonoBehaviour
{

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
        int width = 100;
        int height = 60;
        if (GUI.Button(new Rect(Screen.width / 2, 0, width, height), "保留案发现场"))
        {
            string path = ScreenRecordCtrl.CallScreenRecord();
            Debug.Log(path);
        }
    }
}
