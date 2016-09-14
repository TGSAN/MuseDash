using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace StateMachineModule {
	public class StateMachineObject {
		private StateMachine data;
		private Dictionary<int, OnStateBase> nodes = null;

		public StateMachineObject(StateMachine data) {
			this.data = data;
			if (this.data.nodes == null || this.data.nodes.Count <= 0) {
				return;
			}

			this.nodes = new Dictionary<int, OnStateBase> ();
			for (int i = 0; i < this.data.nodes.Count; i++) {
				StateMachineNode smn = this.data.nodes [i];
				nodes [smn.idx] = new OnStateBase ();
			}
		}

		public int GetId() {
			return this.data.id;
		}
	}

	public class StateMachineManager {
		private static StateMachineManager instance = null;
		public static StateMachineManager Instance {
			get {
				if (instance == null) {
					instance = new StateMachineManager ();
				}

				return instance;
			}
		}

		private static Dictionary<string, Type> TYPE_POLL;
		private static System.Reflection.Assembly assembly = System.Reflection.Assembly.Load("Assembly-CSharp");

		public StateMachineManager() {
			if (TYPE_POLL != null && TYPE_POLL.Count > 0) {
				return;
			}

			if (StateMachineData.Instance.condScripts != null) {
				for (int i = 0; i < StateMachineData.Instance.condScripts.Count; i++) {
					StateMachineScripts smc = StateMachineData.Instance.condScripts [i];
					TYPE_POLL [smc.scriptName] = assembly.GetType ("StateMachineModule." + smc.scriptName);
				}
			}

			if (StateMachineData.Instance.nodeScripts != null) {
				for (int i = 0; i < StateMachineData.Instance.nodeScripts.Count; i++) {
					StateMachineScripts smc = StateMachineData.Instance.nodeScripts [i];
					TYPE_POLL [smc.scriptName] = assembly.GetType ("StateMachineModule." + smc.scriptName);
				}
			}
		}

		public void Create(int stateMachineId) {
			if (StateMachineData.Instance == null ||
				StateMachineData.Instance.StateMachines == null ||
				stateMachineId >= StateMachineData.Instance.StateMachines.Count) {
				Debug.Log ("StateMachineData has no data.");
				return;
			}

			StateMachine sm = StateMachineData.Instance.StateMachines [stateMachineId];
			StateMachineObject smObj = new StateMachineObject (sm);
		}

		public OnStateBase CreateState(string name) {
			if (TYPE_POLL.ContainsKey (name)) {
				return null;
			}

			Type t = TYPE_POLL [name];
			return (OnStateBase)t.Assembly.CreateInstance ("StateMachineModule." + name);
		}

		public OnCondictionBase CreateCondiction(string name) {
			if (TYPE_POLL.ContainsKey (name)) {
				return null;
			}
			
			Type t = TYPE_POLL [name];
			return (OnCondictionBase)t.Assembly.CreateInstance ("StateMachineModule." + name);
		}
	}
}