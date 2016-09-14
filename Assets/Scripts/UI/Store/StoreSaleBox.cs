using UnityEngine;
using System.Collections;
using FormulaBase;
public class StoreSaleBox : MonoBehaviour {
	public UISprite m_GetResIcon;	//获得资源Icon
	public UISprite m_BuyResIcon;	//获取资源ICon
	public UILabel  m_GetResNumber;	//获取资源数量
	public UILabel  m_BuyResNumber;	//消耗资源数量
	public UISprite m_Girl;
	private int m_BuyStyel;
	private int m_BuyNumber;
	private int m_GetStyel;
	private int m_GetNumber;

	// Use this for initialization
	void Start () {
	
	}

	public void SetStoreBox(int _id) {
		StoreManageComponent.Instance.Host.SetDynamicData (SignKeys.ID, _id);
		int type = (int)StoreManageComponent.Instance.UseMoneyType (_id);
		m_BuyStyel = type;
		switch(type)
		{
		case StoreManageComponent.STORE_COST_TYPE_PHY:m_BuyResIcon.spriteName="";break;//RMB
		case StoreManageComponent.STORE_COST_TYPE_DIAMOND:m_BuyResIcon.spriteName="resourse_icon_crystal";break;//Diamond
		case StoreManageComponent.STORE_COST_TYPE_GOLD:m_BuyResIcon.spriteName="resourse_icon_coin";break;//Money
		}

		m_BuyNumber=((int)StoreManageComponent.Instance.UseMoneyNumber(_id));
		m_BuyResNumber.text=m_BuyNumber.ToString();

		//获取的东西
		if ((int)StoreManageComponent.Instance.GetMoneyNumber (_id) != 0) {
			m_GetStyel = StoreManageComponent.STORE_COST_TYPE_PHY;
			m_GetResIcon.spriteName = "resourse_icon_coin";
			m_GetNumber = ((int)StoreManageComponent.Instance.GetMoneyNumber (_id));
		} else if ((int)StoreManageComponent.Instance.GetDiamondNumber (_id) != 0) {
			m_GetStyel = StoreManageComponent.STORE_COST_TYPE_DIAMOND;
			m_GetResIcon.spriteName = "resourse_icon_crystal";
			m_GetNumber = ((int)StoreManageComponent.Instance.GetDiamondNumber (_id));

		} else if ((int)StoreManageComponent.Instance.GetPhysicalNumber (_id) != 0) {
			m_GetStyel = StoreManageComponent.STORE_COST_TYPE_GOLD;
			m_GetResIcon.spriteName = "resourse_cion_energy";
			m_GetNumber = ((int)StoreManageComponent.Instance.GetPhysicalNumber (_id));
		}

		m_GetResNumber.text=m_GetNumber.ToString();
		m_Girl.spriteName=StoreManageComponent.Instance.GetImageName(_id);//女孩图片
	}

	/// <summary>
	/// 点击商店图标
	/// </summary>
	public void ClickStoreButton() {	
		StoreManageComponent.Instance.UseMoneyBuySomething (m_BuyStyel, -m_BuyNumber, m_GetStyel, m_GetNumber);
//		switch(m_BuyStyel)
//		{
//		case 1:
//			
//			break;
//		case 2:
//			if(AccountManagerComponent.Instance.ChangeDiamond(-m_BuyNumber))
//			{
//				GetRes();
//			}
//
//			break;
//		case 3:
		//			if(AccountGoldManagerComponent.Instance.ChangeMoney(-m_BuyNumber))
//			{
//				GetRes();
//			}
//			break;
//		}
		
	}

//	public void GetRes()
//	{
//		switch(m_GetStyel)
//		{
//		case 1:
	//			AccountGoldManagerComponent.Instance.ChangeMoney(m_GetNumber);
//			break;
//		case 2:
//			AccountManagerComponent.Instance.ChangeDiamond(m_GetNumber);
//			break;
//		case 3:
	//			AccountPhysicsManagerComponent.Instance.ChangePhysical(m_GetNumber);
//			break;
//		}
//	}
	// Update is called once per frame
	//void Update () {
	//
	//}
}
