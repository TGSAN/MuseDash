///自定义模块，可定制模块具体行为
using System;
using System.Collections.Generic;
namespace FormulaBase {
	public class TrophyManageComponent : CustomComponentBase {
		private static TrophyManageComponent instance = null;
		private const int HOST_IDX = 11;
		public static TrophyManageComponent Instance {
			get {
				if(instance == null) {
					instance = new TrophyManageComponent();
				}
			return instance;
			}
		}

		private FormulaHost itemHost = null;
		public FormulaHost GetItemHost() {
			if (this.itemHost == null) {
				this.itemHost = FomulaHostManager.Instance.CreateHost (HostKeys.HOST_11);
			}
			return this.itemHost;
		}
		List<RewardData> m_AllPet=new List<RewardData>();

		int  Player_Trophy=6;

		//获取普通食物的盖里
		public float GetNormalFoodProbability()
		{
			GetItemHost().SetDynamicData("ID",6);
			return 0;
		}
		//获取宠物食物
		public float GetPetFoodProbability()
		{
			GetItemHost().SetDynamicData("ID",6);
			return 0;
		}
		//获取角色升星
		public float GetRoleUpStarsProbability()
		{
			GetItemHost().SetDynamicData("ID",6);
			return 0;
		}
		//获取装备升星
		public float GetEquipUpStarsProbability()
		{
			GetItemHost().SetDynamicData("ID",6);
			return 0;
		}
		//获取武器
		public float GetWeaponProbablity()
		{
			GetItemHost().SetDynamicData("ID",6);
			return 0;
		}
		//获取饰品
		public float GetAccessoriesProbality()
		{
			GetItemHost().SetDynamicData("ID",6);
			return 0;
		}
		//获取宠物碎片
		public float GetPetPatchProbality()
		{
			GetItemHost().SetDynamicData("ID",6);
			return 0;
		}
//		public void Init()
//		{
//
//			LitJson.JsonData cfg =ConfigPool.Instance.GetConfigByName("pet");
//			//			int len = cfg.Count;
//
//			foreach(string id in cfg.Keys) {
//
//				LitJson.JsonData _data = cfg[id];
//				RewardData temp=new RewardData();
//				temp.id=int.Parse(id);
//				temp.type=int.Parse(_data["Type"].ToString());
//				temp.Quality=int.Parse(_data["Quality"].ToString());
//				m_AllPet.Add(temp);
//			}
//
//		}
	}
}