using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class ISD_Variable  {

	//Editor Use Only
	public bool IsOpen = true;
	public bool IsListOpen = true;


	public string Name;
	public ISD_PlistValueTypes Type = ISD_PlistValueTypes.String;
	public ISD_PlistValueTypes ArrayType = ISD_PlistValueTypes.String;


	public string StringValue = string.Empty;
	public int IntegerValue = 0;
	public float FloatValue = 0;
	public bool BooleanValue = true;

	
	public List<ISD_VariableListed> ArrayValue =  new List<ISD_VariableListed>();


	//Dictionary is not serializeable type :(
	public List<ISD_VariableListed> DictValues =  new List<ISD_VariableListed>();



	public void AddVarToDictionary(ISD_VariableListed v) {

		bool valid = true;
		foreach(ISD_VariableListed var in DictValues) {
			if(var.DictKey.Equals(v.DictKey)) {
				valid = false;
				break;
			}
		}

		if(valid) {
			DictValues.Add(v);
		}
	}


	public Dictionary<string, ISD_VariableListed> DictionaryValue {
		get {
			Dictionary<string, ISD_VariableListed> dict =  new Dictionary<string, ISD_VariableListed>();

			foreach(ISD_VariableListed var in DictValues) { 
				dict.Add(var.DictKey, var);
			}

			return dict;
		}
	}
}
