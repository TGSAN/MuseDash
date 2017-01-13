///自定义模块，可定制模块具体行为
using System;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using GameLogic;

namespace FormulaBase {
	public class SkillComponent : CustomComponentBase {
		private static SkillComponent instance = null;
		private const int HOST_IDX = 13;
		public static SkillComponent Instance {
			get {
				if(instance == null) {
					instance = new SkillComponent();
				}
			return instance;
			}
		}

		// ------------------------------------------------------------------------------------
		/// <summary>
		/// The result config map.
		/// 
		/// 可配置的技能触发点及其对应关键字
		/// </summary>
		private static Dictionary<uint, string> resultConfigMap = null;

		private Dictionary<string, FormulaHost> skillObjects = null;

		public const string SKILL_CONFIG_NAME = "skill";
		public const string SKILL_CONFIG_ACTKEY = "action_key";
		public const string SKILL_CONFIG_EFFECT = "effect";

		// 主动技能及其技能对象
		public const string SIGN_KEY_ACTIVE_SKILL = "SIGN_KEY_ACTIVE_SKILL";
		public const string SIGN_KEY_ACTIVE_SKILL_OBJ = "SIGN_KEY_ACTIVE_SKILL_OBJ";
		// 主动技能特效对象
		public const string SIGN_KEY_ACTIVE_SKILL_EFFECT = "SIGN_KEY_ACTIVE_SKILL_EFFECT";
		public const string SIGN_KEY_ACTIVE_SKILL_EFFECT_LIST = "SIGN_KEY_ACTIVE_SKILL_EFFECT_LIST";
		// 被动技能及其技能对象
		public const string SIGN_KEY_PASSIVE_SKILL = "SIGN_KEY_PASSIVE_SKILL";
		public const string SIGN_KEY_PASSIVE_SKILL_OBJ = "SIGN_KEY_PASSIVE_SKILL_OBJ";
		// 角色身上技能
		public const string SIGN_KEY_SKILLS = "SIGN_KEY_SKILLS";

		/// <summary>
		/// 技能效果独立开关集合
		/// </summary>
		public const string SING_KEY_SWITCHS = "SING_KEY_SWITCHS";
		public const string SING_KEY_SWITCHS_ATK = "SING_KEY_SWITCHS_ATK";
		public const string SING_KEY_SWITCHS_DEF = "SING_KEY_SWITCHS_DEF";
		public const string SING_KEY_SWITCHS_CRT = "SING_KEY_SWITCHS_CRT";
		public const string SING_KEY_SWITCHS_HP = "SING_KEY_SWITCHS_HP";

		// 持续时间倒计时
		public const string SIGN_KEY_COUNTDOWN = "SIGN_KEY_COUNTDOWN";

		// 技能所属对象（角色、了解
		public const string SIGN_KEY_SKILLPARENT = "SIGN_KEY_SKILLPARENT";

		// 技能发动失败
		public const string SIGN_KEY_SKILL_FAIL = "SIGN_KEY_SKILL_FAIL";

		// 特殊技能对象 护盾
		public const string SIGN_KEY_SKILL_SHIELD = "SIGN_KEY_SKILL_SHIELD";

		/// <summary>
		/// Skill condictions,
		/// 
		/// More than GameMusic.JUMPOVER
		/// </summary>
		public const uint ON_EQUIP = 10;
		public const uint ON_START = 11;
		public const uint ON_COMBO = 12;
		public const uint ON_HP_CHANGE = 13;
		public const uint ON_FEVER = 14;
		public const uint ON_FEVER_END = 15;
		public const uint ON_EAT_ITEM = 16;
		public const uint ON_CRITICAL = 17;
		public const uint ON_TIME_UP = 18;
		public const uint ON_UN_EQUIP = 19;
		public const uint ON_EQUIP_ARM = 20;


		private const int SKILL_EFFECT_OBJECT_INDEX = 4000;

		/// <summary>
		/// Init this instance.
		/// 
		/// 技能配置以每个condition为一个单位，每个单位占3列
		/// 包括 condition， condition动作，condition特效
		/// 例如 perfect perfect_comein  perfect_Skill/skillhp
		/// </summary>
		public void Init() {
			string fileName = FomulaHostManager.Instance.GetFileNameByHostType (HOST_IDX);
			this.skillObjects = FomulaHostManager.Instance.GetHostListByFileName (fileName);
			if (this.skillObjects == null) {
				this.skillObjects = FomulaHostManager.Instance.AddHostListByFileName (fileName);
			}

			SkillEffectComponent.Instance.Init ();
			if (resultConfigMap == null) {
				resultConfigMap = new Dictionary<uint, string> ();
				resultConfigMap [GameMusic.NONE] = "hitempty";
				resultConfigMap [GameMusic.MISS] = "miss";
				resultConfigMap [GameMusic.COOL] = "cool";
				resultConfigMap [GameMusic.GREAT] = "great";
				resultConfigMap [GameMusic.PERFECT] = "perfect";
				resultConfigMap [GameMusic.JUMPOVER] = "jumpover";
				resultConfigMap [ON_EQUIP] = "equip";
				resultConfigMap [ON_START] = "start";
				resultConfigMap [ON_COMBO] = "combo";
				resultConfigMap [ON_HP_CHANGE] = "hpchange";
				resultConfigMap [ON_FEVER] = "fever";
				resultConfigMap [ON_FEVER_END] = "feverend";
				resultConfigMap [ON_EAT_ITEM] = "eatitem";
				resultConfigMap [ON_CRITICAL] = "critical";
				resultConfigMap [ON_TIME_UP] = "timeup";
				resultConfigMap [ON_UN_EQUIP] = "unequip";
				resultConfigMap [ON_EQUIP_ARM] = "equiparm";
			}
		}

		/// <summary>
		/// Creates the skill.
		/// 
		/// 创建技能，技能需要引用其所有者(SIGN_KEY_SKILLPARENT
		/// 所有者如果需要挂载特效动作等表现，建议包含GAME_OBJECT属性
		/// </summary>
		/// <returns>The skill.</returns>
		/// <param name="skillId">Skill identifier.</param>
		public FormulaHost CreateSkill(int skillId, FormulaHost host = null) {
			if (skillId <= 0) {
				return null;
			}

			float tick = -1f;
			JsonData jTick = ConfigPool.Instance.GetConfigValue (SKILL_CONFIG_NAME, skillId.ToString (), "time");
			if (jTick != null) {
				tick = float.Parse (jTick.ToString ());
			}

			FormulaHost skillObject = FomulaHostManager.Instance.CreateHost (HOST_IDX);
			skillObject.SetDynamicData (SignKeys.ID, skillId);
			skillObject.SetDynamicData (SIGN_KEY_COUNTDOWN, tick);
			skillObject.SetDynamicData (SING_KEY_SWITCHS_ATK, new Dictionary<int, float> ());
			skillObject.SetDynamicData (SING_KEY_SWITCHS_DEF, new Dictionary<int, float> ());
			skillObject.SetDynamicData (SING_KEY_SWITCHS_CRT, new Dictionary<int, float> ());
			skillObject.SetDynamicData (SING_KEY_SWITCHS_HP, new Dictionary<int, float> ());
			if (host != null) {
				skillObject.SetDynamicData (SIGN_KEY_SKILLPARENT, host);
			}

			GameObject effectObject = null;
			string effectKey = SkillComponent.SKILL_CONFIG_EFFECT;
			JsonData jEffectName = ConfigPool.Instance.GetConfigValue (SKILL_CONFIG_NAME, skillId.ToString (), effectKey);
			if (jEffectName != null) {
				string effectName = jEffectName.ToString ();
				int _eidx = SKILL_EFFECT_OBJECT_INDEX + skillId;
				string path = GameGlobal.PREFABS_PATH + effectName;
				effectObject = StageBattleComponent.Instance.AddObjWithControllerInit (ref path, _eidx);
				if (effectObject != null) {
					effectObject.SetActive (false);
				}
			}

			skillObject.SetDynamicData (SIGN_KEY_ACTIVE_SKILL_EFFECT, effectObject);

			// Same name effect is the same game object.
			Dictionary<string, GameObject> _tempList = new Dictionary<string, GameObject> ();
			Dictionary<uint, GameObject> _effectList = new Dictionary<uint, GameObject> ();
			foreach (uint _condiction in resultConfigMap.Keys) {
				string strCond = resultConfigMap [_condiction];
				string _effKey = strCond + "_" + SKILL_CONFIG_EFFECT;
				JsonData _jEffName = ConfigPool.Instance.GetConfigValue (SKILL_CONFIG_NAME, skillId.ToString (), _effKey);
				if (_jEffName == null) {
					continue;
				}

				string _effName = _jEffName.ToString ();
				if (_effName == null) {
					continue;
				}

				GameObject _effObject = null;
				if (_tempList.ContainsKey (_effName)) {
					_effObject = _tempList [_effName];
				} else {
					int _eidx = SKILL_EFFECT_OBJECT_INDEX + skillId + (int)_condiction;
					string path = GameGlobal.PREFABS_PATH + _effName;
					_effObject = StageBattleComponent.Instance.AddObjWithControllerInit (ref path, _eidx);
				}

				if (_effObject != null) {
					_effObject.SetActive (false);
					_effectList [_condiction] = _effObject;
					_tempList [_effName] = _effObject;
				}
			}

			skillObject.SetDynamicData (SIGN_KEY_ACTIVE_SKILL_EFFECT_LIST, _effectList);
			
			return skillObject;
		}

		public void RemoveSkill(FormulaHost skillObject) {
			if (skillObject == null) {
				return;
			}

			skillObject.SetDynamicData (SIGN_KEY_SKILLPARENT, null);
			foreach (uint _condiction in resultConfigMap.Keys) {
				int id = skillObject.GetDynamicIntByKey (SignKeys.ID);
				JsonData _jEffId = ConfigPool.Instance.GetConfigValue (SkillComponent.SKILL_CONFIG_NAME, id.ToString (), resultConfigMap [_condiction]);
				if (_jEffId == null) {
					continue;
				}

				int _effId = int.Parse (_jEffId.ToString ());
				SkillEffectComponent.Instance.RemoveSwitchSignValue (_effId, skillObject, null, 0, 0);
			}

			GameObject effectObject = (GameObject)skillObject.GetDynamicObjByKey (SIGN_KEY_ACTIVE_SKILL_EFFECT);
			if (effectObject != null) {
				effectObject.SetActive (false);
			}

			Dictionary<uint, GameObject> _effectList = (Dictionary<uint, GameObject>)skillObject.GetDynamicObjByKey (SIGN_KEY_ACTIVE_SKILL_EFFECT_LIST);
			if (_effectList != null && _effectList.Count > 0) {
				foreach (GameObject obj in _effectList.Values) {
					obj.SetActive (false);
				}
			}

			skillObject = null;
		}

		/// <summary>
		/// Fires the skill.
		/// 
		/// On some condiction, effect on.
		/// 包含数据处理、特效播放、动作播放
		/// </summary>
		public void FireSkill(FormulaHost skillObject, uint condiction) {
			if (skillObject == null) {
				return;
			}

			// Find skill effect.
			if (!resultConfigMap.ContainsKey (condiction)) {
				return;
			}

			int skillId = skillObject.GetDynamicIntByKey (SignKeys.ID);
			if (skillId < 0) {
				return;
			}

			string configKey = resultConfigMap [condiction];
			JsonData jEffectId = ConfigPool.Instance.GetConfigValue (SKILL_CONFIG_NAME, skillId.ToString (), configKey);
			if (jEffectId == null) {
				return;
			}

			string effectId = jEffectId.ToString ();
			// Skill data effect function.
			SkillEffectComponent.Instance.DoSkillEffect (skillObject, int.Parse (effectId));

			if (skillObject.GetDynamicIntByKey (SIGN_KEY_SKILL_FAIL) > 0) {
				return;
			}

			// Skill effect play.
			this.PlayEffect (skillObject, condiction);

			// Skill user action play.
			this.PlayAction (skillObject, condiction);
		}

		public void FireSkill(List<FormulaHost> skillObjectList, uint condiction) {
			if (skillObjectList == null || skillObjectList.Count <= 0) {
				return;
			}

			for (int i = 0; i < skillObjectList.Count; i++) {
				FormulaHost _h = skillObjectList [i];
				if (_h == null) {
					continue;
				}

				this.FireSkill (_h, condiction);
			}
		}

		/// <summary>
		/// Gets the action key.
		/// 
		/// Action key under one condiction is common use for skill user charactor and effect object.
		/// </summary>
		/// <returns>The action key.</returns>
		/// <param name="skillObject">Skill object.</param>
		/// <param name="condiction">Condiction.</param>
		private string GetActionKey(FormulaHost skillObject, uint condiction) {
			int skillId = skillObject.GetDynamicIntByKey (SignKeys.ID);
			if (skillId < 0) {
				return null;
			}
			
			if (!resultConfigMap.ContainsKey (condiction)) {
				return null;
			}
			
			string strCond = resultConfigMap [condiction];
			if (strCond == null || strCond.Length < 2) {
				return null;
			}

			string strActKey = strCond + "_" + SKILL_CONFIG_ACTKEY;
			JsonData jActKey = ConfigPool.Instance.GetConfigValue (SKILL_CONFIG_NAME, skillId.ToString (), strActKey);
			if (jActKey == null) {
				return null;
			}

			string actKey = jActKey.ToString ();
			if (actKey.Length < 2) {
				return null;
			}

			return actKey;
		}

		private void PlayEffect(FormulaHost skillObject, uint condiction) {
			string actKey = this.GetActionKey (skillObject, condiction);
			if (actKey == null) {
				return;
			}

			GameObject effectObject = (GameObject)skillObject.GetDynamicObjByKey (SIGN_KEY_ACTIVE_SKILL_EFFECT);
			if (effectObject != null) {
				effectObject.SetActive (true);
				SpineActionController.Play (actKey, effectObject);
			}

			Dictionary<uint, GameObject> _effectList = (Dictionary<uint, GameObject>)skillObject.GetDynamicObjByKey (SIGN_KEY_ACTIVE_SKILL_EFFECT_LIST);
			if (_effectList != null && _effectList.ContainsKey (condiction)) {
				GameObject _effObj = _effectList [condiction];
				if (_effObj != null) {
					_effObj.SetActive (true);
					SpineActionController.Play (actKey, _effObj);
				}
			}
		}

		private void PlayAction(FormulaHost skillObject, uint condiction) {
			string actKey = this.GetActionKey (skillObject, condiction);
			if (actKey == null) {
				return;
			}

			FormulaHost parent = (FormulaHost)skillObject.GetDynamicObjByKey (SkillComponent.SIGN_KEY_SKILLPARENT);
			if (parent == null) {
				return;
			}

			GameObject obj = (GameObject)parent.GetDynamicObjByKey (SignKeys.GAME_OBJECT);
			SpineActionController.Play (actKey, obj);
		}
	}
}