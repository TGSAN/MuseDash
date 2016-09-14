using UnityEngine;
using System.Collections;
using GameLogic;
using FormulaBase;

public class ChoseSongPanel : MonoBehaviour {
	public static string DEFAULT_GAME_SCENE = "No Song";
	public UILabel m_textNum ;
	public UILabel m_textName ;
	public UILabel m_textScore ;
	public UILabel m_textDes ;
	public UILabel m_textRate;

	uint m_songId;
	int difficultSelected = GameGlobal.DIFF_LEVEL_NORMAL;
	public void InitChoseSongPanel(uint _id)
	{
		m_songId = _id;
		this.difficultSelected = GameGlobal.DIFF_LEVEL_NORMAL;
		this.gameObject.SetActive (true);
		this.ResetPanel ();
	}

	public void EnterBattle()
	{

		//Application.LoadLevel(3);
//		ChoseSongPanelAnimaton.Rebind();
//		ChoseSongPanelAnimaton.Play("ChoseSongPanel_Out");

		//StageBattleComponent.Instance.Enter (m_songId + 1, (uint)this.difficultSelected);

//		temp.Rebind();
//		temp.ResetTrigger("ChoseSongPanel_In");
//		temp.Play("ChoseSongPanel_In");
	}

	Animator ChoseSongPanelAnimaton;
	// Use this for initialization
	void Start () {

//		Debug.Log("Time.timScale:"+Time.timeScale);//	Time.timeScale=1f;
	//	Time.timeScale=1f;
	}
	void OnDisable()
	{
		ChoseSongPanelAnimaton.Stop();
		ChoseSongPanelAnimaton.SetBool("Stand",false);
		ChoseSongPanelAnimaton.SetBool("In",false);
	}
	//void One
	public void ClosePanel()
	{
	//	this.gameObject.SetActive(false);

		//ChoseSongPanelAnimaton.SetBool("Stand",false);


		//ChoseSongPanelAnimaton.SetBool("Exit",true);
		//PanelAn = ChoseSongPanelAnimaton.GetBehaviour <tttttttt> ();  

		// Set the StateMachineBehaviour's reference to an ExampleMonoBehaviour to this.  
		//PanelAn.m_ob = this.gameObject;  
		//ChoseSongPanelAnimaton
		//asdasd(()=>{})
	//	RelayCommand(() => this.AddPerson(), () => this.CanAddPerson());

//		ChoseSongPanelAnimaton.GetBehaviour<tttttttt>().OnStateExit(});
	//	PanelAn.Stop();
		//()=>{this.gameObject.SetActive(false);
	}

	public void OnEnable()
	{
		if(ChoseSongPanelAnimaton==null)
		{
			ChoseSongPanelAnimaton = this.transform.Find ("ChoseSongPanelAnimaton").gameObject.GetComponent<Animator> ();
		}
		ChoseSongPanelAnimaton.SetBool("In",true);
		ChoseSongPanelAnimaton.SetBool("Stand",true);
	//	ChoseSongPanelAnimaton.Play("ChoseSongPanel_In");
	}
	// Update is called once per frame
	void Update () {
	
	}

	public void DifficultSelectNormal() {
		this.difficultSelected = GameGlobal.DIFF_LEVEL_NORMAL;
		this.ResetPanel ();
	}

	public void DifficultSelectHard() {
		this.difficultSelected = GameGlobal.DIFF_LEVEL_HARD;
		this.ResetPanel ();
	}

	public void DifficultSelectSuper() {
		this.difficultSelected = GameGlobal.DIFF_LEVEL_SUPER;
		this.ResetPanel ();
	}

	private void ResetPanel() {
		//m_name.text=
		string sceneDes = "";
		string sceneName = DEFAULT_GAME_SCENE;
		uint cid = this.m_songId + 1;
		FormulaBase.StageBattleComponent.Instance.SetStageId (cid);
		//JudgeLevelData levelData = GameGlobal.gStage.GetStageJudgeLevelBySaveData ();
		
		m_textNum.text = cid.ToString ();
		m_textName.text = FormulaBase.StageBattleComponent.Instance.GetStageName();
		//m_textRate.text = levelData.GetFinRate ().ToString () + "%";
		m_textScore.text = FormulaBase.StageBattleComponent.Instance.GetSorceBest ().ToString ();
		m_textDes.text = sceneDes;
	}
}
