using UnityEngine;
using System.Collections;
using FormulaBase;
public class PetCell : MonoBehaviour {
	public int PetIndex=1;			//宠物栏的标记位
	FormulaHost m_host=null;				//宠物的信息
	public UISprite m_ItemIcon;

	public static string BroadCastRefreshEquipedPetUI="BROADCASTREFRESHEQUIPEDPETUI";

	void OnEnable()
	{
		Messenger.MarkAsPermanent(BroadCastRefreshEquipedPetUI);
		Messenger.AddListener(BroadCastRefreshEquipedPetUI,SetPetData);
		SetPetData();
	}
	void OnDisable()
	{
		Messenger.RemoveListener(BroadCastRefreshEquipedPetUI,SetPetData);
	}
//	public void OnEnable()
//	{
//		SetPetData();
//	}
	public void SetPetData()
	{
		m_host=PetManageComponent.Instance.GetEquipedPet(PetIndex);
		if(m_host==null)
		{
			m_ItemIcon.gameObject.SetActive(false);
		}
		else 
		{
			m_ItemIcon.gameObject.SetActive(true);
			int id=m_host.GetDynamicIntByKey("ID");
			m_ItemIcon.spriteName=id.ToString();
		}

	}
	/// <summary>
	/// 点击宠物1
	/// </summary>
	public void ClickPetCell()
	{
		bagPanel2.m_BagPanelEquipPlace=BagPanelEquipPlace.BagPanelEquipPlace_Pet1;
		UIManageSystem.g_Instance.SetMainMenuBackButton(true);
		if(m_host==null)
		{
			UIManageSystem.g_Instance.AddUI(UIManageSystem.UIBAGPANEL,(int)BagPanel2State.BagPanel2_SHowPet*10+(int)BagPanelState2.BagPanelState2_Equiped);
		}
		else 
		{
			UIManageSystem.g_Instance.AddUI(UIManageSystem.UIITEMINFOPANEL,(int)ItemInfoPanelState.ItemInfoPanel_Replace,m_host);
		}
	}
	/// <summary>
	/// 点击宠物2
	/// </summary>
	public void ClickPet1Cell()
	{
		bagPanel2.m_BagPanelEquipPlace=BagPanelEquipPlace.BagPanelEquipPlace_Pet2;UIManageSystem.g_Instance.SetMainMenuBackButton(true);
		if(m_host==null)
		{
			UIManageSystem.g_Instance.AddUI(UIManageSystem.UIBAGPANEL,(int)BagPanel2State.BagPanel2_SHowPet*10+(int)BagPanelState2.BagPanelState2_Equiped);
		}
		else 
		{
			UIManageSystem.g_Instance.AddUI(UIManageSystem.UIITEMINFOPANEL,(int)ItemInfoPanelState.ItemInfoPanel_Replace,m_host);
		}
	}
	/// <summary>
	/// 点击宠物3
	/// </summary>
	public void ClickPet2Cell()
	{
		bagPanel2.m_BagPanelEquipPlace=BagPanelEquipPlace.BagPanelEquipPlace_Pet3;UIManageSystem.g_Instance.SetMainMenuBackButton(true);
		if(m_host==null)
		{
			UIManageSystem.g_Instance.AddUI(UIManageSystem.UIBAGPANEL,(int)BagPanel2State.BagPanel2_SHowPet*10+(int)BagPanelState2.BagPanelState2_Equiped);
		}
		else 
		{
			UIManageSystem.g_Instance.AddUI(UIManageSystem.UIITEMINFOPANEL,(int)ItemInfoPanelState.ItemInfoPanel_Replace,m_host);
		}
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetData()
	{
		
	}
}
