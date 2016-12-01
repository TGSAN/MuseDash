using System;
using System.Collections;
using UnityEngine;

/*
 * 使用方法：
1 通过调用全局实例gTrigger进行事件的注册／反注册操作：
	int EVENT_TEST = 129
	EventTrigger e = gTrigger.RegEvent (EVENT_TEST);
	成功注册后，获得事件对象e，同一个数字不可重复注册；
	项目使用数字从129开始，之前的作为基础库保留段；

2 向e添加响应方法：
	public void touchTrigger(object sender, EventTrigger.EventTriggerArgs e, params object[] args){
		Debug.Log ("Raise Touch Event " + e.triggerArgs);
	}

	e.Trigger += touchTrigger
	响应方法支持动态参数args
	也可以删除响应方法
	e.Trigger -= touchTrigger;

3 触发事件：
	gTrigger.FireEvent(EVENT_TEST);
	则e.Trigger所添加的响应方法会被依次调用（例如touchTrigger会被调用）。

 */

namespace DYUnityLib
{
    public class EventTrigger
    {
        //定义delegate
        public delegate void TriggerHandler(object sender, uint triggerId, params object[] args);

        //用event 关键字声明事件对象
        public uint id;

        public event TriggerHandler Trigger;

        //引发事件
        public void RaiseEvent(uint triggerId, params object[] args)
        {
            if (Trigger == null)
            {
                return;
            }
            this.id = triggerId;
            Trigger(this, triggerId, args);
        }
    }

    /*
	public class EventSubscriber {
		public void onEvent(object sender, EventTrigger.EventTriggerArgs e){
			// Debug.Log ("Raise an event " + e.triggerArgs);
		}

		//订阅事件
		public void Subscribe(EventTrigger et){
			et.Trigger += new EventTrigger.TriggerHandler (onEvent);
		}

		//取消订阅事件
		public void UnSubscribe(EventTrigger et){
			et.Trigger -= new EventTrigger.TriggerHandler (onEvent);
		}
	}
	*/

    // Use for make a static obj for global.
    internal class gTrigger
    {
        // 基础库保留段
        public const uint DYUL_EVENT_TOUCH_BEGAN = 0;

        public const uint DYUL_EVENT_TOUCH_ENDED = 1;
        public const uint DYUL_EVENT_TOUCH_MOVE = 2;

        // public static EventSubscriber gSubscriber = new EventSubscriber();
        private static Hashtable es = new Hashtable();

        public static EventTrigger RegEvent(uint eventIndex)
        {
            if (es.Contains(eventIndex) && es[eventIndex] != null)
            {
                Debug.Log("Event " + eventIndex + " already Reg.");
                return (EventTrigger)es[eventIndex];
            }

            EventTrigger _es = new EventTrigger();
            _es.id = eventIndex;
            es[eventIndex] = _es;

            return _es;
        }

        public static void UnRegEvent(uint eventIndex)
        {
            if (!es.Contains(eventIndex))
            {
                return;
            }

            EventTrigger _es = es[eventIndex] as EventTrigger;
            es.Remove(eventIndex);

            if (_es == null)
            {
                return;
            }

            _es = null;
        }

        public static void FireEvent(uint eventIndex, params object[] args)
        {
            if (!es.ContainsKey(eventIndex))
            {
                return;
            }

            System.Object esObj = es[eventIndex];
            if (esObj == null)
            {
                return;
            }

            EventTrigger _es = esObj as EventTrigger;
            _es.RaiseEvent(eventIndex, args);
        }

        public static void ClearEvent()
        {
            es.Clear();
        }
    }
}