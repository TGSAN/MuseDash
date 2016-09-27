 ///自定义模块，可定制模块具体行为
/// 包含存放到背包中的箱子和在开启栏的箱子
using System;

using System.Collections.Generic;
using System.Collections;


namespace FormulaBase {
	public class ChestManageComponent : CustomComponentBase {
		private static ChestManageComponent instance = null;
		private const int HOST_IDX = 9;
		public static ChestManageComponent Instance {
			get {
				if(instance == null) {
					instance = new ChestManageComponent();
				}
			return instance;
			}
		}

		public void Init() {
		}
	}
}