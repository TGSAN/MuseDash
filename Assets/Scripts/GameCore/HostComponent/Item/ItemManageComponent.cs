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
		public enum ItemCritStyel {
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

		public void ClearChosedItem() {
			for(int i=0,max=m_ListChosedItem.Count;i<max;i++) {
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

		public void Init() {
			this.itemHost = FomulaHostManager.Instance.LoadHost (HOST_IDX);
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
		}

		public void DeleteItem(FormulaHost _host) {
			_host.Delete (new HttpResponseDelegate (this.DeleteItemCallBack));
		}

		public void DeleteListItem(List<FormulaHost> _listhost) {
			List<FormulaHost> tDeleteItem = new List<FormulaHost> ();
			List<FormulaHost> tReduceItem = new List<FormulaHost> ();
			for (int i = 0; i < _listhost.Count; i++) {
				FormulaHost host = _listhost [i];
				if (host == null) {
					continue;
				}

				int number = host.GetDynamicIntByKey (SignKeys.STACKITEMNUMBER);
				if (number > 1) {
					int ChosedNumber = host.GetDynamicIntByKey (SignKeys.CHOSED);
					number -= ChosedNumber;
					if (number == 0) {
						tDeleteItem.Add (host);
						//	DeleteItem();
					} else {
						host.SetDynamicData (SignKeys.STACKITEMNUMBER, number);
						host.SetDynamicData (SignKeys.CHOSED, 0);
						tReduceItem.Add (host);
						//	.Save();
					}
				} else {
					tDeleteItem.Add (host);
				}
			}

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
				FormulaHost host = _listHost [i];
				if (host == null) {
					continue;
				}

				int ChosedNumber = host.GetDynamicIntByKey (SignKeys.CHOSED);
				int HaveNumber = host.GetDynamicIntByKey (SignKeys.STACKITEMNUMBER);

				if (ChosedNumber < HaveNumber) {//选择的小与堆叠的
					host.SetDynamicData (SignKeys.CHOSED, 0);
					host.SetDynamicData (SignKeys.STACKITEMNUMBER, HaveNumber - ChosedNumber);
					tReduceItem.Add (_listHost [i]);
					//	_listHost[i]
				} else {
					host.IsDelete = true;
					tReduceItem.Add (host);
					tDeleteItem.Add (host);
				}
			}

			//FormulaHost.DeleteList(tDeleteItem);
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
					} else {
						_host.Save (new HttpResponseDelegate (_callback));
					}

					CommonPanel.GetInstance ().ShowWaittingPanel (true);
				})));
			} else {
				int salePrice = this.GetItemMoney (_host);
				AccountGoldManagerComponent.Instance.ChangeMoney (salePrice, true, new HttpResponseDelegate (((bool result) => {
					if (_callback == null) {
						_host.Delete (new HttpResponseDelegate (this.DeleteItemCallBack));
						CommonPanel.GetInstance ().ShowWaittingPanel (true);
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
			//bool stack=false;
			string itemType = _host.GetFileName ();
			FormulaHost t_host = null;
			switch (itemType) {
			case "Equip":
				if (_host.GetDynamicIntByKey (SignKeys.BAGINID) == 0) {//没有的东西
					_host.SetDynamicData (SignKeys.BAGINID, id++);//添加获取物品 时间系数
				}
				_host.Save (new HttpResponseDelegate (this.CallBackAdditem));
				break;
			case "Material":
				t_host = materialManageComponent.Instance.HaveTheSameID (_host.GetDynamicIntByKey (SignKeys.ID));
				if (t_host == null) {//有没有材料
					_host.SetDynamicData (SignKeys.BAGINID, id++);//添加获取物品 时间系数
				} else {
					_host = t_host;
				}
				_host.Save (new HttpResponseDelegate (this.CallBackAdditem));
				break;
			case "Pet":
				if ((int)_host.GetDynamicDataByKey (SignKeys.SMALLlTYPE) == 6) {//碎片
					t_host = PetManageComponent.Instance.HaveTheSameID (_host.GetDynamicIntByKey (SignKeys.ID));
					if (t_host == null) {
						_host.SetDynamicData (SignKeys.BAGINID, id++);//添加获取物品 时间系数
					} else {
						_host = t_host;
					}
					_host.Save (new HttpResponseDelegate (this.CallBackAdditem));
				} else {
					if (_host.GetDynamicIntByKey (SignKeys.BAGINID) == 0) {//没有的东西
						_host.SetDynamicData (SignKeys.BAGINID, id++);//添加获取物品 时间系数
					}
				}
				break;
			}

			CommonPanel.GetInstance ().ShowWaittingPanel ();
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
					}
					break;
				case "Material":
					t_host = materialManageComponent.Instance.HaveTheSameID (_listHost [i].GetDynamicIntByKey ("ID"));
					if (t_host == null) {//有没有材料
						_listHost [i].SetDynamicData (SignKeys.BAGINID, id++);//添加获取物品 时间系数
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
						} else {
							_listHost [i] = t_host;
						}
					} else {
						if (_listHost [i].GetDynamicIntByKey (SignKeys.BAGINID) == 0) {//没有的东西
							_listHost [i].SetDynamicData (SignKeys.BAGINID, id++);//添加获取物品 时间系数
						}
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
			}

			return Money;
		}

		public void AddItemListCallBack(cn.bmob.response.EndPointCallbackData<Hashtable> response) {
			CommonPanel.GetInstance().ShowWaittingPanel(false);
		}

		public void CallBackAdditem(bool _temp) {
			CommonPanel.GetInstance().ShowWaittingPanel(false);

			Init();
		//	NGUIDebug.Log("AdditemCallBack");
		}

		public void ItemLevelUp() {
			//FormulaHost host;
			//host.Delete();
		}

		//List<FormulaHost> Get 
		/// <summary>
		/// 返回所有物品的数量
		/// </summary>
		/// <returns>The all item count.</returns>
		public int GetAllItemCount() {
			int c = 0;
			if (materialManageComponent.Instance.HostList != null) {
				c += materialManageComponent.Instance.HostList.Count;
			}

			if (EquipManageComponent.Instance.HostList != null) {
				c += EquipManageComponent.Instance.HostList.Count;
			}

			if (PetManageComponent.Instance.HostList != null) {
				c += PetManageComponent.Instance.HostList.Count;
			}

			return c;
		}

		/// <summary>
		/// 检测选择物品的 品质
		/// </summary>
		/// <returns><c>true</c>, if chose item quility was checked, <c>false</c> otherwise.</returns>
		public bool CheckChoseItemQuility() {
			for (int i = 0, max = GetChosedItem.Count; i < max; i++) {
				FormulaHost host = GetChosedItem [i];
				if (host == null) {
					continue;
				}

				if (host.GetDynamicIntByKey (SignKeys.QUALITY) > 1)
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
			}
		}
	}
}