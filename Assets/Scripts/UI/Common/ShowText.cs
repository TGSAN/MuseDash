using System.Collections;
using UnityEngine;

public class ShowText : MonoBehaviour
{
    public UILabel m_label;

    public delegate void CallBackFun();

    private Callback m_callBackFun = null;
    public TweenAlpha temp;
    public TweenScale twnScale;

    public void Start()
    {
        //temp=TweenAlpha.Begin(this.gameObject,0.2f,255,0);
        //temp.animationCurve=
        //temp.ResetToBeginning();
    }

    // Use this for initialization
    public void showBox(string _label, Callback _callBackFun, bool _BehindBLur = true)
    {
        if (_BehindBLur == false)
        {
            this.gameObject.layer = 17;
        }
        else
        {
            this.gameObject.layer = 5;
        }
        this.gameObject.SetActive(true);

        //m_callBackFun=_callBackFun;

        if (_label.Length == 0)
        {
            m_label.text = "现在还不可以哦～";
        }
        else
        {
            m_label.text = _label;
        }

        if (_callBackFun == null)
        {
            temp.ResetToBeginning();
            temp.PlayForward();
            twnScale.ResetToBeginning();
            twnScale.PlayForward();
            ///TweenAlpha.Begin(this.gameObject,0.2f,255,0);
            //temp.onFinished.Add(new EventDelegate(AlphaFinish));

            //jianbian
        }
        else
        {
            _callBackFun();
        }
        //		m_callBackFun=null;
        //		if(_label.Length>0)
        //			m_label.text=_label;
        //		else
        //		{
        //			m_label.text="功能正在完善中";
        //		}
        //		if(_CallFun!=null)
        //			m_callBackFun=_CallFun;
    }

    public void AlphaFinish()
    {
        this.gameObject.SetActive(false);
        Debug.Log("Finish");
    }
}