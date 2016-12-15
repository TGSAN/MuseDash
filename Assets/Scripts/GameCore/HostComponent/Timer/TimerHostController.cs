using DYUnityLib;
using FormulaBase;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerHostController : MonoBehaviour
{
    private static TimerHostController instance;

    public static TimerHostController Instance
    {
        get
        {
            return instance;
        }
    }

    private Dictionary<string, Callback> m_CallFunc = new Dictionary<string, Callback>();

    private static int ONE_SECOND = (int)(1 / FixUpdateTimer.dInterval);
    private int secondCounter = 0;

    private List<FormulaHost> _list;

    private void OnEnable()
    {
        this.secondCounter = 0;
        instance = this;

        TimerComponent.Instance.Init();
    }

    /*
	// Use this for initialization
	void Start () {
		this.secondCounter = 0;
		instance = this;

		TimerComponent.Instance.Init ();
	}
	*/

    private void OnDestory()
    {
        this._list.Clear();
        this._list = null;
    }

    private void FixedUpdate()
    {
        // one second
        this.secondCounter += 1;
        if (this.secondCounter < ONE_SECOND)
        {
            return;
        }

        this.secondCounter = 0;

        // check timers every one second
        if (this._list == null || this._list.Count <= 0)
        {
            return;
        }

        for (int i = 0; i < this._list.Count; i++)
        {
            FormulaHost _timerHost = this._list[i];
            if (_timerHost == null)
            {
                continue;
            }

            if (_timerHost.GetRealTimeCountDownNow() > 0)
            {
                continue;
            }

            this._list[i] = null;
            TimerComponent.Instance.TimeUp(_timerHost);
            if (m_CallFunc[_timerHost.objectID] != null)
            {
                m_CallFunc[_timerHost.objectID]();
            }
        }
    }

    public void AddTimerHost(FormulaHost host, Callback callFunc = null)
    {
        if (this._list == null)
        {
            this._list = new List<FormulaHost>();
        }

        if (this._list.Contains(host))
        {
            return;
        }

        this._list.Add(host);
        if (callFunc != null)
        {
            m_CallFunc.Add(host.objectID, callFunc);
        }
    }
}