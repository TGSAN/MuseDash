///自定义模块，可定制模块具体行为
using System;
using UnityEngine;
namespace FormulaBase {
	public class AccountEnergyManagerComponent : CustomComponentBase {
		private static AccountEnergyManagerComponent instance = null;
		private const int HOST_IDX = 1;
		public static AccountEnergyManagerComponent Instance {
			get {
				if(instance == null) {
					instance = new AccountEnergyManagerComponent();
				}
			return instance;
			}
		}

		public bool GainEnergy(FormulaHost host, int number){
			int current = (int)host.GetDynamicDataByKey("ENERGY") + number;
			Debug.Log("adfter gain ENERGY : " + current);
			host.SetDynamicData("ENERGY",current);
			if((int)host.GetDynamicDataByKey("ENERGY") == current){
				return true;
			}else{
				return false;
			}

		}

		public bool ConsumeEnergy(FormulaHost host, int number){
			int current = (int)host.GetDynamicDataByKey("ENERGY") - number;
			Debug.Log("adfter gain ENERGY : " + current);
			host.SetDynamicData("ENERGY",current);
			if((int)host.GetDynamicDataByKey("ENERGY") == current){
				return true;
			}else{
				return false;
			}
		}
	}
}