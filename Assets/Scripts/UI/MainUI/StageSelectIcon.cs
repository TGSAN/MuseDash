using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using GameLogic;

public class StageSelectIcon : MonoBehaviour {
	//[SerializeField]
	public uint stageId; //from 0
	public UILabel m_label;
	public UILabel m_SongName;


	//bool back=true;
	public  void FinishTween()
	{
			
		//	transform.GetComponent<UITweener>().PlayReverse();

	}
	public void InitStageSelectIcon()
	{
		m_label.text=(stageId/5+1).ToString()+"_"+(stageId%5+1).ToString();

		//Debug.Log("Song is ID"+stageId);

//		GameGlobal.gStage.LoadStageDataById (stageId);
//
		//		object stgCfg = StageConfigReader.GetStageConfig ((int)stageId);
//		if (stgCfg != null) {
//			StageData stageData = (StageData)stgCfg;
//			m_SongName.text = stageData.name;
//			//sceneDes = stageData.des;
//		} else {
//			m_SongName.text=ChoseSongPanel.DEFAULT_GAME_SCENE;
//		}

	}
	// Use this for initialization
	void Start () {
//		Button selectButton = this.transform.Find ("Cover").GetComponent<Button> ();
//		selectButton.onClick.AddListener (() => {
//			SongInfoPanel.Instance.ShowUi (this.stageId);
//		});
	}
}
