// (c) copyright Hutong Games, LLC 2010-2013. All rights reserved.
using System;
using System.Collections;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;


/// <summary>
/// Put this component on the GameObject with the Collider used by NGUI.
/// Choose an FSM to send events to (leave blank to target an FSM on the same GameObject).
/// You can rename the events to match descriptive event names in your FSM. E.g., "OK Button Pressed"
/// NOTE: Use the Get Event Info action in PlayMaker to get event arguments.
/// See also: http://www.tasharen.com/?page_id=160
/// </summary>
public class NGuiEventsToPlaymakerFsmEvents : MonoBehaviour
{
	public bool debug = false;
	
	public bool OnlyShowImplemented = false;
	
	public static UICamera.MouseOrTouch currentTouch;
	
	public PlayMakerFSM targetFSM;
	
	public List<NGuiPlayMakerDelegates> customEventsKeys;
	public List<string> customEventsValues;
	
	private int[] _usage; 
	
	private UIInput _input;
	private UIProgressBar _pBar;
	private UIToggle _toggle;
	
	public int getUsage(NGuiPlayMakerDelegates fsmEventDelegate)
	{
		//Debug.Log("get usage for "+fsmEventDelegate);
		if (_usage==null)
		{
			return 0;
		}
		int index = (int)fsmEventDelegate;
		//Debug.Log("get usage for index"+index);
		
		if (index>=_usage.Length)
		{
			return -1;
		}
		
		return _usage[index];
	}
	
	void OnEnable()
	{
		if (_usage==null || _usage.Length==0)
		{
			_usage = new int[Enum.GetNames(typeof(NGuiPlayMakerDelegates)).Length];
		}
		
		if (targetFSM == null)
		{
			targetFSM = GetComponent<PlayMakerFSM>();
		}

		if (targetFSM == null)
		{
			enabled = false;
			Debug.LogWarning("No Fsm Target assigned");
		}
		
		// check if we are using on Submit
		if (DoesTargetImplementsEvent(targetFSM,NGuiPlayMakerDelegates.OnSubmitEvent))
		{
			_input = this.GetComponent<UIInput>();
			if (_input!=null)
			{
				EventDelegate _del = new EventDelegate();
				
				_del.target = this;
				_del.methodName = "OnSubmitChange";
				_input.onSubmit.Add(_del);
			}
			
		}
		
		// check if we are using on Slider change
		if (DoesTargetImplementsEvent(targetFSM,NGuiPlayMakerDelegates.OnSliderChangeEvent))
		{
			_pBar = this.GetComponent<UIProgressBar>();
			if (_pBar!=null)
			{
				EventDelegate _del = new EventDelegate();
				
				_del.target = this;
				_del.methodName = "OnSliderChange";
				_pBar.onChange.Add(_del);
			}
			
		}
		
		// check if we are using on Input or toggle change
		if (DoesTargetImplementsEvent(targetFSM,NGuiPlayMakerDelegates.OnChangeEvent))
		{
			_input = this.GetComponent<UIInput>();
			if (_input!=null)
			{
				EventDelegate _del = new EventDelegate();
				
				_del.target = this;
				_del.methodName = "OnChange";
				_input.onChange.Add(_del);
			}
			
			_toggle = this.GetComponent<UIToggle>();
			if (_toggle!=null)
			{
				EventDelegate _del = new EventDelegate();
				
				_del.target = this;
				_del.methodName = "OnChange";
				_toggle.onChange.Add(_del);
			}
		}

	}
	
	public bool DoesTargetMissEventImplementation(PlayMakerFSM fsm, NGuiPlayMakerDelegates fsmEventDelegate)
	{
		return DoesTargetMissEventImplementation(fsm,NGuiPlayMakerProxy.GetFsmEventEnumValue(fsmEventDelegate));
	}
	
	public bool DoesTargetMissEventImplementation(PlayMakerFSM fsm, string fsmEvent)
	{
		if (DoesTargetImplementsEvent(fsm,fsmEvent))
		{
			return false;
		}
		
		foreach(FsmEvent _event in fsm.FsmEvents)
		{
			if (_event.Name.Equals(fsmEvent))
			{
				return true;
			}
		}
		
		return false;
	}
	
	public bool DoesTargetImplementsEvent(PlayMakerFSM fsm, NGuiPlayMakerDelegates fsmEventDelegate)
	{
		return DoesTargetImplementsEvent(fsm,NGuiPlayMakerProxy.GetFsmEventEnumValue(fsmEventDelegate));
	}
	
	public bool DoesTargetImplementsEvent(PlayMakerFSM fsm, string fsmEvent)
	{
		foreach(FsmTransition _transition in fsm.FsmGlobalTransitions)
		{
			if (_transition.EventName.Equals(fsmEvent))
			{
				return true;
			}
		}
		
		foreach(FsmState _state in fsm.FsmStates)
		{
			
			foreach(FsmTransition _transition in _state.Transitions)
			{
				
				if (_transition.EventName.Equals(fsmEvent))
				{
					return true;
				}
			}
		}

		return false;
	}
	
	void FireNGUIPlayMakerEvent(NGuiPlayMakerDelegates _event)
	{
		if (customEventsKeys.Contains(_event))
		{
			targetFSM.SendEvent(customEventsValues[customEventsKeys.IndexOf(_event)]);
		}else{
			if (debug) Debug.Log("Sending event"+NGuiPlayMakerProxy.GetFsmEventEnumValue(_event));
			targetFSM.SendEvent(NGuiPlayMakerProxy.GetFsmEventEnumValue(_event));
		}
	}

	
	void OnClick()
	{
		if (!enabled || targetFSM == null) return;
		
		_usage[(int)NGuiPlayMakerDelegates.OnClickEvent] ++;
		
		if (debug)	Debug.Log("NGuiEventsToPlaymakerFsmEvents OnClick() "+_usage[(int)NGuiPlayMakerDelegates.OnClickEvent]+" to "+targetFSM.gameObject.name+"/"+targetFSM.FsmName);

		currentTouch = UICamera.currentTouch;
		
		FireNGUIPlayMakerEvent(NGuiPlayMakerDelegates.OnClickEvent);
	}
	
	
	void OnHover(bool isOver)
	{
		if (!enabled || targetFSM == null) return;
		
		_usage[(int)NGuiPlayMakerDelegates.OnHoverEvent] ++;
		
		if (debug) Debug.Log("NGuiEventsToPlaymakerFsmEvents OnHover("+isOver+") "+_usage[(int)NGuiPlayMakerDelegates.OnClickEvent]+" to "+targetFSM.gameObject.name+"/"+targetFSM.FsmName);
		Fsm.EventData.BoolData = isOver;
		
		currentTouch = UICamera.currentTouch;
		
		FireNGUIPlayMakerEvent(NGuiPlayMakerDelegates.OnHoverEvent);
		
		if (isOver)
		{
			FireNGUIPlayMakerEvent(NGuiPlayMakerDelegates.OnHoverEventEnter);
			_usage[(int)NGuiPlayMakerDelegates.OnHoverEventEnter] ++;
		}else{
			FireNGUIPlayMakerEvent(NGuiPlayMakerDelegates.OnHoverEventExit);
			_usage[(int)NGuiPlayMakerDelegates.OnHoverEventExit] ++;
		}
		
	}

	void OnPress(bool pressed)
	{
		if (!enabled || targetFSM == null) return;
		
		_usage[(int)NGuiPlayMakerDelegates.OnPressEvent] ++;
		
		if (debug) Debug.Log("NGuiEventsToPlaymakerFsmEvents OnPress("+pressed+") "+_usage[(int)NGuiPlayMakerDelegates.OnPressEvent]+" to "+targetFSM.gameObject.name+"/"+targetFSM.FsmName);
		
		Fsm.EventData.BoolData = pressed;
		
		currentTouch = UICamera.currentTouch;
		
		FireNGUIPlayMakerEvent(NGuiPlayMakerDelegates.OnPressEvent);
				
		if (pressed)
		{
			FireNGUIPlayMakerEvent(NGuiPlayMakerDelegates.OnPressEventDown);
			_usage[(int)NGuiPlayMakerDelegates.OnPressEventDown] ++;
		}else{
			FireNGUIPlayMakerEvent(NGuiPlayMakerDelegates.OnPressEventUp);
			_usage[(int)NGuiPlayMakerDelegates.OnPressEventUp] ++;
		}
	}

	void OnSelect(bool selected)
	{
		if (!enabled || targetFSM == null) return;
		
		if (debug) Debug.Log("NGuiEventsToPlaymakerFsmEvents OnSelect("+selected+") to "+targetFSM.gameObject.name+"/"+targetFSM.FsmName);
		
		Fsm.EventData.BoolData = selected;
		
		currentTouch = UICamera.currentTouch;
		
		FireNGUIPlayMakerEvent(NGuiPlayMakerDelegates.OnSelectEvent);
	}

	void OnDrag(Vector2 delta)
	{
		if (!enabled || targetFSM == null) return;
		
		if (debug) Debug.Log("NGuiEventsToPlaymakerFsmEvents OnDrag("+delta+") to "+targetFSM.gameObject.name+"/"+targetFSM.FsmName);
		
		Fsm.EventData.Vector3Data = new Vector3(delta.x, delta.y);
		
		currentTouch = UICamera.currentTouch;
		
		FireNGUIPlayMakerEvent(NGuiPlayMakerDelegates.OnDragEvent);
	}
	
	void OnDrop(GameObject go)
	{
		if (!enabled || targetFSM == null) return;
		
		if (debug) Debug.Log("NGuiEventsToPlaymakerFsmEvents OnDrop("+go.name+") to "+targetFSM.gameObject.name+"/"+targetFSM.FsmName);
		
		Fsm.EventData.GameObjectData = go;
		
		currentTouch = UICamera.currentTouch;
		
		FireNGUIPlayMakerEvent(NGuiPlayMakerDelegates.OnDropEvent);
	}

	void OnTooltip(bool show)
	{
		if (!enabled || targetFSM == null) return;
		
		if (debug) Debug.Log("NGuiEventsToPlaymakerFsmEvents OnTooltip("+show+") to "+targetFSM.gameObject.name+"/"+targetFSM.FsmName);
		
		Fsm.EventData.BoolData = show;
		
		currentTouch = UICamera.currentTouch;
		
		FireNGUIPlayMakerEvent(NGuiPlayMakerDelegates.OnTooltipEvent);
	}
	
	void OnSubmitChange()
	{
		if (!enabled || targetFSM == null) return;
		
		_usage[(int)NGuiPlayMakerDelegates.OnSubmitEvent] ++;
		
		string _value = "";
		if (_input!=null)
		{
			_value =_input.value;
			Fsm.EventData.StringData = _value;
		}
		
		if (debug) Debug.Log("NGuiEventsToPlaymakerFsmEvents OnSubmitChange ("+_value+") to "+targetFSM.gameObject.name+"/"+targetFSM.FsmName);
		
			
		FireNGUIPlayMakerEvent(NGuiPlayMakerDelegates.OnSubmitEvent);
	}
	
	void OnSliderChange(float value)
	{
		if (!enabled || targetFSM == null) return;
		
		_usage[(int)NGuiPlayMakerDelegates.OnSliderChangeEvent] ++;
		
		Fsm.EventData.FloatData = value;
		
		float _value = 0f;
		if (_pBar!=null)
		{
			_value =_pBar.value;
			Fsm.EventData.FloatData = _value;
		}

		if (debug) Debug.Log("NGuiEventsToPlaymakerFsmEvents OnSliderChange("+_value+") to "+targetFSM.gameObject.name+"/"+targetFSM.FsmName);
		
	
		FireNGUIPlayMakerEvent(NGuiPlayMakerDelegates.OnSliderChangeEvent);
	}
	
	void OnSelectionChange (string item)
	{
		if (!enabled || targetFSM == null) return;
		
		_usage[(int)NGuiPlayMakerDelegates.OnSelectionChangeEvent] ++;
		
		if (debug) Debug.Log("NGuiEventsToPlaymakerFsmEvents OnSelectionChange("+item+") "+_usage[(int)NGuiPlayMakerDelegates.OnSelectionChangeEvent]+" to "+targetFSM.gameObject.name+"/"+targetFSM.FsmName);
		
		Fsm.EventData.StringData = item;
	
		FireNGUIPlayMakerEvent(NGuiPlayMakerDelegates.OnSelectionChangeEvent);	
	}
	
	void OnChange()
	{
		if (!enabled || targetFSM == null) return;
		
		_usage[(int)NGuiPlayMakerDelegates.OnChangeEvent] ++;
		
		if (_input!=null)
		{
			Fsm.EventData.StringData = _input.value;
			if (debug) Debug.Log("NGuiEventsToPlaymakerFsmEvents UIInput OnChange("+_input.value+") to "+targetFSM.gameObject.name+"/"+targetFSM.FsmName);
		}
		
		if (_toggle!=null)
		{
			Fsm.EventData.BoolData = _toggle.value;
			if (debug) Debug.Log("NGuiEventsToPlaymakerFsmEvents UIToggle OnChange("+_toggle.value+") to "+targetFSM.gameObject.name+"/"+targetFSM.FsmName);
		}
	
		FireNGUIPlayMakerEvent(NGuiPlayMakerDelegates.OnChangeEvent);
	}
	
	public void SetCurrentSelection ()
	{
		if (!enabled || targetFSM == null) return;
		
		
		
		if (UIPopupList.current != null)
		{
			_usage[(int)NGuiPlayMakerDelegates.OnSelectionChangeEvent] ++;
		
			string item =  UIPopupList.current.isLocalized ?
				Localization.Get(UIPopupList.current.value) :
				UIPopupList.current.value;
	
			if (debug) Debug.Log("NGuiEventsToPlaymakerFsmEvents OnSelectionChange("+item+") "+_usage[(int)NGuiPlayMakerDelegates.OnSelectionChangeEvent]+" to "+targetFSM.gameObject.name+"/"+targetFSM.FsmName);
		
			Fsm.EventData.StringData = item;
	
			FireNGUIPlayMakerEvent(NGuiPlayMakerDelegates.OnSelectionChangeEvent);	
			
		}
	}
	
	

}
