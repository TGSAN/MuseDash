///自定义模块，可定制模块具体行为
using System;
using UnityEngine;

namespace FormulaBase {
	public class SettingComponent : CustomComponentBase {
		private static SettingComponent instance = null;
		private const int HOST_IDX = 16;
		public static SettingComponent Instance {
			get {
				if(instance == null) {
					instance = new SettingComponent();
				}
			return instance;
			}
		}

		// -----------------------------------------------------------------
		private static float kbSize = 1024f * 1024f;
		public void Init() {
			if (this.Host == null) {
				this.Host = FomulaHostManager.Instance.LoadHost ("Setting");
			}

			//FomulaHostManager.Instance.AddHost (this.Host);
		}

		public string GetShowStr() {
			uint tam = UnityEngine.Profiler.GetTotalReservedMemory ();
			float m = tam / kbSize;
			return this.GetFps () + "fps " + String.Format ("{0:F}", this.GetMemoryUsage ()) + "Mb\n" + String.Format ("{0:F}", m) + "Mb";
		}

		/// <summary>
		/// Gets the memory usage.
		/// 
		/// 获得内存使用大小 ： x.x MB
		/// </summary>
		/// <returns>The memory usage.</returns>
		public float GetMemoryUsage() {
			//UnityEngine.Profiler.GetTotalUnusedReservedMemory () + 
			uint tam = UnityEngine.Profiler.GetTotalAllocatedMemory ();
			// uint tam = UnityEngine.Profiler.GetTotalReservedMemory ();
			float m = tam / kbSize;
			return m;
		}

		/// <summary>
		/// Gets the cpu usage.
		/// 
		/// 获得cpu占用率 ： x %
		/// </summary>
		/// <returns>The cpu usage.</returns>
		//public int GetCpuUsage() {	
		//	return 0;
		//}

		/// <summary>
		/// Gets the fps.
		/// 
		/// 获得运行时fps，场景内必须有物件挂有ShowFPS
		/// 每秒更新1次
		/// </summary>
		/// <returns>The fps.</returns>
		public int GetFps() {
			if (ShowFPS.Instance == null) {
				return -1;
			}

			return (int)ShowFPS.Instance.GetFps ();
		}
	}
}