using UnityEngine;
using System.Collections;
using DYUnityLib;
using UnityEngine.UI;
using FormulaBase;
using System.Collections.Generic;

namespace GameLogic {
	public class GameMusicScene {
		public const string SCENE_PATH = "game_scenes/";
		// Timer for add cube in scene
		public FixUpdateTimer stepTimer = null;
		private GameObject maskLongPress = null;
		private GameObject sceneObject = null;
		private SceneObjectController sceneObjController = null;
		private Dictionary<int, GameObject> preLoads = null;

		public GameObject SecneObject {
			get {
				return sceneObject;
			}
		}

		struct Node {
			public Transform current;
			public int currentId;
			public int countOfThisLine;
		};

		public void SetTimer(ref FixUpdateTimer timer) {
			this.stepTimer = timer;
		}

		public void Run() {
			if (this.stepTimer == null) {
				Debug.Log ("Run scene with a null timer.");
				return;
			}

			this.stepTimer.Run ();

			if (this.sceneObjController != null) {
				this.sceneObjController.Run ();
			}
		}

		public void Stop() {
			if (this.stepTimer == null) {
				Debug.Log ("Stop scene with a null timer.");
				return;
			}

			this.stepTimer.Cancle ();
		}

		public void Reset() {
			decimal delay = 0.0m;
			decimal total = GameGlobal.DEFAULT_MUSIC_LEN;

			this.InitTimer (delay, total);
		}

		public void LoadScene(int idx, string sceneName) {
			this.InitSceneObject (idx, sceneName);
			this.PreLoad ();
			this.InitEventTrigger ();
			this.Reset ();

			Debug.Log ("Load scene " + sceneName);
		}

		// Run in timer, Add gameobj(cube) here.
		public void AddObjTrigger(object sender, uint triggerId, params object[] args) {
			if (GameGlobal.gCamera == null) {
				Debug.Log ("Game music scene Add obj trigger no camera.");
				return;
			}

			decimal ts = (decimal)args [0];
			int idx = GameGlobal.gGameMusic.GetMusicIndexByGenTick (ts);
			if (idx < 0) {
				return;
			}

			// Create scene object here.
			BattleEnemyManager.Instance.CreateBattleEnemy (idx);
			//Debug.Log (idx + " gen at > " + ts);
			if (GameGlobal.IS_DEBUG) {
				Debug.Log (idx + " gen at > " + ts);
			}
		}

		public void ShowLongPressMask(bool isShow) {
			if (this.maskLongPress == null) {
				return;
			}

			this.maskLongPress.SetActive (isShow);
		}

		public void OnObjRun(int idx) {
			if (this.sceneObjController == null) {
				return;
			}

			string nodeId = BattleEnemyManager.Instance.GetNodeUidByIdx (idx);
			if (nodeId == null) {
				return;
			}

			this.sceneObjController.DoObjectComeoutEvent (nodeId);
		}

		public void OnObjBeAttacked(int idx) {
			if (this.sceneObjController == null) {
				return;
			}

			string nodeId = BattleEnemyManager.Instance.GetNodeUidByIdx (idx);
			if (nodeId == null) {
				return;
			}

			this.sceneObjController.DoObjectOnAttackedEvent (nodeId);
		}

		public void OnObjBeMissed(int idx) {
			if (this.sceneObjController == null) {
				return;
			}

			string nodeId = BattleEnemyManager.Instance.GetNodeUidByIdx (idx);
			if (nodeId == null) {
				return;
			}

			this.sceneObjController.DoObjectOnMissedEvent (nodeId);
		}

		public GameObject GetPreLoadGameObject(int idx) {
			if (this.preLoads == null || idx < 0 || idx >= this.preLoads.Count) {
				return null;
			}

			return this.preLoads [idx];
		}

		public void ChangeAnimationSpeed(float value) {
			if (this.sceneObject == null) {
				return;
			}

			Animator[] animators = this.sceneObject.GetComponentsInChildren<Animator> ();
			if (animators == null) {
				return;
			}

			foreach (var anim in animators) {
				float sp = Mathf.Max (0f, anim.speed + value);
				anim.speed = sp;
			}
		}

		public void SetAnimationSpeed(float value) {
			if (this.sceneObject == null) {
				return;
			}

			Animator[] animators = this.sceneObject.GetComponentsInChildren<Animator> ();
			if (animators == null) {
				return;
			}

			foreach (var anim in animators) {
				anim.speed = value;
			}
		}

		public GameObject PreLoad(int idx) {
			MusicData md = StageBattleComponent.Instance.GetMusicDataByIdx (idx);
			string _path = md.nodeData.prefab_path;
			Object preObj = Resources.Load (_path);
			if (preObj == null) {
				Debug.Log ("Enemy " + idx + " preload has no resource : " + md.nodeData.prefab_path);
				this.preLoads [idx] = null;
				return null;
			}

			GameObject _po = GameObject.Instantiate (preObj) as GameObject;
			_po.name = _po.name + md.objId.ToString ();
			_po.SetActive (false);
			this.preLoads [idx] = _po;
			return _po;
		}

		private void PreLoad() {
			Debug.Log ("Preload enemy.");
			if (this.preLoads == null) {
				this.preLoads = new Dictionary<int, GameObject> ();
			}

			this.preLoads.Clear ();

			ArrayList musicTickData = StageBattleComponent.Instance.GetMusicData ();
			for (int i = 0; i < musicTickData.Count; i++) {
				this.PreLoad (i);
			}
		}

		private void InitSceneObject(int idx, string name) {
			if (this.sceneObject != null) {
				return;
			}

			string filename = name;
			GameObject obj = StageBattleComponent.Instance.AddObj (ref filename);
			if (obj == null) {
				Debug.Log ("Scene " + name + " has no prefabs.");
				return;
			}

			obj.SetActive (true);
			this.sceneObject = obj;

			// add action event module
			int eventId = int.Parse (ConfigPool.Instance.GetConfigValue ("stage", idx.ToString (), "eventId").ToString ());
			this.sceneObjController = this.sceneObject.AddComponent<SceneObjectController> ();
			this.sceneObjController.Init (eventId);
		}

		private void InitTimer(decimal delay, decimal total) {
			// Timer
			if (this.stepTimer == null) {
				Debug.Log ("Load Scene with null timer, before LoadMusicDataByFileName, call method SetTimer.");
				return;
			}

			this.stepTimer.ClearTickEvent ();
			this.stepTimer.Init (total);
			//this.stepTimer.AddTickEvent (0, GameGlobal.SCENE_ADD_OBJ);
			// Add scene obj timer event.
			ArrayList musicTickData = StageBattleComponent.Instance.GetMusicData ();
			if (musicTickData == null || musicTickData.Count <= 0) {
				Debug.Log ("Load Scene with null music data.");
				return;
			}

			for (int i = 0; i < musicTickData.Count; i++) {
				MusicData _md = (MusicData)musicTickData [i];
				this.stepTimer.AddTickEvent (_md.tick - GameGlobal.COMEOUT_TIME_MAX, GameGlobal.SCENE_ADD_OBJ);
			}
		}

		private void InitEventTrigger() {
			// First clear, ensure no repeat.
			gTrigger.UnRegEvent (GameGlobal.SCENE_ADD_OBJ);
			// Add object event
			EventTrigger eAddObj = gTrigger.RegEvent (GameGlobal.SCENE_ADD_OBJ);
			eAddObj.Trigger += AddObjTrigger;
		}
	}
}
