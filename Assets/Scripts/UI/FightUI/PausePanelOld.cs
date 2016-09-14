using UnityEngine;
using System.Collections;
using FormulaBase;
using GameLogic;

public class PausePanelOld : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	//void Update () {
	//
	//}

	public void ClickExitButton()
	{
		this.gameObject.SetActive (false);
		StageBattleComponent.Instance.Exit ();
		
		GameObject uimanage=GameObject.Find("UIManage(Clone)");
		if(uimanage!=null)
		{
			uimanage.GetComponent<UIManage>().SetUIState(UIState.UISTATE_FBEXIT);
		}
	}

	public void ClickRestartButton()
	{
		this.gameObject.SetActive (false);
		StageBattleComponent.Instance.ReEnter ();
	}

	public void ClickResume()
	{
		this.gameObject.SetActive (false);
		Time.timeScale = GameGlobal.TIME_SCALE;
		AudioManager.Instance.ResumeBackGroundMusic ();
		AudioManager.Instance.SetBackGroundMusicTimeScale (GameGlobal.TIME_SCALE);
	}
}
