///自定义模块，可定制模块具体行为
using System;
namespace FormulaBase {
	public class AccountDailyTask : CustomComponentBase {
		private static AccountDailyTask instance = null;
		private const int HOST_IDX = 1;
		public static AccountDailyTask Instance {
			get {
				if(instance == null) {
					instance = new AccountDailyTask();
				}
			return instance;
			}
		}
	}
}