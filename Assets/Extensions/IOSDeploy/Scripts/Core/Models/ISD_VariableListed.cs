using UnityEngine;
using System.Collections;


[System.Serializable]
public class ISD_VariableListed {


	public bool IsOpen = true;

	public ISD_PlistValueTypes Type = ISD_PlistValueTypes.String;


	public string DictKey = string.Empty;
	
	
	public string StringValue = string.Empty;
	public int IntegerValue = 0;
	public float FloatValue = 0;
	public bool BooleanValue = true;
}
