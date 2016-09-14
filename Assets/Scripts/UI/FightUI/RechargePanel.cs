using UnityEngine;
using System.Collections;
using DYUnityLib;
using GameLogic;

public class RechargePanel : MonoBehaviour {

//	// Use this for initialization
//	void Start () {
//	
//	}
//	
//	// Update is called once per frame
//	void Update () {
//	
//	}
	public  GameObject[] equipArr=new GameObject[4];
	public UILabel m_Score;
	public UISlider m_ScoreSlider;
	private GameObject mainLayer = null;

	private const decimal CD_MAX = 0.5m;
	private decimal countDown;
	private Animator animator = null;

	private static RechargePanel instance = null;
	public static RechargePanel Instance {
		get {
			return instance;
		}
	}

	void Start() {
		instance = this;
		this.mainLayer = this.gameObject;
		this.mainLayer.SetActive (false);
		this.animator = this.transform.Find ("Recharge1Animation").gameObject.GetComponent<Animator> ();
		this.animator.Stop ();

	//	SetScoreBarValue(1f);
	//	SetScroeNb(456462316);
	}

	void Update() {
		if (this.mainLayer == null) {
			return;
		}

		if (!this.mainLayer.activeSelf) {
			return;
		}

		this.countDown -= FixUpdateTimer.dInterval;
		if (this.countDown < 0) {
			this.mainLayer.SetActive (false);
			RechargeScorePanel.Instance.ShowUi ();
		}
		PlayAnimation();
	}
	void PlayAnimation()
	{
		if(SetScoreBarAnimation)      //播放进度Bar动画
		{
			PlaySetScoreBarAnimation();
		}
		if(SetScoreNumberAnimation)
		{
			playSetScoreAnimation();
		}
	}
	public void ShowUi() {
		if (this.mainLayer == null) {
			return;
		}
		
		if (this.mainLayer.activeSelf) {
			return;
		}
		
		if (Time.timeScale > 0) {
			// Time.timeScale = 0;
			AudioManager.Instance.PauseBackGroundMusic ();
		}

		FightMenuPanel.Instance.UnShow ();

		this.countDown = CD_MAX;
		this.mainLayer.SetActive (true);
		//JudgeScoreData judgeData = GameGlobal.gStage.GetStageJudgeScoreData ();
		//this.SetScroeNb ((int)judgeData.score);
		this.animator.Stop ();
		this.animator.Rebind ();
		this.animator.Play ("Recharge1Panel_In");
	}
	#region SetEquip
	/// <summary>
	/// 设置装备属性
	/// </summary>
	void SetEquip()
	{
		//		for(int i=0; i<4;i++)
		//		{
		//			//if()
		//		}
	}
			
	#endregion
	#region 设置分数值
	int DesScoreNumber=0;
	bool SetScoreNumberAnimation=false;
	float SetScroeNbBeginTime=0f;
	float SetScoreNbEndTime=0f;
	string NowString;
	string DesString;
	/// <summary>
	/// 设置分数显示
	/// </summary>
	/// <param name="m_Nb">M_ nb.</param>
	public void SetScroeNb(int m_Nb)
	{
		SetScoreNumberAnimation=true;
		DesScoreNumber=m_Nb;
		SetScroeNbBeginTime=RealTime.time;
		DesString=DesScoreNumber.ToString();
		NowString=new string('1',DesString.Length);
	//	m_Score.text = m_Nb.ToString ();
	}

	void playSetScoreAnimation()
	{

		if(NowString==DesScoreNumber.ToString())
		{
			//分数已经到达目标分数
			SetScoreNumberAnimation=false;
			return;
		}
		
		SetScoreNbEndTime=RealTime.time;
		if(SetScoreNbEndTime-SetScroeNbBeginTime<0.06f)
		{
			return;
		}
		SetScroeNbBeginTime=RealTime.time;

		for(int i=0;i<NowString.Length;i++)
		{
			if(DesString[i]!=NowString[i])
			{
				NowString=ReplaceChar(NowString,i);
			}
		}
		m_Score.text =NowString;
	}

	static string ReplaceChar(string str, int index)
	{
		char[] carr = str.ToCharArray();
		int temp=(int)str[index];
		temp++;
		if(temp>=58)
			temp=48;
		carr[index]=(char)temp;
		return new string(carr);
	}
	#endregion
	#region 设置分数进度条
	bool SetScoreBarAnimation=false;
	public float BarFullTime=0.8f;

	/// <summary>
	/// 设置分数进度条值
	/// </summary>
	/// <param name="_nb">_nb.</param>
	public void SetScoreBarValue(float _nb)
	{
		if(_nb==0)
		{
			return;
		}
		SetScoreBarAnimation=true;
		if (_nb > 1) {
			_nb = 1;
		}
		desBarVaule=_nb;
		BeginBarVaule=m_ScoreSlider.value;
		BarVauleBeginTime=RealTime.time;

	}
	float desBarVaule=0;
	float BeginBarVaule=0f;
	float BarVauleBeginTime=0f;
	float BarVauleEndTime=0f;
	void PlaySetScoreBarAnimation()
	{
		if(m_ScoreSlider.value>=1f)
		{
			SetScoreBarAnimation=false;
			return;
		}
		BarVauleEndTime=RealTime.time;

		m_ScoreSlider.value=Mathf.Lerp(BeginBarVaule,desBarVaule,(BarVauleEndTime-BarVauleBeginTime)/BarFullTime);
	}
	#endregion 

}