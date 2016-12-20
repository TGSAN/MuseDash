////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;


#if UNITY_4_6 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
#else
using UnityEngine.SceneManagement;
#endif


public class BaseIOSFeaturePreview : MonoBehaviour {

	protected GUIStyle style;

	protected int buttonWidth = 200;
	protected int buttonHeight = 50;
	protected float StartY = 20;
	protected float StartX = 10;



	protected float XStartPos = 10;
	protected float YStartPos = 10;

	protected float XButtonStep = 220;
	protected float YButtonStep = 60;

	protected float YLableStep = 40;
	

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	protected virtual void InitStyles () {
		style =  new GUIStyle();
		style.normal.textColor = Color.white;
		style.fontSize = 16;
		style.fontStyle = FontStyle.BoldAndItalic;
		style.alignment = TextAnchor.UpperLeft;
		style.wordWrap = true;
		
	}


	public virtual void Start() {
		InitStyles();
	}

	public void UpdateToStartPos() {
		StartY = YStartPos;
		StartX = XStartPos;
	}

	public void LoadLevel(string levelName) {
		#if UNITY_4_6 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
		Application.LoadLevel(levelName);
		#else
		SceneManager.LoadScene(levelName);
		#endif
	}
}

