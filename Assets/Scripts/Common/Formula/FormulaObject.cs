using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using LitJson;

namespace FormulaBase {
	/// <summary>
	/// Formula object.
	/// </summary>
	public class FormulaObject {
		public FormulaStruct formula;
		private FormulaHost host = null;
		private FormulaParamObject[] fparam = null;
		public FormulaObject(int idx) {
			if (idx >= FormulaData.Instance.Formulas.Length) {
				return;
			}

			this.formula = FormulaData.Instance.Formulas [idx];
			this.InitParams ();
		}

		public void SetHost(FormulaHost host) {
			this.host = host;
		}

		public FormulaHost GetHost() {
			return this.host;
		}

		public FormulaObject GetFormulaFromHost(string name) {
			if (name == null) {
				return null;
			}


			if (this.host == null) {
				return null;
			}

			return this.host.GetFormulaObject (name);
		}

		/// <summary>
		/// Result this instance.
		/// Format : return (GetParamValue(0) + GetParamValue(1) * ...)
		/// </summary>
		public float Result () {
			return CustomFormula.Result (this.formula.idx, this);
		}

		public float GetParamValue(int pIdx) {
			FormulaParamObject fpo = this.GetParamObject (pIdx);
			if (fpo == null) {
				return 0f;
			}

			return fpo.GetData ();
		}

		public FormulaParamObject GetParamObject(int pIdx) {
			if (this.fparam == null || pIdx >= this.fparam.Length) {
				return null;
			}

			return this.fparam [pIdx];
		}

		public void UpDataDynamicValue() {
			if (this.fparam == null || this.fparam.Length <= 0) {
				return;
			}

			for (int i = 0; i < this.fparam.Length; i++) {
				FormulaParamObject fpo = this.fparam [i];
				if (fpo == null) {
					continue;
				}

				fpo.UpDataDynamicValue ();
			}
		}

		private void InitParams() {
			if (this.formula.fparams == null || this.formula.fparams.Length <= 0) {
				return;
			}

			this.fparam = new FormulaParamObject[this.formula.fparams.Length];
			for (int i = 0; i < this.formula.fparams.Length; i++) {
				this.fparam [i] = new FormulaParamObject (this, this.formula.fparams [i]);
			}
		}
	}
}