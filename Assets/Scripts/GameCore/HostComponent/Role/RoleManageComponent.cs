///自定义模块，可定制模块具体行为
using System;
using System.Collections.Generic;
using System.Collections;


namespace FormulaBase {
	public class RoleManageComponent : CustomComponentBase {
		public static int GirlNumber=3;
		private static RoleManageComponent instance = null;
		private const int HOST_IDX = 0;
		public static RoleManageComponent Instance {
			get {
				if(instance == null) {
					instance = new RoleManageComponent();
				}
			return instance;
			}
		}


		// -----------------
		private List<FormulaHost> roles = new List<FormulaHost>();//初设1，2，3

		public List<FormulaHost> GetRolesList
		{
			get
			{
				return roles;
			}
			set
			{
				roles=value;
			}
		}

		public static int RoleIndexToId(int idx) {
			return idx;
		}

		/// <summary>
		/// 获取指定的角色
		/// </summary>
		/// <returns>The role.</returns>
		/// <param name="idx">Index.</param>
		public FormulaHost GetRole(int idx=0)
		{

			//	Debugger.Log("获取角色信息");
			FormulaHost host = null;
//			for(int i=0,max=roles.Count;i<max;i++)
//			{
//				if((idx+1)*1000+1==roles [i].GetDynamicIntByKey("ID"))
//					return roles[i];
//			}
			if (roles.Count == 0) {
				CreateInitialRole ();
			}

			for (int i = 0; i < GirlNumber; i++) {//防止服务器顺序 改变
				int _idx = idx + 1;
				if (_idx == roles [i].GetDynamicIntByKey (SignKeys.ID))
					return roles [i];
			}

			Debugger.LogError ("get error fight girl");
			return null;
		}

		/// <summary>
		/// 创建初始的角色
		/// </summary>
		/// <param name="idx">Index.</param>
		public void CreateInitialRole() {
			Debugger.Log ("创建角色中");
			for (int i = 0; i < GirlNumber; i++) {
				int idx = i + 1;
				FormulaHost host = FomulaHostManager.Instance.CreateHost (HOST_IDX);
				host.SetDynamicData (SignKeys.ID, idx);
				host.SetDynamicData (SignKeys.WHO, idx);
				if (i == 0) {
					host.SetDynamicData (SignKeys.LOCKED, 0);
					host.SetDynamicData (SignKeys.FIGHTHERO, 1);
				}
				this.roles.Add (host);
			}

			//Messenger.Broadcast (MainMenuPanel.BroadcastChangePhysical);
			FormulaHost.SaveList (roles, new HttpEndResponseDelegate (BuyHeroCallBack));
			CommonPanel.GetInstance ().ShowWaittingPanel ();
		}

		public void BuyHeroCallBack(cn.bmob.response.EndPointCallbackData<Hashtable> response)
		{
			CommonPanel.GetInstance().ShowWaittingPanel(false);
		}

		public  void InitRole() {
		//	Debugger.Log("初始化角色数据"+);

			Dictionary<string, FormulaHost> _RolesList = FomulaHostManager.Instance.GetHostListByFileName("Role");
			Debugger.Log("初始化角色数据"+_RolesList.Count);
			if(_RolesList.Count!=GirlNumber)
			{
				CreateInitialRole();
			}
			else
			{
				roles.Clear();
				Debugger.Log("玩家拥有的角色数量:"+_RolesList.Count);
				foreach(string oid in _RolesList.Keys) {
				//	int who=_RolesList[oid].GetDynamicIntByKey(SignKeys.WHO);
					roles.Add(_RolesList[oid]);
				}
			}
		}

		public void GetExpAndCost(ref int Exp,ref int Cost)
		{
			Exp=0;
			Cost=0;
			FormulaHost tempEquip=new FormulaHost(HOST_IDX);
			List<FormulaHost> tList=ItemManageComponent.Instance.GetChosedItem;
			for(int i=0,max=tList.Count;i<max;i++)
			{
				Exp+=(int)tList[i].Result(FormulaKeys.FORMULA_40)*tList[i].GetDynamicIntByKey(SignKeys.CHOSED);
				Cost+=(int)tList[i].Result(FormulaKeys.FORMULA_41)*tList[i].GetDynamicIntByKey(SignKeys.CHOSED);
			}
		}
		/// <summary>
		/// 获取升级后的host
		/// </summary>
		public FormulaHost GetLevelUpHost(FormulaHost _host)
		{
			int Exp = 0;
			int Cost = 0;
			GetExpAndCost (ref Exp, ref Cost);
			FormulaHost thost = new FormulaHost (HOST_IDX);
			thost.SetDynamicData (SignKeys.ID, _host.GetDynamicIntByKey (SignKeys.ID));
			thost.SetDynamicData (SignKeys.LEVEL_STAR, _host.GetDynamicIntByKey (SignKeys.LEVEL_STAR));
			thost.SetDynamicData (SignKeys.LEVEL, _host.GetDynamicIntByKey (SignKeys.LEVEL));
			Exp += _host.GetDynamicIntByKey (SignKeys.EXP);

			int LevelUpExp = (int)_host.Result (FormulaKeys.FORMULA_15);
			int Level = _host.GetDynamicIntByKey (SignKeys.LEVEL);
			while (LevelUpExp <= Exp) {
				Level++;
				Exp -= LevelUpExp;
				thost.SetDynamicData (SignKeys.LEVEL, Level);
				LevelUpExp = (int)thost.Result (FormulaKeys.FORMULA_15);
				if (Level == (int)thost.Result (FormulaKeys.FORMULA_14)) {
					NGUIDebug.Log ("到达等级上限");
					return thost;
				}
			}

			//thost.SetDynamicData(SignKeys.LEVEL,Level);
			thost.SetDynamicData (SignKeys.EXP, Exp);
			return thost;
		}

		public void UnlockRole(int _index, Callback _callBack=null) {
			int ttype = 0;
			int tcost = 0;
			bool _result = true;
			GetUnLockRoleMoeny (_index, ref ttype, ref tcost);
			if (ttype == 1) {
				_result = AccountGoldManagerComponent.Instance.ChangeMoney (tcost, true, new HttpResponseDelegate (((bool result) => {
					if (!result) {
						CommonPanel.GetInstance ().ShowTextLackDiamond ();
						return;
					}

					FormulaHost thost = GetRole (_index);
					thost.SetDynamicData (SignKeys.LOCKED, 0);
					Messenger.Broadcast (MainMenuPanel.BroadcastChangePhysical);
					thost.Save (new HttpResponseDelegate (UnlockedHeroCallBack));
					_callBack ();
					CommonPanel.GetInstance ().ShowWaittingPanel ();
				})));
			} else if (ttype == 2) {
				_result = AccountCrystalManagerComponent.Instance.ChangeDiamond (tcost, true, new HttpResponseDelegate (((bool result) => {
					if (!result) {
						CommonPanel.GetInstance ().ShowTextLackDiamond ();
						return;
					}

					FormulaHost thost = GetRole (_index);
					thost.SetDynamicData (SignKeys.LOCKED, 0);
					Messenger.Broadcast (MainMenuPanel.BroadcastChangePhysical);
					thost.Save (new HttpResponseDelegate (UnlockedHeroCallBack));
					_callBack ();
					CommonPanel.GetInstance ().ShowWaittingPanel ();
				})));
			}

			if (!_result) {
				CommonPanel.GetInstance ().ShowTextLackDiamond ();
			}
		}

		public void UnlockedHeroCallBack(bool _success) {
			if (_success) {
				CommonPanel.GetInstance ().ShowWaittingPanel (false);
			} else {
				CommonPanel.GetInstance ().ShowText ("connect is fail");
			}

			CommonPanel.GetInstance ().ShowWaittingPanel (false);
		}

		/// <summary>
		/// Gets the state of the role locked.
		/// </summary>
		/// <returns><c>true</c>, false==解锁 true==锁定 <c>false</c> otherwise.</returns>
		/// <param name="_index">Index.</param>
		public bool GetRoleLockedState(int _index) {
			FormulaHost _role = this.GetRole (_index);
			if (_role == null) {
				return true;
			}

			return _role.GetDynamicIntByKey (SignKeys.LOCKED) == 1;
		}

		/// <summary>
		/// 返回当前战斗Girl ID
		/// </summary>
		/// <returns>The fight girl index.</returns>
		public int GetFightGirlIndex() {
			for (int i = 0; i < roles.Count; i++) {
				FormulaHost _role = this.roles [i];
				if (_role == null) {
					continue;
				}

				if (_role.GetDynamicIntByKey (SignKeys.FIGHTHERO) != 1) {
					continue;
				}

				return _role.GetDynamicIntByKey (SignKeys.ID);
			}

			Debugger.LogWarning ("get error fight girl");
			return -1;
		}

		/// <summary>
		/// 设置战斗女孩的索引
		/// </summary>
		/// <param name="_index">Index.</param>
		public void SetFightGirlIndex(int _index,Callback _callBack=null) {
			for (int i = 0; i < GirlNumber; i++) {
				int _idx = i + 1;
				if (_idx == roles [i].GetDynamicIntByKey (SignKeys.ID)) {
					roles [i].SetDynamicData (SignKeys.FIGHTHERO, 1);
				} else {
					roles [i].SetDynamicData (SignKeys.FIGHTHERO, 0);
				}
			}

			_callBack ();
			Messenger.Broadcast (AdvenTure5.AdvenTure5BraodChangeHero);
			FormulaHost.SaveList (roles, new HttpEndResponseDelegate (SetFightGirlCallBack));
			CommonPanel.GetInstance ().ShowWaittingPanel ();
		}

		public void SetFightGirlCallBack(cn.bmob.response.EndPointCallbackData<Hashtable> response) {
			CommonPanel.GetInstance ().ShowWaittingPanel (false);
		}

		public FormulaHost GetNowFightFightGirl() {
			for (int i = 0; i < GirlNumber; i++) {
				if (roles [i].GetDynamicIntByKey (SignKeys.FIGHTHERO) == 1)
					return roles [i];
			}
			Debugger.LogError ("获取错误的战斗女孩");
			return null;
		}

		public string GetName(int _index) {
			FormulaHost thost = GetRole (_index);
			thost.Result (FormulaKeys.FORMULA_178);
			return thost.GetDynamicStrByKey (SignKeys.NAME);
		}

		public string GetDes(int _index) {
			FormulaHost thost = GetRole (_index);
			thost.Result (FormulaKeys.FORMULA_178);
			return thost.GetDynamicStrByKey (SignKeys.DESCRIPTION);
		}

		public void GetUnLockRoleMoeny(int _index,ref int _type,ref int _Cost ) {
			_type=(int)GetRole(_index).Result(FormulaKeys.FORMULA_4);
			_Cost=(int)GetRole(_index).Result(FormulaKeys.FORMULA_3);

		}

		public ChoseHeroDefine.RESULT_EQUIP GetRoleState(int _index) {
			FormulaHost thost = GetRole (_index);
			int Locked = thost.GetDynamicIntByKey (SignKeys.LOCKED);
			int Chose = thost.GetDynamicIntByKey (SignKeys.FIGHTHERO);
			if (Locked == 0) {//解锁
				if (Chose == 1) {//战斗
					return ChoseHeroDefine.RESULT_EQUIP.EQUIP_GET;
				} else {
					return ChoseHeroDefine.RESULT_EQUIP.BUY_GET;
				}
			} else {
				return ChoseHeroDefine.RESULT_EQUIP.NO_GET;
			}
		}

		/// <summary>
		/// 获取角色的体力加成
		/// </summary>
		/// <returns>The role physical add.</returns>
		public int  GetRolePhysicalAdd() {
			//for(int i=0;i<GirlNumber;i++)
			int tPhysical = 0;
			if (GetRole (0).GetDynamicIntByKey (SignKeys.LOCKED) == 0)
				tPhysical += 10;
			if (GetRole (1).GetDynamicIntByKey (SignKeys.LOCKED) == 0)
				tPhysical += 20;
			if (GetRole (2).GetDynamicIntByKey (SignKeys.LOCKED) == 0)
				tPhysical += 20;
			return tPhysical;
		}


		public void SetHeroAttribute(int _index,ref int _effect1,ref int _effect2,ref int effect3,ref int effect4) {
			int _idx = _index + 1;
			FormulaHost thost = new FormulaHost (HostKeys.HOST_0);
			thost.SetDynamicData (SignKeys.ID, _idx);
			thost.SetDynamicData (SignKeys.WHO, _idx);

			_effect1 = (int)thost.Result (FormulaKeys.FORMULA_14);

			_effect2 = (int)thost.Result (FormulaKeys.FORMULA_184);

			effect3 = 0;

			effect4 = _index > 0 ? 20 : 10;
		}
	}
}