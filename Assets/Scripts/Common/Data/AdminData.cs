using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AdminData : MonoBehaviour {
	private const string PATH = "Prefabs/AdminData";
	private static GameObject dataObject;
	private static AdminData instance = null;

	[SerializeField]
	public int DefaultStage;
	public int DefaultRoleIdx;
	public int DefaultClothUid;
	public List<int> DefaultSkills;
	public int DefaultPetSkill;

	public static AdminData Instance {
		get { 
			if (instance == null) {
				dataObject = Resources.Load (PATH) as GameObject;
				instance = dataObject.GetComponent<AdminData> ();
			}
			
			return instance;
		}
	}

	public static GameObject GetDataObject() {
		return dataObject;
	}

	public void AfterSave() {
		instance = null;
	}
}
