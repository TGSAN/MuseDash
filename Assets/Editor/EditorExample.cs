using System;
using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;

public class EditorExample : EditorWindow {

	public Texture TxTexture;
	string m_string = "<Insert>";
	string description = "<Insert>";
	public Color color;
	GameObject m_GameObject;

	public int toolbarInt = 0;
	public string[] toolbarStrings = new string[] {"1", "2", "3"};

	EditorExample () {
		this.titleContent=new GUIContent("Editor Name");// 标签名字
	}


	[MenuItem("m_Tool/EditorExample")]// 菜单位置
	static void showWindow () {
		EditorWindow.GetWindow(typeof (EditorExample));
	}

	// 绘制窗口界面
	//----------------------------------------------------------------------------
	void OnGUI () {
		GUILayout.BeginVertical();

		EditorGUILayout.Space();
		EditorGUILayout.Space();

		// 基本信息
		//---------------------------------------------------------------------------------------------------------------

		// 标签
		GUILayout.BeginHorizontal();
		GUI.skin.label.fontSize = 14;// 字号
		GUI.skin.label.alignment = TextAnchor.UpperLeft;// 对齐位置
		GUILayout.Label("Label");// 文字内容
		GUILayout.EndHorizontal();

		// GameObject
		m_GameObject = (GameObject) EditorGUILayout.ObjectField("Game Object", m_GameObject, typeof (GameObject), true);

		// 颜色选择
		color =(Color) EditorGUILayout.ColorField("Color", color);
		
		// 单行文本输入
		m_string = EditorGUILayout.TextField("文本输入", m_string);// 文本输入

		EditorGUILayout.Space();
		EditorGUILayout.Space();

		// 多行文本输入
		GUILayout.Label("label");
		description = EditorGUILayout.TextArea(description, GUILayout.MaxHeight(50));

		// 按钮类
		//------------------------------------------------------------------------------------------------------------------

		GUILayout.BeginHorizontal();
		EditorGUILayout.Space();
		if (GUILayout.Button("<")) {
		}

		toolbarInt = GUILayout.Toolbar(toolbarInt, toolbarStrings);

		if (GUILayout.Button(">")) {
		}
		GUILayout.EndHorizontal();

		// 保存Bug信息按钮
		EditorGUILayout.Space();
		if (GUILayout.Button("Save Bug")) {
			SaveBug();
		}

		// 保存Bug信息和截图按钮
		if (GUILayout.Button("Save Bug With Screenshoot")) {
			SaveBugWithScreeshot();
		}

		// 其它
		//----------------------------------------------------------------------------------------------------
		GUI.skin.label.fontSize = 12;// 字号
		GUI.skin.label.alignment = TextAnchor.UpperLeft;// 对齐位置

		EditorGUILayout.Space ();
		GUILayout.Label("这里是Editor的使用说明。妈的智障这文字要是多起来怎么换行啊。");
		EditorGUILayout.Space ();
		GUILayout.Label("Currently Scene:"+EditorSceneManager.GetActiveScene().name);// 获取当前场景名称
		GUILayout.Label("Time:"+System.DateTime.Now);// 获取系统时间

		toolbarInt = GUILayout.Toolbar(toolbarInt, toolbarStrings);

		EditorGUILayout.EndVertical();//布局开始和结束相对应，缺少时可能出现窗口中的元素无法自适应的情况
	}


	// 方法
	//------------------------------------------------------------------------------------------------------------
	private void SaveBugWithScreeshot()
	{
		Writer();
		Application.CaptureScreenshot("Assets/BugReports/" + m_string + "/" + m_string + ".png");
	}

	private void SaveBug()
	{
		Writer();
	}

	//IO类，用来写入保存信息
	void Writer()
	{
		Directory.CreateDirectory("Assets/BugReports/" + m_string);
		StreamWriter sw = new StreamWriter("Assets/BugReports/" + m_string + "/" + m_string + ".txt");
		sw.WriteLine(m_string);
		sw.WriteLine(DateTime.Now.ToString());
		sw.WriteLine(EditorSceneManager.GetActiveScene().name);
		sw.WriteLine(description);
		sw.Close();
	}
}