using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using HutongGames.PlayMakerEditor;

[CustomEditor(typeof(NGuiEventsToPlaymakerFsmEvents))]
public class NGuiEventsToPlaymakerFsmEventsInspector : Editor
{

	public override void OnInspectorGUI()
    {
		 NGuiEventsToPlaymakerFsmEvents _target = (NGuiEventsToPlaymakerFsmEvents)this.target;
		
        EditorGUI.indentLevel = 0;
		
		
		EditorGUILayout.Space();
		_target.debug = EditorGUILayout.Toggle("Debug",_target.debug);
		
		_target.OnlyShowImplemented =  EditorGUILayout.Toggle("Hide unused events",_target.OnlyShowImplemented);
		EditorGUILayout.Separator();
		
		PlayMakerFSM _targetFsm = _target.targetFSM;
		
		if (_target.targetFSM == null)
		{
			_targetFsm = _target.GetComponent<PlayMakerFSM>();
			
			if (_targetFsm!=null)
			{
				PlayMakerFSM _newTargetFSM = (PlayMakerFSM)EditorGUILayout.ObjectField("The Target defaults to",_targetFsm,typeof(PlayMakerFSM),true,null);	
				if (_newTargetFSM!=_targetFsm)
				{
					_target.targetFSM = _newTargetFSM;
				}
				//GUILayout.Label("The Target will be the first Fsm found on this GameObject:\n <"+_targetFsm.FsmName+">");
			}
			
		}else{
			_target.targetFSM = (PlayMakerFSM)EditorGUILayout.ObjectField("Fsm Target",_targetFsm,typeof(PlayMakerFSM),true,null);	
		}
		
		if (_targetFsm!=null)
		{
			EditorGUILayout.Separator();
		
			OnGUI_DrawNGuiEventImplementation(_targetFsm);
		}else{
			
			_target.targetFSM = (PlayMakerFSM)EditorGUILayout.ObjectField("Fsm Target",_targetFsm,typeof(PlayMakerFSM),true,null);
			 EditorGUI.indentLevel = -2;
			EditorGUILayout.HelpBox("No Fsm Found. Please select one or add one to this GameObject",MessageType.Error);
			
			/*
			GUI.color = PlayMakerPhotonEditorUtility.lightOrange;
			GUILayout.BeginHorizontal("","box",GUILayout.ExpandWidth(true));
				GUI.color = Color.white;
				GUILayout.Label("No Fsm Found. Please select one or add one to this GameObject");
			GUILayout.EndHorizontal();
			*/
			
			
			if (GUILayout.Button("Add Fsm to this GameObject"))
			{
				PlayMakerFSM _new = _target.gameObject.AddComponent<PlayMakerFSM>();
				_new.FsmName = _target.gameObject.name+" NGUI Events Receiver";
				_new.FsmDescription = "Implement the NGUI / XXX global events in this FSM to start getting feedback from NGUI";
			}
		}
		
		EditorGUIUtility.LookLikeControls();
	}
	
	public void OnGUI_DrawNGuiEventImplementation(PlayMakerFSM fsm)
	{
		NGuiEventsToPlaymakerFsmEvents _target = (NGuiEventsToPlaymakerFsmEvents)this.target;
		
		
		bool _noImplementation = true;
		
		foreach (NGuiPlayMakerDelegates _value in Enum.GetValues(typeof(NGuiPlayMakerDelegates)))
		{
			
			string _fsmEvent = NGuiPlayMakerProxy.GetFsmEventEnumValue(_value);
			bool _customEvent = false;
			int _customEventIndex = 0;
			
			// check if we use default event or custom event
			if (_target.customEventsKeys!=null && _target.customEventsKeys.Contains(_value))
			{
				_customEventIndex = _target.customEventsKeys.IndexOf(_value);
				_customEvent = true;
				_fsmEvent = _target.customEventsValues[_customEventIndex];
			}
			
			bool _isImplemented = _target.DoesTargetImplementsEvent(fsm,_fsmEvent);
			
			
			
			
			
			int _counter = _target.getUsage(_value);
		//	if (Application.isPlaying)
		//	{
				//_fsmEvent  += " "+_target.getUsage(_value);
		//	}
			
			string _feedback = "Not implemented";
			Color _color = Color.white;
			
			if (_isImplemented)
			{
				_noImplementation = false;
				_feedback = "Used";
				_color = Color.green;
			}else{ 
				
				if (_target.DoesTargetMissEventImplementation(fsm,_fsmEvent))
				{
					_color = new Color(255,178,102) ; // PlayMakerPhotonEditorUtility.lightOrange;
					_feedback = "Not used";
				}
			
				
			}
			
			if (_counter>0)
			{
				_feedback += " "+_counter.ToString();		
			}
			
			if ( !_target.OnlyShowImplemented || (_target.OnlyShowImplemented && _isImplemented) )
			{
				GUI.color = _color;
				GUILayout.BeginVertical("","box",GUILayout.ExpandWidth(true));
				
					GUI.color = Color.white;
				
				if (_customEvent)
				{
				
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
							if (GUILayout.Button("reset",GUILayout.Width(40)) )
							{
								if (_target.customEventsKeys!=null)
								{
									_customEventIndex = _target.customEventsKeys.IndexOf(_value);
									_target.customEventsKeys.RemoveAt(_customEventIndex);
									_target.customEventsValues.RemoveAt(_customEventIndex);
								}
								return;
							}
		

						GUILayout.Label(_value.ToString());
					
				}else{
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
					if (GUILayout.Button("edit",GUILayout.Width(40)) )
					{
						if (_target.customEventsKeys==null)
						{
							_target.customEventsKeys = new List<NGuiPlayMakerDelegates>();
							_target.customEventsValues = new List<string>();
						}
						
						_target.customEventsKeys.Add(_value);
						_customEventIndex = _target.customEventsKeys.IndexOf(_value);
						
						_target.customEventsValues.Add(("MY "+_value+" EVENT").ToUpper());
						return;
					}
					
					GUILayout.Label(_fsmEvent);
					//EditorGUILayout.LabelField(_fsmEvent,_feedback);
				}
				
				GUILayout.Label(_feedback,GUILayout.Width(100));
				
				GUILayout.EndHorizontal();
				
				if (_customEvent)
				{
				_target.customEventsValues[_customEventIndex] = GUILayout.TextField(_fsmEvent,GUILayout.MinHeight(18),GUILayout.ExpandWidth(true));// EditorGUILayout.TextField(_fsmEvent,GUILayout.MaxWidth(200));
				}
				
				GUILayout.EndVertical();
				
				
			}
			
		}
		
			
			if (_noImplementation)
			{
				EditorGUI.indentLevel = -2;
				EditorGUILayout.HelpBox("The Fsm Target does not implement any NGUI Events. Edit your Fsm to add Global Transitions or State Transitions from 'Custom Events/NGUI'",MessageType.Error);
			}
	}
}