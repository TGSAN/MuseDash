///自定义模块，可定制模块具体行为
using System;
using System.Collections;


namespace FormulaBase {
	public class RoleLevelComponent : CustomComponentBase {
		private static RoleLevelComponent instance = null;
		private const int HOST_IDX = 0;
		public static RoleLevelComponent Instance {
			get {
				if(instance == null) {
					instance = new RoleLevelComponent();
				}
			return instance;
			}
		}

		public void LevelUp(FormulaHost host, int count, bool isave = true) {
			host.AddDynamicValue (SignKeys.LEVEL, count);
			if (isave) {
				host.Save (new HttpResponseDelegate (this.LevelUpCallBack));
			}
		}

		public void LevelUpCallBack(cn.bmob.response.EndPointCallbackData<Hashtable> response) {
			this.LevelUpCallBack (true);
		}

		public void LevelUpCallBack(bool _success) {
			if (_success) {
//				Messenger.Broadcast (CharactorPanel2.BroadCast_SetHeroData, RoleManageComponent.Instance.GetRole ());
			} else {
				CommonPanel.GetInstance ().ShowText ("connect is fail");
			}
		}

		public int GetLevel(FormulaHost host) {
			return (int)host.GetDynamicDataByKey (SignKeys.LEVEL);
		}

		public int GetLevelStar(FormulaHost host) {
			return (int)host.GetDynamicDataByKey (SignKeys.LEVEL_STAR);
		}
	}
}