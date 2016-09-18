//
//  WindowAdjustor.cs UIRoot 自适应
//  Unity 4.6 Ngui 3.8
// 此脚本拖到UIroot上 ScreenW ScreenH 分别设置成编辑时的窗口大小 误锁定自带自适应
// 目前只支持横屏

using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class WindowAdjustor : MonoBehaviour
{


    public float ScreenW;
    public float ScreenH;
    void Start()
    {
        
            Screen.SetResolution((int)((Screen.height) * (ScreenW / ScreenH)), (int)((Screen.height)), Screen.fullScreen);
    }
}