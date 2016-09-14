using UnityEngine;
using UnityEditor;
using System.Collections;

public class EngineWindowEditor :Editor
{
	[MenuItem("RHY/场景选择/AdminScene")]
	static void EngineEditorLevelEditor1()
	{
		EditorApplication.OpenScene( "Assets/Scenes/AdminScene.unity");
	}
	
	[MenuItem("RHY/场景选择/战斗场景")]
	static void EngineEditorLevelEditor2()
	{
		EditorApplication.OpenScene( "Assets/Scenes/GameScene.unity");
	}
	
	
	[MenuItem("RHY/场景选择/主界面场景")]
	static void EngineEditorLevelEditor3()
	{
		EditorApplication.OpenScene( "Assets/Scenes/ChooseSongs.unity");
	}

	[MenuItem("RHY/场景选择/加载场景")]
	static void EngineEditorLevelEditor4()
	{
		EditorApplication.OpenScene( "Assets/Scenes/LoadingScene.unity");
	}

	[MenuItem("RHY/场景选择/登陆场景")]
	static void EngineEditorLevelEditor5()
	{
		EditorApplication.OpenScene( "Assets/Scenes/Welcome.unity");
	}

	[MenuItem("RHY/场景选择/热更新场景")]
	static void EngineEditorLevelEditor6()
	{
		EditorApplication.OpenScene( "Assets/Scenes/UpdateScene.unity");
	}
}