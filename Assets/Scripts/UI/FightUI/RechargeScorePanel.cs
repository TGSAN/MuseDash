using UnityEngine;
using System.Collections;
using GameLogic;

public class RechargeScorePanel : MonoBehaviour {

	public UIWidget m_MaxRange;
	public UILabel m_label;
	public UIWidget []m_maxVaule=new UIWidget[4];
	public UISprite []m_NowVaule=new UISprite[4];
	public UILabel []m_Number=new UILabel[4];
	private GameObject mainLayer = null;
	private Animator animator = null;
	private bool isExitting = false;

	private static RechargeScorePanel instance = null;
	public static RechargeScorePanel Instance {
		get {
			return instance;
		}
	}
	
	void Start() {
		instance = this;
		this.mainLayer = this.gameObject;
		this.mainLayer.SetActive (false);
		this.animator = this.transform.Find ("Recharge2Animation").gameObject.GetComponent<Animator> ();
//		this.SetScore (1f, 0.5f, 987654321);
	}
	void Update()
	{
		if(bPlaySetVauleAnimation)
		{
			PlaySetVauleAnimation();
		}
		if(bPlaySetVauleAnimation2)
		{
		 	PlaySetVauleAnimation2();
		}
	}
	#region// 数字
	float SetVauleBeginTime2=0f;
	float SetVauleEndTime2=0f;
	
	string []ArrNowString=new string[4];
	string []ArrDesString=new string[4];
	int []ArrDesVaule=new int[4];

	bool bPlaySetVauleAnimation2=false;

	bool FinishAnimation()
	{
		for(int i=0;i<4;i++)
		{
			if(ArrNowString[i]!=ArrDesString[i])
				return false;
		}
		return true;
	}
	void PlaySetVauleAnimation2()
	{
		if(FinishAnimation())
		{
			//finish
			bPlaySetVauleAnimation2=false;
			return ;
		}
		SetVauleEndTime2=RealTime.time;
		if(SetVauleEndTime2-SetVauleBeginTime2<0.16f)
		{
			return;
		}
		SetVauleBeginTime2=RealTime.time;
	//	Debug.Log("change Vaule");
		for(int j=0;j<4;j++)
		{
			for(int i=0;i<ArrNowString[j].Length;i++)
			{
				if(ArrDesString[j][i]!=ArrNowString[j][i])
				{
					ArrNowString[j]=ReplaceChar(ArrNowString[j],i);
				}
			}
			m_Number[j].text =ArrNowString[j];
		}
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
	void BeginPlayAnimation2()
	{
		SetVauleBeginTime2=RealTime.time;
	}
	#endregion
	#region 数值进度动画
	bool bPlaySetVauleAnimation=false;
	float SetVauleBeginTime=0f;
	float SetVauleEndTime=0f;
	float DesScore=0f;
	float DesCombo=0f;
	float DesClear=0f;
	float DesSetalt=0f;
	void PlaySetVauleAnimation()
	{
		if(m_NowVaule [0].fillAmount==DesScore)//&&4 ge
		{
			bPlaySetVauleAnimation=false;
			return;
		}
		SetVauleEndTime=RealTime.time;
		if(SetVauleEndTime-SetVauleBeginTime<0.05)
		{
			return;
		}
		m_NowVaule [0].fillAmount=Mathf.Lerp(0,DesScore,(SetVauleEndTime-SetVauleBeginTime));
		m_NowVaule [1].fillAmount=Mathf.Lerp(0,DesCombo,(SetVauleEndTime-SetVauleBeginTime));
		m_NowVaule [2].fillAmount=Mathf.Lerp(0,DesClear,(SetVauleEndTime-SetVauleBeginTime));
		m_NowVaule [3].fillAmount=Mathf.Lerp(0,DesSetalt,(SetVauleEndTime-SetVauleBeginTime));
		for(int i=0;i<4;i++)
		{
			m_Number[i].transform.localPosition=new Vector3(m_NowVaule[i].width*m_NowVaule[i].fillAmount+10,0,0);
		}

	}

	void BeginPlayAnimation()
	{
		SetVauleBeginTime=RealTime.time;

	}
	#endregion

	public void ShowUi() {
		if (this.mainLayer == null) {
			return;
		}
		
		if (this.mainLayer.activeSelf) {
			return;
		}

		this.isExitting = false;
		this.mainLayer.SetActive (true);
		// set ui data
		string songName = FormulaBase.StageBattleComponent.Instance.GetMusicName ();
		//JudgeScoreData judgeData = GameGlobal.gStage.GetStageJudgeScoreData ();
		this.SetSongName (songName);
		/*
		this.SetScore (1f, 0.5f, (int)judgeData.score);
		this.SetCombo (1f, 0.5f, (int)judgeData.combo);
		this.SetClear (1f, 0.5f, (int)judgeData.bosshp);
		this.SetSetalth (1f, 0.5f, (int)judgeData.hidenode); 
*/
	}

	public void SetScore(float _max,float _nowScore,int nb)
	{
		bPlaySetVauleAnimation=true;
		DesScore=_nowScore;



		//*****************all vaule
		ArrDesVaule[0]=nb;
		ArrDesString[0]=nb.ToString();
		ArrNowString[0]=new string('1',ArrDesString[0].Length);
		//*****************all vaule

		bPlaySetVauleAnimation2=true;
		BeginPlayAnimation();
		BeginPlayAnimation2();

		m_maxVaule [0].width = (int)(m_MaxRange.width * _max);
	//	m_Number [0].text = nb.ToString ();
	}
	public void SetCombo(float _max,float _nowScore,int nb)
	{
		DesCombo=_nowScore;
		m_maxVaule [1].width = (int)(m_MaxRange.width * _max);
		ArrDesVaule[1]=nb;
		ArrDesString[1]=nb.ToString();
		ArrNowString[1]=new string('1',ArrDesString[1].Length);
		//m_NowVaule [1].fillAmount = _nowScore;
		m_Number [1].text = nb.ToString ();
	}
	public void SetClear(float _max,float _nowScore,int nb)
	{

		DesClear=_nowScore;
		ArrDesVaule[2]=nb;
		ArrDesString[2]=nb.ToString();
		ArrNowString[2]=new string('1',ArrDesString[2].Length);
		m_maxVaule [2].width = (int)(m_MaxRange.width * _max);
		//m_NowVaule [2].fillAmount = _nowScore;
		m_Number [2].text = nb.ToString ();
	}
	public void SetSetalth(float _max,float _nowScore,int nb)
	{
		DesSetalt=_nowScore;
		ArrDesVaule[3]=nb;
		ArrDesString[3]=nb.ToString();
		ArrNowString[3]=new string('1',ArrDesString[3].Length);
		m_maxVaule [3].width = (int)(m_MaxRange.width * _max);
		//m_NowVaule [3].fillAmount = _nowScore;
		m_Number [3].text = nb.ToString ();
	}
	public void ClickContinueButton()
	{
		if (this.isExitting) {
			return;
		}

		this.isExitting = true;
		this.Exit ();
	}

	public void SetSongName(string _name)
	{
		if (_name == null) {
			return;
		}
		m_label.text = _name;
	}

	public void SetRewordEquip()
	{
		//for()
	}
	public void Exit() {
		// this.mainLayer.SetActive (false);
		Time.timeScale = GameGlobal.TIME_SCALE;
		this.animator.Stop ();
		this.animator.Rebind ();
		this.animator.Play ("Recharge2Panel_Out");
	}
}
