using FormulaBase;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class CommonPanel : MonoBehaviour
{
    public ShowText showText;
    public GameObject waittingPanel;
    public GameObject yesNoPanel, btnYes, btnNo, btnMask;
    public UILabel txtYesNoContent, TxtDebug;

    public GameObject GMRobot;                //GM机器人
    public UIMask UIMask;                     //各界面间的渐黑
    public UIGrid debugGrid;
    public GameObject theDebugPrefab;
    public UIToggle tglAutoPlay;
    public bool showDebug;
    private static CommonPanel m_Instance;

    public static CommonPanel GetInstance()
    {
        return m_Instance;
    }

    private readonly Callback m_CallBack = null;

    private void Start()
    {
        transform.position = new Vector3(0, 0, -4f);
        this.SignltonCheck();
    }

    private void OnEnable()
    {
        this.SignltonCheck();
    }

    public void ResetMask()
    {
        this.UIMask.Reset();
    }

    private void SignltonCheck()
    {
        if (CommonPanel.GetInstance() == this)
        {
            return;
        }

        if (CommonPanel.GetInstance() == null)
        {
            m_Instance = this;
            DontDestroyOnLoad(this.gameObject);
            return;
        }

        GameObject.Destroy(this.gameObject);
    }

    /// <summary>
    /// 显示等待界面
    /// </summary>
    /// <param name="_show">If set to <c>true</c> show.</param>
    public void ShowWaittingPanel(bool _show = true)
    {
        waittingPanel.gameObject.SetActive(_show);
    }

    /// <summary>
    /// OK
    /// </summary>
    /// <param name="_str">String.</param>
    /// <param name="_callBack">Call back.</param>
    public void ShowOkBox(string _str = "", Callback _callBack = null)
    {
    }

    public void ShowYesNo(string content, Callback yesCallFunc, Callback noCallFunc = null)
    {
        yesNoPanel.SetActive(true);
        var animator = yesNoPanel.GetComponent<Animator>();
        animator.Play("pnl_yes_or_no_in");
        txtYesNoContent.text = content;
        UIEventListener.Get(btnYes.gameObject).onClick = go =>
        {
            if (yesCallFunc != null)
            {
                yesCallFunc();
            }
            animator.Play("pnl_yes_or_no_out");
            DOTweenUtils.Delay(() =>
            {
                yesNoPanel.SetActive(false);
            }, 0.2f);
        };
        UIEventListener.VoidDelegate quitCallFunc = go =>
        {
            if (noCallFunc != null)
            {
                noCallFunc();
            }
            animator.Play("pnl_yes_or_no_out");
            DOTweenUtils.Delay(() =>
            {
                yesNoPanel.SetActive(false);
            }, 0.2f);
        };
        UIEventListener.Get(btnNo.gameObject).onClick = quitCallFunc;
        UIEventListener.Get(btnMask.gameObject).onClick = quitCallFunc;
    }

    public void SetAutoPlay(bool isTo)
    {
        if (StageBattleComponent.Instance != null)
        {
            StageBattleComponent.Instance.SetAutoPlay(tglAutoPlay.value);
        }
    }

    /// <summary>
    /// debug信息
    /// </summary>
    /// <param name="info"></param>
    public void DebugInfo(string info)
    {
        if (debugGrid == null)
        {
            return;
        }
        debugGrid.enabled = true;
        debugGrid.gameObject.SetActive(true);
        var go = GameObject.Instantiate(theDebugPrefab, debugGrid.transform, false) as GameObject;
        if (go != null)
        {
            var label = go.GetComponent<UILabel>();
            label.text = info;
            DOTweenUtils.Delay(() =>
            {
                Destroy(go);
            }, 10.0f);
        }
    }

    public void DebugTxt(string info)
    {
        TxtDebug.gameObject.SetActive(showDebug);
        TxtDebug.text = info;
    }

    public void SetDebugTxtVisible()
    {
        showDebug = !showDebug;
        TxtDebug.gameObject.SetActive(showDebug);
    }

    /// <summary>
    /// 文本显示  自带渐隐动画
    /// </summary>
    /// <param name="str">String.</param>
    /// <param name="callBack">Call back.</param>
    public void ShowText(string str = "", Callback callBack = null, bool behindBlur = true)
    {
        showText.showBox(str, callBack, behindBlur);
    }

    public void ShowTextLackDiamond(string str = "钻石不足", Callback callBack = null, bool behindBlur = true)
    {
        showText.showBox(str, callBack, behindBlur);
    }

    public void ShowTextLackMoney(string str = "金币不足", Callback callBack = null, bool behindBlur = true)
    {
        showText.showBox(str, callBack, behindBlur);
    }

    /// <summary>
    /// 关闭通用界面
    /// </summary>
    public void CloseCommonPanel()
    {
        if (m_CallBack != null)
        {
            m_CallBack();
        }
        this.gameObject.SetActive(false);
    }

    #region 各界面间的渐黑

    public void SetMask(bool whiteToBlack = true, Callback callBack = null, bool blur = false)
    {
        UIMask.SetMask(whiteToBlack, callBack, blur);
    }

    #endregion 各界面间的渐黑
}