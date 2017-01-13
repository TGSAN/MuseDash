using UnityEngine;
using System.Collections;

/*
 * 使用方法：
用户可向该模块注册自定义的触摸响应事件

1 TouchScript touchScripte = new TouchScript ();
	生成touchScripte实例，建议直接生成全局静态；

2 生成自定义事件
	int TOUCH_EVENT = 129
	EventTrigger e1 = gTrigger.RegEvent (TOUCH_EVENT);

3 注册到touchScripte
	touchScripte.AddCustomEvent (e1);
	也可以反注册
	this.touchScripte.RemoveCustomEvent (e1);

4 添加响应方法：
	public void TouchTrigger(object sender, EventTrigger.EventTriggerArgs e, params object[] args){
		int ts = (int)args [0];
		if (ts == gTrigger.DYUL_EVENT_TOUCH_MOVE) {
			return;
		}

		Debug.Log (“TOUCH_EVENT " + e.triggerArgs + " " + ts);
	}
	这里动态参数第一个是触摸事件id，过滤掉了move操作
	e1.Trigger += TouchTrigger;

5 在使用touchScripte的对象的start中调用
	this.touchScripte.OnStart ();

6 在使用touchScripte的对象的update中调用
	this.touchScripte.TouchEvntPhase ();
 */
using UnityEngine.EventSystems;


namespace DYUnityLib {
	public class TouchScript {
		private const int CANCLE = -1;
		private const int BEGIN = 0;
		private const int MOVE = 1;
		private const int END = 2;

		private int _state = CANCLE;
		private int _singleFignerId = -1;
		private int[] _states = new int[]{CANCLE, CANCLE};
		private float touchx = 0;
		private float touchy = 0;

		private UICamera uiCamera = null;

		private ArrayList customEvent = new ArrayList();

		public void AssignTouchPosition() {
			#if !UNITY_EDITOR && !UNITY_EDITOR_OSX && !UNITY_EDITOR_64
			if (Input.touchCount > 0) {
				if (Input.touchCount == 1) {
					Touch _tch = Input.GetTouch (0);
					touchx = _tch.position.x;
					touchy = _tch.position.y;
					return;
				}

				int idx = 0;
				for (int i = 0; i < Input.touchCount; i++) {
					Touch t = Input.GetTouch(i);
					if (t.fingerId != this._singleFignerId) {
						idx = i;
						break;
					}
				}

				Touch tch = Input.GetTouch (idx);
				touchx = tch.position.x;
				touchy = tch.position.y;
			}
			#else
			if (Input.anyKey) {
				touchx = Input.mousePosition.x;
				touchy = Input.mousePosition.y;
			}
			#endif
		}

		public float GetTouchX() {
			return this.touchx;
		}

		public float GetTouchY() {
			return this.touchy;
		}

		public void AddCustomEvent(EventTrigger e) {
			if (e == null) {
				return;
			}

			if (this.customEvent.Contains (e)) {
				return;
			}

			this.customEvent.Add (e);
		}

		public void RemoveCustomEvent(EventTrigger e) {
			if (e == null) {
				return;
			}

			if (!this.customEvent.Contains (e)) {
				return;
			}

			this.customEvent.Remove (e);
		}

		public void ClearCustomEvent() {
			if (this.customEvent == null) {
				return;
			}

			this.customEvent.Clear ();
		}

		public void RegUICamera(UICamera obj) {
			this.uiCamera = obj;

			if (obj == null) {
				return;
			}

			Debug.Log ("Reg ngui ui camera " + obj.name);
		}

		public void TouchTrigger(object sender, uint triggerId, params object[] args) {
			// Debug.Log ("Raise Touch Event " + e.triggerArgs);
			if (this.customEvent == null) {
				return;
			}

			if (this.customEvent.Count <= 0) {
				return;
			}

			#if !UNITY_EDITOR && !UNITY_EDITOR_OSX && !UNITY_EDITOR_64
			int tc = Input.touchCount;
			Ray ray = this.uiCamera.cachedCamera.ScreenPointToRay (Input.GetTouch(tc - 1).position);
			#else
			if (this.uiCamera == null || this.uiCamera.cachedCamera == null) {
				return;
			}

			Ray ray = this.uiCamera.cachedCamera.ScreenPointToRay (Input.mousePosition);
			#endif

			RaycastHit hit;
			if (Physics.Raycast (ray, out hit) && hit.collider.gameObject.tag == "UI") {
				return;
			}

			foreach (EventTrigger _e in this.customEvent) {
				if (_e == null) {
					continue;
				}

				gTrigger.FireEvent (_e.id, triggerId);
			}
		}
		
		public int GetTouchCount() {
			return Input.touchCount;
		}

		public void OnStart() {
			//zhuce事件
			EventTrigger eBegan = gTrigger.RegEvent (gTrigger.DYUL_EVENT_TOUCH_BEGAN);
			EventTrigger eEnded = gTrigger.RegEvent (gTrigger.DYUL_EVENT_TOUCH_ENDED);
			EventTrigger eMoved = gTrigger.RegEvent (gTrigger.DYUL_EVENT_TOUCH_MOVE);

			eBegan.Trigger += TouchTrigger;
			eEnded.Trigger += TouchTrigger;
			eMoved.Trigger += TouchTrigger;
		}

		public void TouchEvntPhase() {
			#if !UNITY_EDITOR && !UNITY_EDITOR_OSX && !UNITY_EDITOR_64
			int c = Input.touchCount;
			if (c >= this._states.Length + 1) {
				return;
			}

			if (c > 0) {
				c -= 1;
				int _s = this._states [c];

				// Begin
				if (_s == CANCLE) {
					this._states [c] = BEGIN;
					// Do begin.
					gTrigger.FireEvent(gTrigger.DYUL_EVENT_TOUCH_BEGAN);
					return;
				}
				
				// Move
				this._states [c] = MOVE;
				// Do move.
				gTrigger.FireEvent(gTrigger.DYUL_EVENT_TOUCH_MOVE);
				// return;

				if (c == 0) {
					this._singleFignerId = Input.GetTouch(0).fingerId;
					if (this._states [1] > CANCLE && this._states [1] != END) {
						this._states [1] = END;
						// Do end.
						gTrigger.FireEvent(gTrigger.DYUL_EVENT_TOUCH_ENDED);
						return;
					}

					this._states [1] = CANCLE;
				} else {
					return;
				}
			}

			// End
			if (this._states [0] > CANCLE && this._states [0] != END) {
				this._states [0] = END;
				// Do end.
				gTrigger.FireEvent(gTrigger.DYUL_EVENT_TOUCH_ENDED);
				return;
			}
			
			this._states [0] = CANCLE;
			
			#else
			// Begin
			if (Input.anyKeyDown) {
				if (this._state != BEGIN) {
					this._state = BEGIN;
					// Do begin.
					gTrigger.FireEvent(gTrigger.DYUL_EVENT_TOUCH_BEGAN);
				}
				
				return;
			}
			
			// Move
			if (Input.anyKey) {
				this._state = MOVE;
				// Do move.
				gTrigger.FireEvent(gTrigger.DYUL_EVENT_TOUCH_MOVE);
				return;
			}
			
			// End
			if (this._state > CANCLE && this._state != END) {
				this._state = END;
				// Do end.
				gTrigger.FireEvent(gTrigger.DYUL_EVENT_TOUCH_ENDED);
				return;
			}
			
			this._state = CANCLE;
			#endif
		}
	}
}
