///自定义模块，可定制模块具体行为
using System;
using System.Collections.Generic;
using UnityEngine;
using GameLogic;


namespace FormulaBase {
	public class StageRewardComponent : CustomComponentBase {
		private static StageRewardComponent instance = null;
		private const int HOST_IDX = 3;
		public static StageRewardComponent Instance {
			get {
				if(instance == null) {
					instance = new StageRewardComponent();
				}
			return instance;
			}
		}

		// ------------------------------------------------------------------------------------
		public const string REWARD_CONFIG_NAME = "stage";
		private FormulaHost stage = null;

		public void SetStage(FormulaHost host) {
			this.stage = host;
		}

		private static int[] REWAR_EXP_FORMULA_KEY = new int[]{
			-1,
			FormulaKeys.FORMULA_343,
			FormulaKeys.FORMULA_342,
			FormulaKeys.FORMULA_341,
			FormulaKeys.FORMULA_339
		};

		private static int[] REWAR_GOLD_FORMULA_KEY = new int[]{
			-1,
			FormulaKeys.FORMULA_338,
			FormulaKeys.FORMULA_337,
			FormulaKeys.FORMULA_336,
			FormulaKeys.FORMULA_332
		};

		private static int[] REWAR_CHARM_FORMULA_KEY = new int[]{
			-1,
			FormulaKeys.FORMULA_335,
			FormulaKeys.FORMULA_334,
			FormulaKeys.FORMULA_333,
			FormulaKeys.FORMULA_331
		};

		/// <summary>
		/// Stages the reward.
		/// 
		/// 关卡奖励
		/// </summary>
		/// <param name="rank">Rank.</param>
		/// <param name="rankStr">Rank string.</param>
		/// <param name="rechargeData">Recharge data.</param>
		public void StageReward(FormulaHost stageHost, bool isNewRank) {
			this.SetStage (stageHost);

			// 通关基础奖励
			int rwExp = 0;
			int rwGold = 0;
			int rwCharm = 0;
			// reward gold
			stageHost.SetDynamicData (SignKeys.GOLD, rwGold);
			// reward exp
			stageHost.SetDynamicData (SignKeys.EXP, rwExp);
			// reward diamond
			stageHost.SetDynamicData (SignKeys.DIAMOND, rwCharm);

			int combo = TaskStageTarget.Instance.GetComboMax ();
			int comboFull = stageHost.GetDynamicIntByKey (SignKeys.FULL_COMBO);
			int perfect = TaskStageTarget.Instance.Host.GetDynamicIntByKey (TaskStageTarget.TASK_SIGNKEY_EVLUATE_HEAD + GameMusic.PERFECT);
			int perfuctFull = stageHost.GetDynamicIntByKey (SignKeys.FULL_PERFECT);
			int stars = TaskStageTarget.Instance.GetHideNodeCount ();
			int starsFull = stageHost.GetDynamicIntByKey (SignKeys.FULL_STAR);
			// is play combo full
			stageHost.SetDynamicData (SignKeys.ITEMEFFECT1, ((combo >= comboFull)?1:0));
			// is play perfect full
			stageHost.SetDynamicData (SignKeys.ITEMEFFECT2, ((perfect >= perfuctFull)?1:0));
			// is play stars full
			stageHost.SetDynamicData (SignKeys.ITEMEFFECT3, ((stars >= starsFull)?1:0));

			// is new trophy
			stageHost.SetDynamicData (SignKeys.LEVEL_STAR, (isNewRank?1:0));

			int comboRewardType = (int)this.stage.Result (FormulaKeys.FORMULA_345);
			int comboReward = (int)this.stage.Result (FormulaKeys.FORMULA_346);

			int perfectRewardType = (int)this.stage.Result (FormulaKeys.FORMULA_348);
			int perfectReward = (int)this.stage.Result (FormulaKeys.FORMULA_349);

			int hideNodeRewardType = (int)this.stage.Result (FormulaKeys.FORMULA_351);
			int hideNodeReward = (int)this.stage.Result (FormulaKeys.FORMULA_352);

			int clearRewardType = (int)this.stage.Result (FormulaKeys.FORMULA_354);
			int clearReward = (int)this.stage.Result (FormulaKeys.FORMULA_355);
		}

		public int GetBaseRewardFromConfig(uint diff, string cfgKey) {
			string sid = this.stage.GetDynamicIntByKey (SignKeys.ID).ToString ();
			string _cfgKey = cfgKey + diff;

			int v = ConfigPool.Instance.GetConfigIntValue(REWARD_CONFIG_NAME, sid.ToString(), _cfgKey);
			Debug.Log("Get stage " + sid + " reward " + _cfgKey + " : " + v);

			return v;
		}

		/// <summary>
		/// Results the rank.
		/// 通过关卡数据对照配置获取评价等级
		/// 具体评价字母对应 GameGlobal.STAGE_EVLUATE_MAP
		/// </summary>
		/// <returns>The rank.</returns>
		/// <param name="key">Key.</param>
		/// <param name="cfgKey">Cfg key.</param>
		public int GetRank(int result, string cfgKey) {
			string sid = this.stage.GetDynamicIntByKey (SignKeys.ID).ToString ();
			for (int i = 0; i < GameGlobal.STAGE_EVLUATE_MAP.Length; i++) {
				string _strRank = GameGlobal.STAGE_EVLUATE_MAP [i];
				string _key = cfgKey + _strRank;
				int _result = ConfigPool.Instance.GetConfigIntValue (REWARD_CONFIG_NAME, sid, _key);
				if (result < _result) {
					return i;
				}
			}

			return 0;
		}

		public int GetRank(string key, string cfgKey) {
			string sid = this.stage.GetDynamicIntByKey (SignKeys.ID).ToString ();
			int result = TaskStageTarget.Instance.Host.GetDynamicIntByKey (key);
			for (int i = 0; i < GameGlobal.STAGE_EVLUATE_MAP.Length; i++) {
				string _strRank = GameGlobal.STAGE_EVLUATE_MAP [i];
				string _key = cfgKey + _strRank;
				int _result = ConfigPool.Instance.GetConfigIntValue (REWARD_CONFIG_NAME, sid, _key);
				if (result < _result) {
					return i;
				}
			}

			return 0;
		}

		/// <summary>
		/// Determines whether this instance is new rank the specified key.
		/// 通过对比历史数据判断某个数据是否新纪录
		/// </summary>
		/// <returns><c>true</c> if this instance is new rank the specified key; otherwise, <c>false</c>.</returns>
		/// <param name="key">Key.</param>
		public bool IsNewRank(string key, string cfgKey) {
			int resultMax = TaskStageTarget.Instance.GetXMax (key);
			int resultOldMax = TaskStageTarget.Instance.GetOldXMax (key);
			int rankMax = this.GetRank (resultMax, cfgKey);
			int rankOldMax = this.GetRank (resultOldMax, cfgKey);

			return rankMax > rankOldMax;
		}

		// ------------------------------------------------------------------------------------------------------------
	}
}