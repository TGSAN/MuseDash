///自定义模块，可定制模块具体行为
using System;
namespace FormulaBase {
	public class ItemComponent : CustomComponentBase {
		private static ItemComponent instance = null;
		private const int HOST_IDX = 7;
		public static ItemComponent Instance {
			get {
				if(instance == null) {
					instance = new ItemComponent();
				}
			return instance;
			}
		}

		private FormulaHost itemHost = null;
		public FormulaHost GetItemHost() {
			if (this.itemHost == null) {
				this.itemHost = FomulaHostManager.Instance.CreateHost (HostKeys.HOST_7);
			}
			
			return this.itemHost;
		}
	}
}