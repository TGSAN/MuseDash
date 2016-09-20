//EDIT：SF 2016/6/29
//UI 管理类

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FormulaBase;
//一个UI 可能有多种状态 用数值定义不同面板的状态
//想有代码 控制层级 粒子效果怎么处理？
public class UIManageSystem : System.Object {

	#region 所有面板的名字
	public static string UILEVELPREPAREPANEL="LevelPreparePanel";
	public static string UIADVENTUREPANEL="AdventurePanel5";
	public static string UICHARACTORPANEL="CharactorPanel2";
	public static string UIBAGPANEL="BagPanel";
	public static string UISTOREPANEL="StorePanel";
	public static string UIITEMINFOPANEL="ItemInfoPanel";
	public static string UILEVELUPPANEL="LevelUpPanel";
	public static string UIDAILYTASHPANEL="DailyTashPanel";
	public static string UICHOSEHEROPANEL="ChoseHeroPanel";
	public static string UIOTERHPANEL="OtherPanel";
	public static string UILEVELGOALSPANEL="LevelGoalsPanel";
	public static string UIEQUIPANDPET_LEVELUPPANEL="EquipAndPetLevelUpPanel";
	public static string UI_HEROLEVEUP_PANEL="HeroLevelUpPanel";
	#endregion
//	public  delegate void  ReomveUICallBack();
//	public ReomveUICallBack m_ReomveUICallBack=null;
	//MainMenuPanel m_MainMenuPanel=null;
	/*
	MainMenuPanel GetMainMenuPanel
	{
		get
		{
			if(m_MainMenuPanel==null)
			{
				//m_MainMenuPanel=GameObject.Find("MainMenuPanel").GetComponent<MainMenuPanel>();
			}
			return m_MainMenuPanel;
		}
	}*/

	static UIManageSystem m_Instance=null;
	static public  UIManageSystem g_Instance
	{
		get
		{
			if(m_Instance==null)
			{
				m_Instance=new UIManageSystem(); 
			}
			return m_Instance;
		}
	}

	Dictionary<string,Object> m_dicUIManageSystem=new Dictionary<string, Object>();

	Stack<PanelInfo> m_StackStack=new Stack<PanelInfo>();			//界面队列
	class PanelInfo
	{
		public string m_name;			//面板名字
		public int m_State;				//面板状态
		public GameObject m_Object;		//面板的预设
	}		
//	Stack<string> m_StackStack2=new Stack<string>();			//界面队列
	/// <summary>
	/// 注册面板  手动添加吧
	/// </summary>
	string [] panels=
	{ 
		"LevelPreparePanel",
		"AdventurePanel5",
		"CharactorPanel2",
		"BagPanel",
		"StorePanel",
		"ItemInfoPanel",
		"LevelUpPanel",
		"DailyTashPanel",
		"ChoseHeroPanel",
		"OtherPanel",
		"LevelGoalsPanel",
		"EquipAndPetLevelUpPanel",
		"HeroLevelUpPanel"
	};
	public void RegisterPanel()
	{
		for(int i=0,max=panels.Length;i<max;i++)
		{
			Object temp=Resources.Load("UIResource/UIprefabs/"+panels[i]);
			if(temp==null)
			{
				Debug.LogError("读取Panel失败---"+panels[i]);
			}
			m_dicUIManageSystem[panels[i]]=temp;
		}
	}
	/// <summary>
	/// 添加UI
	/// </summary>
	public void AddUI(string _name,int _State=1,FormulaBase.FormulaHost _host=null,bool _repeat=false,bool behind=false)
	{

			foreach (var tobject in m_StackStack) 
			{
				if(tobject.m_name==_name)
				{
					if(_repeat)
					{
					PanelInfo tInfo2=new PanelInfo();
					tInfo2.m_name=_name;
					tInfo2.m_State=_State;
					GameObject temp3=	Object.Instantiate(m_dicUIManageSystem[tobject.m_name]) as GameObject;
					temp3.transform.parent=GameObject.Find("UI Root").transform;
					temp3.transform.localScale=new Vector3(1,1,1);
					temp3.name=_name;
					tInfo2.m_Object=temp3;
					m_StackStack.Push(tInfo2);
					temp3.GetComponent<UIPanelBase>().ShowPanel(_State,_host,m_StackStack.Count);
					temp3.GetComponent<UIPanelBase>().SetPanelLayer(tobject.m_Object.layer/5,2);
					tobject.m_Object.SetActive(false);
					//temp3.GetComponent<UIPanelBase>().ShowPanel(_State,_host,tobject.m_Object.layer/5);
					return ;
							
					}
					Debug.Log("已经包含相同的UI");
					return ;
				}
			}
		PanelInfo tInfo=new PanelInfo();
		tInfo.m_name=_name;
		tInfo.m_State=_State;
		if(m_dicUIManageSystem.ContainsKey(_name))
		{
			GameObject temp=Object.Instantiate(m_dicUIManageSystem[_name]) as GameObject;
			temp.transform.parent=GameObject.Find("UI Root").transform;
			temp.transform.localScale=new Vector3(1,1,1);
			temp.name=_name;
			tInfo.m_Object=temp;
			m_StackStack.Push(tInfo);
			temp.GetComponent<UIPanelBase>().ShowPanel(_State,_host,m_StackStack.Count);
		}
		else 
		{
			//Debug.LogError("添加错误的UI界面");
		}
	}
	/// <summary>
	/// 删除UI  默认删除到只剩一级
	/// </summary>
	/// <param name="_index">Index.</param>
	public void RomoveUI(int _index=1)
	{
		if(m_StackStack.Count==_index)//没有UI 的状况 为基础UI
			return ;
		Debug.Log("delete UI");
		PanelInfo temp=m_StackStack.Pop();

		foreach (var tobject in m_StackStack) 
		{
			if(tobject.m_name==temp.m_name)
			{
				tobject.m_Object.SetActive(true);
			}
		}
		GameObject.DestroyObject(temp.m_Object);
		if(m_StackStack.Count==1)
		{
			//GetMainMenuPanel.SetExitButtonShow(false);
		}
	}
	/// <summary>
	/// 删除UI 到根目录
	/// </summary>
	public void RemoveToRoot()
	{
		while(HaveUI()>0)
		{
			RomoveUI(0);
		}
		ItemManageComponent.Instance.ClearChosedItem();

	}
	/// <summary>
	/// Shows the U. -1为添加的默认状态
	/// </summary>
	/// <param name="_State">State.</param>
	public void ShowUI(int _State=-1)
	{

		if(m_StackStack.Count==0)//没有UI 的状况
			return ;
		PanelInfo temp=m_StackStack.Peek();
		if(temp.m_Object==null)
		{
			if(m_dicUIManageSystem.ContainsKey(temp.m_name))
			{
				GameObject tempObject=Object.Instantiate(m_dicUIManageSystem[temp.m_name]) as GameObject;
				tempObject.transform.parent=GameObject.Find("UI Root").transform;
				tempObject.transform.localScale=new Vector3(1,1,1);
				tempObject.name=temp.m_name;
				//tempObject.GetComponent<UIPanelBase>().ShowPanel(_State);
				temp.m_Object=tempObject;
			}
			else 
			{
				Debug.LogError("添加错误的UI界面");
			}
		}
		if(_State==-1)
		{
			temp.m_Object.GetComponent<UIPanelBase>().ShowPanel(temp.m_State);
		}
		else 
		{
			temp.m_Object.GetComponent<UIPanelBase>().ShowPanel(_State);
		}
	}
	/// <summary>
	/// 设置栈里面未显示UI的状态  有个底层UI切换的动画
	/// </summary>
	public void SetBackUIState(string _name,int _State=1)
	{
		foreach (var tobject in m_StackStack) 
		{
			if(tobject.m_name==_name)
			{
				tobject.m_Object.GetComponent<UIPanelBase>().ShowPanel(_State);
				return ;
			}
		}
	}
	/// <summary>
	/// 不同界面点击退出按钮
	/// </summary>
	public void ClickBackButton()
	{
		PanelInfo temp=m_StackStack.Peek();
		temp.m_Object.GetComponent<UIPanelBase>().PanelClickBack();
	}
	public int HaveUI()
	{
		return m_StackStack.Count;
	}

	public void SetMainMenuBackButton(bool _show=false)
	{
	
		//GetMainMenuPanel.SetExitButtonShow(_show);
	}
	/// <summary>
	/// 设置栈顶的UI状态
	/// </summary>
	/// <param name="_show">If set to <c>true</c> show.</param>
	public void SetTopPanel(bool _show)
	{
		PanelInfo temp=m_StackStack.Peek();
		temp.m_Object.SetActive(_show);
	}
}
