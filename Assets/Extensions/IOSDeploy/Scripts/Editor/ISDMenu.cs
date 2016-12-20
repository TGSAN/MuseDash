using UnityEngine;
using System.Collections;
using UnityEditor;

public class ISDMenu : EditorWindow
{
#if UNITY_EDITOR
	[MenuItem("Window/Stan's Assets/IOS Deploy/Edit Settings")]
	public static void Edit() {
		Selection.activeObject = ISDSettings.Instance;
	}

	
	[MenuItem("Window/Stan's Assets/IOS Deploy/Documentation/Introduction")]
	public static void ISDSetupPluginSetUp() {
		string url = "https://unionassets.com/ios-deploy/introduction-92";
		Application.OpenURL(url);
	}
#endif
}
