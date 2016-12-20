using UnityEngine;
using System.IO;
using System.Collections.Generic;


#if UNITY_EDITOR
using UnityEditor;
[InitializeOnLoad]
#endif

public class ISDSettings : ScriptableObject
{

	public const string VERSION_NUMBER = "1.8";

	public bool IsfwSettingOpen;
	public bool IsLibSettingOpen;
	public bool IslinkerSettingOpne;
	public bool IscompilerSettingsOpen;
	public bool IsPlistSettingsOpen;
	public bool IsLanguageSettingOpen = true;

	public List<ISD_Framework> Frameworks =  new List<ISD_Framework>();
	public List<ISD_Lib> Libraries =  new List<ISD_Lib>();





	public List<string> compileFlags =  new List<string>();
	public List<string> linkFlags =  new List<string>();


	public List<ISD_Variable>  PlistVariables =  new List<ISD_Variable>();

	public List<string> langFolders = new List<string>();


	private const string ISDAssetPath = "Extensions/IOSDeploy/Resources";
	private const string ISDAssetName = "ISDSettingsResource";
	private const string ISDAssetExtension = ".asset";

	private static ISDSettings instance;

	

	public static ISDSettings Instance
	{
		get
		{
			if(instance == null)
			{
				instance = Resources.Load(ISDAssetName) as ISDSettings;
				if(instance == null)
				{
					instance = CreateInstance<ISDSettings>();
					#if UNITY_EDITOR


					FileStaticAPI.CreateFolder(ISDAssetPath);
					
					string fullPath = Path.Combine(Path.Combine("Assets", ISDAssetPath), ISDAssetName + ISDAssetExtension );
					
					AssetDatabase.CreateAsset(instance, fullPath);
					#endif

				}
			}
			return instance;
		}
	}



	public bool ContainsFreamworkWithName(string name) {
		foreach(ISD_Framework f in ISDSettings.Instance.Frameworks) {
			if(f.Name.Equals(name)) {
				return true;
			}
		}
		
		return false;
	}

	public bool ContainsPlistVarkWithName(string name) {
		foreach(ISD_Variable var in ISDSettings.Instance.PlistVariables) {
			if(var.Name.Equals(name)) {
				return true;
			}
		}
		
		return false;
	}
	
	
	public bool ContainsLibWithName(string name) {
		foreach(ISD_Lib l in ISDSettings.Instance.Libraries) {
			if(l.Name.Equals(name)) {
				return true;
			}
		}
		
		return false;
	}


}
