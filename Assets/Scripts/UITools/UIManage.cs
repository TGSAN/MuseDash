//edit:sf 5.3
//UI管理类
using UnityEngine;
using System.Collections;

public enum UIState
{
	UISTATE_MAINMENU,		//主界面
	UISTATE_CLEAR,			//结算界面
	UISTATE_FBFAIL,			//FB失败界面	
	UISTATE_FBEXIT			//FB退出
	
	
}
public class UIManage:MonoBehaviour
{

//	public int NowChoseChesGrid;
	//int NowGuanKaIndex=0;//当前关卡

	//public int GetNowGuanKaiNDEX
	//{
	//	get
	//	{
	//		return NowGuanKaIndex;
	//	}
	//	set
	//	{
	///		NowGuanKaIndex=value;
	//	}
	//}
	public OperationTools m_InsOperationTools=OperationTools.g_Instan;

//	static UIManage m_Instan=null;
//	public static UIManage g_Instan
//	{
//		get
//		{
//			if(m_Instan==null)
//			{
//				m_Instan=new UIManage();
//			}
//			return m_Instan;
//		}
//	}
	public  void OnEnable()
	{
		Debug.Log("UIManage 的显示");
		Debug.Log("UIManage 的显示");
		Debug.Log("UIManage 的显示");
		Debug.Log("UIManage 的显示");
	}

	public void SetUIState(UIState _UIState)
	{
		m_UIState=_UIState;
	}
//
//	public int GetNowLevel
//	{
//		get{return NowGuanKaIndex;}
//		set{NowGuanKaIndex=value;}
//	}

	void Awake()
	{
		DontDestroyOnLoad(this.gameObject);
	}
	UIState m_UIState=UIState.UISTATE_MAINMENU;
	public UIState GetUIState
	{
		get
		{
			return m_UIState;
		}
		set
		{
			m_UIState=value;
		}
	}
	public  void Update()
	{
		m_InsOperationTools.UpdateTools();
	}

}
