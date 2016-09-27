using UnityEngine;
using System.Collections;
using FormulaBase;
public class StrengthenItem : MonoBehaviour {

	public GameObject m_EmptyIcon;
	public UISprite m_SomeThingSprite;
	public UISprite m_Rare;
	public int m_Index=0;
	FormulaHost m_host=null;
	FormulaHost m_ingoreHost=null;
	public LevelUpPanel m_LevelUpPanel;

	public void ClickItem()
	{
//		GetItemInfoPanel.gameObject.SetActive(false);
//		GetLevelUpPanel.gameObject.SetActive(false);
		switch(LevelUpPanel.m_LevelUpPanelState)
		{
		case LevelUpPanelState.LevelUpPanelState_ChoseEquip:
			//UIManageSystem.g_Instance.AddUI(UIManageSystem.UIBAGPANEL,(int)BagPanel2State.BagPanel2_ShowEquip*10+(int)BagPanelState2.BagPanelState2_Chosed,m_LevelUpPanel.m_host,true);
			break;
		case LevelUpPanelState.LevelUpPanelState_ChoseHeroFood:
		case LevelUpPanelState.LevelUpPanelState_ChosePetFood:

			Debug.Log("显示食物");
			//UIManageSystem.g_Instance.AddUI(UIManageSystem.UIBAGPANEL,(int)BagPanel2State.BagPanel2_ShowFood*10+(int)BagPanelState2.BagPanelState2_Chosed,m_LevelUpPanel.m_host,true);
			break;
		}
		//Messenger.Broadcast<bool>(ItemInfoPanel.ItemInfoChoseItem,true);
		//m_LevelUpPanel.ChoseItem(true);
		StartCoroutine("ShowBag");
//		if(m_host==null)
//		{
//			BagPanel.ShowTypeBag(UIPerfabsManage.g_Instan.GetUIState,m_ingoreHost);
//		}
//		else 
//		{
//			if(UIPerfabsManage.g_Instan.GetUIState==UIPerfabsManage.GameUIState.CHARACTOR_ITEMLEVELUPFOOD
//				||UIPerfabsManage.g_Instan.GetUIState==UIPerfabsManage.GameUIState.CHARACTOR_ITEMLEVELUPEQUIP
//			)
//			{
//				BagPanel.ShowTypeBag(UIPerfabsManage.g_Instan.GetUIState,m_ingoreHost);
//			}
//			if(UIPerfabsManage.g_Instan.GetUIState==UIPerfabsManage.GameUIState.STRENGTHERNITEN_EQUIPLEVELUP)
//			{
//				BagPanel.ShowTypeBag(UIPerfabsManage.GameUIState.STRENGTHERNITEN_EQUIPREPLACELEVELUP,m_ingoreHost);
//			}
//			else if(UIPerfabsManage.g_Instan.GetUIState==UIPerfabsManage.GameUIState.STRENGTHERNITEN_FOODLEVELUP)
//			{
//				BagPanel.ShowTypeBag(UIPerfabsManage.GameUIState.STRENGTHERNITEN_FOODREPLACELEVELUP);
//			}
//		}
	}
	IEnumerator ShowBag()
	{
		yield return new WaitForSeconds(0.2f);
//		switch(LevelUpPanel.m_LevelUpPanelState)
//		{
//		case LevelUpPanelState.LevelUpPanelState_ChoseEquip:
//			UIManageSystem.g_Instance.AddUI(UIManageSystem.UIBAGPANEL,(int)BagPanel2State.BagPanel2_ShowEquip*10+(int)BagPanelState2.BagPanelState2_Chosed,null,true);
//			break;
//		case LevelUpPanelState.LevelUpPanelState_ChoseHeroFood:
//		case LevelUpPanelState.LevelUpPanelState_ChosePetFood:
//
//			Debug.Log("显示食物");
//			UIManageSystem.g_Instance.AddUI(UIManageSystem.UIBAGPANEL,(int)BagPanel2State.BagPanel2_ShowFood*10+(int)BagPanelState2.BagPanelState2_Chosed,null,true);
//			break;
//		}
	}
	public void SetItem(FormulaHost _host,FormulaHost _ingorehost=null)
	{

		m_ingoreHost=_ingorehost;
		if(_host==null)
		{
			m_EmptyIcon.SetActive(true);
			m_SomeThingSprite.gameObject.SetActive(false);
			return ;
		}
		else 
		{
			m_SomeThingSprite.gameObject.SetActive(true);
			m_EmptyIcon.SetActive(false);
			m_SomeThingSprite.spriteName=_host.GetDynamicStrByKey("ID");
		}
	}
	void OnEnable()
	{
		SetItem(null);
	}
	// Use this for initialization
	void Start () {
	}
	// Update is called once per frame
	void Update () {
	}
}
