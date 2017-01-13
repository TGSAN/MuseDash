using UnityEngine;  
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

  
public class StateMachineEditor : EditorWindow {
	private const string CODE_PATH_STATE = "/Scripts/StatesNodes/";
	private const string CODE_PATH_COND = "/Scripts/StatesCondictions/";
	private const string DES_1 = "状态机";
	private const string DES_2 = "开始";
	private const string DES_3 = "状态";
	private const string DES_4 = "详细";
	private const string DES_5 = "条件";
	private const string DES_6 = "节点脚本";
	private const string DES_7 = "条件脚本";

	private const int CHILD_WINDOW_POS = 50;
	private const int CHILD_WINDOW_SIZE_WIDTH = 100;
	private const int CHILD_WINDOW_SIZE_WIDTH2 = 300;
	private const int CHILD_WINDOW_SIZE_HEIGH = 40;
	private const int CHILD_WINDOW_SIZE_HEIGH2 = 20;
	private const int CONDICTION_WINDOW_IDX = 999;
	private const int DES_WINDOW_IDX = 998;

	private string[] DEFAULT_STR_LIST = new string[]{};
	private StateMachineNode DEFAULT_NODE_DATA = new StateMachineNode();

	private bool showCondictionWindow = false;
	private bool showDesWindow = false;

	private int currentNodeScriptIdx = 0;
	private string currentNodeScriptName = "";
	private string currentNodeDes = "";

	private int currentCondScriptIdx = 0;
	private string currentCondScriptName = "";
	private string currentCondDes = "";

	private Vector2 scorllVecAction;
	private int msId = 0;
	private int ndId = -1;
	private int condId = -1;
	private StateMachine currentData;
	private StateMachineNode currentNodeData;
	private StateMachineCondiction currentCondData;

	//窗口的矩形
	private Rect windowCondictionRect = new Rect (100, 100, CHILD_WINDOW_SIZE_WIDTH, CHILD_WINDOW_SIZE_HEIGH);
	private Rect windowDesRect = new Rect (100, 100, CHILD_WINDOW_SIZE_WIDTH2, CHILD_WINDOW_SIZE_HEIGH);

	[MenuItem("RHY/" + DES_1)]
	static void ShowEditor() {
		StateMachineEditor editor = EditorWindow.GetWindow<StateMachineEditor> ();
		editor.ndId = -1;
		editor.condId = -1;
		editor.Show ();
	}

	void OnGUI() {
		this.MkScripts ();
		this.MkTitle ();
		this.MkStates ();
	}

	private void MkScripts() {
		string[] ns = StateMachineData.Instance.GetNodeScriptsNamesWithDes ();
		string[] cs = StateMachineData.Instance.GetCondScriptsNamesWithDes ();
		EditorGUILayout.BeginHorizontal ();
		this.currentNodeScriptIdx = EditorGUILayout.Popup (DES_6, this.currentNodeScriptIdx, ns);
		this.currentNodeScriptName = EditorGUILayout.TextField (this.currentNodeScriptName);
		if ((this.currentNodeScriptName == null || this.currentNodeScriptName.Length <= 0) && ns != null && this.currentNodeScriptIdx < ns.Length) {
			string[] _ns = StateMachineData.Instance.GetNodeScriptsNames ();
			this.currentNodeScriptName = _ns [this.currentNodeScriptIdx];
		}

		this.currentNodeDes = EditorGUILayout.TextField (this.currentNodeDes);
		if (GUILayout.Button ("Set")) {
			if (!StateMachineData.Instance.SetNodeScriptDes (this.currentNodeScriptName, this.currentNodeDes)) {
				StateMachineData.Instance.AddNodeScript (this.currentNodeScriptName);
				StateMachineData.Instance.SetNodeScriptDes (this.currentNodeScriptName, this.currentNodeDes);
			}
		}
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.BeginHorizontal ();
		this.currentCondScriptIdx = EditorGUILayout.Popup (DES_7, this.currentCondScriptIdx, cs);
		this.currentCondScriptName = EditorGUILayout.TextField (this.currentCondScriptName);
		if ((this.currentCondScriptName == null || this.currentCondScriptName.Length <= 0) && cs != null && this.currentCondScriptIdx < cs.Length) {
			string[] _cs = StateMachineData.Instance.GetCondScriptsNames ();
			this.currentCondScriptName = _cs [this.currentCondScriptIdx];
		}
		
		this.currentCondDes = EditorGUILayout.TextField (this.currentCondDes);
		if (GUILayout.Button ("Set")) {
			if (!StateMachineData.Instance.SetCondScriptDes (this.currentCondScriptName, this.currentCondDes)) {
				StateMachineData.Instance.AddCondScript (this.currentCondScriptName);
				StateMachineData.Instance.SetCondScriptDes (this.currentCondScriptName, this.currentCondDes);
			}
		}
		EditorGUILayout.EndHorizontal ();
	}

	private string[] GetMSDes() {
		if (StateMachineData.Instance.StateMachines == null || StateMachineData.Instance.StateMachines.Count <= 0) {
			return DEFAULT_STR_LIST;
		}

		string[] s = new string[StateMachineData.Instance.StateMachines.Count];
		for (int i = 0; i < StateMachineData.Instance.StateMachines.Count; i++) {
			s [i] = StateMachineData.Instance.StateMachines [i].des;
		}

		return s;
	}

	private void NewNode(int parentIdx) {
		if (this.msId >= StateMachineData.Instance.StateMachines.Count) {
			return;
		}

		this.currentNodeData = new StateMachineNode ();
		if (this.currentData.nodes == null) {
			this.currentData.nodes = new System.Collections.Generic.List<StateMachineNode> ();
		}

		int _idx = -1;
		for (int i = 0; i < this.currentData.nodes.Count; i++) {
			if (this.currentData.nodes [i].idx > _idx) {
				_idx = this.currentData.nodes [i].idx;
			}
		}

		int _pNdId = this.ndId;

		this.ndId = _idx + 1;
		this.currentNodeData.idx = this.ndId;
		this.currentNodeData.parentIdx = _pNdId;

		string _des = DES_3 + this.currentNodeData.idx;
		this.currentNodeData.des = _des;

		this.currentData.nodes.Add (this.currentNodeData);

		StateMachineData.Instance.StateMachines [this.msId] = this.currentData;
		StateMachineData.Instance.SetCondictionParent (this.msId, this.ndId, parentIdx);
		StateMachineData.Instance.SetCondictionIdx (this.msId, this.ndId, this.ndId);
		if (parentIdx >= 0) {
			StateMachineData.Instance.SetCondictionDes (this.msId, this.ndId, _des);
		} else {
			StateMachineData.Instance.SetCondictionDes (this.msId, this.ndId, DES_2);
		}
		StateMachineData.Instance.SetCondictionRect (this.msId, this.ndId, new Rect (CHILD_WINDOW_POS, CHILD_WINDOW_POS, CHILD_WINDOW_SIZE_WIDTH, CHILD_WINDOW_SIZE_HEIGH));

		if (parentIdx >= 0) {
			StateMachineData.Instance.SetCondictionChild (this.msId, parentIdx, this.ndId);
		}

		//this.currentNodeData = this.currentData.nodes [this.ndId];

		this.Repaint ();
	}

	private void NewData() {
		this.ndId = -1;

		this.currentData = new StateMachine ();
		this.currentData.des = DES_1 + this.msId;
		StateMachineData.Instance.StateMachines.Add (this.currentData);

		this.msId = StateMachineData.Instance.StateMachines.Count - 1;
		this.currentData = StateMachineData.Instance.StateMachines [this.msId];

		this.Repaint ();

		this.NewNode (-1);
	}

	private void SaveData() {
		EditorCommon.SaveStateMachine ();
		if (StateMachineData.Instance.nodeScripts != null) {
			for (int i = 0; i < StateMachineData.Instance.nodeScripts.Count; i++) {
				StateMachineScripts sms = StateMachineData.Instance.nodeScripts[i];
				StateMachineGenerator.WriteState (CODE_PATH_STATE, "StateMachineModule", sms.scriptName, sms.des);
			}
		}

		if (StateMachineData.Instance.condScripts != null) {
			for (int i = 0; i < StateMachineData.Instance.condScripts.Count; i++) {
				StateMachineScripts sms = StateMachineData.Instance.condScripts[i];
				StateMachineGenerator.WriteCondiction (CODE_PATH_COND, "StateMachineModule", sms.scriptName, sms.des);
			}
		}
		
		this.Repaint();
	}

	private void MkTitle() {
		EditorGUILayout.BeginVertical ();

		EditorGUILayout.BeginHorizontal ();
		this.msId = EditorGUILayout.Popup (DES_1, this.msId, this.GetMSDes ());
		if (this.msId < StateMachineData.Instance.StateMachines.Count) {
			this.currentData = StateMachineData.Instance.StateMachines [this.msId];
		}

		StateMachineData.Instance.SetDes (this.msId, EditorGUILayout.TextField (this.currentData.des));

		if (GUILayout.Button ("New")) {
			this.NewData ();
			return;
		}

		if (GUILayout.Button ("Save")) {
			this.SaveData ();
			return;
		}
		
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.EndVertical ();
	}

	private void MkStates() {
		if (this.currentData.nodes == null || this.currentData.nodes.Count <= 0) {
			return;
		}

		this.scorllVecAction = EditorGUILayout.BeginScrollView (this.scorllVecAction);
		//绘画窗口
		this.BeginWindows ();

		for (int i = 0; i < this.currentData.nodes.Count; i++) {
			StateMachineNode nd = this.currentData.nodes [i];
			float h = CHILD_WINDOW_SIZE_HEIGH;
			if (nd.condictions != null) {
				h = (CHILD_WINDOW_SIZE_HEIGH * (nd.condictions.Count + 1));
			}

			string _title = nd.des;
			if (i == 0) {
				_title = DES_2;
			}

			nd.nodeWindow.Set (nd.nodeWindow.x, nd.nodeWindow.y, CHILD_WINDOW_SIZE_WIDTH, h);
			nd.nodeWindow = GUI.Window (nd.idx, nd.nodeWindow, this.DrawNodeWindow, _title);
			this.currentData.nodes [i] = nd;
		}

		for (int i = 0; i < this.currentData.nodes.Count; i++) {
			StateMachineNode nd = this.currentData.nodes [i];
			if (nd.condictions != null) {
				for (int k = 0; k < nd.condictions.Count; k++) {
					StateMachineCondiction smc = nd.condictions [k];
					//StateMachineNode _nd = this.currentData.nodes [smc.childIdx];
					StateMachineNode _nd = this.currentData.nodes.Find (delegate (StateMachineNode _node) {
						return _node.idx == smc.childIdx;
					});

					this.DrawNodeCurve (nd.nodeWindow, _nd.nodeWindow, Color.green);
				}
			}
		}

		if (this.ndId >= 0 && this.showCondictionWindow) {
			//this.windowCondictionRect.position = this.currentNodeData.nodeWindow.position;
			this.windowCondictionRect.height = CHILD_WINDOW_SIZE_HEIGH;
			if (this.currentNodeData.condictions != null) {
				StateMachineCondiction smc = this.currentNodeData.condictions.Find (delegate (StateMachineCondiction _condiction) {
					return _condiction.idx == this.condId;
				});

				if (smc.condictionScripts != null) {
					this.windowCondictionRect.height = CHILD_WINDOW_SIZE_HEIGH2 * smc.condictionScripts.Count + CHILD_WINDOW_SIZE_HEIGH;
				}
			}

			this.windowCondictionRect = GUI.Window (CONDICTION_WINDOW_IDX, this.windowCondictionRect, this.DrawCondictionWindow, DES_5);
		}

		if (this.ndId >= 0 && this.showDesWindow) {
			this.windowDesRect = GUI.Window (DES_WINDOW_IDX, this.windowDesRect, this.DrawDesWindow, DES_4);
		}

		this.EndWindows ();
		
		EditorGUILayout.EndScrollView ();
	}

	private void NewCondictionScript() {
		StateMachineData.Instance.NewCondictionScript (this.msId, this.ndId, this.condId);
	}

	private void SetSelectedNode(int idx) {
		this.ndId = idx;
		if (this.ndId < 0) {
			return;
		}

		if (this.currentData.nodes != null) {
			this.currentNodeData = this.currentData.nodes.Find (delegate (StateMachineNode _node) {
				return _node.idx == this.ndId;
			});
		}
	}

	private void SetSelectedCondiction(int condIdx) {
		this.condId = condId;
	}

	private void DeleteNode(int id, int idx) {
		if (id < 0 || StateMachineData.Instance.StateMachines == null || id >= StateMachineData.Instance.StateMachines.Count) {
			return;
		}

		StateMachine sm = StateMachineData.Instance.StateMachines [id];
		if (idx < 0 || sm.nodes == null) {
			return;
		}

		StateMachineNode snd = new StateMachineNode ();
		snd.idx = -1;
		for (int i = 0; i < sm.nodes.Count; i++) {
			if (sm.nodes [i].idx == idx) {
				snd = sm.nodes [i];
			}
		}

		if (snd.idx < 0) {
			return;
		}

		// delete self.
		sm.nodes.Remove (snd);
		StateMachineData.Instance.StateMachines [id] = sm;

		// delete condiction from parent.
		int pidx = -1;
		StateMachineNode parendSnd = new StateMachineNode ();
		parendSnd.idx = -1;
		for (int i = 0; i < sm.nodes.Count; i++) {
			if (sm.nodes [i].idx != snd.parentIdx) {
				continue;
			}

			pidx = i;
			parendSnd = sm.nodes [i];
			break;
		}

		if (parendSnd.idx < 0 || parendSnd.condictions == null || parendSnd.condictions.Count <= 0) {
			return;
		}

		for (int k = 0; k < parendSnd.condictions.Count; k++) {
			StateMachineCondiction smc = parendSnd.condictions [k];
			if (smc.childIdx != snd.idx) {
				continue;
			}
			
			parendSnd.condictions.Remove (smc);
			break;
		}

		sm.nodes [pidx] = parendSnd;
		StateMachineData.Instance.StateMachines [id] = sm;
	}

	private string[] GetNodeNames() {
		if (this.currentData.nodes == null) {
			return null;
		}

		string[] names = new string[this.currentData.nodes.Count];
		for (int i = 0; i < this.currentData.nodes.Count; i++) {
			StateMachineNode smn = this.currentData.nodes [i];
			names [i] = smn.des;
		}

		return names;
	}

	private int MatchNameIdxByCondictionChildIdx() {
		if (this.currentData.nodes == null) {
			return -1;
		}

		StateMachineCondiction smc = this.currentNodeData.condictions.Find (delegate (StateMachineCondiction _condiction) {
			return _condiction.idx == this.condId;
		});
		for (int i = 0; i < this.currentData.nodes.Count; i++) {
			StateMachineNode smn = this.currentData.nodes [i];
			if (smn.idx == smc.childIdx) {
				return i;
			}
		}

		return -1;
	}

	//绘画窗口函数
	/// <summary>
	/// Draws the node window.
	/// 
	/// 节点
	/// </summary>
	/// <param name="id">Identifier.</param>
	private void DrawNodeWindow(int id) {
		//this.ndId = id;
		EditorGUILayout.BeginVertical ();

		EditorGUILayout.BeginHorizontal ();
		if (GUILayout.Button (DES_4)) {
			this.showCondictionWindow = false;
			if (this.ndId == id) {
				this.showDesWindow = !this.showDesWindow;
			} else {
				this.showDesWindow = true;
			}
			this.SetSelectedNode (id);
		}

		if (GUILayout.Button ("+")) {
			this.showCondictionWindow = false;
			this.showDesWindow = false;
			this.SetSelectedNode (id);
			this.NewNode (id);
			return;
		}

		if (GUILayout.Button ("-")) {
			this.showCondictionWindow = false;
			this.showDesWindow = false;
			this.SetSelectedNode (id);
			this.DeleteNode (this.msId, id);
			return;
		}

		EditorGUILayout.EndHorizontal ();

		StateMachineNode smn = this.currentData.nodes.Find (delegate (StateMachineNode _node) {
			return _node.idx == id;
		});

		if (smn.condictions != null && smn.condictions.Count > 0) {
			for (int i = 0; i < smn.condictions.Count; i++) {
				EditorGUILayout.BeginHorizontal ();

				StateMachineCondiction smc = smn.condictions [i];
				if (GUILayout.Button ("->")) {
					this.condId = smc.idx;
					this.SetSelectedNode (id);
					this.SetSelectedCondiction (smc.idx);
					//this.showCondictionWindow = !this.showCondictionWindow;
					this.showCondictionWindow = true;
					this.showDesWindow = false;
					// this.Repaint ();
					return;
				}
				
				EditorGUILayout.EndHorizontal ();
			}
		}

		EditorGUILayout.EndVertical ();

		//设置改窗口可以拖动  
		GUI.DragWindow ();
	}

	/// <summary>
	/// Draws the DES window.
	/// 
	/// 节点说明窗口
	/// </summary>
	/// <param name="id">Identifier.</param>
	private void DrawDesWindow(int id) {
		string[] nodeScripts = StateMachineData.Instance.GetNodeScriptsNames ();
		int sidx = StateMachineData.Instance.GetNodeScriptsIndexByNames (this.currentNodeData.nodeScript);
		if (sidx < 0 && nodeScripts != null && nodeScripts.Length > 0) {
			sidx = 0;
		}

		EditorGUILayout.BeginHorizontal ();

		StateMachineData.Instance.SetNodeDes (this.msId, this.ndId, EditorGUILayout.TextField (this.currentNodeData.des));
		if (sidx >= 0) {
			StateMachineData.Instance.SetNodeScript (this.msId, this.ndId, nodeScripts [EditorGUILayout.Popup (sidx, nodeScripts)]);
		}

		this.SetSelectedNode (this.ndId);

		EditorGUILayout.EndHorizontal ();
		//设置改窗口可以拖动  
		GUI.DragWindow ();
	}

	/// <summary>
	/// Draws the condiction window.
	/// 
	/// 条件设置窗口
	/// </summary>
	/// <param name="id">Identifier.</param>
	private void DrawCondictionWindow(int id) {
		EditorGUILayout.BeginVertical ();
		
		EditorGUILayout.BeginHorizontal ();
		if (GUILayout.Button ("+")) {
			this.NewCondictionScript ();
			this.Repaint ();
		}

		string[] nns = this.GetNodeNames ();
		int _cidx = this.MatchNameIdxByCondictionChildIdx ();
		if (nns != null) {
			//this.currentCondData.childIdx
			_cidx = EditorGUILayout.Popup (_cidx, nns);
			StateMachineNode _smn = this.currentData.nodes [_cidx];
			StateMachineData.Instance.SetCondictionChildIdx (this.msId, this.ndId, this.condId, _smn.idx);
		}

		EditorGUILayout.EndHorizontal ();
		
		EditorGUILayout.EndVertical ();
		
		if (this.currentNodeData.condictions == null) {
			//设置改窗口可以拖动  
			GUI.DragWindow ();
			return;
		}

		string[] cs = StateMachineData.Instance.GetCondScriptsNames ();
		List<string> _slist = cs.ToList ();
		EditorGUILayout.BeginVertical ();
		for (int i = 0; i < this.currentNodeData.condictions.Count; i++) {
			StateMachineCondiction smc = this.currentNodeData.condictions [i];
			if (smc.idx != this.condId) {
				continue;
			}

			if (smc.condictionScripts == null || smc.condictionScripts.Count <= 0) {
				break;
			}

			for (int k = 0; k < smc.condictionScripts.Count; k++) {
				string s = smc.condictionScripts [k];
				int __idx = _slist.IndexOf (s);
				if (__idx < 0) {
					__idx = 0;
				}
				smc.condictionScripts [k] = _slist [EditorGUILayout.Popup (__idx, cs)];
			}
			
			this.currentNodeData.condictions [i] = smc;
			this.currentCondData = this.currentNodeData.condictions [i];
			
			break;
		}
		
		EditorGUILayout.EndVertical ();
		
		//设置改窗口可以拖动  
		GUI.DragWindow ();
	}

	// 绘画连线
	private void DrawNodeCurve(Rect start, Rect end, Color color) {
		Vector3 startPos = new Vector3 (start.x + start.width, start.y + start.height * 0.5f, 0);
		Vector3 endPos = new Vector3 (end.x, end.y + end.height * 0.5f, 0);
		Vector3 startTan = startPos + Vector3.right * 10;
		Vector3 endTan = endPos + Vector3.left * 10;
		Handles.DrawBezier (startPos, endPos, startTan, endTan, color, null, 4);
	}
}