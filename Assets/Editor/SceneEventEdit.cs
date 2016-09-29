using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using FormulaBase;

public class SceneEventEdit : EditorWindow {
	private const float STAGE_TIME_MAX = 300f;
	private const string LOAD_BUTTON = "Load";
	private const string SAVE_BUTTON = "Save";
	private const string ADD_BUTTON = "+";
	private const string DEL_BUTTON = "-";
	private const string REFLESH_BUTTON = "排序";
	private const string DEL_BUTTON2 = "Del";
	private const string LIST_DES = "生成";
	private const string LIST_DES2 = "击中";
	private const string LIST_DES3 = "miss";
	private Vector2 scrollVec;
	private Vector2 scorllVecAction;

	// Data about.
	private int eventId;
	private int stageId;
	private int currentTimeEditEventIndex;
	private float currentEventTime;
	private StageEvent editData;
	private string timeStr;

	[MenuItem("RHY/场景编辑器")]
	static void Init () {
		SceneEventEdit window = (SceneEventEdit)EditorWindow.GetWindow (typeof(SceneEventEdit));
		window.Show ();
	}

	void OnGUI() {
		EditorGUILayout.BeginVertical ();
		this.MkTitle ();

		this.editData.flodAction = EditorGUILayout.Foldout (this.editData.flodAction, "行为事件");
		if (this.editData.flodAction) {
			if (GUILayout.Button (ADD_BUTTON)) {
				StageActionEvent item = new StageActionEvent ();
				this.AddStageActionEvent (ref item, ref this.editData.actionEvents);
				this.SaveData ();
				this.Repaint ();
			}

			this.MkActionList ();
		}

		//this.SetCurrentTimeEditIndex (0);
		this.editData.flodTime = EditorGUILayout.Foldout (this.editData.flodTime, "时间事件");
		this.currentEventTime = EditorGUILayout.Slider (this.currentEventTime, 0f, STAGE_TIME_MAX);
		this.SetCurrentEditEventTime ();

		if (this.editData.flodTime) {
			EditorGUILayout.BeginHorizontal ();
			this.timeStr = EditorGUILayout.TextField (this.timeStr);
			if (GUILayout.Button (ADD_BUTTON)) {
				float _t = float.Parse (this.timeStr);
				StageTimeEvent item = new StageTimeEvent ();
				item.time = _t;
				this.AddStageTimeEvent (ref item, ref this.editData.timeEvents);
				this.SaveData ();
				this.Repaint ();
			}

			if (GUILayout.Button (REFLESH_BUTTON)) {
				this.SortStageTimeEvent(ref this.editData.timeEvents);
				this.SaveData ();
				this.SetCurrentTimeEditIndex(0);
				this.Repaint ();
			}

			EditorGUILayout.EndHorizontal ();

			this.MkTimeList ();
		}

		EditorData.Instance.StageEvents [this.eventId] = this.editData;

		EditorGUILayout.EndVertical ();
	}

	private void MkTitle() {
		string[] dataText = new string[]{ "0" };
		if (EditorData.Instance.StageEvents != null) {
			dataText = new string[EditorData.Instance.StageEvents.Length];
			for (int i = 0; i < dataText.Length; i++) {
				dataText [i] = i.ToString ();
			}
		}

		EditorGUILayout.BeginHorizontal ();
		this.eventId = EditorGUILayout.Popup ("Event Id", this.eventId, dataText);
		this.editData = EditorData.Instance.GetStageEventDataById (this.eventId);

		if (GUILayout.Button (ADD_BUTTON)) {
			this.AddData ();
		}

		if (GUILayout.Button (SAVE_BUTTON)) {
			this.SaveData();
		}
		
		if (GUILayout.Button (LOAD_BUTTON)) {
			this.Repaint();
		}

		EditorGUILayout.EndHorizontal ();
	}

	// --------------------   Action --------------------
	private void MkActionList() {
		this.scorllVecAction = EditorGUILayout.BeginScrollView (this.scorllVecAction);
		EditorGUILayout.BeginVertical ();
		int len = 0;
		if (this.editData.actionEvents != null) {
			len = this.editData.actionEvents.Length;
		}

		for (int i = 0; i < len; i++) {
			this.MkActionItem (i);
		}

		EditorGUILayout.EndVertical ();
		EditorGUILayout.EndScrollView ();
	}

	private void MkActionItem(int idx) {
		if (idx >= this.editData.actionEvents.Length) {
			return;
		}

		StageActionEvent eData = this.editData.actionEvents [idx];
		GameLogic.NodeConfigReader.Instance.Init ();
		ArrayList array = GameLogic.NodeConfigReader.Instance.GetData ();
		string[] text = new string[array.Count];
		for (int i = 0; i < text.Length; i++) {
			//if (i <= 0) {
			//	text[i] = "0";
			//	continue;
			//}

			GameLogic.NodeConfigData nd = (GameLogic.NodeConfigData)array [i];
			string tx = i + "/" + nd.uid + ": " + nd.prefab_path;
			text [i] = tx;
			//if (GUILayout.Button (tx)) {
			//}
		}

		EditorGUILayout.BeginHorizontal ();
		//eData.nodeIndex = EditorGUILayout.Popup (eData.nodeIndex, text);
		eData.nodeUid = EditorGUILayout.TextField (eData.nodeUid);
		eData.sceneObject = (GameObject)EditorGUILayout.ObjectField (eData.sceneObject, typeof(GameObject), true);
		eData.bornActionIndex = EditorGUILayout.Popup (LIST_DES, eData.bornActionIndex, EditorData.Instance.SpineActionDes);
		eData.hittedActionIndex = EditorGUILayout.Popup (LIST_DES2, eData.hittedActionIndex, EditorData.Instance.SpineActionDes);
		eData.missActionIndex = EditorGUILayout.Popup (LIST_DES3, eData.missActionIndex, EditorData.Instance.SpineActionDes);
		if (GUILayout.Button (DEL_BUTTON2)) {
			if (this.DelStageActionEvent (idx, ref this.editData.actionEvents)) {
				this.SaveData ();
				this.Repaint ();
				return;
			}
		}

		this.editData.actionEvents [idx] = eData;

		EditorGUILayout.EndHorizontal ();
	}

	// --------------------   Time ----------------------
	private void MkTimeList() {
		int len = 0;
		if (this.editData.timeEvents != null) {
			len = this.editData.timeEvents.Length;
		}

		this.scrollVec = EditorGUILayout.BeginScrollView (this.scrollVec);
		for (int i = 0; i < len; i++) {
			this.MkTimeItem (i);
		}

		EditorGUILayout.EndScrollView ();
	}

	private void MkTimeItem(int idx) {
		if (idx >= this.editData.timeEvents.Length) {
			return;
		}

		EditorGUILayout.BeginHorizontal ();
		StageTimeEvent tData = this.editData.timeEvents [idx];
		string strTime = tData.time.ToString ();
		tData.flod = EditorGUILayout.Foldout (tData.flod, idx.ToString ());
		if (GUILayout.Button (strTime)) {
			this.SetCurrentTimeEditIndex (idx);
		}

		if (this.editData.timeEvents [idx].flod) {
			if (GUILayout.Button (ADD_BUTTON)) {
				StageTimeEventItem item = new StageTimeEventItem ();
				this.AddStageTimeEventItem (ref item, ref tData.eventItems);
				this.editData.timeEvents [idx] = tData;
				this.SaveData ();
				this.Repaint ();
				return;
			}

			if (GUILayout.Button (DEL_BUTTON)) {
				if (tData.eventItems.Length > 0) {
					this.DelStageTimeEventItem (tData.eventItems.Length - 1, ref tData.eventItems);
					this.editData.timeEvents [idx].eventItems = tData.eventItems;
					this.SaveData ();
					this.Repaint ();
				} else {
					if (this.DelStageTimeEvent (idx, ref this.editData.timeEvents)) {
						this.SaveData ();
						this.Repaint ();
						return;
					}
				}

				return;
			}

			this.MkObjectItem (idx);
		}

		this.editData.timeEvents [idx] = tData;

		EditorGUILayout.EndHorizontal ();
	}

	private void MkObjectItem(int idx) {
		StageTimeEvent tData = this.editData.timeEvents [idx];
		if (tData.eventItems == null || tData.eventItems.Length == 0) {
			return;
		}

		EditorGUILayout.BeginVertical ();
		for (int i = 0; i < tData.eventItems.Length; i++) {
			this.MkEventItem (idx, i, ref tData);
		}

		this.editData.timeEvents [idx] = tData;

		EditorGUILayout.EndVertical ();
	}

	private void MkEventItem(int idx, int eventIndex, ref StageTimeEvent tData) {
		StageTimeEventItem item = tData.eventItems [eventIndex];

		EditorGUILayout.BeginHorizontal ();
		item.sceneObject = (GameObject)EditorGUILayout.ObjectField (item.sceneObject, typeof(GameObject), true);
		item.actionIndex = EditorGUILayout.Popup (item.actionIndex, EditorData.Instance.SpineActionDes);
		// ------------ drop audio effect and record path ------------
		Rect sfxPathRect = EditorGUILayout.GetControlRect (GUILayout.Width (300));

		item.soundPath = EditorGUI.TextField (sfxPathRect, item.soundPath);

		if ((Event.current.type == EventType.DragUpdated
			|| Event.current.type == EventType.DragExited)
			&& sfxPathRect.Contains (Event.current.mousePosition)) {
			// 判断是否拖拽了文件 
			if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0) {
				string sfxPath = DragAndDrop.paths [0];
				// 拖拽的过程中，松开鼠标之后，拖拽操作结束，此时就可以使用获得的 sfxPath 变量了 
				if (!string.IsNullOrEmpty (sfxPath) && Event.current.type == EventType.DragExited) {
					DragAndDrop.AcceptDrag ();
				}
				item.soundPath = sfxPath;
			}
		}

//		if(Event.current.type == EventType.)
		
		EditorGUILayout.EndHorizontal ();

		tData.eventItems [eventIndex] = item;
	}
	
	// ---------------------   Data  about   ---------------------
	private void AddStageActionEvent(ref StageActionEvent item, ref StageActionEvent[] list) {
		List<StageActionEvent> _list;
		if (list != null && list.Length > 0) {
			_list = list.ToList ();
		} else {
			_list = new List<StageActionEvent> ();
		}
		
		_list.Add (item);
		
		list = _list.ToArray ();
	}

	private static int SortCompare(StageTimeEvent AF1, StageTimeEvent AF2) {
		int res = 0;
		if (AF1.time < AF2.time) {
			res = -1;
		} else if (AF1.time > AF2.time) {
			res = 1;
		}
		return res;
	}

	private void AddStageTimeEvent(ref StageTimeEvent item, ref StageTimeEvent[] list) {
		// Check same time.
		for (int i = 0; i < list.Length; i++) {
			StageTimeEvent _event = list [i];
			if (_event.time == item.time) {
				return;
			}
		}

		List<StageTimeEvent> _list;
		if (list != null && list.Length > 0) {
			_list = list.ToList ();
		} else {
			_list = new List<StageTimeEvent> ();
		}
		
		_list.Add (item);

		// Queue by .time
		_list.Sort (SortCompare);
		
		list = _list.ToArray ();
	}

	private void SortStageTimeEvent(ref StageTimeEvent[] list) {
		List<StageTimeEvent> _list = list.ToList ();
		_list.Sort (SortCompare);
		list = _list.ToArray ();
	}

	private void AddStageTimeEventItem(ref StageTimeEventItem item, ref StageTimeEventItem[] list) {
		List<StageTimeEventItem> _list;
		if (list != null && list.Length > 0) {
			_list = list.ToList ();
		} else {
			_list = new List<StageTimeEventItem> ();
		}
		
		_list.Add (item);
		
		list = _list.ToArray ();
	}

	private bool DelStageActionEvent(int idx, ref StageActionEvent[] strList) {
		if (strList == null) {
			return false;
		}
		
		if (strList.Length <= idx) {
			return false;
		}
		
		List<StageActionEvent> _list = strList.ToList ();
		_list.RemoveAt (idx);
		strList = _list.ToArray ();

		return true;
	}

	private bool DelStageTimeEvent(int idx, ref StageTimeEvent[] strList) {
		if (strList == null) {
			return false;
		}
		
		if (strList.Length <= idx) {
			return false;
		}
		
		List<StageTimeEvent> _list = strList.ToList ();
		_list.RemoveAt (idx);
		strList = _list.ToArray ();

		return true;
	}

	private void DelStageTimeEventItem(int idx, ref StageTimeEventItem[] strList) {
		if (strList == null) {
			return;
		}
		
		if (strList.Length <= idx) {
			return;
		}

		List<StageTimeEventItem> _list = strList.ToList ();
		_list.RemoveAt (idx);
		strList = _list.ToArray ();
	}

	private void AddData() {
		StageEvent stg = new StageEvent ();
		EditorData.Instance.AddStageEvent (ref stg);
	}

	private void SaveData() {
		EditorData.Instance.StageEvents [this.eventId] = this.editData;
		this.Save ();
	}

	private void Save() {
		GameObject dataObject = EditorData.GetDataObject ();
		if (dataObject == null) {
			Debug.Log ("Editor Data prefab is null.");
			return;
		}
		
		//PrefabUtility.ReplacePrefab (this.gameObject, dataObject);
		PropertyModification[] temp = new PropertyModification[1];
		temp [0] = new PropertyModification ();
		temp [0].target = dataObject;
		PrefabUtility.SetPropertyModifications (dataObject, temp);
		EditorApplication.SaveAssets ();
		EditorData.Instance.AfterSave ();
	}

	private void SetCurrentTimeEditIndex(int value) {
		this.currentTimeEditEventIndex = value;

		if (this.currentTimeEditEventIndex > this.editData.timeEvents.Length) {
			return;
		}

		StageTimeEvent tData = this.editData.timeEvents [this.currentTimeEditEventIndex];
		this.currentEventTime = tData.time;
	}

	private void SetCurrentEditEventTime() {
		if (this.editData.timeEvents == null) {
			return;
		}

		if (this.currentTimeEditEventIndex >= this.editData.timeEvents.Length) {
			return;
		}
		
		StageTimeEvent tData = this.editData.timeEvents [this.currentTimeEditEventIndex];
		tData.time = this.currentEventTime;
		this.editData.timeEvents [this.currentTimeEditEventIndex] = tData;
	}
}