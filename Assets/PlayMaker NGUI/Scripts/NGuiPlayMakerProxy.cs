using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class NGuiPlayMakerProxy : MonoBehaviour {
	
	public static Dictionary<NGuiPlayMakerDelegates,string> NGuiPlayMakerDelegates_LUT ;
	
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public static string GetFsmEventEnumValue(Enum value)
    {
        string output = null;
        Type type = value.GetType();

        //Check first in our cached results...

        //Look for our 'NGuiPlayMakerDelegates Attribute' 

        //in the field's custom attributes

        FieldInfo fi = type.GetField(value.ToString());
        PlayMakerUtils_FsmEvent[] attrs = fi.GetCustomAttributes(typeof(PlayMakerUtils_FsmEvent),false) as PlayMakerUtils_FsmEvent[];
        if (attrs.Length > 0)
        {
            output = attrs[0].Value;
        }

        return output;
    }
	
}

public enum NGuiPlayMakerDelegates {
		[PlayMakerUtils_FsmEvent("NGUI / ON CLICK")]
		OnClickEvent,
	
		[PlayMakerUtils_FsmEvent("NGUI / ON HOVER")]
		OnHoverEvent,
	
		[PlayMakerUtils_FsmEvent("NGUI / ON HOVER ENTER")]
		OnHoverEventEnter,
	
		[PlayMakerUtils_FsmEvent("NGUI / ON HOVER EXIT")]
		OnHoverEventExit,
	
		[PlayMakerUtils_FsmEvent("NGUI / ON PRESS")]
		OnPressEvent,
	
		[PlayMakerUtils_FsmEvent("NGUI / ON PRESS UP")]
		OnPressEventUp,
	
		[PlayMakerUtils_FsmEvent("NGUI / ON PRESS DOWN")]
		OnPressEventDown,
	
		[PlayMakerUtils_FsmEvent("NGUI / ON SELECT")]
		OnSelectEvent,
	
		[PlayMakerUtils_FsmEvent("NGUI / ON DRAG")]
		OnDragEvent,
	
		[PlayMakerUtils_FsmEvent("NGUI / ON DROP")]
		OnDropEvent,
	
		[PlayMakerUtils_FsmEvent("NGUI / ON SUBMIT")]
		OnSubmitEvent,
	
		[PlayMakerUtils_FsmEvent("NGUI / ON SLIDER CHANGE")]
		OnSliderChangeEvent,
	
		[PlayMakerUtils_FsmEvent("NGUI / ON SELECTION CHANGE")]
		OnSelectionChangeEvent,
	
		[PlayMakerUtils_FsmEvent("NGUI / ON TOOLTIP")]
		OnTooltipEvent,
	
		[PlayMakerUtils_FsmEvent("NGUI / ON CHANGE")]
		OnChangeEvent,
}
	



public class PlayMakerUtils_FsmEvent : System.Attribute
{
    private string _value;

    public PlayMakerUtils_FsmEvent(string value)
    {
        _value = value;
    }

    public string Value
    {
        get { return _value; }
    }

}
