#define DISABLED

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Reflection;
using System;

[CustomEditor(typeof(ISDSettings))]
public class ISDSettingsEditor : Editor {

	
	GUIContent SdkVersion   = new GUIContent("Plugin Version [?]", "This is Plugin version.  If you have problems or compliments please include this so we know exactly what version to look out for.");
	GUIContent SupportEmail = new GUIContent("Support [?]", "If you have any technical quastion, feel free to drop an e-mail");
	



	public override void OnInspectorGUI () {


		GUI.changed = false;




		EditorGUILayout.LabelField("IOS Deploy Settings", EditorStyles.boldLabel);
		EditorGUILayout.Space();


		#if DISABLED
		GUI.enabled = false;
		#endif

		Frameworks();
		EditorGUILayout.Space();
		Library ();
		EditorGUILayout.Space();
		LinkerFlags();
		EditorGUILayout.Space();
		CompilerFlags();
		EditorGUILayout.Space();
		PlistValues ();
		EditorGUILayout.Space();
		LanguageValues();
		EditorGUILayout.Space();
		AboutGUI();

		if(GUI.changed) {
			DirtyEditor();
		}

	}


	private string newFreamwork = string.Empty;
	private void Frameworks() {
		

		ISDSettings.Instance.IsfwSettingOpen = EditorGUILayout.Foldout(ISDSettings.Instance.IsfwSettingOpen, "Frameworks");

		if(ISDSettings.Instance.IsfwSettingOpen) {
			if (ISDSettings.Instance.Frameworks.Count == 0) {

				EditorGUILayout.HelpBox("No Frameworks added", MessageType.None);
			}

			EditorGUI.indentLevel++; {

	
				foreach(ISD_Framework framework in ISDSettings.Instance.Frameworks) {
					

					EditorGUILayout.BeginVertical (GUI.skin.box);

					EditorGUILayout.BeginHorizontal();
					framework.IsOpen = EditorGUILayout.Foldout(framework.IsOpen, framework.Name);
					
					if(framework.IsOptional) {
						EditorGUILayout.LabelField("(Optional)");
					}

					
					bool ItemWasRemoved = DrawSrotingButtons((object) framework, ISDSettings.Instance.Frameworks);
					if(ItemWasRemoved) {
						return;
					}

					EditorGUILayout.EndHorizontal();

					if(framework.IsOpen) {

						EditorGUI.indentLevel++; {

							EditorGUILayout.BeginHorizontal();
							EditorGUILayout.LabelField("Optional");
							framework.IsOptional = EditorGUILayout.Toggle (framework.IsOptional);
							EditorGUILayout.EndHorizontal();
		

						}EditorGUI.indentLevel--;
					}

					EditorGUILayout.EndVertical ();
				}

					

			}EditorGUI.indentLevel--;
				

				
		
			EditorGUILayout.Space();

			EditorGUILayout.BeginVertical (GUI.skin.box);

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Add New Framework", GUILayout.Width(120));
			newFreamwork = EditorGUILayout.TextField(newFreamwork);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.EndVertical ();



			EditorGUILayout.BeginHorizontal();
			
			EditorGUILayout.Space();

			if(GUILayout.Button("Add",  GUILayout.Width(100))) {
				if(!ISDSettings.Instance.ContainsFreamworkWithName(newFreamwork) && newFreamwork.Length > 0) {
					ISD_Framework f =  new ISD_Framework();
					f.Name = newFreamwork;
					ISDSettings.Instance.Frameworks.Add(f);

					newFreamwork = string.Empty;
				}
				
			}
			EditorGUILayout.EndHorizontal();
		}
	}

	private string NewLibrary = string.Empty;
	void Library () {
		ISDSettings.Instance.IsLibSettingOpen = EditorGUILayout.Foldout(ISDSettings.Instance.IsLibSettingOpen, "Libraries");

		if(ISDSettings.Instance.IsLibSettingOpen){
			if (ISDSettings.Instance.Libraries.Count == 0) {
				EditorGUILayout.HelpBox("No Libraries added", MessageType.None);
			}

			EditorGUI.indentLevel++; {

				foreach(ISD_Lib lib in ISDSettings.Instance.Libraries) {
					
					
					EditorGUILayout.BeginVertical (GUI.skin.box);
					
					EditorGUILayout.BeginHorizontal();
					lib.IsOpen = EditorGUILayout.Foldout(lib.IsOpen, lib.Name);
					if(lib.IsOptional) {
						EditorGUILayout.LabelField("(Optional)");
					}
		
					bool ItemWasRemoved = DrawSrotingButtons((object) lib, ISDSettings.Instance.Libraries);
					if(ItemWasRemoved) {
						return;
					}
					

					
					EditorGUILayout.EndHorizontal();

					if(lib.IsOpen) {
						
						EditorGUI.indentLevel++; {
							
							EditorGUILayout.BeginHorizontal();
							EditorGUILayout.LabelField("Optional");
							lib.IsOptional = EditorGUILayout.Toggle (lib.IsOptional);
							EditorGUILayout.EndHorizontal();
							
							
						}EditorGUI.indentLevel--;
					}

					EditorGUILayout.EndVertical ();
					
				}
			}EditorGUI.indentLevel--;
			
			EditorGUILayout.Space();

			EditorGUILayout.BeginVertical (GUI.skin.box);
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Add New Library", GUILayout.Width(120));
			NewLibrary = EditorGUILayout.TextField(NewLibrary);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndVertical ();
			


			EditorGUILayout.BeginHorizontal();

			EditorGUILayout.Space();
			if(GUILayout.Button("Add",  GUILayout.Width(100))) {
				if(!ISDSettings.Instance.ContainsLibWithName(NewLibrary) && NewLibrary.Length > 0 ) {
					ISD_Lib lib = new ISD_Lib();
					lib.Name = NewLibrary;
					ISDSettings.Instance.Libraries.Add(lib);
					NewLibrary = string.Empty;
				}
			}
			EditorGUILayout.EndHorizontal();
		}
	}

	private string NewLinkerFlag = string.Empty;
	private void LinkerFlags() {
		
		
		ISDSettings.Instance.IslinkerSettingOpne = EditorGUILayout.Foldout(ISDSettings.Instance.IslinkerSettingOpne, "Linker Flags");
		
		if(ISDSettings.Instance.IslinkerSettingOpne) {
			if (ISDSettings.Instance.linkFlags.Count == 0) {
				
				EditorGUILayout.HelpBox("No Linker Flags added", MessageType.None);
			}
			

			foreach(string flasg in ISDSettings.Instance.linkFlags) {
				
				
				EditorGUILayout.BeginVertical (GUI.skin.box);
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.SelectableLabel(flasg, GUILayout.Height(18));
				EditorGUILayout.Space();
				
				bool pressed  = GUILayout.Button("x",  EditorStyles.miniButton, GUILayout.Width(20));
				if(pressed) {
					ISDSettings.Instance.linkFlags.Remove(flasg);
					return;
				}
				
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.EndVertical ();
				
			}

			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();
			
			EditorGUILayout.LabelField("Add New Flag");
			NewLinkerFlag = EditorGUILayout.TextField(NewLinkerFlag, GUILayout.Width(200));
			EditorGUILayout.EndHorizontal();
			
			
			
			
			
			EditorGUILayout.BeginHorizontal();
			
			EditorGUILayout.Space();
			
			if(GUILayout.Button("Add",  GUILayout.Width(100))) {
				if(!ISDSettings.Instance.linkFlags.Contains(NewLinkerFlag) && NewLinkerFlag.Length > 0) {
					ISDSettings.Instance.linkFlags.Add(NewLinkerFlag);
					NewLinkerFlag = string.Empty;
				}
				
			}
			EditorGUILayout.EndHorizontal();
		}
	}


	private string NewCompilerFlag = string.Empty;
	private void CompilerFlags() {
		
		
		ISDSettings.Instance.IscompilerSettingsOpen = EditorGUILayout.Foldout(ISDSettings.Instance.IscompilerSettingsOpen, "Compiler Flags");
		
		if(ISDSettings.Instance.IscompilerSettingsOpen) {
			if (ISDSettings.Instance.compileFlags.Count == 0) {
				EditorGUILayout.HelpBox("No Linker Flags added", MessageType.None);
			}

			foreach(string flasg in ISDSettings.Instance.compileFlags) {
				
				
				EditorGUILayout.BeginVertical (GUI.skin.box);
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.SelectableLabel(flasg, GUILayout.Height(18));
				
				EditorGUILayout.Space();
				
				bool pressed  = GUILayout.Button("x",  EditorStyles.miniButton, GUILayout.Width(20));
				if(pressed) {
					ISDSettings.Instance.compileFlags.Remove(flasg);
					return;
				}
				
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical ();
				
			}

			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();
			
			EditorGUILayout.LabelField("Add New Flag");
			NewCompilerFlag = EditorGUILayout.TextField(NewCompilerFlag, GUILayout.Width(200));
			EditorGUILayout.EndHorizontal();

			
			EditorGUILayout.BeginHorizontal();
			
			EditorGUILayout.Space();
			
			if(GUILayout.Button("Add",  GUILayout.Width(100))) {
				if(!ISDSettings.Instance.compileFlags.Contains(NewCompilerFlag) && NewCompilerFlag.Length > 0) {
					ISDSettings.Instance.compileFlags.Add(NewCompilerFlag);
					NewCompilerFlag = string.Empty;
				}
				
			}
			EditorGUILayout.EndHorizontal();
		}
	}

	private string NewPlistValueName = string.Empty;

	void PlistValues ()
	{
		ISDSettings.Instance.IsPlistSettingsOpen = EditorGUILayout.Foldout(ISDSettings.Instance.IsPlistSettingsOpen, "Plist values");
		
		if(ISDSettings.Instance.IsPlistSettingsOpen) {
			if (ISDSettings.Instance.PlistVariables.Count == 0) {
				EditorGUILayout.HelpBox("No Plist values added", MessageType.None);
			}

			EditorGUI.indentLevel++; {
				
				
				foreach(ISD_Variable var in ISDSettings.Instance.PlistVariables) {
					
					
					EditorGUILayout.BeginVertical (GUI.skin.box);
					
					EditorGUILayout.BeginHorizontal();
					var.IsOpen = EditorGUILayout.Foldout(var.IsOpen, var.Name);
					

					EditorGUILayout.LabelField("(" + var.Type.ToString() +  ")");

					
					bool ItemWasRemoved = DrawSrotingButtons((object) var, ISDSettings.Instance.PlistVariables);
					if(ItemWasRemoved) {
						return;
					}
					
					EditorGUILayout.EndHorizontal();


					
					if(var.IsOpen) {
						
						EditorGUI.indentLevel++; {
							
							EditorGUILayout.BeginHorizontal();
							EditorGUILayout.LabelField("Type");
							var.Type = (ISD_PlistValueTypes) EditorGUILayout.EnumPopup (var.Type);
							EditorGUILayout.EndHorizontal();


							if(var.Type == ISD_PlistValueTypes.String || var.Type == ISD_PlistValueTypes.Integer || var.Type == ISD_PlistValueTypes.Float || var.Type == ISD_PlistValueTypes.Boolean) {

								EditorGUILayout.BeginHorizontal();
								EditorGUILayout.LabelField("Value");
								
								switch(var.Type) {
								case ISD_PlistValueTypes.Boolean:
									var.BooleanValue	 = EditorGUILayout.Toggle (var.BooleanValue);
									break;
									
								case ISD_PlistValueTypes.Float:
									var.FloatValue = EditorGUILayout.FloatField(var.FloatValue);
									break;
									
								case ISD_PlistValueTypes.Integer:
									var.IntegerValue = EditorGUILayout.IntField (var.IntegerValue);
									break;
									
								case ISD_PlistValueTypes.String:
									var.StringValue = EditorGUILayout.TextField (var.StringValue);
									break;
								}
								
								
								EditorGUILayout.EndHorizontal();
							}


							if(var.Type == ISD_PlistValueTypes.Array) {
								DrawArrayValues(var);
							}

							if(var.Type == ISD_PlistValueTypes.Dictionary) {
								DrawDicitionarryValues(var);
							}


						}EditorGUI.indentLevel--;
					}
					
					EditorGUILayout.EndVertical ();
				}

				EditorGUILayout.Space();

			
				
			}EditorGUI.indentLevel--;
			
	


			EditorGUILayout.BeginVertical (GUI.skin.box);
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("Add New Variable");
			NewPlistValueName = EditorGUILayout.TextField(NewPlistValueName);
			EditorGUILayout.EndHorizontal();
			



			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space();
			if(GUILayout.Button("Add",  GUILayout.Width(100))) {
				ISD_Variable var =  new ISD_Variable();
				var.Name = NewPlistValueName;
				ISDSettings.Instance.PlistVariables.Add(var);

				NewPlistValueName = string.Empty;

			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
			EditorGUILayout.EndVertical ();
		}
	}

	string newVarKey = string.Empty;

	void DrawDicitionarryValues (ISD_Variable var) {
		var.IsListOpen = EditorGUILayout.Foldout(var.IsListOpen, "Dictionary Values");
		
		//var.ArrayValue.co
		
		if(var.IsListOpen) {
			
			foreach(KeyValuePair<string, ISD_VariableListed> pair  in var.DictionaryValue) {

				EditorGUI.indentLevel++; {

				
					EditorGUILayout.BeginHorizontal();
					ISD_VariableListed v = pair.Value;


					v.IsOpen = EditorGUILayout.Foldout(v.IsOpen, v.DictKey);
					EditorGUILayout.LabelField("(" + var.Type.ToString() +  ")");
					bool removed = DrawSrotingButtons((object) v, var.ArrayValue);
					if(removed) {
						return;
					}



					EditorGUILayout.EndHorizontal();

					if(v.IsOpen) {
						EditorGUILayout.BeginHorizontal();
						v.Type = (ISD_PlistValueTypes) EditorGUILayout.EnumPopup (v.Type);
						DrawValueFiled(v);
						EditorGUILayout.EndHorizontal();
					}


				} EditorGUI.indentLevel--;
			}
			
			EditorGUILayout.BeginHorizontal();

			EditorGUILayout.LabelField("Add New Value With Key:");
			newVarKey = EditorGUILayout.TextField(newVarKey);

			EditorGUILayout.Space();
			if(GUILayout.Button("Add",  GUILayout.Width(100))) {
				if(newVarKey.Length > 0) {
					ISD_VariableListed newDictValue =  new ISD_VariableListed();
					newDictValue.DictKey = newVarKey;
					var.AddVarToDictionary(newDictValue);
				}
			}
			EditorGUILayout.EndHorizontal();
		}
		
		
		EditorGUILayout.Space();
	}

	void DrawArrayValues (ISD_Variable var) {

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Values Type");

		ISD_PlistValueTypes type = (ISD_PlistValueTypes) EditorGUILayout.EnumPopup (var.ArrayType);

		if(type == ISD_PlistValueTypes.Array ||type == ISD_PlistValueTypes.Dictionary) {
			type = var.ArrayType;
		}

		var.ArrayType = type;
		

		EditorGUILayout.EndHorizontal();

		var.IsListOpen = EditorGUILayout.Foldout(var.IsListOpen, "Array Values");




		
		//var.ArrayValue.co
		
		if(var.IsListOpen) {
			
			foreach(ISD_VariableListed v  in var.ArrayValue) {
				EditorGUILayout.BeginHorizontal();
				

				GUI.enabled = false;
				v.Type = (ISD_PlistValueTypes) EditorGUILayout.EnumPopup (var.ArrayType);
				GUI.enabled = true;

				DrawValueFiled(v);
			
				
				
				bool removed = DrawSrotingButtons((object) v, var.ArrayValue);
				if(removed) {
					return;
				}
				
				EditorGUILayout.EndHorizontal();
				
			}
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space();
			if(GUILayout.Button("Add New Value",  GUILayout.Width(100))) {
				var.ArrayValue.Add(new ISD_VariableListed());
			}
			EditorGUILayout.EndHorizontal();
		}
		
		
		EditorGUILayout.Space();
	}

	private void DrawValueFiled(ISD_VariableListed var, string caption = "") {


		if(var.Type == ISD_PlistValueTypes.Array || var.Type == ISD_PlistValueTypes.Dictionary) {
			var.Type = ISD_PlistValueTypes.String;
		}

		switch(var.Type) {
		case ISD_PlistValueTypes.Boolean:
			var.BooleanValue	 = EditorGUILayout.Toggle (var.BooleanValue);
			break;
			
		case ISD_PlistValueTypes.Float:
			var.FloatValue = EditorGUILayout.FloatField("Test", var.FloatValue);
			break;
			
		case ISD_PlistValueTypes.Integer:
			var.IntegerValue = EditorGUILayout.IntField (var.IntegerValue);
			break;
			
		case ISD_PlistValueTypes.String:
			var.StringValue = EditorGUILayout.TextField (var.StringValue);
			break;
		}


	}


	private void AboutGUI() {
		GUI.enabled = true;
		EditorGUILayout.HelpBox("About the Plugin", MessageType.None);
		EditorGUILayout.Space();
		
		SelectableLabelField(SdkVersion, ISDSettings.VERSION_NUMBER);
		SelectableLabelField(SupportEmail, "support@stansassets.com");


		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Note: This version of IOS Deploy designed for Stan's Assets");
		EditorGUILayout.LabelField("plugins internal use only. If you want to use IOS Deploy  ");
		EditorGUILayout.LabelField("for your project needs, please, ");
		EditorGUILayout.LabelField("purchase a copy of IOS Deploy plugin.");
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.Space();
		
		if(GUILayout.Button("Documentation",  GUILayout.Width(150))) {
			Application.OpenURL("https://goo.gl/sOJFXJ");
		}
		
		if(GUILayout.Button("Purchase",  GUILayout.Width(150))) {
			Application.OpenURL("https://goo.gl/Nqbuuv");
		}
		
		EditorGUILayout.EndHorizontal();
	}
	
	private void SelectableLabelField(GUIContent label, string value) {
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(label, GUILayout.Width(180), GUILayout.Height(16));
		EditorGUILayout.SelectableLabel(value, GUILayout.Height(16));
		EditorGUILayout.EndHorizontal();
	}




	private bool DrawSrotingButtons(object currentObject, IList ObjectsList) {
		
		int ObjectIndex = ObjectsList.IndexOf(currentObject);
		if(ObjectIndex == 0) {
			GUI.enabled = false;
		} 
		
		bool up 		= GUILayout.Button("↑", EditorStyles.miniButtonLeft, GUILayout.Width(20));
		if(up) {
			object c = currentObject;
			ObjectsList[ObjectIndex]  		= ObjectsList[ObjectIndex - 1];
			ObjectsList[ObjectIndex - 1] 	=  c;
		}
		
		
		if(ObjectIndex >= ObjectsList.Count -1) {
			GUI.enabled = false;
		} else {
			GUI.enabled = true;
		}
		
		bool down 		= GUILayout.Button("↓", EditorStyles.miniButtonMid, GUILayout.Width(20));
		if(down) {
			object c = currentObject;
			ObjectsList[ObjectIndex] =  ObjectsList[ObjectIndex + 1];
			ObjectsList[ObjectIndex + 1] = c;
		}
		
		
		GUI.enabled = true;
		bool r 			= GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(20));
		if(r) {
			ObjectsList.Remove(currentObject);
		}
		
		return r;
	}

	private string NewLanguage = string.Empty;
	private void LanguageValues ()
	{
		ISDSettings.Instance.IsLanguageSettingOpen = EditorGUILayout.Foldout(ISDSettings.Instance.IsLanguageSettingOpen, "Languages");
		
		if(ISDSettings.Instance.IsLanguageSettingOpen)
		{
			if (ISDSettings.Instance.langFolders.Count == 0)
			{
				EditorGUILayout.HelpBox("No Languages added", MessageType.None);
			}

			foreach(string lang in ISDSettings.Instance.langFolders)
			{
				EditorGUILayout.BeginVertical (GUI.skin.box);
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.SelectableLabel(lang, GUILayout.Height(18));
				EditorGUILayout.Space();
				
				bool pressed  = GUILayout.Button("x",  EditorStyles.miniButton, GUILayout.Width(20));
				if(pressed)
				{
					ISDSettings.Instance.langFolders.Remove(lang);
					return;
				}
				
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical ();
			}
			
			EditorGUILayout.Space();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Add New Language");
			NewLanguage = EditorGUILayout.TextField(NewLanguage, GUILayout.Width(200));
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			
			EditorGUILayout.Space();
			
			if(GUILayout.Button("Add",  GUILayout.Width(100)))
			{
				if(!ISDSettings.Instance.langFolders.Contains(NewLanguage) && NewLanguage.Length > 0)
				{
					ISDSettings.Instance.langFolders.Add(NewLanguage);
					NewLanguage = string.Empty;
				}
				
			}
			EditorGUILayout.EndHorizontal();
		}
	}

	private static void DirtyEditor() {
			#if UNITY_EDITOR
		EditorUtility.SetDirty(ISDSettings.Instance);
			#endif
	}
}
