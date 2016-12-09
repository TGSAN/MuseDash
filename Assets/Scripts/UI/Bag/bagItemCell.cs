using FormulaBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public enum ItemCellState
//{
//
//}
public class bagItemCell : MonoBehaviour
{
    public static string BraodCast_HideChoseTag = "BraodCast_HideChoseTag";
    public static string BroadCast_HideMask = "BroadCast_HideMask";
    public static string BroadCast_ShowMask = "BroadCast_ShowMask";
    public static string BraodCast_BagSaleItemPluralNumber = "BraodCast_BagSaleItemPluralNumber";

    public GameObject m_ChosedTag;
    public bagPanel2 m_bagPanel;

    public UISprite m_Rare;         //品质
    public GameObject m_Empty;      //空的UI
    public GameObject m_Something;  //有东西的UI
    public GameObject m_Lock;       //锁
    public UILabel m_TestLabel;
    public GameObject m_Mask;       //半透明挡板
    public UISprite m_ItemIcon;
    public UILabel m_ItemNumber = null; //物品的数量
    public UILabel LevelAndNumberAndTime = null;    //等级数量和时间
    public GameObject m_CubeObject;
    public GameObject[] m_ArrCube = new GameObject[4];

    /// <summary>
    /// 隐藏选择的标签
    /// </summary>
    public void HideChoseTag()
    {
        m_ChosedTag.SetActive(false);
    }

    ///物品选择选满时的挡板
    ///隐藏挡板
    public void HideMask()
    {
        if (m_host != null && m_host.GetDynamicDataByKey("CHOSED") == 0)
        {
            m_Mask.SetActive(false);
        }
    }

    /// <summary>
    /// 显示物品选择满时的挡板
    /// </summary>
    public void ShowMask()
    {
        if (m_host != null && m_host.GetDynamicDataByKey("CHOSED") == 0)
        {
            m_Mask.SetActive(true);
        }
    }

    /// <summary>
    /// 获取当前UI上的数据
    /// </summary>
    /// <returns>The formula host.</returns>
    public FormulaHost GetFormulaHost()
    {
        return m_host;
    }

    #region 初始化UICell

    private FormulaHost m_host = null;

    public delegate void ClickItem(FormulaHost _host);

    //	public ClickItem ClickCallBackFun;
    /// <summary>
    /// 设置当前UI上的数据
    /// </summary>
    /// <param name="_data">Data.</param>
    public void SetUI(FormulaHost _data)
    {
        if (_data == null)
        {
            m_CubeObject.SetActive(false);
            m_Empty.SetActive(true);
            m_Something.SetActive(false);
            m_Mask.SetActive(false);
            m_host = null;
            return;
        }
        m_host = _data;
        //	ClickCallBackFun=_ClickItem;			//点击后的回调函数
        int id = (int)_data.GetDynamicDataByKey("ID");
        m_ItemIcon.spriteName = id.ToString();      //Item的Icon
        if (m_host.GetDynamicDataByKey("CHOSED") >= 1)
        {
            m_ChosedTag.SetActive(true);
            m_Mask.SetActive(false);
        }
        else
        {
            m_ChosedTag.SetActive(false);
        }
        //		else
        //		{
        //			if(UIPerfabsManage.g_Instan.GetUIState==UIPerfabsManage.GameUIState.SuitCase_Main)
        //			{
        //				m_Mask.SetActive(false);
        //			}
        //			else if(UIPerfabsManage.g_Instan.GetUIState==UIPerfabsManage.GameUIState.SuitCase_Sale)
        //			{
        //				if(ItemManageComponent.Instance.GetChosedItem.Count==20)
        //				{
        //					m_Mask.SetActive(true);
        //				}
        //				else
        //				{
        //					m_Mask.SetActive(false);
        //				}
        //			}
        //			else if(UIPerfabsManage.g_Instan.GetUIState==UIPerfabsManage.GameUIState.BAG_CHARACTER_HEROLEVELUP_TOBAG
        //				||UIPerfabsManage.g_Instan.GetUIState==UIPerfabsManage.GameUIState.STRENGTHERNITEN_EQUIPLEVELUP
        //				||UIPerfabsManage.g_Instan.GetUIState==UIPerfabsManage.GameUIState.STRENGTHERNITEN_EQUIPREPLACELEVELUP
        //				||UIPerfabsManage.g_Instan.GetUIState==UIPerfabsManage.GameUIState.STRENGTHERNITEN_FOODLEVELUP
        //				||UIPerfabsManage.g_Instan.GetUIState==UIPerfabsManage.GameUIState.STRENGTHERNITEN_FOODREPLACELEVELUP
        //				||UIPerfabsManage.g_Instan.GetUIState==UIPerfabsManage.GameUIState.CHARACTOR_ITEMLEVELUPFOOD
        //				||UIPerfabsManage.g_Instan.GetUIState==UIPerfabsManage.GameUIState.CHARACTOR_ITEMLEVELUPEQUIP
        //				||UIPerfabsManage.g_Instan.GetUIState==UIPerfabsManage.GameUIState.CHARACTOR_HEROLEVELUp
        //			)
        //			{
        //				if(ItemManageComponent.Instance.GetChosedItem.Count==4)
        //				{
        //					m_Mask.SetActive(true);
        //				}
        //				else
        //				{
        //					m_Mask.SetActive(false);
        //				}
        //
        //			}
        //			m_ChosedTag.SetActive(false);
        //		}

        ///设置 锁的状态
        int locked = (int)m_host.GetDynamicDataByKey(SignKeys.LOCKED);
        if (locked == 0)
        {
            m_Lock.SetActive(false);
        }
        else
        {
            m_Lock.SetActive(true);
        }

        #region 物品复数

        SetItemPlural();

        #endregion 物品复数

        m_ItemIcon.gameObject.SetActive(true);
        string temp = _data.GetFileName();
        switch (temp)
        {
            case "Equip":
                //Debug.Log("Equip");
                m_CubeObject.SetActive(false);
                ShowEquip(_data); break;
            case "Material":
                //Debug.Log("Material");
                m_CubeObject.SetActive(false);
                ShowMaterial(_data); break;
            case "Pet":
                //Debug.Log("Pet");
                m_CubeObject.SetActive(false);
                ShowPet(_data); break;
            case "Chest":
                //Debug.Log("Chest");
                m_CubeObject.SetActive(true);
                ShowChest(_data); break;
        }
        m_Empty.SetActive(false);
        m_Something.SetActive(true);
    }

    /// <summary>
    /// 设置物品的复数信息
    /// </summary>
    public void SetItemPlural(bool _clear = false)
    {
        if (m_ItemNumber == null)//信息面板上的特例化
            return;
        if (m_host == null)
            return;

        if (_clear)
        {
            m_ChosedTag.SetActive(false);
            m_host.SetDynamicData(SignKeys.CHOSED, 0);
        }
        if (m_host.GetDynamicIntByKey(SignKeys.STACK_NUMBER, 1) != 1)
        {
            int number = (int)m_host.GetDynamicIntByKey(SignKeys.STACKITEMNUMBER);
            int ChosedNumber = m_host.GetDynamicIntByKey(SignKeys.CHOSED);
            m_ItemNumber.text = ChosedNumber.ToString() + "/" + number.ToString();
        }
        else
        {
            m_ItemNumber.text = 1 + "/" + 1;
        }

        //		if(m_host.GetDynamicIntByKey(SignKeys.STACK_NUMBER,1)!=1)
        //		{
        //			m_ItemNumber.gameObject.SetActive(true);
        //			if(
        //				bagPanel2.GetBagPanelState2()==BagPanelState2.BagPanelState2_Sale
        //				||bagPanel2.GetBagPanelState2()==BagPanelState2.BagPanelState2_Chosed
        ////				UIPerfabsManage.g_Instan.GetUIState==UIPerfabsManage.GameUIState.SuitCase_Sale
        ////				||UIPerfabsManage.g_Instan.GetUIState==UIPerfabsManage.GameUIState.BAG_CHARACTER_HEROLEVELUP_TOBAG
        ////				||UIPerfabsManage.g_Instan.GetUIState==UIPerfabsManage.GameUIState.STRENGTHERNITEN_FOODLEVELUP
        ////				||UIPerfabsManage.g_Instan.GetUIState==UIPerfabsManage.GameUIState.CHARACTOR_ITEMLEVELUPFOOD
        ////				||UIPerfabsManage.g_Instan.GetUIState==UIPerfabsManage.GameUIState.CHARACTOR_ITEMLEVELUPEQUIP
        ////				||UIPerfabsManage.g_Instan.GetUIState==UIPerfabsManage.GameUIState.CHARACTOR_HEROLEVELUp
        //			)
        //			{
        //				int number=(int)m_host.GetDynamicIntByKey(SignKeys.STACKITEMNUMBER);
        //				int ChosedNumber=m_host.GetDynamicIntByKey(SignKeys.CHOSED);
        //				m_ItemNumber.text=ChosedNumber.ToString()+"/"+number.ToString();
        //			}
        //			else
        //			{
        //				int number=(int)m_host.GetDynamicIntByKey(SignKeys.STACKITEMNUMBER);
        //				m_ItemNumber.text=number.ToString();
        //			}
        //		}
        //		else
        //		{
        //			m_ItemNumber.gameObject.SetActive(false);
        //		}
    }

    /// <summary>
    /// 显示的是装备数据
    /// </summary>
    /// <param name="_data">Data.</param>
    private void ShowEquip(FormulaHost _data)
    {
        ushort id = (ushort)_data.GetDynamicDataByKey("ID");
        _data.Result(FormulaKeys.FORMULA_13);

        int Quality = (int)_data.GetDynamicDataByKey(SignKeys.QUALITY);
        //	int TimeID =(int)_data.GetDynamicDataByKey(SignKeys.BAGINID);
        int Type = (int)_data.GetDynamicDataByKey(SignKeys.TYPE);
        int t_number = (int)_data.GetDynamicDataByKey(SignKeys.STACKITEMNUMBER);
        //	m_TestLabel.text="STACKITEMNUMBER : "+t_number+"TYPE:"+Type;
        LevelAndNumberAndTime.text = "LV. " + _data.GetDynamicIntByKey(SignKeys.LEVEL);
        switch (Quality)
        {
            case 0: m_Rare.spriteName = ""; break;
            case 1: m_Rare.spriteName = "groove_1"; break;
            case 2: m_Rare.spriteName = "groove_2"; break;
            case 3: m_Rare.spriteName = "groove_3"; break;
            case 4: m_Rare.spriteName = "groove_4"; break;
        }
    }

    /// <summary>
    /// 显示的是材料数据
    /// </summary>
    /// <param name="_data">Data.</param>
    private void ShowMaterial(FormulaHost _data)
    {
        //ushort id=(ushort)_data.GetDynamicDataByKey("ID");
        _data.Result(FormulaKeys.FORMULA_26);
        int Quality = (int)_data.GetDynamicDataByKey(SignKeys.QUALITY);
        // TimeID =(int)_data.GetDynamicDataByKey(SignKeys.BAGINID);
        //int Type =(int)_data.GetDynamicDataByKey(SignKeys.TYPE);
        //m_TestLabel.text="Material";
        int t_number = (int)_data.GetDynamicDataByKey(SignKeys.STACKITEMNUMBER);
        //m_TestLabel.text="STACKITEMNUMBER : "+t_number;
        //	m_TestLabel.text="Material Q: "+Quality+"\nTime: "+TimeID+"\nType: "+Type+"\n id;"+id;
        LevelAndNumberAndTime.text = "X " + t_number.ToString();
        switch (Quality)
        {
            case 0: m_Rare.spriteName = ""; break;
            case 1: m_Rare.spriteName = "groove_1"; break;
            case 2: m_Rare.spriteName = "groove_2"; break;
            case 3: m_Rare.spriteName = "groove_3"; break;
            case 4: m_Rare.spriteName = "groove_4"; break;
        }
    }

    /// <summary>
    /// 显示宠物
    /// </summary>
    /// <param name="_data">Data.</param>
    private void ShowPet(FormulaHost _data)
    {
        //	ushort id=(ushort)_data.GetDynamicDataByKey("ID");
        //_data.Result(FormulaKeys.FORMULA_91);
        int Quality = (int)_data.GetDynamicDataByKey(SignKeys.QUALITY);
        //int TimeID =(int)_data.GetDynamicDataByKey(SignKeys.BAGINID);
        int Type = (int)_data.GetDynamicDataByKey(SignKeys.TYPE);
        //m_TestLabel.text="Pet";
        int t_number = (int)_data.GetDynamicDataByKey(SignKeys.STACKITEMNUMBER);
        //int smallType=(int)_data.Result(FormulaKeys.FORMULA_115);
        //m_TestLabel.text="STACKITEMNUMBER : "+t_number+"smallType"+smallType;
        //m_TestLabel.text="Pet Q: "+Quality+"\nTime: "+TimeID+"\nType: "+Type+"\n id;"+id;

        if (true)
        {
            LevelAndNumberAndTime.text = "X " + t_number.ToString();
        }
        else
        {
            LevelAndNumberAndTime.text = "LV. " + _data.GetDynamicIntByKey(SignKeys.LEVEL);
        }
        switch (Quality)
        {
            case 0: m_Rare.spriteName = ""; break;
            case 1: m_Rare.spriteName = "groove_1"; break;
            case 2: m_Rare.spriteName = "groove_2"; break;
            case 3: m_Rare.spriteName = "groove_3"; break;
            case 4: m_Rare.spriteName = "groove_4"; break;
        }
    }

    /// <summary>
    /// 显示宝箱
    /// </summary>
    /// <param name="_data">Data.</param>
    private void ShowChest(FormulaHost _data)
    {
        //	ushort id=(ushort)_data.GetDynamicDataByKey("ID");
        //_data.Result(FormulaKeys.FORMULA_90);
        int Quality = (int)_data.GetDynamicDataByKey(SignKeys.QUALITY);
        //	int TimeID =(int)_data.GetDynamicDataByKey(SignKeys.BAGINID);
        //	int Type =(int)_data.GetDynamicDataByKey(SignKeys.TYPE);
        //int alltime=(int)_data.Result(FormulaKeys.FORMULA_94);
        int t_number = (int)_data.GetDynamicDataByKey(SignKeys.STACKITEMNUMBER);
        //m_TestLabel.text="STACKITEMNUMBER : "+t_number;
        //m_TestLabel.text="Chest Q: "+Quality+"\nTime: "+TimeID+"\nType: "+Type+"\n id;"+id;
        LevelAndNumberAndTime.text = ChestManageComponent.Instance.GetBagCellTime(_data);
        m_ItemIcon.gameObject.SetActive(false);
        switch (Quality)
        {
            case 0: m_Rare.spriteName = ""; break;
            case 1: m_Rare.spriteName = "groove_1"; break;
            case 2: m_Rare.spriteName = "groove_2"; break;
            case 3: m_Rare.spriteName = "groove_3"; break;
            case 4: m_Rare.spriteName = "groove_4"; break;
        }
        ShowCubeInBag(Quality - 1);
    }

    /// <summary>
    /// 显示宝箱3D
    /// </summary>
    /// <param name="_index">Index.</param>
    private void ShowCubeInBag(int _index)
    {
        for (int i = 0; i < 4; i++)
        {
            if (i == _index)
            {
                m_ArrCube[i].SetActive(true);
                m_ArrCube[i].GetComponent<Animator>().Play(string.Format("cube_{0}_unlocking", i + 1));
            }
            else
            {
                m_ArrCube[i].SetActive(false);
            }
        }
    }

    #endregion 初始化UICell

    //	void ChoseTag()
    //	{
    //		m_ChosedTag.SetActive(false);
    //	}
    /// <summary>
    /// 点击背包物品栏
    /// </summary>
    public void ClickItemCell()
    {
        if (m_host == null)
            return;
        if (bagPanel2.GetBagPanelState() == BagPanel2State.BagPanel2_ShowAll)
        {
            if (bagPanel2.GetBagPanelState2() == BagPanelState2.BagPanelState2_Normal)
            {
                string ItemStyel = m_host.GetFileName();
                if (ItemStyel == "Chest")
                {
                    Debug.Log("选择宝箱");
                    //CommonPanel.GetInstance().ShowChestEquip(m_host);
                    return;
                }
                else
                {
                    UIManageSystem.g_Instance.AddUI(UIManageSystem.UIITEMINFOPANEL, 1, m_host);
                }
            }
            else if (bagPanel2.GetBagPanelState2() == BagPanelState2.BagPanelState2_Sale)
            {
                if (m_host.GetDynamicDataByKey("CHOSED") >= 1)
                {
                    int MaxItemNumber = m_host.GetDynamicIntByKey(SignKeys.STACKITEMNUMBER);
                    int ChoseNumber = m_host.GetDynamicIntByKey("CHOSED");
                    if (ChoseNumber < MaxItemNumber)//选择的数量小于最大数量
                    {
                        if (GetAllChoseItemNumber() == 20)//从20个过来选满20个 取消复数选择
                        {
                            ChoseNumber = 0;
                            m_ChosedTag.SetActive(false);
                            ItemManageComponent.Instance.GetChosedItem.Remove(m_host);
                        }
                        else
                        {
                            ChoseNumber++;
                        }
                        m_host.SetDynamicData("CHOSED", ChoseNumber);
                    }
                    else//现在数量等于最大数量  再点击就 边城未选择
                    {
                        m_host.SetDynamicData("CHOSED", 0);
                        m_ChosedTag.SetActive(false);
                        ItemManageComponent.Instance.GetChosedItem.Remove(m_host);
                    }
                }
                else
                {
                    if (GetAllChoseItemNumber() == 20)//从20个过来选满20个 屏蔽点击消息
                    {
                        return;
                    }
                    m_host.SetDynamicData("CHOSED", 1);
                    m_ChosedTag.SetActive(true);
                    ItemManageComponent.Instance.GetChosedItem.Add(m_host);
                }
                SetItemPlural();
                if (GetAllChoseItemNumber() == 20)//到达20个
                {
                    Messenger.Broadcast(BroadCast_ShowMask);
                }
                else
                {
                    Messenger.Broadcast(BroadCast_HideMask);
                }
                m_bagPanel.SetSaleInfo();
            }
        }
        else if (bagPanel2.GetBagPanelState() == BagPanel2State.BagPanel2_ShowCube)
        {
            if (bagPanel2.GetBagPanelState2() == BagPanelState2.BagPanelState2_Replace)
            {
                //				int index=(int)bagPanel2.m_MysticalHost.GetDynamicDataByKey(SignKeys.CHESTQUEUE);
                //				Debug.Log("ChestManagcount:"+ChestManageComponent.Instance.GetChestList.Count+"index:"+index);
                //				bagPanel2.m_MysticalHost.SetDynamicData(SignKeys.CHESTQUEUE,0);
                //				m_host.SetDynamicData(SignKeys.CHESTQUEUE,index);
                //				FormulaHost.SaveList(new List<FormulaHost>{m_host,bagPanel2.m_MysticalHost},new HttpEndResponseDelegate(RepleaseChestCallBack));
                //			//	ChestManageComponent.Instance.GetChestList[index-1]=m_host;
                //				ChestManageComponent.Instance.GetChestList.Add(m_host);
                //				ChestManageComponent.Instance.GetChestList.Remove(bagPanel2.m_MysticalHost);
                //				ItemManageComponent.Instance.GetChestList.Add(bagPanel2.m_MysticalHost);
                //				ItemManageComponent.Instance.GetChestList.Remove(m_host);
            }
            else
            {
                //CommonPanel.GetInstance().ShowChestEquip(m_host);
            }
        }
        else if (bagPanel2.GetBagPanelState() == BagPanel2State.BagPanel2_SHowPet)
        {
            if (bagPanel2.GetBagPanelState2() == BagPanelState2.BagPanelState2_Equiped)
            {
                //				if(	  bagPanel2.GetBagPanelEquipPlace()==BagPanelEquipPlace.BagPanelEquipPlace_Pet1		///???
                //					||bagPanel2.GetBagPanelEquipPlace()==BagPanelEquipPlace.BagPanelEquipPlace_Pet2		///???
                //					||bagPanel2.GetBagPanelEquipPlace()==BagPanelEquipPlace.BagPanelEquipPlace_Pet3		///???
                //				)
                //				{
                int tindex = (int)bagPanel2.GetBagPanelEquipPlace() % 10;

                if (PetManageComponent.Instance.CheckHaveSamePet(m_host, tindex))
                {
                    CommonPanel.GetInstance().ShowText("有相同装备");
                    return;
                }
                m_host.SetDynamicData(SignKeys.EQUIPEDQUEUE, tindex + RoleManageComponent.Instance.GetFightGirlIndex() * 10);
                PetManageComponent.Instance.GetListEquipedPetHosts.Add(m_host);
                ItemManageComponent.Instance.GetPetList.Remove(m_host);
                m_host.Save(new HttpResponseDelegate(EquipedPetCallBack));
            }
            else if (bagPanel2.GetBagPanelState2() == BagPanelState2.BagPanelState2_Replace)
            {
                int tindex = (int)bagPanel2.GetBagPanelEquipPlace() % 10;
                FormulaHost EquipedItem = PetManageComponent.Instance.GetEquipedPet(tindex);            //原先装备的东西
                if (PetManageComponent.Instance.CheckHaveSamePet(m_host, tindex))
                {
                    CommonPanel.GetInstance().ShowText("有相同装备");
                    return;
                }
                m_host.SetDynamicData(SignKeys.EQUIPEDQUEUE, tindex + RoleManageComponent.Instance.GetFightGirlIndex() * 10);
                EquipedItem.SetDynamicData(SignKeys.EQUIPEDQUEUE, 0);
                PetManageComponent.Instance.GetListEquipedPetHosts.Add(m_host);
                PetManageComponent.Instance.GetListEquipedPetHosts.Remove(EquipedItem);
                ItemManageComponent.Instance.GetPetList.Remove(m_host);
                ItemManageComponent.Instance.GetPetList.Add(EquipedItem);
                FormulaHost.SaveList(new List<FormulaHost> { m_host, EquipedItem }, new HttpEndResponseDelegate(PetRepleaseCallBack));

                //m_host.Save(new HttpResponseDelegate(EquipedEquipCallback));
                //EquipedItem.Save(new HttpResponseDelegate(EquipedEquipCallback));
                CommonPanel.GetInstance().ShowWaittingPanel(true);
                //Debug.Log("替换宠物");
            }
        }
        else if (bagPanel2.GetBagPanelState() == BagPanel2State.BagPanel2_ShowWeapon || bagPanel2.GetBagPanelState() == BagPanel2State.BagPanel2_ShowChip)
        {
            if (bagPanel2.GetBagPanelState2() == BagPanelState2.BagPanelState2_Equiped)
            {
                int tindex = (int)bagPanel2.GetBagPanelEquipPlace() % 10;
                if (EquipManageComponent.Instance.CheckHaveSameEquip(m_host, tindex))
                {
                    CommonPanel.GetInstance().ShowText("有相同装备");
                    return;
                }
                m_host.SetDynamicData(SignKeys.EQUIPEDQUEUE, tindex + RoleManageComponent.Instance.GetFightGirlIndex() * 10);
                EquipManageComponent.Instance.GetEquipedEquipList.Add(m_host);
                ItemManageComponent.Instance.GetEquipList.Remove(m_host);
                m_host.Save(new HttpResponseDelegate(EquipedEquipCallback));
                CommonPanel.GetInstance().ShowWaittingPanel(true);
            }
            else if (bagPanel2.GetBagPanelState2() == BagPanelState2.BagPanelState2_Replace)
            {
                int tindex = (int)bagPanel2.GetBagPanelEquipPlace() % 10;
                FormulaHost EquipedItem = EquipManageComponent.Instance.GetEquipedEquip(tindex);            //原先装备的东西
                if (EquipManageComponent.Instance.CheckHaveSameEquip(m_host, tindex))
                {
                    CommonPanel.GetInstance().ShowText("有相同装备");
                    return;
                }
                m_host.SetDynamicData(SignKeys.EQUIPEDQUEUE, tindex + RoleManageComponent.Instance.GetFightGirlIndex() * 10);
                EquipedItem.SetDynamicData(SignKeys.EQUIPEDQUEUE, 0);
                EquipManageComponent.Instance.GetEquipedEquipList.Add(m_host);
                EquipManageComponent.Instance.GetEquipedEquipList.Remove(EquipedItem);
                ItemManageComponent.Instance.GetEquipList.Remove(m_host);
                ItemManageComponent.Instance.GetEquipList.Add(EquipedItem);
                FormulaHost.SaveList(new List<FormulaHost> { m_host, EquipedItem }, new HttpEndResponseDelegate(EquipRepleaseCallBack));
                CommonPanel.GetInstance().ShowWaittingPanel(true);
            }
        }
        else if (bagPanel2.GetBagPanelState2() == BagPanelState2.BagPanelState2_Chosed)
        {
            if (m_host.GetDynamicDataByKey("CHOSED") >= 1)
            {
                int MaxItemNumber = m_host.GetDynamicIntByKey(SignKeys.STACKITEMNUMBER);
                int ChoseNumber = m_host.GetDynamicIntByKey("CHOSED");
                if (ChoseNumber < MaxItemNumber)//选择的数量小于最大数量
                {
                    if (GetAllChoseItemNumber() == 4)//从20个过来选满20个 取消复数选择
                    {
                        ChoseNumber = 0;
                        m_ChosedTag.SetActive(false);
                        ItemManageComponent.Instance.GetChosedItem.Remove(m_host);
                    }
                    else
                    {
                        ChoseNumber++;
                    }
                    m_host.SetDynamicData("CHOSED", ChoseNumber);
                }
                else//现在数量等于最大数量  再点击就 边城未选择
                {
                    m_host.SetDynamicData("CHOSED", 0);
                    m_ChosedTag.SetActive(false);
                    ItemManageComponent.Instance.GetChosedItem.Remove(m_host);
                }
            }
            else
            {
                if (GetAllChoseItemNumber() == 4)//从20个过来选满20个 屏蔽点击消息
                {
                    return;
                }
                m_host.SetDynamicData("CHOSED", 1);
                m_ChosedTag.SetActive(true);
                ItemManageComponent.Instance.GetChosedItem.Add(m_host);
            }
            SetItemPlural();
            if (GetAllChoseItemNumber() == 4)//到达20个
            {
                Messenger.Broadcast(BroadCast_ShowMask);
            }
            else
            {
                Messenger.Broadcast(BroadCast_HideMask);
            }
            m_bagPanel.SetSaleInfo();
        }
    }

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void OnEnable()
    {
        Messenger.MarkAsPermanent(BroadCast_HideMask);
        Messenger.MarkAsPermanent(BroadCast_ShowMask);
        Messenger.MarkAsPermanent(BraodCast_HideChoseTag);
        Messenger.MarkAsPermanent(BraodCast_BagSaleItemPluralNumber);
        Messenger.AddListener<bool>(BraodCast_BagSaleItemPluralNumber, SetItemPlural);
        Messenger.AddListener(BroadCast_HideMask, HideMask);
        Messenger.AddListener(BroadCast_ShowMask, ShowMask);
        Messenger.AddListener(BraodCast_HideChoseTag, HideChoseTag);
    }

    private void OnDisable()
    {
        Messenger.RemoveListener(BroadCast_HideMask, HideMask);
        Messenger.RemoveListener(BroadCast_ShowMask, ShowMask);
        Messenger.RemoveListener(BraodCast_HideChoseTag, HideChoseTag);
        Messenger.RemoveListener<bool>(BraodCast_BagSaleItemPluralNumber, SetItemPlural);
    }

    #region 背包物品的滑动

    public UIScrollView scrollView;

    private void OnPress(bool pressed)
    {
        if (scrollView && enabled && NGUITools.GetActive(gameObject))
        {
            scrollView.Press(pressed);
        }
    }

    private void OnDrag(Vector2 delta)
    {
        if (scrollView && NGUITools.GetActive(this))
        {
            scrollView.Drag();
        }
    }

    #endregion 背包物品的滑动

    #region 日了狗的复数选择

    public int GetAllChoseItemNumber()
    {
        int NUmber = 0;
        for (int i = 0, max = ItemManageComponent.Instance.GetChosedItem.Count; i < max; i++)
        {
            NUmber += ItemManageComponent.Instance.GetChosedItem[i].GetDynamicIntByKey(SignKeys.CHOSED);
        }
        return NUmber;
    }

    #endregion 日了狗的复数选择

    #region 所有的回调

    public void EquipedPetCallBack(bool _success)
    {
        CommonPanel.GetInstance().ShowWaittingPanel(false);
        if (_success)
        {
            UIManageSystem.g_Instance.RomoveUI();
            //m_bagPanel.gameObject.SetActive(false);
            Messenger.Broadcast(PetCell.BroadCastRefreshEquipedPetUI);
            Messenger.Broadcast(CharactorPanel2.BroadCast_SetHeroData);
        }
        else
        {
            CommonPanel.GetInstance().ShowText("Conect fail");
        }
    }

    public void PetRepleaseCallBack(cn.bmob.response.EndPointCallbackData<Hashtable> response)
    {
        CommonPanel.GetInstance().ShowWaittingPanel(false);
        UIManageSystem.g_Instance.RomoveUI();
        UIManageSystem.g_Instance.RomoveUI();
        Messenger.Broadcast(PetCell.BroadCastRefreshEquipedPetUI);
        Messenger.Broadcast(CharactorPanel2.BroadCast_SetHeroData);
    }

    /// <summary>
    /// 装上装备的回调
    /// </summary>
    public void EquipedEquipCallback(bool _success)
    {
        CommonPanel.GetInstance().ShowWaittingPanel(false);
        if (_success)
        {
            UIManageSystem.g_Instance.RomoveUI();
            Messenger.Broadcast(EquipCell.BroadCastRefreshEquipedEquipUI);
            Messenger.Broadcast(CharactorPanel2.BroadCast_SetHeroData);
        }
        else
        {
            CommonPanel.GetInstance().ShowText("Conect fail");
        }
    }

    public void EquipRepleaseCallBack(cn.bmob.response.EndPointCallbackData<Hashtable> response)
    {
        CommonPanel.GetInstance().ShowWaittingPanel(false);
        Messenger.Broadcast(CharactorPanel2.BroadCast_SetHeroData);
        UIManageSystem.g_Instance.RomoveUI();
        UIManageSystem.g_Instance.RomoveUI();
        Messenger.Broadcast(EquipCell.BroadCastRefreshEquipedEquipUI);
    }

    //	/// <summary>
    //	/// 替换宝箱的回调
    //	/// </summary>
    //	public void RepleaseChestCallBack(cn.bmob.response.EndPointCallbackData<Hashtable> response)
    //	{
    //			//Messenger.Broadcast(LevelPrepaerPanel.BraodCast_RestChestEmpty);
    //			UIManageSystem.g_Instance.RomoveUI();
    //	}

    #endregion 所有的回调
}