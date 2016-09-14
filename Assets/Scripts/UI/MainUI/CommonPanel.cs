using UnityEngine;
using System.Collections;
using FormulaBase;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;


public class CommonPanel : MonoBehaviour {
	public  OkBox m_Okbox;
	public 	ShowText m_ShowText;
	//public  UseResBox m_UseResBox;
	public GameObject m_WaittingPanel;
	public TweenAlpha m_Mask;
	public TweenAlpha m_BlurMask;				//毛玻璃的黑色遮罩
	public GameObject m_GMRobot;				//GM机器人
	public UIMask m_UIMask;						//各界面间的渐黑

	private static CommonPanel instance;	
	public static CommonPanel GetInstance() {
		return instance;
	}

	public UnityStandardAssets.ImageEffects.BlurOptimized m_UICamera=null;
	public UnityStandardAssets.ImageEffects.BlurOptimized GetUICamera {
		get {
			if (m_UICamera == null) {
				m_UICamera = GameObject.Find ("UI Root/Camera").GetComponent<UnityStandardAssets.ImageEffects.BlurOptimized> ();
			}
			return m_UICamera;
		}
	}

	public GameObject m_BlurCamera=null;
	public GameObject GetBlurCamera {
		get {
			if (m_BlurCamera == null) {
				m_BlurCamera = GameObject.Find ("UI Root").transform.FindChild ("Camera_DontBlur").gameObject;
			}
			return m_BlurCamera;
		}
	}

	public UIPanel m_mainmenu=null;
	public UIPanel GetMainmenu {
		get {
			if (m_mainmenu == null) {
				m_mainmenu = GameObject.Find ("UI Root").transform.FindChild ("MainMenuPanel").gameObject.GetComponent<UIPanel> ();
			}
			return m_mainmenu;
		}
	}
	//public BlurPanel m_blurPanel;
	//public GameObject m_CommonObjcet;

	//public delegate void CallBackFun();
	Callback m_CallBack=null;

	void Start() {
		this.SignltonCheck ();
	}

	void OnEnable() {
		this.SignltonCheck ();
	}

	private void SignltonCheck() {
		if (CommonPanel.GetInstance () == this) {
			return;
		}

		if (CommonPanel.GetInstance () == null) {
			instance = this;
			DontDestroyOnLoad (this.gameObject);
			return;
		}

		GameObject.Destroy (this.gameObject);
	}

	/// <summary>
	/// 显示等待界面
	/// </summary>
	/// <param name="_show">If set to <c>true</c> show.</param>
	public void ShowWaittingPanel(bool _show=true)
	{
//		Debug.Log("Showwaitting");
		m_WaittingPanel.gameObject.SetActive(_show);
	}

	/// <summary>
	/// OK  
	/// </summary>
	/// <param name="_str">String.</param>
	/// <param name="_callBack">Call back.</param>
	public void ShowOkBox(string _str="",Callback _callBack=null) {
		m_CallBack = _callBack;
		this.gameObject.SetActive (true);
		m_Okbox.showBox (_str, m_CallBack);
	}

	/// <summary>
	/// 播放遮罩
	/// </summary>
	/// <param name="_FromA">From a.</param>
	/// <param name="_ToA">To a.</param>
	/// <param name="_duration">Duration.</param>
	/// <param name="_delay">Delay.</param>
	public void PlayMask(float _FromA,float _ToA,float _duration,float _delay)
	{
		m_Mask.gameObject.SetActive (true);
		m_Mask.ResetToBeginning ();
		m_Mask.from = _FromA;
		m_Mask.to = _ToA;
		m_Mask.duration = _duration;
		m_Mask.delay = _delay;
		m_Mask.Play ();
	}

	/// <summary>
	/// 文本显示  自带渐隐动画
	/// </summary>
	/// <param name="_str">String.</param>
	/// <param name="_callBack">Call back.</param>
	public void ShowText(string _str="",Callback _callBack=null,bool _BehindBLur=true) {
		m_ShowText.showBox (_str, _callBack, _BehindBLur);
	}

	public void ShowTextLackDiamond(string _str="钻石不足",Callback _callBack=null,bool _BehindBLur=true) {
		m_ShowText.showBox (_str, _callBack, _BehindBLur);
	}

	public void ShowTextLackMoney(string _str="金币不足",Callback _callBack=null,bool _BehindBLur=true) {
		m_ShowText.showBox (_str, _callBack, _BehindBLur);
	}

	/// <summary>
	/// 关闭通用界面
	/// </summary>
	public void CloseCommonPanel() {
		if (m_CallBack != null) {
			m_CallBack ();
		}
		this.gameObject.SetActive (false);
	}

	/// <summary>
	/// 显示 使用资源的BOX
	/// </summary>
	public void ShowUseResBox(string _str,int _ResNumber,int type=0,Callback _OKFun=null,bool afterOKDisable=true,Callback _CancelFUn=null)
	{
		//m_UseResBox.SetData(_str,_ResNumber,type,_OKFun,afterOKDisable,_CancelFUn);
	}

	/// <summary>
	/// 显示装备上的宝箱
	/// </summary>
	/// <param name="m_host">M host.</param>
	/// <param name="_Queue">If set to <c>true</c> queue.</param>
	/// <param name="_QueueIndex">Queue index.</param>
	public void ShowChestEquip(FormulaBase.FormulaHost m_host,bool _Queue=false,int _QueueIndex=0)
	{
		//m_ChestShow.SetData(m_host,_Queue,_QueueIndex);
	}

	void CountChestTime()
	{
		if(ChestManageComponent.Instance.GetOwnedChestNumber()==0)
			return ;
		Debug.Log("减宝箱激活时间");
//		int time=(int)ChestManageComponent.Instance.GetChestList[0].GetDynamicDataByKey(SignKeys.CHESTREMAINING_TIME);
//		time--;
//
//		ChestManageComponent.Instance.GetChestList[0].SetDynamicData(SignKeys.CHESTREMAINING_TIME,time);
//		ChestManageComponent.Instance.GetChestList[0].Save();
	}

	void OnDisable()
	{
		//ExpandBmobLogin.ServerTime;
	}

	#region 毛玻璃
//	public void SetBlurAdd(UIPanel _Panel)
//	{
//		m_blurPanel.SetRenderOredrAddOne(_Panel);
//		
//	}
	//----------------------------------------------------过程
	public float  alltime=0.12f;
	public float BtweenChange=0.02f;
	public float LaterTime=0.0f;
	int realTime=0;
	int allTimes=0;
	public static int todownsample=3;
	public static float toblurSize=15;
	public static int toblurIterations=2;
	public void BeginBlur()
	{
		CancelInvoke("ChangeBlur");
		allTimes=(int)(alltime/BtweenChange);
		GetUICamera.GetComponent<BlurOptimized>().blurIterations=1;
		GetUICamera.GetComponent<BlurOptimized>().blurSize=0;
		GetUICamera.GetComponent<BlurOptimized>().downsample=0;
		realTime=1;
		StartCoroutine("LaterPlay");
	}

	IEnumerator LaterPlay()
	{
		yield return new WaitForSeconds(LaterTime);
		InvokeRepeating("ChangeBlur",BtweenChange,BtweenChange);
	}

	IEnumerator LaterPlay2()
	{
		yield return new WaitForSeconds(LaterTime);
		InvokeRepeating("SubBlur",BtweenChange,BtweenChange);
	}

	/// <summary>
	/// 改变毛玻璃数值增加
	/// </summary>
	public void ChangeBlur()
	{
		
		GetUICamera.GetComponent<BlurOptimized>().blurIterations=1+(toblurIterations-1)*realTime/allTimes;
		GetUICamera.GetComponent<BlurOptimized>().blurSize=toblurSize*realTime/allTimes;
		GetUICamera.GetComponent<BlurOptimized>().downsample=todownsample*realTime/allTimes;
		realTime++;
		if(realTime>allTimes)
		{
			CancelInvoke("ChangeBlur");
		}
	}

	/// <summary>
	/// 关闭毛玻璃
	/// </summary>
	public void CloseBlur()
	{
		realTime=allTimes;
		CancelInvoke("SubBlur");

		StartCoroutine("LaterPlay2");
	}

	/// <summary>
	/// 数值变小
	/// </summary>
	public void SubBlur()
	{
		
		GetUICamera.GetComponent<BlurOptimized>().blurIterations=1+(toblurIterations-1)*realTime/allTimes;
		GetUICamera.GetComponent<BlurOptimized>().blurSize=toblurSize*realTime/allTimes;
		GetUICamera.GetComponent<BlurOptimized>().downsample=todownsample*realTime/allTimes;
		realTime--;
		if(realTime<1)
		{
			CancelInvoke("SubBlur");
			SetMainMenuBlur(true);
			//GetMainmenu.gameObject.layer=5;
			//GetMainmenu.GetComponent<MainMenuPanel>().se
			GetUICamera.enabled=false;

			GetBlurCamera.GetComponent<UICamera>().enabled=false;
			GetUICamera.GetComponent<UICamera>().enabled=true;
			GetBlurCamera.SetActive(false);
		}
	}

	//----------------------------------------------------过程
	public void SetBlurSub(UIPanel _Panel,bool containMainMenu=false)
	{
		GetBlurCamera.GetComponent<UICamera> ().enabled = true;
		GetUICamera.GetComponent<UICamera> ().enabled = false;
		GetBlurCamera.SetActive (true);
		if (GetUICamera.enabled) {
			return;
		}
		GetUICamera.enabled = true;
		if (_Panel != null) {
			_Panel.gameObject.layer = 17;
		}
		if (containMainMenu) {
			SetMainMenuBlur (false);
		}
		BeginBlur ();
		m_BlurMask.ResetToBeginning ();
		m_BlurMask.PlayForward ();
	}

	/// <summary>
	/// 设置主界面的毛玻璃层级
	/// </summary>
	/// <param name="_Blur">If set to <c>true</c> blur.</param>
	public void SetMainMenuBlur(bool _Blur=true)
	{
		if (_Blur) {
			GetMainmenu.gameObject.layer = 5;
			GetMainmenu.GetComponent<MainMenuPanel> ().SetModelLayer (false);
		} else {
			GetMainmenu.gameObject.layer = 17;
			GetMainmenu.GetComponent<MainMenuPanel> ().SetModelLayer (true);
		}
	}

	/// <summary>
	/// 关闭毛玻璃
	/// </summary>
	/// <param name="_ob">Ob.</param>
	public void CloseBlur(GameObject _ob)
	{
		if(_ob!=null)
		{
			_ob.layer=5;
		}
		CloseBlur();
		m_BlurMask.PlayReverse();
	//	m_blurPanel.CloseBlur();
	}

	#endregion

	FormulaHost t_OpenChestHost=null;	//临时开宝箱的host
	public void ShowRewardCubePanel(int _index,UIPerfabsManage.CallBackFun _CallBack=null,FormulaHost _host=null)
	{
		/*
		if(_CallBack!=null)
		{
			m_RewardCubePanel.Show(_index,_CallBack);
		}
		else 
		{
			t_OpenChestHost=_host;
			m_RewardCubePanel.Show(_index,ShowRewardCubePanelCallBack);
		}*/
	}

	public void ShowRewardCubePanelCallBack()
	{
		//m_RewardPanel.ShowGetItem(t_OpenChestHost);
	}

	public void ShowEnSureSalePanel()
	{
		//m_EnSureSalePanel.
	}

	/// <summary>
	/// 设置面板是否模糊
	/// </summary>
	/// <param name="_ob">Ob.</param>
	/// <param name="_blur">If set to <c>true</c> blur.</param>
	public void SetPanelBlur(UIPanel  _Panel,bool _blur)
	{
		if (_blur) {
			_Panel.gameObject.layer = 17;
//			_ob.layer=17;
//			foreach(Transform trans in _ob.GetComponentsInChildren<Transform>())
//			{
//				trans.gameObject.layer=17;
//			}
			
		} else {
			_Panel.gameObject.layer = 5;
//			_ob.layer=5;
//			foreach(Transform trans in _ob.GetComponentsInChildren<Transform>())
//			{
//				trans.gameObject.layer=5;
//			}
		}
	}

	/// <summary>
	/// 显示确定出售按钮
	/// </summary>
	/// <param name="_Money">Money.</param>
	/// <param name="_OKCallBack">OK call back.</param>
	/// <param name="_CancelCallBack">Cancel call back.</param>
	/// <param name="_BlueQulity">If set to <c>true</c> blue qulity.</param>
	/// <param name="_blur">If set to <c>true</c> blur.</param>
	public void ShowEnSureSalePanel(int _Money,Callback _OKCallBack,Callback _CancelCallBack,bool _BlueQulity=false,bool _blur=false)
	{
		//m_EnSureSalePanel.ShowPanel(_Money,_OKCallBack,_CancelCallBack,_BlueQulity,_blur);
	}

	/// <summary>
	/// 隐藏确定出售按钮
	/// </summary>
	/// <param name="_brue">If set to <c>true</c> brue.</param>
	public void HidEnSureSalePanel(bool _brue=false)
	{
		//m_EnSureSalePanel.CloseEnSureSalePanel(_brue);
	}

	#region 毛玻璃

	#endregion

	public void ShowGMRobot(bool _Show=true)
	{
		if (m_GMRobot.activeSelf == _Show)
			return;
		m_GMRobot.SetActive (_Show);
	}

	#region 各界面间的渐黑

	public void SetMask(bool _WhiteToBlack=true,Callback _CallBack=null,bool Blur=false)
	{
		m_UIMask.SetMask (_WhiteToBlack, _CallBack, Blur);
	}

	#endregion
}
