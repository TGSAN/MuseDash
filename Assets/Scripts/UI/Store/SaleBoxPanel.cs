using UnityEngine;
using System.Collections;
using FormulaBase;
public class SaleBoxPanel : MonoBehaviour {

	GameObject AddSpace;		//增加容量面板
	UILabel AddSpaceDanmond;    //增加空间的钻石


	GameObject SaleItem;		//出售物品的面板		



	GameObject m_Text;			//提示的面板
	public UILabel m_TextLabel;



	public GameObject m_Mask;	//背包挡板



	public CommonPanel m_commonPanel;
	// Use this for initialization
	void Awake()
	{
		AddSpace=transform.FindChild("AddBagSpace").gameObject;
		AddSpaceDanmond=transform.FindChild("AddBagSpace/OKButton/NBLabel").GetComponent<UILabel>();
		SaleItem=transform.FindChild("SaleItem").gameObject;
		m_Text=transform.FindChild("Text").gameObject;
		HideAll();
	}
	void HideAll()
	{
		m_Mask.SetActive(false);
		AddSpace.SetActive(false);
		SaleItem.SetActive(false);
		m_Text.SetActive(false);
	}
	#region 添加背包空间
	public void ShowAddSpace()
	{

		int UpDanmond=BagManageComponent.Instance.GetUpDiamond();
		if(UpDanmond==-1)//仓库已经最大
		{
			CommonPanel.GetInstance().ShowText("背包的容量已经是最大了!");
			
		}
		else if(AccountCrystalManagerComponent.Instance.GetDiamond()<UpDanmond)// 钻石不够
		{
			Debug.Log("钻石不够");
			
		}
		else //if() 背包可升级 钻石够
		{
			m_Mask.SetActive(true);
			AddSpace.SetActive(true);
			SaleItem.SetActive(false);
			m_Text.SetActive(false);
			AddSpaceDanmond.text=UpDanmond.ToString();
		}

	}
	public void ClickAddSpaceButton()
	{
		BagManageComponent.Instance.UpBag();
		//Messenger.Broadcast(bagPanel2.BroadcastBagUpSize);


		HideAll();

	}
	#endregion
	#region 显示提示文字

	public void CloseText()
	{
		m_Text.SetActive(false);
	}
	public void ShowText()
	{
		m_Mask.SetActive(true);
		AddSpace.SetActive(false);
		SaleItem.SetActive(false);
		m_Text.SetActive(true);
	}
	#endregion
	public void ClickCancelButton()
	{
		m_Mask.SetActive(false);
		AddSpace.SetActive(false);
		SaleItem.SetActive(false);
		m_Text.SetActive(false);


	}
	public void ShowSaleItem()
	{
		m_Mask.SetActive(true);
		AddSpace.SetActive(false);
		SaleItem.SetActive(true);
		m_Text.SetActive(false);
	}
	/// <summary>
	/// 按下退出
	/// </summary>
	public void ClickCancel()
	{

		HideAll();
	}

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
