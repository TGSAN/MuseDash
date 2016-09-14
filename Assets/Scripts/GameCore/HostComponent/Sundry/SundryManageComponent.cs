///自定义模块，可定制模块具体行为
using System;
namespace FormulaBase {


	public enum SundyEnum
	{
		/// <summary>
		/// 开绿装概率
		/// </summary>
		SUNDRY_1=1,
		/// <summary>
		/// 开蓝装概率
		/// </summary>
		SUNDRY_2=2,
		/// <summary>
		/// 开紫装概率
		/// </summary>
		SUNDRY_3=3,
		/// <summary>
		/// 第一个宝箱开启消耗的钻石
		/// </summary>
		SUNDRY_5=5,//1
		SUNDRY_6,//第2个宝箱开启需要消耗的钻石
		SUNDRY_7,//3
		SUNDRY_8,//4
		SUNDRY_9,//5
		SUNDRY_10,//6
		SUNDRY_11=11,//每钻石 减少的分数
	}
	public class SundryManageComponent : CustomComponentBase {
		private static SundryManageComponent instance = null;
		private const int HOST_IDX = 13;
		public static SundryManageComponent Instance {
			get {
				if(instance == null) {
					instance = new SundryManageComponent();
				}
			return instance;
			}
		}
		public float GetVaule(int _index)
		{
			FormulaHost host = FomulaHostManager.Instance.CreateHost (HOST_IDX);
			host.SetDynamicData("ID",_index);
			return host.Result(FormulaKeys.FORMULA_122);
			
		}


	}
}