using System;
using DYUnityLib;
using FormulaBase;
using GameLogic;
using System.Collections;
using UnityEngine;

/// <summary>
/// Game scene main.
///
/// 战斗场景主入口
/// </summary>
public class GameSceneMainController : MonoBehaviour
{
    private float secondCounter = 0f;
    private GameObject gameCamera;

    private void Awake()
    {
        GameGlobal.stopwatch.Reset();
        GameGlobal.stopwatch.Start();
    }

    private void Start()
    {
        this._ScreenFit();
        this.secondCounter = 0f;
        Application.targetFrameRate = 60;
        GameGlobal.gCamera = this;
#if UNITY_EDITOR || UNITY_EDITOR_OSX || UNITY_EDITOR_64
        if (StageBattleComponent.Instance.Host == null)
        {
            StageBattleComponent.Instance.InitById((int)GameGlobal.DEBUG_DEFAULT_STAGE);
        }
#endif

        CommonPanel.GetInstance().SetMask(false);
        this.StartCoroutine(this.__OnStart());
        InvokeRepeating("EarphoneDetect", 1f, GameGlobal.EARPHONE_DETECT_TIME);
    }

    //void OnEnable() {
    //}

    private void OnDestory()
    {
    }

    private void FixedUpdate()
    {
        if (FixUpdateTimer.IsPausing())
        {
            return;
        }

        if (GameGlobal.gTouch == null || GameGlobal.gGameMusic == null)
        {
            return;
        }

        GameGlobal.gTouch.TouchEvntPhase();
        GameGlobal.gGameMusic.GameMusicFixTimerUpdate();
        this.FpsMemoryShowUpdate();
    }

    private void EarphoneDetect()
    {
        //耳机检测
        if (!GameGlobal.IS_EARPHONE)
        {
            TaskStageTarget.isInEarphone = false;
        }
    }

    private IEnumerator __OnStart()
    {
        yield return new WaitForSeconds(0.01f);

        StageBattleComponent.Instance.Init();

        // for fps memory show
        if (SettingComponent.Instance.Host == null)
        {
            SettingComponent.Instance.Init();
            SettingComponent.Instance.Host.SetAsUINotifyInstance();
        }

        // 所有数据 对象准备完毕后才展示ui
        UISceneHelper.Instance.Show();
    }

    private void FpsMemoryShowUpdate()
    {
        if (this.secondCounter < 100)
        {
            this.secondCounter += 1f;
            return;
        }

        // Fps memory show init.
        if (SettingComponent.Instance.Host == null)
        {
            this.secondCounter = 0;
            return;
        }

        this.secondCounter = 0;
        string fpsShow = SettingComponent.Instance.GetShowStr();
        SettingComponent.Instance.Host.SetDynamicData(SignKeys.SMALLlTYPE, fpsShow);
    }

    private void _ScreenFit()
    {
        this.gameCamera = GameObject.Find("GameCamera").gameObject;
        if (this.gameCamera == null)
        {
            return;
        }

        Camera cam = this.gameCamera.GetComponent<Camera>();
        ScreenFit.CameraFit(cam);
    }
}