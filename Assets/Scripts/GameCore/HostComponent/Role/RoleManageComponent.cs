///自定义模块，可定制模块具体行为
using System;
using System.Collections.Generic;
using System.Collections;


namespace FormulaBase {
	public class RoleManageComponent : CustomComponentBase {
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
		public static int RoleIndexToId(int idx) {
			return idx;
		}

		/// <summary>
		/// 获取指定的角色
		/// </summary>
		/// <returns>The role.</returns>
		/// <param name="idx">Index.</param>
		public FormulaHost GetRole(int idx=0) {
			return this.GetHostByKeyValue (SignKeys.ID, idx);
		}

		public void BuyHeroCallBack(cn.bmob.response.EndPointCallbackData<Hashtable> response) {
			CommonPanel.GetInstance ().ShowWaittingPanel (false);
		}

		public void InitRole() {
			Debugger.Log ("初始化角色数据");
			if (this.HostList == null) {
				this.GetList ("Role");
			}

			// 如果有服务器数据,设置当前战斗角色
			if (this.HostList != null && this.HostList.Count > 0) {
				foreach (FormulaHost _role in this.HostList.Values) {
					if (_role == null) {
						continue;
					}

					if (_role.GetDynamicIntByKey (SignKeys.FIGHTHERO) < 1) {
						continue;
					}

					this.Host = _role;
					break;
				}

				return;
			}

			// 如果没有服务器数据,用配置生成本地对象
			LitJson.JsonData roleCfg = ConfigPool.Instance.GetConfigByName ("character");
			if (roleCfg == null) {
				return;
			}

			foreach (string key in roleCfg.Keys) {
				FormulaHost _role = FomulaHostManager.Instance.LoadHost ("Role");
				_role.SetDynamicData (SignKeys.ID, int.Parse (key));
				_role.Result (FormulaKeys.FORMULA_178);
				FomulaHostManager.Instance.AddHost (_role);
			}

			// 初始化本模块缓存表
			this.GetList ("Role");
			// 初始化默认战斗角色
			this.Host = this.GetHostByKeyValue (SignKeys.ID, 1);
		}

		public void GetExpAndCost(ref int Exp,ref int Cost) {
			Exp = 0;
			Cost = 0;
			FormulaHost tempEquip = new FormulaHost (HOST_IDX);
			List<FormulaHost> tList = ItemManageComponent.Instance.GetChosedItem;
			for (int i = 0, max = tList.Count; i < max; i++) {
				FormulaHost _item = tList [i];
				if (_item == null) {
					continue;
				}

				Exp += (int)_item.Result (FormulaKeys.FORMULA_40) * _item.GetDynamicIntByKey (SignKeys.CHOSED);
				Cost += (int)_item.Result (FormulaKeys.FORMULA_41) * _item.GetDynamicIntByKey (SignKeys.CHOSED);
			}
		}

		/// <summary>
		/// 获取升级后的host
		/// </summary>
		public FormulaHost GetLevelUpHost(FormulaHost _host) {
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
					//Messenger.Broadcast (MainMenuPanel.BroadcastChangePhysical);
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
					//Messenger.Broadcast (MainMenuPanel.BroadcastChangePhysical);
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
			if (this.Host == null) {
				return -1;
			}

			return this.Host.GetDynamicIntByKey (SignKeys.ID);
		}

		/// <summary>
		/// 设置战斗女孩的索引
		/// </summary>
		/// <param name="_index">Index.</param>
		public void SetFightGirlIndex(int _index, Callback _callBack = null) {
			if (this.HostList == null) {
				return;
			}

			foreach (string oid in this.HostList.Keys) {
				FormulaHost _role = this.HostList [oid];
				if (_role == null) {
					continue;
				}

				if (_index == _role.GetDynamicIntByKey (SignKeys.ID)) {
					_role.SetDynamicData (SignKeys.FIGHTHERO, 1);
					this.Host = _role;
					this.Host.SetAsUINotifyInstance ();
				} else {
					_role.SetDynamicData (SignKeys.FIGHTHERO, 0);
				}
			}

			_callBack ();
			//Messenger.Broadcast (AdvenTure5.AdvenTure5BraodChangeHero);
			FormulaHost.SaveList (new List<FormulaHost>(this.HostList.Values), new HttpEndResponseDelegate (SetFightGirlCallBack));
			CommonPanel.GetInstance ().ShowWaittingPanel ();
		}

		public void SetFightGirlCallBack(cn.bmob.response.EndPointCallbackData<Hashtable> response) {
			CommonPanel.GetInstance ().ShowWaittingPanel (false);
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
			_type = (int)GetRole (_index).Result (FormulaKeys.FORMULA_4);
			_Cost = (int)GetRole (_index).Result (FormulaKeys.FORMULA_3);
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
		public int GetRolePhysicalAdd() {
			int tPhysical = 0;
			foreach (FormulaHost _role in this.HostList.Values) {
				if (_role == null) {
					continue;
				}

				tPhysical += _role.GetDynamicIntByKey (SignKeys.PHYSICAL);
			}

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