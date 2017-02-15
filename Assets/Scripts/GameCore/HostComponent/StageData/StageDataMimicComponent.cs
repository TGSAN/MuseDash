///自定义模块，可定制模块具体行为
using System;
namespace FormulaBase {
	public class StageDataMimicComponent : CustomComponentBase {
		private static StageDataMimicComponent instance = null;
		private const int HOST_IDX = 10;
		public static StageDataMimicComponent Instance {
			get {
				if (instance == null) {
					instance = new StageDataMimicComponent ();
				}

				return instance;
			}
		}

		// ------------------------------------------------------------------
		private const string SIGN_KEY_STAGEMIMIC = "STAGEMIMIC_";

		public int GetMimicByStageId(int stageId) {
			FormulaHost host = FomulaHostManager.Instance.LoadHost (HostKeys.HOST_14);
			if (host == null) {
				return 0;
			}

			string signkey = SIGN_KEY_STAGEMIMIC + stageId;
			int _m = (int)host.GetDynamicDataByKey (signkey);

			return _m;
		}

		public void SetMimicToStages() {
			int[] mimics = this.GetMimicMonsters ();
			if (mimics == null || mimics.Length == 0) {
				return;
			}

			int alivemimicCount = 0;
			int stageCount = ConfigPool.Instance.GetConfigByName ("stage").Count;
			FormulaHost host = FomulaHostManager.Instance.LoadHost (HostKeys.HOST_14);
			for (int i = 0; i < stageCount; i++) {
				string signkey = SIGN_KEY_STAGEMIMIC + i;
				float _m = host.GetDynamicDataByKey (signkey);
				if (_m == 0) {
					continue;
				}

				alivemimicCount += 1;
			}

			if (alivemimicCount >= mimics.Length) {
				return;
			}

			// If stage already has mimic, no cover
			float usefulSid = 0;
			for (int k = 0; k < mimics.Length - alivemimicCount; k++) {
				int mid = mimics [k];
				while (usefulSid == 0) {
					int sid = UnityEngine.Random.Range (0, stageCount);
					string signkey = SIGN_KEY_STAGEMIMIC + sid;
					float _m = host.GetDynamicDataByKey (signkey);
					if (_m == 0) {
						usefulSid = sid;
					}
				}

				host.SetDynamicData (SIGN_KEY_STAGEMIMIC + (int)usefulSid, mid);
				usefulSid = 0;
			}
		}

		private int[] GetMimicMonsters() {
			return new int[]{1, 2, 3};
		}
	}
}