using System;
using DYUnityLib;
using System.Collections;
using System.Collections.Generic;
using Boo.Lang;
using FormulaBase;
using UnityEngine;

/*
 * 使用方法：
 * 该timer标准单位是秒， 精度默认0.01
可以在timer生命周期内设置不同时刻触发事件
事件队列需要在启动timer前设置好

0 在一个可靠的FixedUpdate中加入FixUpdateTimer.RollTimer ();

1 创建需要响应的事件
	int TIMER_EVENT = 255
	EventTrigger t1 = gTrigger.RegEvent (TIMER_EVENT);

2 添加事件响应方法
	public void TimerTrigger(object sender, EventTrigger.EventTriggerArgs e, params object[] args){
		decimal ts = (decimal)args [0];
		Debug.Log ("Timer " + e.triggerArgs + " Tick " + ts);
	}
	args 0 是当前出发的秒（毫秒）数
	添加事件响应方法
	t1.Trigger += TimerTrigger;

3 创建timer对象
	FixUpdateTimer objTimer = new FixUpdateTimer();

4 初始化timer对象，（总时间(-1表示无限)，计时器类型）
	objTimer.init (10.0f, FixUpdateTimer.TIMER_TYPE_EVENT_ARRAY);
	时刻 —> 触发的消息
	objTimer.AddTickEvent (2.34m, TIMER_EVENT);
	objTimer.AddTickEvent (3.54m, TIMER_EVENT);
	objTimer.AddTickEvent (6.22m, TIMER_EVENT);
	TIMER_TYPE_EVENT_ARRAY  默认类型，不规则时间事件表

	TIMER_TYPE_STEP_ARRAY   固定时间间隔触发固定事件
							需要调用一次AddTickEvent注册固定事件id
							第一个参数tick此时无效可随意，建议0

5 启动timer
	objTimer.Run ();

然后，timer会在2.22秒后开启，
在开启后的2.34、3.54、6.22秒各触发一次TIMER_EVENT消息
也可以在这几个时刻触发其他消息

小技巧：
objTimer.AddTickEvent (0.0m, XXX); // 启动timer触发，相当于onBegan
objTimer.AddTickEvent (10.0m, XXX); //   结束timer触发，相当于onEnded
 */

namespace DYUnityLib
{
    public class FixUpdateTimer
    {
        private static bool _isPausing = false;
        public static ArrayList timers = new ArrayList();

        public static void RollTimer()
        {
            for (int j = 0; j < timers.Count; j++)
            {
                var ft = (FixUpdateTimer)timers[j];
                ft.OnTick();
            }
        }

        public static void PauseTimer()
        {
            for (int i = 0; i < timers.Count; i++)
            {
                FixUpdateTimer ft = (FixUpdateTimer)timers[i];
                ft.Pause();
            }

            _isPausing = true;
        }

        public static void ResumeTimer()
        {
            for (int i = 0; i < timers.Count; i++)
            {
                FixUpdateTimer ft = (FixUpdateTimer)timers[i];
                ft.Resume();
            }

            _isPausing = false;
        }

        public static bool IsPausing()
        {
            return _isPausing;
        }

        public const int TIMER_TYPE_EVENT_ARRAY = 0;
        public const int TIMER_TYPE_STEP_ARRAY = 1;

        private int iType = 0;
        private const int interval = 1;
        private const int precision = 100;
        public const decimal dInterval = 0.01m;
        public const float fInterval = 0.01f;
        private int passedTick = -1;
        private int prePassedTick;
        private int passedCount;
        private int totalTick;
        private uint defaultEvent;
        private bool isPause;
        private int startTick;
        private bool isTick = false;

        private Hashtable eventTbl = new Hashtable();

        // totalTick < 0 is unlimit time
        // totalTick is 0.00 with unit is second
        // all float force to int here
        public void Init(decimal totalTick, int timerType = TIMER_TYPE_EVENT_ARRAY)
        {
            this.isPause = true;
            this.passedTick = -1;
            this.passedCount = 0;
            this.iType = timerType;
            this.totalTick = (int)(totalTick * precision);

            if (!timers.Contains(this))
            {
                timers.Add(this);
            }
        }

        public void Run()
        {
            //			this.Cancle ();
            this.passedTick = -1;
            this.passedCount = 0;
            this.prePassedTick = 0;
            this.startTick = 0;
            this.Resume();
        }

        public bool IsRunning()
        {
            return !this.isPause;
        }

        public void Cancle()
        {
            // Debug.Log ("Cancle Timer at " + this.passedTick);
            this.passedTick = -1;
            this.passedCount = 0;
            if (timers.Contains(this))
            {
                timers.Remove(this);
            }
        }

        public void Pause()
        {
            if (this.eventTbl == null)
            {
                return;
            }

            this.isPause = true;
        }

        public void Resume()
        {
            this.isPause = false;
        }

        public void AddTickEvent(decimal tick, uint eventIndex)
        {
            if (this.eventTbl == null)
            {
                return;
            }

            if (this.iType == TIMER_TYPE_STEP_ARRAY)
            {
                this.defaultEvent = eventIndex;
            }

            int _tick = Mathf.RoundToInt((float)(tick * (decimal)precision));
            this.eventTbl[_tick] = eventIndex;
        }

        public void RemoveTickEvent(decimal tick)
        {
            if (this.eventTbl == null)
            {
                return;
            }

            int _tick = (int)(tick * precision);
            this.eventTbl[_tick] = null;
        }

        public void ClearTickEvent()
        {
            this.defaultEvent = 0;

            if (this.eventTbl == null)
            {
                return;
            }

            this.eventTbl.Clear();
        }

        public decimal GetPassTick()
        {
            return this.passedTick * dInterval;
        }

        public int GetPassCount()
        {
            return this.passedCount;
        }

        public int Count()
        {
            if (this.eventTbl == null)
            {
                return 0;
            }

            if (this.iType == TIMER_TYPE_STEP_ARRAY)
            {
                if (this.defaultEvent != 0)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }

            return this.eventTbl.Count;
        }

        public void PassedCountAdd()
        {
            this.passedCount += 1;
        }

        public void SetProgress(decimal tick)
        {
            int tempPassCount = 0;
            var tickCount = Mathf.RoundToInt((float)tick * (float)precision);
            SetPassTick(tickCount);
            foreach (DictionaryEntry de in this.eventTbl)
            {
                int _idx = int.Parse(de.Key.ToString());
                if (this.passedTick > _idx)
                {
                    tempPassCount += 1;
                }
            }
            this.passedCount = tempPassCount;
        }

        private void __OnTickEventArray()
        {
            if (this.eventTbl == null)
            {
                return;
            }
            // Over time check.
            if (this.totalTick >= 0)
            {
                if (this.passedTick > this.totalTick)
                {
                    this.Cancle();
                    return;
                }
            }

            var oIdxs = new Dictionary<int, object>();
            for (int i = prePassedTick + 1; i <= passedTick; i++)
            {
                var e = eventTbl[i];
                if (e != null)
                {
                    oIdxs.Add(i, e);
                }
            }

            if (oIdxs.Count <= 0)
            {
                return;
            }

            foreach (var oIdx in oIdxs)
            {
                var eventIndex = (uint)oIdx.Value;
                if (eventIndex <= 0) continue;
                gTrigger.FireEvent(eventIndex, oIdx.Key * dInterval);
            }

            this.passedCount += 1;
        }

        private void __OnTickStepArray()
        {
            // Over time check.
            if (this.totalTick >= 0)
            {
                if (this.passedTick > this.totalTick)
                {
                    this.Cancle();
                    return;
                }
            }
            //Debug.Log(" Trig event ---->>> " + this.defaultEvent + " at " + (this.passedTick * dInterval));
            /*for (int i = this.prePassedTick + 1; i <= this.passedTick; i++)
            {
                gTrigger.FireEvent(this.defaultEvent, i * dInterval);
            }*/

            gTrigger.FireEvent(this.defaultEvent, this.passedTick * dInterval);
        }

        private void OnTick()
        {
            if (this.isPause)
            {
                return;
            }
            if (startTick == 0)
            {
                startTick = StageBattleComponent.Instance.musicStartTime == 0 ? StageBattleComponent.Instance.realTimeTick : StageBattleComponent.Instance.musicStartTime;
            }
            var curTick = (decimal)((StageBattleComponent.Instance.realTimeTick - startTick) / 1000f);
            var curPassedTick = Mathf.RoundToInt((float)curTick * (float)precision);

            if (curPassedTick == this.passedTick) return;
            SetPassTick(curPassedTick);
            if (this.iType == TIMER_TYPE_STEP_ARRAY)
            {
                this.__OnTickStepArray();
            }
            else if (this.iType == TIMER_TYPE_EVENT_ARRAY)
            {
                this.__OnTickEventArray();
            }
        }

        private void SetPassTick(int tick)
        {
            this.prePassedTick = this.passedTick;
            this.passedTick = tick;
        }

        private void SetPassTick()
        {
            var curTick = (decimal)((StageBattleComponent.Instance.realTimeTick - startTick) / 1000f);
            var curPassedTick = Mathf.RoundToInt((float)curTick * (float)precision);
            this.passedTick = curPassedTick;
        }
    }
}