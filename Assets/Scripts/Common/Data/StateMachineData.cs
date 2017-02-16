using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public struct StateMachineScripts {
	public string des;
	public string scriptName;
}

[System.Serializable]
public struct StateMachineCondiction {
	public int idx;
	public int childIdx;
	public List<string> condictionScripts;
}

[System.Serializable]
public struct StateMachineNode {
	public int idx;
	public int parentIdx;
	public string des;
	public string nodeScript;
	public Rect nodeWindow;
	public List<StateMachineCondiction> condictions;
}

[System.Serializable]
public struct StateMachine {
	public int id;
	public string des;
	public List<StateMachineNode> nodes;
}

[System.Serializable]
public class StateMachineData : MonoBehaviour {
	private static string PATH = "Prefabs/StateMachineData";
	private static GameObject dataObject;
	private static StateMachineData instance;

	[SerializeField]
	public List<StateMachine> StateMachines;
	public List<StateMachineScripts> nodeScripts;
	public List<StateMachineScripts> condScripts;

	public static StateMachineData Instance {
		get { 
			if (instance == null) {
				dataObject = Resources.Load (PATH) as GameObject;
				instance = dataObject.GetComponent<StateMachineData> ();
			}

			return instance;
		}
	}

	public static GameObject GetDataObject() {
		return dataObject;
	}

	public int GetNodeScriptsIndexByNames(string name) {
		if (this.nodeScripts == null) {
			return -1;
		}
		
		StateMachineScripts ms = this.nodeScripts.Find (delegate (StateMachineScripts _ms) {
			return _ms.scriptName == name;
		});
		
		return this.nodeScripts.IndexOf (ms);
	}

	public string[] GetNodeScriptsNames() {
		if (this.nodeScripts == null) {
			return null;
		}

		string[] names = new string[this.nodeScripts.Count];
		for (int i = 0; i < this.nodeScripts.Count; i++) {
			StateMachineScripts _sms = this.nodeScripts [i];
			names [i] = _sms.scriptName;
		}

		return names;
	}

	public string[] GetNodeScriptsNamesWithDes() {
		if (this.nodeScripts == null) {
			return null;
		}
		
		string[] names = new string[this.nodeScripts.Count];
		for (int i = 0; i < this.nodeScripts.Count; i++) {
			StateMachineScripts _sms = this.nodeScripts [i];
			names [i] = _sms.scriptName + " " + _sms.des;
		}
		
		return names;
	}

	public bool SetNodeScriptDes(string name, string des) {
		if (name == null || name.Length <= 0) {
			return false;
		}

		if (des == null || des.Length <= 0) {
			return false;
		}

		if (this.nodeScripts == null) {
			return false;
		}

		StateMachineScripts sms = this.nodeScripts.Find (delegate (StateMachineScripts _script) {
			return _script.scriptName == name;
		});

		int __idx = this.nodeScripts.IndexOf (sms);
		if (__idx < 0) {
			return false;
		}

		sms.des = des;
		this.nodeScripts [__idx] = sms;
		return true;
	}

	public void AddNodeScript(string name) {
		if (name == null || name.Length <= 0) {
			return;
		}

		if (this.nodeScripts == null) {
			this.nodeScripts = new List<StateMachineScripts> ();
		}

		StateMachineScripts sms = new StateMachineScripts ();
		sms.scriptName = name;

		this.nodeScripts.Add (sms);
	}

	public string[] GetCondScriptsNames() {
		if (this.condScripts == null) {
			return null;
		}
		
		string[] names = new string[this.condScripts.Count];
		for (int i = 0; i < this.condScripts.Count; i++) {
			names [i] = this.condScripts [i].scriptName;
		}
		
		return names;
	}

	public string[] GetCondScriptsNamesWithDes() {
		if (this.condScripts == null) {
			return null;
		}
		
		string[] names = new string[this.condScripts.Count];
		for (int i = 0; i < this.condScripts.Count; i++) {
			StateMachineScripts _sms = this.condScripts [i];
			names [i] = _sms.scriptName + " " + _sms.des;
		}
		
		return names;
	}

	public bool SetCondScriptDes(string name, string des) {
		if (name == null || name.Length <= 0) {
			return false;
		}
		
		if (des == null || des.Length <= 0) {
			return false;
		}
		
		if (this.condScripts == null) {
			return false;
		}
		
		StateMachineScripts sms = this.condScripts.Find (delegate (StateMachineScripts _script) {
			return _script.scriptName == name;
		});
		
		int __idx = this.condScripts.IndexOf (sms);
		if (__idx < 0) {
			return false;
		}
		
		sms.des = des;
		this.condScripts [__idx] = sms;
		return true;
	}
	
	public void AddCondScript(string name) {
		if (name == null || name.Length <= 0) {
			return;
		}
		
		if (this.condScripts == null) {
			this.condScripts = new List<StateMachineScripts> ();
		}
		
		StateMachineScripts sms = new StateMachineScripts ();
		sms.scriptName = name;
		
		this.condScripts.Add (sms);
	}

	public void SetDes(int id, string des) {
		if (id >= this.StateMachines.Count) {
			return;
		}

		StateMachine sm = this.StateMachines [id];
		sm.des = des;
		this.StateMachines [id] = sm;
	}

	public void SetNodeDes(int id, int idx, string des) {
		if (id >= this.StateMachines.Count) {
			return;
		}

		StateMachine sm = this.StateMachines [id];
		if (idx < 0 || sm.nodes == null || idx >= sm.nodes.Count) {
			return;
		}
		
		StateMachineNode snd = sm.nodes.Find (delegate (StateMachineNode _node) {
			return _node.idx == idx;
		});
		
		int __idx = sm.nodes.IndexOf (snd);
		if (__idx < 0) {
			return;
		}

		snd.des = des;
		sm.nodes [__idx] = snd;
		this.StateMachines [id] = sm;
	}

	public void SetNodeScript(int id, int idx, string scriptName) {
		if (id >= this.StateMachines.Count) {
			return;
		}
		
		StateMachine sm = this.StateMachines [id];
		if (idx < 0 || sm.nodes == null || idx >= sm.nodes.Count) {
			return;
		}
		
		StateMachineNode snd = sm.nodes.Find (delegate (StateMachineNode _node) {
			return _node.idx == idx;
		});
		
		int __idx = sm.nodes.IndexOf (snd);
		if (__idx < 0) {
			return;
		}
		
		snd.nodeScript = scriptName;
		sm.nodes [__idx] = snd;
		this.StateMachines [id] = sm;
	}

	public void SetCondictionIdx(int id, int idx, int nidx) {
		if (id >= this.StateMachines.Count) {
			return;
		}
		
		StateMachine sm = this.StateMachines [id];
		if (idx < 0 || sm.nodes == null || idx >= sm.nodes.Count) {
			return;
		}
		
		StateMachineNode snd = sm.nodes.Find (delegate (StateMachineNode _node) {
			return _node.idx == idx;
		});

		int __idx = sm.nodes.IndexOf (snd);
		if (__idx < 0) {
			return;
		}

		snd.idx = nidx;
		sm.nodes [__idx] = snd;
		this.StateMachines [id] = sm;
	}

	public void SetCondictionParent(int id, int idx, int nidx) {
		if (id >= this.StateMachines.Count) {
			return;
		}
		
		StateMachine sm = this.StateMachines [id];
		if (idx < 0 || sm.nodes == null || idx >= sm.nodes.Count) {
			return;
		}
		
		StateMachineNode snd = sm.nodes.Find (delegate (StateMachineNode _node) {
			return _node.idx == idx;
		});

		int __idx = sm.nodes.IndexOf (snd);
		if (__idx < 0) {
			return;
		}

		snd.parentIdx = nidx;
		sm.nodes [__idx] = snd;
		this.StateMachines [id] = sm;
	}

	public void SetCondictionDes(int id, int idx, string des) {
		if (id >= this.StateMachines.Count) {
			return;
		}

		StateMachine sm = this.StateMachines [id];
		//Debug.Log ("SetCondictionDes " + id + " / " + idx + " / " + sm.nodes.Count + " / " + des);
		if (idx < 0 || sm.nodes == null || idx >= sm.nodes.Count) {
			return;
		}

		StateMachineNode snd = sm.nodes.Find (delegate (StateMachineNode _node) {
			return _node.idx == idx;
		});

		int __idx = sm.nodes.IndexOf (snd);
		if (__idx < 0) {
			return;
		}

		snd.des = des;
		sm.nodes [__idx] = snd;
		this.StateMachines [id] = sm;
	}

	public void SetCondictionRect(int id, int idx, Rect rect) {
		if (id >= this.StateMachines.Count) {
			return;
		}
		
		StateMachine sm = this.StateMachines [id];
		if (idx < 0 || sm.nodes == null) {
			return;
		}
		
		StateMachineNode snd = sm.nodes.Find (delegate (StateMachineNode _node) {
			return _node.idx == idx;
		});

		int __idx = sm.nodes.IndexOf (snd);
		if (__idx < 0) {
			return;
		}

		snd.nodeWindow = rect;
		sm.nodes [__idx] = snd;
		this.StateMachines [id] = sm;
	}

	public void SetCondictionChild(int id, int idx, int childIdx) {
		if (id >= this.StateMachines.Count) {
			return;
		}
		
		StateMachine sm = this.StateMachines [id];
		if (idx < 0 || sm.nodes == null) {
			return;
		}
		
		StateMachineNode snd = sm.nodes.Find (delegate (StateMachineNode _node) {
			return _node.idx == idx;
		});

		int __idx = sm.nodes.IndexOf (snd);
		if (__idx < 0) {
			return;
		}

		if (snd.condictions == null) {
			snd.condictions = new List<StateMachineCondiction> ();
		}

		int condIdx = 0;
		for (int k = 0; k < snd.condictions.Count; k++) {
			StateMachineCondiction _smc = snd.condictions [k];
			if (_smc.idx > condIdx) {
				condIdx = _smc.idx;
			}
		}

		StateMachineCondiction smc = new StateMachineCondiction ();
		smc.idx = condIdx + 1;
		smc.childIdx = childIdx;
		snd.condictions.Add (smc);

		int __cndidx = snd.condictions.Count - 1;
		snd.condictions [__cndidx] = smc;

		sm.nodes [__idx] = snd;
		this.StateMachines [id] = sm;
	}

	public void SetCondictionChildIdx(int id, int idx, int condIdx, int childIdx) {
		if (id >= this.StateMachines.Count) {
			return;
		}
		
		StateMachine sm = this.StateMachines [id];
		if (idx < 0 || sm.nodes == null) {
			return;
		}
		
		StateMachineNode snd = sm.nodes.Find (delegate (StateMachineNode _node) {
			return _node.idx == idx;
		});
		
		int __idx = sm.nodes.IndexOf (snd);
		if (__idx < 0) {
			return;
		}
		
		if (snd.condictions == null) {
			return;
		}
		
		StateMachineCondiction smc = snd.condictions.Find (delegate (StateMachineCondiction _condiction) {
			return _condiction.idx == condIdx;
		});

		int __cndidx = snd.condictions.IndexOf (smc);
		if (__cndidx < 0) {
			return;
		}

		smc.childIdx = childIdx;
		snd.condictions [__cndidx] = smc;
		sm.nodes [__idx] = snd;
		this.StateMachines [id] = sm;
	}

	public void SetCondictionData(int id, int idx, int condIdx, StateMachineCondiction data) {
		if (id >= this.StateMachines.Count) {
			return;
		}
		
		StateMachine sm = this.StateMachines [id];
		if (idx < 0 || sm.nodes == null) {
			return;
		}

		StateMachineNode snd = sm.nodes.Find (delegate (StateMachineNode _node) {
			return _node.idx == idx;
		});
		
		int __idx = sm.nodes.IndexOf (snd);
		if (__idx < 0) {
			return;
		}

		if (condIdx < 0 || snd.condictions == null) {
			return;
		}
		
		StateMachineCondiction smc = snd.condictions.Find (delegate (StateMachineCondiction _condiction) {
			return _condiction.idx == condIdx;
		});
		
		int __cndidx = snd.condictions.IndexOf (smc);
		if (__cndidx < 0) {
			return;
		}

		snd.condictions [__cndidx] = data;
		sm.nodes [__idx] = snd;
		this.StateMachines [id] = sm;
	}

	public void NewCondictionScript(int id, int idx, int condIdx) {
		if (id >= this.StateMachines.Count) {
			return;
		}

		StateMachine sm = this.StateMachines [id];
		if (idx < 0 || sm.nodes == null) {
			return;
		}

		StateMachineNode snd = sm.nodes.Find (delegate (StateMachineNode _node) {
			return _node.idx == idx;
		});

		int __idx = sm.nodes.IndexOf (snd);
		if (__idx < 0) {
			return;
		}

		if (condIdx < 0 || snd.condictions == null) {
			return;
		}

		StateMachineCondiction smc = snd.condictions.Find (delegate (StateMachineCondiction _condiction) {
			return _condiction.idx == condIdx;
		});

		int __cndidx = snd.condictions.IndexOf (smc);
		if (__cndidx < 0) {
			return;
		}

		if (smc.condictionScripts == null) {
			smc.condictionScripts = new List<string> ();
		}

		smc.condictionScripts.Add ("empty");

		snd.condictions [__cndidx] = smc;
		sm.nodes [__idx] = snd;
		this.StateMachines [id] = sm;
	}

	public void AfterSave() {
		instance = null;
	}
}