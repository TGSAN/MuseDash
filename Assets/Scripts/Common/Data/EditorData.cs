using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public struct StageTimeEventItem {
	public int actionIndex;
	public string soundPath;
	public GameObject sceneObject;
}

[System.Serializable]
public struct StageActionEvent{
	public string nodeUid;
	public GameObject sceneObject;
	public int bornActionIndex;
	public int hittedActionIndex;
	public int missActionIndex;
}

[System.Serializable]
public struct StageTimeEvent{
	public bool flod;
	public float time;
	public StageTimeEventItem[] eventItems;
}

[System.Serializable]
public struct StageEvent {
	public bool flodAction;
	public bool flodTime;
	public StageActionEvent[] actionEvents;
	public StageTimeEvent[] timeEvents;
}

[System.Serializable]
public class EditorData : MonoBehaviour {
	private static string PATH = "Prefabs/EditorData";
	private static GameObject dataObject;
	private static EditorData instance;
	[SerializeField]
	public string[] SpineActionKeys;
	public string[] SpineActionDes;
	public string[] SpineEventName;
	public string[] SpineEventDes;
	public string[] SpineModeName;
	public string[] SpineModeDes;

	public StageEvent[] StageEvents;

	public static EditorData Instance {
		get { 
			if (instance == null) {
				dataObject = Resources.Load (PATH) as GameObject;
				instance = dataObject.GetComponent<EditorData> ();
			}

			return instance;
		}
	}

	public static GameObject GetDataObject() {
		return dataObject;
	}

	public void AddStringListItem(ref string item, ref string[] strList) {
		List<string> _list;
		if (strList != null && strList.Length > 0) {
			_list = strList.ToList ();
		} else {
			_list = new List<string> ();
		}
		
		_list.Add (item);
		
		strList = _list.ToArray ();
	}

	public void DelStringListItem(int idx, ref string[] strList) {
		if (strList == null) {
			return;
		}
		
		if (strList.Length <= idx) {
			return;
		}
		
		List<string> _list = strList.ToList ();
		_list.RemoveAt (idx);
		strList = _list.ToArray ();
	}

	public void AddIntListItem(int item, ref int[] intList) {
		List<int> _list;
		if (intList != null && intList.Length > 0) {
			_list = intList.ToList ();
		} else {
			_list = new List<int> ();
		}
		
		_list.Add (item);
		
		intList = _list.ToArray ();
	}
	
	public void DelIntListItem(int idx, ref int[] intList) {
		if (intList == null) {
			return;
		}
		
		if (intList.Length <= idx) {
			return;
		}
		
		List<int> _list = intList.ToList ();
		_list.RemoveAt (idx);
		intList = _list.ToArray ();
	}

	public void AddGameObjectListItem(ref GameObject item, ref GameObject[] objList) {
		List<GameObject> _list;
		if (objList != null && objList.Length > 0) {
			_list = objList.ToList ();
		} else {
			_list = new List<GameObject> ();
		}
		
		_list.Add (item);
		
		objList = _list.ToArray ();
	}
	
	public void DelGameObjectListItem(int idx, ref GameObject[] objList) {
		if (objList == null) {
			return;
		}
		
		if (objList.Length <= idx) {
			return;
		}
		
		List<GameObject> _list = objList.ToList ();
		_list.RemoveAt (idx);
		objList = _list.ToArray ();
	}

	public void AfterSave() {
		instance = null;
	}

	// -----------------  For stage event data ---------------------
	public StageEvent GetStageEventDataById(int idx) {
		if (this.StageEvents.Length <= idx) {
			for (int i = this.StageEvents.Length; i < idx + 2; i++) {
				StageEvent item = new StageEvent ();
				this.AddStageEvent (ref item);
			}
		}

		return this.StageEvents [idx];
	}

	public void AddStageEvent(ref StageEvent item) {
		List<StageEvent> _list;
		if (this.StageEvents != null && this.StageEvents.Length > 0) {
			_list = this.StageEvents.ToList ();
		} else {
			_list = new List<StageEvent> ();
		}

		_list.Add (item);
		
		this.StageEvents = _list.ToArray ();
	}
}