/// UI分析工具自动生成代码
/// PnlSuitcaseUI主模块
/// 
using System;
using UnityEngine;
using FormulaBase;
using System.Collections.Generic;

namespace PnlSuitcase {
	public class PnlSuitcase : UIPhaseBase {
		private static PnlSuitcase instance = null;
		public static PnlSuitcase Instance {
			get {
					return instance;
			}
		}

		private string showGridName;
		private List<GameObject> gridBaseList;

		public GameObject gridBase;
		public GameObject gridEquip;
		public GameObject gridMaterial;
		public GameObject gridPet;
		public UIGrid grid;
		public UIScrollView scroll;
		public int itemMax = 40;

		void Start() {
			instance = this;
			this.ReShow ();
		}

		public override void OnShow () {
			this.ReShow ();
		}

		public override void OnHide () {
		}

		public override void BeCatched () {
			this.InitGrids ();
		}

		public override void ReShow () {
			if (this.showGridName == this.gridMaterial.name) {
				this.ShowMaterials ();
				return;
			}

			if (this.showGridName == this.gridPet.name) {
				this.ShowPets ();
				return;
			}

			this.ShowEquips ();	
		}

		/// <summary>
		/// Inits the grids.
		/// 用itemMax初始化所有物品格子 预先占用掉所有位置形成正确排版
		/// </summary>
		private void InitGrids() {
			this.gridBaseList = new List<GameObject> ();
			for (int i = 0; i < itemMax; i++) {
				GameObject _girdBase = GameObject.Instantiate (this.gridBase) as GameObject;
				GameObject _gridEquip = GameObject.Instantiate (this.gridEquip) as GameObject;
				GameObject _gridMaterial = GameObject.Instantiate (this.gridMaterial) as GameObject;
				GameObject _gridPet = GameObject.Instantiate (this.gridPet) as GameObject;

				_gridEquip.name = this.gridEquip.name;
				_gridMaterial.name = this.gridMaterial.name;
				_gridPet.name = this.gridPet.name;
				_girdBase.name = this.gridBase.name;

				_gridEquip.transform.parent = _girdBase.transform;
				_gridMaterial.transform.parent = _girdBase.transform;
				_gridPet.transform.parent = _girdBase.transform;

				this.grid.AddChild (_girdBase.transform);

				_girdBase.transform.localScale = Vector3.one;
				this.gridBaseList.Add (_girdBase);
				//_girdBase.SetActive (false);
			}
		}

		public void ShowEquips() {
			this.showGridName = this.gridEquip.name;
			this.FillItems (this.showGridName, EquipManageComponent.Instance.HostList);
		}

		public void ShowMaterials() {
			this.showGridName = this.gridMaterial.name;
			this.FillItems (this.showGridName, materialManageComponent.Instance.HostList);
		}

		public void ShowPets() {
			this.showGridName = this.gridPet.name;
			this.FillItems (this.showGridName, PetManageComponent.Instance.HostList);
		}

		/// <summary>
		/// Shows the info.
		/// 
		/// 展示物品信息面板
		/// </summary>
		public void ShowInfo(string uiName) {
			if (!UISceneHelper.Instance.IsUiActive (uiName)) {
				UISceneHelper.Instance.ShowUi (uiName);
			}
		}

		/// <summary>
		/// Fills the items.
		/// 根据数据源 物品类型展示物品面板
		/// </summary>
		/// <param name="gridName">Grid name.</param>
		/// <param name="hostList">Host list.</param>
		private void FillItems(string gridName, Dictionary<string, FormulaHost> hostList) {
			if (this.gridBaseList == null) {
				return;
			}

			this.HideAll ();
			if (hostList == null || hostList.Count <= 0) {
				return;
			}

			int i = 0;
			foreach (FormulaHost host in hostList.Values) {
				if (host == null) {
					continue;
				}

				GameObject _girdBase = this.gridBaseList [i];
				if (_girdBase == null) {
					continue;
				}

				i++;
				this.ShowGridByName (_girdBase, gridName, host);
			}
		}

		/// <summary>
		/// Shows the name of the grid by.
		/// 根据数据源 物品类型展示物品格子单元
		/// </summary>
		/// <param name="grid">Grid.</param>
		/// <param name="name">Name.</param>
		/// <param name="host">Host.</param>
		private void ShowGridByName(GameObject grid, string name, FormulaHost host) {
			if (grid == null || grid.transform.childCount <= 0) {
				return;
			}

			grid.SetActive (true);
			TglSuitcaseGroove.TglSuitcaseGroove tgl = grid.GetComponent<TglSuitcaseGroove.TglSuitcaseGroove> ();
			for (int i = 0; i < grid.transform.childCount; i++) {
				Transform t = grid.transform.GetChild (i);
				if (t == null) {
					continue;
				}

				UIRootHelper urh = t.gameObject.GetComponent<UIRootHelper> ();
				if (urh == null) {
					continue;
				}

				bool isActive = (t.gameObject.name == name);
				t.gameObject.SetActive (isActive);
				if (isActive) {
					host.SetAsUINotifyInstance (urh);
					UIPhaseBase upb = t.gameObject.GetComponent<UIPhaseBase> ();
					if (upb != null) {
						upb.OnShow (host);
					}

					if (tgl != null) {
						tgl.SetItemHost (host);
					}
				}
			}
		}

		private void HideAll() {
			if (this.gridBaseList == null) {
				return;
			}

			for (int i = 0; i < itemMax; i++) {
				GameObject _girdBase = this.gridBaseList [i];
				if (_girdBase == null) {
					continue;
				}

				_girdBase.SetActive (false);
			}
		}
	}
}