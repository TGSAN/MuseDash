///自定义模块，可定制模块具体行为
using System;
namespace FormulaBase {
	public class AccountTask : CustomComponentBase {
		private static AccountTask instance = null;
		private const int HOST_IDX = 1;
		public static AccountTask Instance {
			get {
				if(instance == null) {
					instance = new AccountTask();
				}
			return instance;
			}
		}
	}
}