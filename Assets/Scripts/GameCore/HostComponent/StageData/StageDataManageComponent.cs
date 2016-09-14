///自定义模块，可定制模块具体行为
using System;
namespace FormulaBase {
	public class StageDataManageComponent : CustomComponentBase {
		private static StageDataManageComponent instance = null;
		private const int HOST_IDX = 10;
		public static StageDataManageComponent Instance {
			get {
				if(instance == null) {
					instance = new StageDataManageComponent();
				}
			return instance;
			}
		}
	}
}