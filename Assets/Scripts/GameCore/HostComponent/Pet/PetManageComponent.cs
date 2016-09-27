///自定义模块，可定制模块具体行为
using System;
using System.Collections.Generic;
using FormulaBase;
namespace FormulaBase {
	public class PetManageComponent : CustomComponentBase {
		private static PetManageComponent instance = null;
		private const int HOST_IDX = 8;
		public static PetManageComponent Instance {
			get {
				if(instance == null) {
					instance = new PetManageComponent();
				}
			return instance;
			}
		}

		List<FormulaHost> m_ListEquipedPetHosts=new List<FormulaHost>();

		public List<FormulaHost>  GetListEquipedPetHosts
		{
			get
			{
				return m_ListEquipedPetHosts;
			}
			set
			{
				m_ListEquipedPetHosts=value;
			}
		}
		/// <summary>
		/// 装备升级
		/// </summary>
		/// <param name="_host">Host.</param>
		/// <param name="_UpNumber">Up number.</param>
		public void PetLevelUp(FormulaHost _host,int _UpNumber)
		{

			int tlevel=_host.GetDynamicIntByKey(SignKeys.LEVEL);
			int tMaxLevel=(int)_host.Result(FormulaKeys.FORMULA_130);
			tlevel+=_UpNumber;
			if(tlevel>tMaxLevel)
			{
				tlevel=tMaxLevel;
				NGUIDebug.Log("宠物等级到达上限");
			}
			_host.SetDynamicData(SignKeys.LEVEL,tlevel);
			CommonPanel.GetInstance().ShowWaittingPanel();
			_host.Save(new HttpResponseDelegate(PetLevelUpCallback));
		}

		public void GetExpAndCost(ref int Exp,ref int Cost)
		{
			Exp=0;
			Cost=0;
			FormulaHost tempEquip=new FormulaHost(HOST_IDX);
			List<FormulaHost> tList=ItemManageComponent.Instance.GetChosedItem;
			for(int i=0,max=tList.Count;i<max;i++)
			{
				Exp+=(int)tList[i].Result(FormulaKeys.FORMULA_40)*tList[i].GetDynamicIntByKey(SignKeys.CHOSED);
				Cost+=(int)tList[i].Result(FormulaKeys.FORMULA_41)*tList[i].GetDynamicIntByKey(SignKeys.CHOSED);
			}
		}
		/// <summary>
		/// 获取升级后的host
		/// </summary>
		public FormulaHost GetLevelUpHost(FormulaHost _host)
		{
			int Exp=0;int Cost=0;
			GetExpAndCost(ref Exp,ref Cost);
			FormulaHost thost=new FormulaHost(HOST_IDX);
			thost.SetDynamicData("ID",_host.GetDynamicIntByKey("ID"));
			//			thost.SetDynamicData(SignKeys.LEVEL,Level);
			//			thost.SetDynamicData(SignKeys.EXP,Exp);
			//thost
			Exp+=_host.GetDynamicIntByKey(SignKeys.EXP);
			//Exp+=1000;
			int LevelUpExp=(int)_host.Result(FormulaKeys.FORMULA_141);
			int Level=_host.GetDynamicIntByKey(SignKeys.LEVEL);
			while(LevelUpExp<=Exp)
			{
				Level++;
				Exp-=LevelUpExp;
				thost.SetDynamicData(SignKeys.LEVEL,Level);
				LevelUpExp=(int)thost.Result(FormulaKeys.FORMULA_141);
				Debugger.Log("最大等级为:"+thost.Result(FormulaKeys.FORMULA_157));
				if(Level==(int)thost.Result(FormulaKeys.FORMULA_157))
				{
					//thost.SetDynamicData(SignKeys.LEVEL,Level);
					thost.SetDynamicData(SignKeys.EXP,0);
					return thost;
				}
			}
			thost.SetDynamicData(SignKeys.LEVEL,Level);
			thost.SetDynamicData(SignKeys.EXP,Exp);
			return thost;
		}
		void PetLevelUpCallback(bool _success)
		{
			if(_success)
			{
				CommonPanel.GetInstance().ShowWaittingPanel(false);
				NGUIDebug.Log("接受宠物升级的回调");
			}
			else
			{
				CommonPanel.GetInstance().ShowText("Conect is fail");
			}
		}
		public FormulaHost GetEquipedPet(int _index)
		{
			for(int i=0,max=m_ListEquipedPetHosts.Count;i<max;i++)
			{
				if(_index+RoleManageComponent.Instance.GetFightGirlIndex()*10==m_ListEquipedPetHosts[i].GetDynamicIntByKey(SignKeys.EQUIPEDQUEUE))
				{
					return m_ListEquipedPetHosts[i];
				}
			}
			return null;
		}
		public bool CheckHaveSamePet(FormulaHost _host,int _index)
		{
			for(int i=0,max=m_ListEquipedPetHosts.Count;i<max;i++)
			{
				if(m_ListEquipedPetHosts[i].GetDynamicIntByKey(SignKeys.EQUIPEDQUEUE)/10==RoleManageComponent.Instance.GetFightGirlIndex())
				{
					if(_host.GetDynamicIntByKey("ID")==m_ListEquipedPetHosts[i].GetDynamicIntByKey("ID"))
					{
						if(_index==m_ListEquipedPetHosts[i].GetDynamicIntByKey(SignKeys.EQUIPEDQUEUE))
						{
							return false;
						}
						else 
						{
							return true;
						}
					}
				}
			}
			return false;
		}
		public void AddEquipedPet(FormulaHost _host)
		{
			m_ListEquipedPetHosts.Add(_host);
		}

		/// <summary>
		/// 判断宠物中有没相同的ID 或者不能堆叠
		/// </summary>
		/// <returns>The the same I.</returns>
		/// <param name="_ID">I.</param>
		public  FormulaHost HaveTheSameID(int _ID) {
			if (this.HostList == null) {
				return null;
			}

			int targetId = 0;
			List<FormulaHost> templist = new List<FormulaHost> (this.HostList.Values);
			for (int i = 0, max = templist.Count; i < max; i++) {
				targetId = (int)templist [i].GetDynamicDataByKey ("ID");
				if (_ID == targetId) {
					if (templist [i].GetDynamicIntByKey (SignKeys.STACK_NUMBER) != 1) {//可以堆叠
						int stackNumber = templist [i].GetDynamicIntByKey (SignKeys.STACKITEMNUMBER);
						stackNumber++;
						templist [i].SetDynamicData (SignKeys.STACKITEMNUMBER, stackNumber);
						return templist [i];
					}
				}
			}
			return  null;
		}

		public FormulaHost CreateItem(int idx) {
			FormulaHost host = FomulaHostManager.Instance.CreateHost (HOST_IDX);
			if (host != null) {
				host.SetDynamicData ("ID", idx);
				//ItemManageComponent.Instance.AddItem(host);
			}
			return host;
		}

		public void CreateItem(List<int> _listIndex)
		{

			List<FormulaHost> TempListItem=new List<FormulaHost>();
			for(int i=0;i<_listIndex.Count;i++)
			{
				FormulaHost temp=HaveTheSameID(_listIndex[i]);
				if(temp==null)//没有相同的ID
				{
					FormulaHost host = FomulaHostManager.Instance.CreateHost (HOST_IDX);
					if (host != null) {
						host.SetDynamicData("ID",_listIndex[i]);
						//	ItemManageComponent.Instance.AddItem(host);
					}
					TempListItem.Add(host);
				}
				else 
				{
					TempListItem.Add(temp);
				}

			}
			ItemManageComponent.Instance.AddItemList(TempListItem);
		}
		#region 宝箱开启用
		List<RewardData> m_AllPet=new List<RewardData>();
		public void Init() {
			this.GetList ("Pet");
			if (this.HostList != null) {
				foreach (FormulaHost host in this.HostList.Values) {
					host.Result (FormulaKeys.FORMULA_268);
				}
			}

			LitJson.JsonData cfg = ConfigPool.Instance.GetConfigByName ("pet");
			foreach (string id in cfg.Keys) {
				LitJson.JsonData _data = cfg [id];
				RewardData temp = new RewardData ();
				temp.id = int.Parse (id);
				temp.type = int.Parse (_data ["smallType"].ToString ());
				temp.Quality = int.Parse (_data ["Quality"].ToString ());
				m_AllPet.Add (temp);
			}
		}

		public List<RewardData> GetLimitItem(int _Quality=-1,int _Type=-1)
		{
			List<RewardData> temp=new List<RewardData>();
			for(int i=0,max = m_AllPet.Count ;i<max;i++)
			{
				if(_Quality==-1&&_Type==-1)
				{
					temp.Add(m_AllPet[i]);
					continue;
				}
				if(_Quality==-1)
				{
					if(_Type==m_AllPet[i].type)
					{
						temp.Add(m_AllPet[i]);
						continue;
					}
				}
				if(_Type==-1)
				{
					if(_Quality==m_AllPet[i].Quality)
					{
						temp.Add(m_AllPet[i]);
						continue;
					}
				}
				if(_Quality==m_AllPet[i].Quality&&_Type==m_AllPet[i].type)
				{
					temp.Add(m_AllPet[i]);
				}
			}
			return temp;
		}
		#endregion


		public void GetAllEquipedPet(ref int _hp,ref int _df,ref int _att,ref int _crit)
		{
			for(int i=0;i<m_ListEquipedPetHosts.Count;i++)
			{
				if(m_ListEquipedPetHosts[i].GetDynamicIntByKey(SignKeys.EQUIPEDQUEUE)/10==RoleManageComponent.Instance.GetFightGirlIndex())
				{
					_hp+=(int)m_ListEquipedPetHosts[i].Result(FormulaKeys.FORMULA_137);					//Hp
					_att+=(int)m_ListEquipedPetHosts[i].Result(FormulaKeys.FORMULA_139);					//Def
					_df+=(int)m_ListEquipedPetHosts[i].Result(FormulaKeys.FORMULA_141);	//Att
					_crit+=(int)m_ListEquipedPetHosts[i].Result(FormulaKeys.FORMULA_143);					//Crt
				}
			}
		}
	}
}