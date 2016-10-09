using UnityEngine;
using System.Collections;
using FormulaBase;
using System.Collections.Generic;
public enum EquipCellClick
{
	EQUIP_SHOWEQUIP,//panduan xianzai zai na xianshi zhuangbei
	EQUIP_ININFO

}

public class EquipDate
{

}
public class EquipCell : MonoBehaviour {

	public GameObject m_EmptyObject;			//显示空的状态
	public GameObject m_SomeThing;				//显示有东西的状态
	public UISprite m_EquipIcon;				//装备的图标
	public UISprite m_QualityIcon;				//装备品质的图标
	ItemInfoPanel m_ItemInfoPanel=null;			//显示物品信息的面板
	public ItemInfoPanel ItemInfoPanel
	{
		get
		{
			if(m_ItemInfoPanel==null)
				m_ItemInfoPanel=GameObject.Find("UI Root").gameObject.transform.FindChild("ItemInfoPanel").GetComponent<ItemInfoPanel>();
			return m_ItemInfoPanel;
		}
	}

	public static string BroadCastRefreshEquipedEquipUI="BROADCASTREFRESHEQUIPEDEQUIPUI";

	//public UISprite m_Iconsprite;
	public int m_Index=0;

	FormulaHost m_host=null;
	bagPanel2 m_BagPanel=null;
	bagPanel2 BagPanel
	{
		get
		{
			if(m_BagPanel==null)
			{
				m_BagPanel=GameObject.Find("UI Root").gameObject.transform.FindChild("BagPanel").GetComponent<bagPanel2>();
			}
			return m_BagPanel;
		}
	}

	public void SetUI()
	{
		SetData(EquipManageComponent.Instance.GetEquipedEquip(m_Index));
	}

	void OnEnable()
	{
		Messenger.MarkAsPermanent(BroadCastRefreshEquipedEquipUI);
		Messenger.AddListener(BroadCastRefreshEquipedEquipUI,SetUI);
		SetUI();
	}
	void OnDisable()
	{
		Messenger.RemoveListener(BroadCastRefreshEquipedEquipUI,SetUI);
	}
	public void SetData(FormulaHost _host)
	{
		m_host=_host;
		if(_host==null)
		{
			m_EmptyObject.SetActive(true);
			m_SomeThing.SetActive(false);
		}
		else 
		{			
			m_EmptyObject.SetActive(false);
			m_SomeThing.SetActive(true);
			m_EquipIcon.spriteName=_host.GetDynamicStrByKey("ID");
		}
	}
	/// <summary>
	/// 设置装备栏为空
	/// </summary>
	void SetEmpty()
	{
		
	}
	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void Update () {
	
	}
	public void ClickEquipButton()
	{
		bagPanel2.m_BagPanelEquipPlace=BagPanelEquipPlace.BagPanelEquipPlace_Equip1;UIManageSystem.g_Instance.SetMainMenuBackButton(true);
		if(m_host==null)
		{
			UIManageSystem.g_Instance.AddUI(UIManageSystem.UIBAGPANEL,(int)BagPanel2State.BagPanel2_ShowWeapon*10+(int)BagPanelState2.BagPanelState2_Equiped);
		}
		else 
		{
			UIManageSystem.g_Instance.AddUI(UIManageSystem.UIITEMINFOPANEL,(int)ItemInfoPanelState.ItemInfoPanel_Replace,m_host);
		}
	}
	public void ClickChip0Button()
	{
		bagPanel2.m_BagPanelEquipPlace=BagPanelEquipPlace.BagPanelEquipPlace_Equip2;UIManageSystem.g_Instance.SetMainMenuBackButton(true);
		if(m_host==null)
		{
			UIManageSystem.g_Instance.AddUI(UIManageSystem.UIBAGPANEL,(int)BagPanel2State.BagPanel2_ShowChip*10+(int)BagPanelState2.BagPanelState2_Equiped);
		}
		else 
		{
			UIManageSystem.g_Instance.AddUI(UIManageSystem.UIITEMINFOPANEL,(int)ItemInfoPanelState.ItemInfoPanel_Replace,m_host);
		}
	}
	public void ClickChip1Button()
	{
		bagPanel2.m_BagPanelEquipPlace=BagPanelEquipPlace.BagPanelEquipPlace_Equip3;UIManageSystem.g_Instance.SetMainMenuBackButton(true);
		if(m_host==null)
		{
			UIManageSystem.g_Instance.AddUI(UIManageSystem.UIBAGPANEL,(int)BagPanel2State.BagPanel2_ShowChip*10+(int)BagPanelState2.BagPanelState2_Equiped);
		}
		else 
		{
			UIManageSystem.g_Instance.AddUI(UIManageSystem.UIITEMINFOPANEL,(int)ItemInfoPanelState.ItemInfoPanel_Replace,m_host);
		}
	}
	public void ClickChip2Button()
	{
		bagPanel2.m_BagPanelEquipPlace=BagPanelEquipPlace.BagPanelEquipPlace_Equip4;UIManageSystem.g_Instance.SetMainMenuBackButton(true);
		if(m_host==null)
		{
			UIManageSystem.g_Instance.AddUI(UIManageSystem.UIBAGPANEL,(int)BagPanel2State.BagPanel2_ShowChip*10+(int)BagPanelState2.BagPanelState2_Equiped);
		}
		else 
		{
			UIManageSystem.g_Instance.AddUI(UIManageSystem.UIITEMINFOPANEL,(int)ItemInfoPanelState.ItemInfoPanel_Replace,m_host);
		}
	}
}
