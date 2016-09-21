using UnityEngine;
using System.Collections;
using DYUnityLib;
using System.Collections.Generic;

namespace GameLogic {
	public class GameGlobal {
		public static bool IS_DEBUG = false;
		public static bool IS_NODE_DEBUG = false;
		public static bool IS_UNLOCK_ALL_STAGE = false;
		public static bool ENABLE_LOCAL_SAVE = false;
		//public static uint DEBUG_DEFAULT_STAGE = 0;
		public static uint DEBUG_DEFAULT_STAGE {
			get {
				if (AdminData.Instance == null) {
					return 0;
				}

				return (uint)AdminData.Instance.DefaultStage;
			}
		}

		public static int DEBUG_CLOTH_UID {
			get {
				if (AdminData.Instance == null) {
					return 0;
				}

				return (int)AdminData.Instance.DefaultClothUid;
			}
		}

		public const int LIMITE_INT = 999999;
		public const float TIME_SCALE = 1f;
		public const decimal DEFAULT_MUSIC_LEN = 240m;
		public const decimal DEFAULT_END_CD = 0.5m;
		public const decimal CONTINUE_ATTACK_IVR = 0.5m;
		public const decimal COMEOUT_TIME_MAX = 3m;
		public const int TOUCH_PHASE_COUNT = 5;
		// combo distance
		public const int COMBO_INTERVAL = 10;
		// Game finish star level
		public const uint FINISH_LEVEL_1 = 1;
		public const uint FINISH_LEVEL_2 = 2;
		public const uint FINISH_LEVEL_3 = 3;

		// Game diffcult level
		public const int DIFF_LEVEL_NORMAL = 1;
		public const int DIFF_LEVEL_HARD = 2;
		public const int DIFF_LEVEL_SUPER = 3;

		public const int FIX_FATAL_MULT = 2;
		public const uint FIX_FEVER_ADD = 1;

		public const string LOADING_SCENE_NAME = "LoadingScene";
		public const string PREFABS_PATH = "Prefabs/";

		public static string[] STAGE_EVLUATE_MAP = new string[]{ "D", "C", "B", "A", "S" };

		public const int TEAM_PLACE_ATK = 0;
		public const int TEAM_PLACE_DEF = 1;
		public const int TEAM_PLACE_BUF = 2;

		public const uint ITEM_TYPE_NONE = 0;
		public const uint ITEM_TYPE_ITEM = 1;
		public const uint ITEM_TYPE_EQUIP = 2;

		public const uint EQUIPMENT_PART_WEAPON = 0;
		public const uint EQUIPMENT_PART_CLOTH = 1;
		public const uint EQUIPMENT_PART_OTHER = 9;

		public const uint JUDGE_LEVEL_LIMITE = 5;
		public static Hashtable JUDGE_MAP = new Hashtable ();

		public const uint DEFAULT_BAG_LIMITE = 20;
		public const int TMPKEY = 99999;
		public const int SKILL_CARRY_LIMITE = 5;
		public const int ROLE_CARRY_LIMITE = 5;
		public const uint FEVER_MAX = 100;

		public const uint PLAY_RESULT_LOCK_LEVEL_LONG_PRESS = 1;
		public const uint PLAY_RESULT_LOCK_LEVEL_BUFF = 2;

		public static KeyCode KC_PUMCH = KeyCode.X;
		public static KeyCode KC_JUMP = KeyCode.Z;
		public const uint PRESS_STATE_NONE = 0;
		public const uint PRESS_STATE_PUMCH = 1;
		public const uint PRESS_STATE_JUMP = 2;

		public const float REDUCE_ENERGY_TIME = 0.2f;
		public const float FEVER_LAST_TIME = 5f;

		public const int LONG_PRESS_NODE_TYPE = 12;
		public const decimal MISS_NO_CHECK_TICK = -5m;

		public const int SIGN_KEY_MIN_LEN = 2;

		// 资源类型 金钱
		public const int RESOURCE_TYPE_GOLD = 1;
		// 资源类型 钻石
		public const int RESOURCE_TYPE_DIAMOND = 2;
		// 资源类型 奖杯
		public const int RESOURCE_TYPE_CUP = 3;

		//无操作逻辑的note
		public const uint NODE_TYPE_NONE = 0;
		//普通note
		public const uint NODE_TYPE_MONSTER = 1;
		//金币
		public const uint NODE_TYPE_GOLD = 2;
		//血瓶
		public const uint NODE_TYPE_HP = 3;
		//音符
		public const uint NODE_TYPE_MUSIC = 4;
		//隐藏
		public const uint NODE_TYPE_HIDE = 5;
		//长按
		public const uint NODE_TYPE_PRESS = 6;
		//boss
		public const uint NODE_TYPE_BOSS = 7;
		//空中打击
		public const uint NODE_TYPE_AIR_BEAT = 8;

		//miss不处理类型
		public static List<uint> NODE_TYPES_NO_MISS = new List<uint> (){NODE_TYPE_GOLD, NODE_TYPE_HP, NODE_TYPE_MUSIC, NODE_TYPE_HIDE};

		public const string LANGUAGE_VERSION = "chs";

		public const string SOUND_TYPE_HURT = "HURT";
		public const string SOUND_TYPE_FEVER = "FEVER";
		public const string SOUND_TYPE_MAIN_BOARD_TOUCH = "MAIN_BOARD_TOUCH";
		public const string SOUND_TYPE_ENTER_STAGE = "ENTER_STAGE";
		public const string SOUND_TYPE_STAGE_START = "STAGE_START";
		public const string SOUND_TYPE_LAST_NODE = "LAST_NODE";
		public const string SOUND_TYPE_DEAD = "DEAD";
		public const string SOUND_TYPE_ON_EQUIP = "ON_EQUIP";
		public const string SOUND_TYPE_ON_EXP = "ON_EXP";
		public const string SOUND_TYPE_ON_CHEST = "ON_CHEST";
		public const string SOUND_TYPE_ON_CHEST_OPEN = "ON_CHEST_OPEN";
		public const string SOUND_TYPE_ON_TEN_COMBO = "ON_TEN_COMBO";
		public const string SOUND_TYPE_UI = "UI";
		public const string SOUND_TYPE_UI_BGM = "UI_BGM";
		public const string SOUND_TYPE_UI_ATTACK_MISS = "ON_ATTACK_MISS";

		// Game logic event, from 129, in this project, >= 1000 is for buff
		public const uint MUSIC_SINGLE_PRESS = 129;
		public const uint MUSIC_SLIDE_UP_PRESS = 130;
		public const uint MUSIC_SLIDE_DOWN_PRESS = 131;
		public const uint MUSIC_START_PRESS =  132;
		public const uint MUSIC_END_PRESS = 133;
		public const uint MUSIC_TOUCH_EVENT = 134;

		public const uint SCENE_ADD_OBJ = 135;

		public const uint MUSIC_STEP_EVENT = 136;
		public const uint MUSIC_COMBO_CANCLE = 137;
		public const uint LOADING = 138;
		public const uint MUSIC_DELAY_START = 139;
		public const uint GAME_CONDICTION = 140;
		public const uint SCENO_OBJ_STEP_EVENT = 141;
		public const uint SKILL_EVENT_ADD_HP = 142;

		// Global config loader
		public static ConfigLoader gConfigLoader = new ConfigLoader ();

		// Global touch script obj.
		public static TouchScript gTouch = new TouchScript ();

		// Global game miss phaser
		public static GameMissPlay gGameMissPlay = null;

		// Global game touch phaser
		public static GameTouchPlay gGameTouchPlay = null;

		// Global camera obj pointer
		public static GameSceneMainController gCamera = null;

		// Global scene obj pointer
		public static GameMusic gGameMusic = null;

		// Global music obj pointer
		public static GameMusicScene gGameMusicScene = null;

		// Global GirlJump During Time
		public const float JUMP_WHOLE_TIME = 0.2f;

		public const bool BZAAAAAAA=true;

		public const string TipsSF="现在还不可以哦～";

	}

	public struct BUFF_TRIGGER {
		public const uint SAMPLE1 = 1000;
		public const uint SAMPLE2 = 1001;
		public const uint RECOVER = 1002;
	}
}

public struct ACTION_KEYS {
	public const string COMEIN = "in";
	public const string COMEOUT = "out";

	public const string MELEE1 = "boss_close_atk_1";
	public const string MELEE2 = "boss_close_atk_2";
	public const string MELEE3 = "melee3";
	public const string MELEE4 = "melee4";
	public const string MELEE1_FAILED = "boss_hurt";
	public const string MELEE2_FAILED = "boss_hurt";
	public const string SUMMON1 = "boss_far_atk_1_start";
	public const string SUMMON2 = "summon2";
	public const string SUMMON3 = "summon3";
	public const string SUMMON2_END = "boss_far_atk_1_end";
	public const string LONGNOTE_STAR = "char_press";
	public const string LONGNOTE = "char_press";
	public const string COMEOUT1 = "note_out_g";
	public const string COMEOUT2 = "note_out_g";
	public const string COMEOUT3 = "note_out_p";

	public const string STAND = "standby";
	public const string PRESS = "char_press";
	//public const string AIR_STAND = "airstandby";
	public const string HURT = "char_hurt";
	public const string RUN = "char_run";
	public const string ATTACK_PERFECT = "char_atk_p";
	public const string ATTACK_GREAT = "char_atk_g";
	public const string ATTACK_COOL = "";
	public const string ATTACK_MISS = "char_atk_miss";
	public const string JUMP_HURT = "char_jump_hurt";
	public const string JUMP = "char_jump";
	public const string CHAR_DEAD = "char_die";
	public const string PET_SKILL = "servant_skill";
}
