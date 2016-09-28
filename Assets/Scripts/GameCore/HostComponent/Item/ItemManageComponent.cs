///自定义模块，可定制模块具体行为
using System;
using System.Collections.Generic;
using FormulaBase;
using System.Collections;
using UnityEngine;


public class RewardData
{
	public int id;		//物品品质
	public int type;	//物品类型
	public int Quality;	//物品品质

}

namespace FormulaBase {
	public class ItemManageComponent : CustomComponentBase {
		public enum ItemCritStyel
		{
			CRITE_ONE,
			CRITE_ONEPOINTFIVE,
			CRITE_TWO
		}
		private static ItemManageComponent instance = null;
		private const int HOST_IDX = 7;
		public static ItemManageComponent Instance {
			get {
				if(instance == null) {
					instance = new ItemManageComponent();
				}
			return instance;
			}
		}
		private FormulaHost itemHost = null;
		public FormulaHost GetItemHost() {
			if (this.itemHost == null) {
				this.itemHost = FomulaHostManager.Instance.CreateHost (HostKeys.HOST_7);
			}
			return this.itemHost;
		}
		#region 选择的东西
		List<FormulaHost> m_ListChosedItem=new List<FormulaHost>();			//选择的东西		通用的选择的东西
		public List<FormulaHost> GetChosedItem
		{
			get
			{
				return m_ListChosedItem;
			}
			set
			{
				m_ListChosedItem=value;
			}
		}

		public void ClearChosedItem()
		{
			for(int i=0,max=m_ListChosedItem.Count;i<max;i++)
			{
				m_ListChosedItem[i].SetDynamicData(SignKeys.CHOSED,0);
			}
			m_ListChosedItem.Clear();
		}
		public int GetChoseCount()
		{
			int Number=0;
			for(int i=0,max=GetChosedItem.Count;i<max;i++)
			{
				Number+=GetChosedItem[i].GetDynamicIntByKey(SignKeys.CHOSED);
			}
			return Number;
		}
//
		#endregion
		#region 所有物品
		List<FormulaHost> m_AllItem=new List<FormulaHost>();
		public List<FormulaHost> GetAllItem
		{
			get
			{
			//	BulidAllList();
				return m_AllItem;
			}
		}
		void BulidAllList()
		{
			m_AllItem.Clear();
			for(int i=0,max=m_material.Count;i<max;i++)
			{
				m_AllItem.Add(m_material[i]);
			}
			for(int i=0,max=m_Equip.Count;i<max;i++)
			{
				m_AllItem.Add(m_Equip[i]);
			}
			for(int i=0,max=m_pet.Count;i<max;i++)
			{
				m_AllItem.Add(m_pet[i]);
			}
			for(int i=0,max=m_Chest.Count;i<max;i++)
			{
				m_AllItem.Add(m_Chest[i]);
			}
		}
		#endregion
		#region 材料链表
		List<FormulaHost> m_material=new List<FormulaHost>();
		public List<FormulaHost> GetMaterialList
		{
			get
			{
				return m_material;
			}
			set
			{
				m_material=value;
			}
			
		}
		#endregion

		#region 装备链表
		List<FormulaHost> m_Equip=new List<FormulaHost>();

		public List<FormulaHost> GetEquipList
		{
			get
			{
				return m_Equip;
			}
			set
			{
				m_Equip=value;
			}
		}
		#endregion

		#region 宠物链表
		List<FormulaHost> m_pet=new List<FormulaHost>();
		public List<FormulaHost> GetPetList
		{
			get
			{
				return m_pet;
			}
			set
			{
				m_pet=value;
			}

		}
		#endregion
		#region 宝箱
		List<FormulaHost> m_Chest=new List<FormulaHost>();
		public List<FormulaHost> GetChestList
		{
			get
			{
				return m_Chest;
			}
			set
			{
				m_Chest=value;
			}
		}
		#endregion

		public void Init() {
			this.itemHost = FomulaHostManager.Instance.LoadHost (HOST_IDX);
			//加载所有素材
			Dictionary<string, FormulaHost> _materialdic = FomulaHostManager.Instance.GetHostListByFileName ("Material");
			if (_materialdic == null) {
				Debugger.LogWarning ("_materialdic is null");
			} else {
				m_material.Clear ();
				Debugger.LogWarning ("角色拥有的材料数量:" + _materialdic.Count);
				foreach (string oid in _materialdic.Keys) {
					m_material.Add (_materialdic [oid]);
					int temp = (int)_materialdic [oid].Result (FormulaKeys.FORMULA_45);
					if (temp > id) {
						id = temp + 1;
					}
				}
				//	Debugger.Log("material count is :"+m_material.Count);
			}
			//加载所有装备
			Dictionary<string, FormulaHost> _EquipList = FomulaHostManager.Instance.GetHostListByFileName ("Equip");
			if (_EquipList == null) {
				Debugger.LogWarning ("_EquipList is null");
			} else {
				m_Equip.Clear ();
				Debugger.LogWarning ("角色拥有的装备数量:" + _EquipList.Count);
				foreach (string oid in _EquipList.Keys) {

					if (_EquipList [oid].GetDynamicDataByKey (SignKeys.EQUIPEDQUEUE) == 0) {
						m_Equip.Add (_EquipList [oid]);
					} else {
						EquipManageComponent.Instance.AddEquipedItem (_EquipList [oid]);
					}
					int temp = (int)_EquipList [oid].GetDynamicDataByKey (SignKeys.BAGINID);
					if (temp > id) {
						id = temp + 1;
					}
				}
			}
			//加载所有宠物
			Dictionary<string, FormulaHost> _PetList = FomulaHostManager.Instance.GetHostListByFileName ("Pet");
			if (_PetList == null) {
				Debugger.LogWarning ("Pet is null");
			} else {
				m_pet.Clear ();
				Debugger.LogWarning ("角色拥有的宠物碎片和宠物数量:" + _PetList.Count);
				foreach (string oid in _PetList.Keys) {

					if (_PetList [oid].GetDynamicDataByKey (SignKeys.EQUIPEDQUEUE) == 0) {
						m_pet.Add (_PetList [oid]);
					} else {
						PetManageComponent.Instance.AddEquipedPet (_PetList [oid]);
					}
					int temp = (int)_PetList [oid].GetDynamicDataByKey (SignKeys.BAGINID);

					if (temp > id) {
						id = temp + 1;
					}
				}
				CheckChestTime ();
			}
			//加载所有宝箱
			Dictionary<string, FormulaHost> _ChestList = FomulaHostManager.Instance.GetHostListByFileName ("Chest");
			if (_ChestList == null) {
				Debugger.LogWarning ("Chest is null");
			} else {
				Debugger.LogWarning ("角色拥有的宝箱个数:" + _ChestList.Count);
				m_Chest.Clear ();
				foreach (string oid in _ChestList.Keys) {
					int Type = (int)_ChestList [oid].GetDynamicDataByKey (SignKeys.CHESTQUEUE);
					if (Type == 0) {//判断是不是在开启栏的宝箱
						m_Chest.Add (_ChestList [oid]);
					} else {

						if (_ChestList [oid].GetDynamicIntByKey (SignKeys.CHESTREMAINING_TIME) > 0) {
							ChestManageComponent.Instance.GetChestList.Add (_ChestList [oid]);
						} else {
							ChestManageComponent.Instance.GetTimeDownChest.Add (_ChestList [oid]);
						}
					}
					int temp = (int)_ChestList [oid].GetDynamicDataByKey (SignKeys.BAGINID);
					if (temp > id) {
						id = temp + 1;
					}
				}
			}
		}

		public void CheckChestTime() {
			if (ChestManageComponent.Instance.GetChestList.Count <= 0) {
				return;
			}

			FormulaHost temp = ChestManageComponent.Instance.GetTaggetQueue (1);
			int allLeftTime = temp.GetRealTimeCountDownNow ();
			Debugger.Log ("ChestALL Time:" + allLeftTime);
			if (allLeftTime > 0)
				return;
			
			temp.SetDynamicData (SignKeys.CHESTREMAINING_TIME, 0);
			//计算时间
			int moveNumber = 1;
			for (int i = 2, max = ChestManageComponent.Instance.GetChestList.Count + 1; i < max; i++) {
				FormulaHost tempNow = ChestManageComponent.Instance.GetTaggetQueue (i);
				int OpenTime = tempNow.GetDynamicIntByKey (SignKeys.CHESTREMAINING_TIME);
				if (allLeftTime + OpenTime < 0) {
					tempNow.SetDynamicData (SignKeys.CHESTREMAINING_TIME, 0);
					tempNow.SetRealTimeCountDown (0);
					allLeftTime += OpenTime;
					moveNumber++;
				} else {
					tempNow.SetDynamicData (SignKeys.CHESTREMAINING_TIME, allLeftTime + OpenTime);
					tempNow.SetRealTimeCountDown (allLeftTime + OpenTime);
				}
			}

			//刷新队列
			for (int i = 0, max = ChestManageComponent.Instance.GetChestList.Count; i < max; i++) {
				int que = ChestManageComponent.Instance.GetChestList [i].GetDynamicIntByKey (SignKeys.CHESTQUEUE);
				if (ChestManageComponent.Instance.GetChestList [i].GetDynamicIntByKey (SignKeys.CHESTREMAINING_TIME) != 0) {
					ChestManageComponent.Instance.GetChestList [i].SetDynamicData (SignKeys.CHESTQUEUE, que - moveNumber);
				} else {
					ChestManageComponent.Instance.GetChestList [i].SetDynamicData (SignKeys.CHESTQUEUE, AccountManagerComponent.Instance.GetChestGirdNumber () - ChestManageComponent.Instance.GetTimeDownChest.Count - que + 1);
				}
			}

			int count1 = ChestManageComponent.Instance.GetTimeDownChest.Count;
			int count2 = ChestManageComponent.Instance.GetChestList.Count;

			for (int i = 0; i < ChestManageComponent.Instance.GetChestList.Count; i++) {
				//int que=ChestManageComponent.Instance.GetChestList[i].GetDynamicIntByKey(SignKeys.CHESTQUEUE);
				if (ChestManageComponent.Instance.GetChestList [i].GetDynamicIntByKey (SignKeys.CHESTREMAINING_TIME) == 0) {
					ChestManageComponent.Instance.GetTimeDownChest.Add (ChestManageComponent.Instance.GetChestList [i]);
					ChestManageComponent.Instance.GetChestList.Remove (ChestManageComponent.Instance.GetChestList [i]);
					i--;
				}
			}

			int count3 = ChestManageComponent.Instance.GetTimeDownChest.Count;
			int count4 = ChestManageComponent.Instance.GetChestList.Count;

			List<FormulaHost> All = new List<FormulaHost> ();
			All.AddRange (ChestManageComponent.Instance.GetChestList);
			All.AddRange (ChestManageComponent.Instance.GetTimeDownChest);
			FormulaHost.SaveList (All, new HttpEndResponseDelegate (this.CheckChestTimeCallBack));
		}

		public void CheckChestTimeCallBack(cn.bmob.response.EndPointCallbackData<Hashtable> response) {	
		}

		int id=0;//装备获取ID
		public int GetItemTimeId {
			get {
				return id;
			}
			set {
				id = value;
			}
		}

		public void DeleteAllItem() {
			Debug.Log ("删除所有物品");
			m_Equip.Clear ();
			m_material.Clear ();
			m_pet.Clear ();
			m_Chest.Clear ();
		}

		public void DeleteItem(FormulaHost _host) {
			string temp = _host.GetFileName ();
			switch (temp) {
			case "Equip":
				NGUIDebug.Log ("Sale Equip");
				m_Equip.Remove (_host);
				break;
			case "Material":
				NGUIDebug.Log ("Sale Mtaerial");
				m_material.Remove (_host);
				break;
			case "Pet":
				NGUIDebug.Log ("Sale Pet");
				m_pet.Remove (_host);
				break;
			case "Chest":
				NGUIDebug.Log ("Sale Chest");
				m_Chest.Remove (_host);
				break;
			}

			_host.Delete (new HttpResponseDelegate (DeleteItemCallBack));
		}

		private void DeleteItemLoacl(List<FormulaHost> _listhost ) {
			for (int i = 0, max = _listhost.Count; i < max; i++) {
				string temp = _listhost [i].GetFileName ();
				switch (temp) {
				case "Equip":
					NGUIDebug.Log ("Sale Equip");
					m_Equip.Remove (_listhost [i]);
					break;
				case "Material":
					NGUIDebug.Log ("Sale Mtaerial");
					m_material.Remove (_listhost [i]);
					break;
				case "Pet":
					NGUIDebug.Log ("Sale Pet");
					m_pet.Remove (_listhost [i]);
					break;
				case "Chest":
					NGUIDebug.Log ("Sale Chest");
					m_Chest.Remove (_listhost [i]);
					break;
				}
			}
		}

		public void DeleteListItem(List<FormulaHost> _listhost) {
			List<FormulaHost> tDeleteItem = new List<FormulaHost> ();
			List<FormulaHost> tReduceItem = new List<FormulaHost> ();
			for (int i = 0; i < _listhost.Count; i++) {
				int number = _listhost [i].GetDynamicIntByKey (SignKeys.STACKITEMNUMBER);
				if (number > 1) {
					int ChosedNumber = _listhost [i].GetDynamicIntByKey (SignKeys.CHOSED);
					number -= ChosedNumber;
					if (number == 0) {
						tDeleteItem.Add (_listhost [i]);
						//	DeleteItem();
					} else {
						_listhost [i].SetDynamicData (SignKeys.STACKITEMNUMBER, number);
						_listhost [i].SetDynamicData (SignKeys.CHOSED, 0);
						tReduceItem.Add (_listhost [i]);
						//	.Save();
					}
				} else {
					tDeleteItem.Add (_listhost [i]);
				}
			}
			DeleteItemLoacl (tDeleteItem);
			FormulaHost.SaveList (tReduceItem, new HttpEndResponseDelegate (DeleteListCallBack));
			FormulaHost.DeleteList (tDeleteItem, new HttpEndResponseDelegate (DeleteListCallBack));
			CommonPanel.GetInstance ().ShowWaittingPanel (true);
		}

		public void DeleteListCallBack(cn.bmob.response.EndPointCallbackData<Hashtable> response) {
			CommonPanel.GetInstance ().ShowWaittingPanel (false);
		}

		public void UseItemList(List<FormulaHost> _listHost) {
			List<FormulaHost> tDeleteItem = new List<FormulaHost> ();
			List<FormulaHost> tReduceItem = new List<FormulaHost> ();
			for (int i = 0; i < _listHost.Count; i++) {
				int ChosedNumber = _listHost [i].GetDynamicIntByKey (SignKeys.CHOSED);
				int HaveNumber = _listHost [i].GetDynamicIntByKey (SignKeys.STACKITEMNUMBER);

				if (ChosedNumber < HaveNumber) {//选择的小与堆叠的
					_listHost [i].SetDynamicData (SignKeys.CHOSED, 0);
					_listHost [i].SetDynamicData (SignKeys.STACKITEMNUMBER, HaveNumber - ChosedNumber);
					tReduceItem.Add (_listHost [i]);
					//	_listHost[i]
				} else {
					_listHost [i].IsDelete = true;
					tReduceItem.Add (_listHost [i]);
					tDeleteItem.Add (_listHost [i]);
				}
			}

			//FormulaHost.DeleteList(tDeleteItem);
			DeleteItemLoacl (tDeleteItem);
			FormulaHost.SaveList (tReduceItem, new HttpEndResponseDelegate (UseItemListCallBack));	
			CommonPanel.GetInstance ().ShowWaittingPanel (true);
		}

		public void UseItemListCallBack(cn.bmob.response.EndPointCallbackData<Hashtable> response) {
			CommonPanel.GetInstance ().ShowWaittingPanel (false);
		}

		public void SaleItem(FormulaHost _host,HttpResponseDelegate _callback=null) {
			string fileName = _host.GetFileName ();
			int Monye = 0;
			int StackitemNumber = _host.GetDynamicIntByKey (SignKeys.STACKITEMNUMBER);
			if (StackitemNumber > 1) {
				_host.SetDynamicData (SignKeys.STACKITEMNUMBER, StackitemNumber - 1);
				int salePrice = this.GetItemMoney (_host);
				AccountGoldManagerComponent.Instance.ChangeMoney (salePrice, true, new HttpResponseDelegate (((bool result) => {
					if (_callback == null) {
						_host.Save (new HttpResponseDelegate (this.DeleteItemCallBack));
						CommonPanel.GetInstance ().ShowWaittingPanel (true);
						Messenger.Broadcast (bagPanel2.BroadcastBagItem);
					} else {
						_host.Save (new HttpResponseDelegate (_callback));
						CommonPanel.GetInstance ().ShowWaittingPanel (true);
					}
				})));
			} else {
				switch (fileName) {
				case "Equip":
					Debug.Log ("Sale Equip");	
					this.m_Equip.Remove (_host);
					break;
				case "Material":
					Debug.Log ("Sale Mtaerial");		
					this.m_material.Remove (_host);
					break;
				case "Pet":
					Debug.Log ("Sale Pet");		
					this.m_pet.Remove (_host);
					break;
				case "Chest":
					Debug.Log ("Sale Chest");
					this.m_Chest.Remove (_host);
					break;
				}

				int salePrice = this.GetItemMoney (_host);
				AccountGoldManagerComponent.Instance.ChangeMoney (salePrice, true, new HttpResponseDelegate (((bool result) => {
					if (_callback == null) {
						_host.Delete (new HttpResponseDelegate (this.DeleteItemCallBack));
						CommonPanel.GetInstance ().ShowWaittingPanel (true);
						Messenger.Broadcast (bagPanel2.BroadcastBagItem);
					} else {
						_host.Delete (new HttpResponseDelegate (_callback));
						CommonPanel.GetInstance ().ShowWaittingPanel (true);
					}
				})));
			}
		}

		/// <summary>
		/// 删除物品的反馈
		/// </summary>
		public void DeleteItemCallBack(bool _success) {
			CommonPanel.GetInstance ().ShowWaittingPanel (false);
			if (true) {
				//	UIManageSystem.g_Instance.RomoveUI();
				//刷新背包
			} else {
				NGUIDebug.Log ("connet is fail");
			}
		}

		public void LockItem(FormulaHost _host,bool _Locked) {
			_host.SetDynamicData(SignKeys.LOCKED,_Locked==false?1:0);
			_host.Save(new HttpResponseDelegate(LockedCallBack));
			CommonPanel.GetInstance().ShowWaittingPanel();
		//	NGUIDebug.Log("存档报错");
		}

		public void LockedCallBack(bool _success) {
			if (_success) {
				CommonPanel.GetInstance ().ShowWaittingPanel (false);
			} else {
				CommonPanel.GetInstance ().ShowText ("connect is fail", null, false);
			}
		}

		public void AddItem(FormulaHost _host) {
			BagManageComponent.Instance.SetBagHaveNew ();
			CommonPanel.GetInstance ().ShowWaittingPanel ();
			//bool stack=false;
			string itemType = _host.GetFileName ();
			FormulaHost t_host = null;
			switch (itemType) {
			case "Equip":
				if (_host.GetDynamicIntByKey (SignKeys.BAGINID) == 0) {//没有的东西
					_host.SetDynamicData (SignKeys.BAGINID, id++);//添加获取物品 时间系数
					m_Equip.Add (_host);
				}
				_host.Save (new HttpResponseDelegate (CallBackAdditem));
				break;
			case "Material":
				t_host = materialManageComponent.Instance.HaveTheSameID (_host.GetDynamicIntByKey ("ID"));
				if (t_host == null) {//有没有材料
					_host.SetDynamicData (SignKeys.BAGINID, id++);//添加获取物品 时间系数
					m_material.Add (_host);
				} else {
					_host = t_host;
				}
				_host.Save (new HttpResponseDelegate (CallBackAdditem));
				break;
			case "Pet":
				if ((int)_host.GetDynamicDataByKey (SignKeys.SMALLlTYPE) == 6) {//碎片
					t_host = PetManageComponent.Instance.HaveTheSameID (_host.GetDynamicIntByKey ("ID"));
					if (t_host == null) {
						_host.SetDynamicData (SignKeys.BAGINID, id++);//添加获取物品 时间系数
						m_pet.Add (_host);
					} else {
						_host = t_host;
					}
					_host.Save (new HttpResponseDelegate (CallBackAdditem));
				} else {
					if (_host.GetDynamicIntByKey (SignKeys.BAGINID) == 0) {//没有的东西
						_host.SetDynamicData (SignKeys.BAGINID, id++);//添加获取物品 时间系数
					}
				}
				break;
			case "Chest":
				if (_host.GetDynamicIntByKey (SignKeys.BAGINID) == 0) {//没有的东西
					_host.SetDynamicData (SignKeys.BAGINID, id++);//添加获取物品 时间系数
					m_Chest.Add (_host);
				}
				_host.Save (new HttpResponseDelegate (CallBackAdditem));
				break;
			}
		}

		public void AddItemList(List<FormulaHost> _listHost) {
			BagManageComponent.Instance.SetBagHaveNew ();
			for (int i = 0; i < _listHost.Count; i++) {
				string itemType = _listHost [i].GetFileName ();
				FormulaHost t_host = null;
				switch (itemType) {
				case "Equip":
					if (_listHost [i].GetDynamicIntByKey (SignKeys.BAGINID) == 0) {//没有的东西
						_listHost [i].SetDynamicData (SignKeys.BAGINID, id++);//添加获取物品 时间系数
						m_Equip.Add (_listHost [i]);
					}
					break;
				case "Material":
					t_host = materialManageComponent.Instance.HaveTheSameID (_listHost [i].GetDynamicIntByKey ("ID"));
					if (t_host == null) {//有没有材料
						_listHost [i].SetDynamicData (SignKeys.BAGINID, id++);//添加获取物品 时间系数
						m_material.Add (_listHost [i]);
					} else {
						_listHost [i] = t_host;

					}
					break;
				case "Pet":
					_listHost [i].Result (FormulaKeys.FORMULA_91);
					if ((int)_listHost [i].GetDynamicDataByKey (SignKeys.SMALLlTYPE) == 6) {//碎片
						t_host = PetManageComponent.Instance.HaveTheSameID (_listHost [i].GetDynamicIntByKey ("ID"));
						if (t_host == null) {
							_listHost [i].SetDynamicData (SignKeys.BAGINID, id++);//添加获取物品 时间系数
							m_pet.Add (_listHost [i]);
						} else {
							_listHost [i] = t_host;
						}
					} else {
						if (_listHost [i].GetDynamicIntByKey (SignKeys.BAGINID) == 0) {//没有的东西
							_listHost [i].SetDynamicData (SignKeys.BAGINID, id++);//添加获取物品 时间系数
							m_pet.Add (_listHost [i]);
						}
					}
					break;
				case "Chest":
					if (_listHost [i].GetDynamicIntByKey (SignKeys.BAGINID) == 0) {//没有的东西
						Debugger.Log ("添加宝箱 : " + _listHost [i].objectID);
						_listHost [i].SetDynamicData (SignKeys.BAGINID, id++);//添加获取物品 时间系数
						m_Chest.Add (_listHost [i]);
					}
					break;
				}
			}

			FormulaHost.SaveList (_listHost, new HttpEndResponseDelegate (AddItemListCallBack));//所有的都是添加物品
			CommonPanel.GetInstance ().ShowWaittingPanel (true);
		}

		public int GetItemMoney(FormulaHost _host) {
			string hostname = _host.GetFileName ();
			int Money = 0;
			switch (hostname) {
			case "Equip":
				_host.Result (FormulaKeys.FORMULA_19);
				Money = _host.GetDynamicIntByKey (SignKeys.SOLD);
				break;
			case "Material":
				_host.Result (FormulaKeys.FORMULA_93);
				Money = _host.GetDynamicIntByKey (SignKeys.SOLD);
				break;
			case "Pet":		
				_host.Result (FormulaKeys.FORMULA_91);
				Money = _host.GetDynamicIntByKey (SignKeys.SOLD);
				break;
			case "Chest":  
				_host.Result (FormulaKeys.FORMULA_90);
				Money = _host.GetDynamicIntByKey (SignKeys.SOLD);	
				break;
			}

			return Money;
		}

		public void AddItemListCallBack(cn.bmob.response.EndPointCallbackData<Hashtable> response) {
			CommonPanel.GetInstance().ShowWaittingPanel(false);
			Messenger.Broadcast(bagPanel2.BroadcastBagItem);
			Messenger.Broadcast(bagPanel2.BroadcastBagUpSize);
		}

		public void AddItemLoacl(FormulaHost _host) {
			string temp = _host.GetFileName ();
			switch (temp) {
			case "Equip":
				m_Equip.Add (_host);
				break;
			case "Material":
				m_material.Add (_host);
				break;
			case "Pet":
				m_pet.Add (_host);
				break;
			case "Chest":
				m_Chest.Add (_host);
				break;
			}
		}

		public void CallBackAdditem(bool _temp) {
			CommonPanel.GetInstance().ShowWaittingPanel(false);

			if (!UISceneHelper.Instance.IsUiActive ("PnlSuitcase")) {
				return;
			}

			UISceneHelper.Instance.ReShowUi ("PnlSuitcase");
		}

		public void ItemLevelUp() {
			//FormulaHost host;
			//host.Delete();
		}

		public List<FormulaHost> GetAppointItem(int _BigType,int _smallType=-1) {
			switch (_BigType) {
			case 0:
				return GetChest (_BigType, _smallType);
			//break;
			case 1:
				return GetEquip (_BigType, _smallType);
			//	break;
			case 2:
				return GetEquip (_BigType, _smallType);
			//	break;
			case 3:
				return GetEquip (_BigType, _smallType);
			//	break;
			case 4:
				return GetPet (_BigType, _smallType);
			//	break;
			case 5:
				return GetMaterials (_BigType, _smallType);
			//	break;
			}
			return null;
		}

		List<FormulaHost> GetChest(int _BigType,int _smallType=-1) {
			return m_Chest;
		}

		//装备没有小类型
		List<FormulaHost> GetEquip(int _BigType,int _smallType=-1) {
			List<FormulaHost> temp = new List<FormulaHost> ();
			for (int i = 0, max = m_Equip.Count; i < max; i++) {
				m_Equip [i].Result (FormulaKeys.FORMULA_19);


				int tt = (int)m_Equip [i].GetDynamicDataByKey (SignKeys.TYPE);
//				Debugger.Log("Equip Type:"+tt);
				if ((int)m_Equip [i].GetDynamicDataByKey (SignKeys.TYPE) == _BigType)
					temp.Add (m_Equip [i]);
			}
			return temp;
		}

		List<FormulaHost> GetPet(int _BigType,int _smallType=-1) {
			List<FormulaHost> temp = new List<FormulaHost> ();
			if (_smallType == -1) {
				return m_pet;
			} else {
				for (int i = 0, max = m_pet.Count; i < max; i++) {
					if (m_pet [i].Result (FormulaKeys.FORMULA_115) == _smallType) {
						temp.Add (m_pet [i]);
					}
				}
				return temp;
			}
		}

		List<FormulaHost> GetMaterials(int _BigType,int _smallType=-1) {
			List<FormulaHost> temp = new List<FormulaHost> ();
			if (_smallType == -1) {
				return m_material;
			} else {
				for (int i = 0, max = m_material.Count; i < max; i++) {
					if (m_material [i].Result (FormulaKeys.FORMULA_38) == _smallType) {
						temp.Add (m_material [i]);
					}
				}
				return temp;
			}
		}

		//List<FormulaHost> Get 
		/// <summary>
		/// 返回所有物品的数量
		/// </summary>
		/// <returns>The all item count.</returns>
		public int GetAllItemCount() {
			return m_Equip.Count + m_material.Count + m_pet.Count + m_Chest.Count;
		}

		public List<FormulaHost> SortAllTime() {
			BulidAllList ();

			m_AllItem.Sort (ItemSort.EquipSort_Time);
			return m_AllItem;
		}

		public List<FormulaHost> SortAllQuality() {
			BulidAllList ();

			m_AllItem.Sort (ItemSort.EquipSort_Quality);
			return m_AllItem;
		}

		public List<FormulaHost> SortAllType() {
			BulidAllList ();
			m_AllItem.Sort (ItemSort.EquipSort_Type);
			return m_AllItem;
		}

		public void SortTime(List<FormulaHost> _listFormula) {
			_listFormula.Sort (ItemSort.EquipSort_Time);
		}

		public void SotrQuality(List<FormulaHost> _listFormula) {
			_listFormula.Sort (ItemSort.EquipSort_Quality);
		}

		public void SortType(List<FormulaHost> _listFormula) {
			_listFormula.Sort (ItemSort.EquipSort_Type);
		}

		/// <summary>
		/// 检测选择物品的 品质
		/// </summary>
		/// <returns><c>true</c>, if chose item quility was checked, <c>false</c> otherwise.</returns>
		public bool CheckChoseItemQuility() {
			for (int i = 0, max = GetChosedItem.Count; i < max; i++) {
				string type = GetChosedItem [i].GetFileName ();
				switch (type) {
				case "Equip":
					GetChosedItem [i].Result (FormulaKeys.FORMULA_19);
					break;
				case "Material":
					GetChosedItem [i].Result (FormulaKeys.FORMULA_93);
					break;
				case "Pet":		
					GetChosedItem [i].Result (FormulaKeys.FORMULA_91);
					break;
				case "Chest":  
					GetChosedItem [i].Result (FormulaKeys.FORMULA_90);
					break;
				}
				if (GetChosedItem [i].GetDynamicIntByKey (SignKeys.QUALITY) > 1)
					return true;
			}
			return false;
		}

		float Multiplying=1.0f;
		public ItemCritStyel GetMultiplying {
			get {
				if (Multiplying == 1f) {
					return ItemCritStyel.CRITE_ONE;
				} else if (Multiplying == 1.5) {
					return ItemCritStyel.CRITE_ONEPOINTFIVE;
				} else {
					return ItemCritStyel.CRITE_TWO;
				}

				//	return Multiplying;
			}
		}

		public int UseCritVaule(int _value) {
			Debugger.Log ("这地方是提前算的 有问题1");
			//先判断随机爆率
			int tempNumber = UnityEngine.Random.Range (1, 100);
			if (tempNumber >= 0 && tempNumber < 85) {
				Multiplying = 1f;
			} else if (tempNumber >= 85 && tempNumber < 95) {
				//if(Multiplying<1.5)
				Multiplying = 1.5f;
			} else {
				//2bei
				//if(Multiplying<2f)
				Multiplying = 2f;
			}
			return (int)(Multiplying * _value);
		}
		//public int ttttlevelUp()
	}
}

public class ItemSort {
	/// <summary>
	/// 根据获取道具的先后顺序排序  获取时间ID 唯一
	/// </summary>
	/// <returns>The sort time.</returns>
	/// <param name="host1">Host1.</param>
	/// <param name="host2">Host2.</param>
	public static int EquipSort_Time(FormulaHost host1,FormulaHost host2) {
		int TimeID1 = (int)host1.GetDynamicDataByKey (SignKeys.BAGINID);

		int TimeID2 = (int)host2.GetDynamicDataByKey (SignKeys.BAGINID);
		if (TimeID1 > TimeID2) {
			return -1;
		} else if (TimeID1 == TimeID2) {
			return 0;
		} else {
			return 1;
		}
	}

	/// <summary>
	/// 装备按品质排->类型
	/// </summary>
	/// <returns>The sort quality.</returns>
	/// <param name="host1">Host1.</param>
	/// <param name="host2">Host2.</param>
	public static int EquipSort_Quality(FormulaHost host1,FormulaHost host2) {
		int Quality1 = (int)host1.GetDynamicDataByKey (SignKeys.QUALITY);
		int Quality2 = (int)host2.GetDynamicDataByKey (SignKeys.QUALITY);

		if (Quality1 > Quality2) {
			return -1;
		} else if (Quality1 == Quality2) {
			ushort Type1 = (ushort)host1.GetDynamicDataByKey (SignKeys.TYPE);
			ushort Type2 = (ushort)host2.GetDynamicDataByKey (SignKeys.TYPE);

			if (Type1 < Type2) {
				return -1;
			} else if (Type1 == Type2) {
				return 0;
			} else {
				return 1;	
			}
		} else {
			return 1;
		}
	}

	/// <summary>
	/// 装备按类型排-》品质
	/// </summary>
	/// <returns>The sort type.</returns>
	/// <param name="host1">Host1.</param>
	/// <param name="host2">Host2.</param>
	public static int EquipSort_Type(FormulaHost host1,FormulaHost host2) {
		ushort Type1 = (ushort)host1.GetDynamicDataByKey (SignKeys.TYPE);
		ushort Type2 = (ushort)host2.GetDynamicDataByKey (SignKeys.TYPE);
		ushort SmallType1 = (ushort)host1.GetDynamicDataByKey (SignKeys.SMALLlTYPE);
		ushort SmallType2 = (ushort)host2.GetDynamicDataByKey (SignKeys.SMALLlTYPE);
		if (Type1 < Type2) {
			return -1;
		} else if (Type1 == Type2) {
			if (SmallType1 < SmallType2) {
				return -1;
			} else if (SmallType1 == SmallType2) {
				int Quality1 = (int)host1.GetDynamicDataByKey (SignKeys.QUALITY);
				int Quality2 = (int)host2.GetDynamicDataByKey (SignKeys.QUALITY);

				if (Quality1 > Quality2) {
					return -1;
				} else if (Quality1 == Quality2) {
					return 0;
				} else {
					return 1;	
				}
			} else {
				return 1;
			}
		} else {
			return 1;
		}
	}
}