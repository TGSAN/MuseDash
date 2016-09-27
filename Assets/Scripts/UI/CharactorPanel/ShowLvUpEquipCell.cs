using UnityEngine;
using System.Collections;
using FormulaBase;

public class ShowLvUpEquipCell : MonoBehaviour {
	public GameObject m_EmptyIcon;
	public UISprite m_SomeThingSprite;
	public UISprite m_Rare;
	public int m_Index=0;

	/// <summary>
	/// 点击升级装备界面的选择物品栏位
	/// </summary>
	public void ClickItem()
	{

		bagPanel2 BagPanel=GameObject.Find("UI Root").gameObject.transform.FindChild("BagPanel").GetComponent<bagPanel2>();
		UIPerfabsManage.g_Instan.GetUIState=UIPerfabsManage.GameUIState.BAG_CHARACTER_HEROLEVELUP_TOBAG;
	//	BagPanel.ShowTypeBag(UIPerfabsManage.GameUIState.BAG_CHARACTER_HEROLEVELUP_TOBAG);

	}
	/// <summary>
	/// 设置升级栏位的信息
	/// </summary>
	/// <param name="_host">Host.</param>
	public void SetItem(FormulaHost _host)
	{
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
