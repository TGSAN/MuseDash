using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public struct SkeletonMountData {
	public int actionId;
	public GameObject instance;
	public int moduleId;
}

public class SpineMountController : MonoBehaviour {
	private string[] sklNames;
	private GameObject[] dymanicObjects;

	[SerializeField]
	private SkeletonMountData[] mountData;

	static bool FindBones(Spine.Bone elem, string _name) {
		if (elem.Data.Name == _name)
			return true;

		return false;
	}

	// Use this for initialization
	void Start () {
		this.InitSkeletonName ();
		this.dymanicObjects = new GameObject[this.mountData.Length];
		for (int i = 0, max = this.mountData.Length; i < max; i++) {
			SkeletonMountData _data = this.mountData [i];
			// Instance from perfabs in config and as dynamic gameobject.
			GameObject obj = (GameObject)Instantiate (_data.instance);
			// obj.SetActive (true);
			BoneFollower temp = obj.GetComponent<BoneFollower> ();
			if (temp == null) {
				temp = obj.AddComponent<BoneFollower> ();
			}

			temp.SkeletonRenderer = this.gameObject.GetComponent<SkeletonAnimation> () as SkeletonRenderer;
			temp.boneName = this.sklNames [_data.actionId];

			SpineActionController.InitTypePoll ();
			Type partType = SpineActionController.TYPE_POLL [_data.moduleId];
			obj.AddComponent (partType);

			this.dymanicObjects [i] = obj;
		}
	}

	public void AddData() {
		SkeletonMountData d = new SkeletonMountData ();
		List<SkeletonMountData> _list;
		if (this.mountData != null && this.mountData.Length > 0) {
			_list = this.mountData.ToList ();
		} else {
			_list = new List<SkeletonMountData> ();
		}
		
		_list.Add (d);
		
		this.mountData = _list.ToArray ();
	}

	public void DelData(int idx) {
		if (this.mountData == null) {
			return;
		}
		
		if (this.mountData.Length <= idx) {
			return;
		}
		
		List<SkeletonMountData> _list = this.mountData.ToList ();
		_list.RemoveAt (idx);
		this.mountData = _list.ToArray ();
	}

	public void SetData(int idx, SkeletonMountData d) {
		if (this.mountData == null) {
			return;
		}
		
		if (this.mountData.Length <= idx) {
			return;
		}
		this.mountData [idx] = d;
	}

	public int DataCount() {
		if (this.mountData == null) {
			return 0;
		}
		
		return this.mountData.Length;
	}

	public SkeletonMountData GetData(int idx) {
		return this.mountData [idx];
	}

	public void DestoryDynamicObjects() {
		if (this.dymanicObjects == null || this.dymanicObjects.Length == 0) {
			return;
		}

		for (int i = 0; i < this.dymanicObjects.Length; i++) {
			GameObject obj = this.dymanicObjects [i];
			if (obj == null) {
				continue;
			}

			GameObject.Destroy (obj);
		}
	}

	public GameObject[] GetMountObjects(){
		return this.dymanicObjects;
	}

	public GameObject GetMountObjectByIdx(int index) {
		if (this.dymanicObjects == null) {
			return null;
		}

		if (index >= this.dymanicObjects.Length) {
			return null;
		}

		return this.dymanicObjects[index];
	}

	public void OnControllerStart () {
		if (this.dymanicObjects == null || this.dymanicObjects.Length == 0) {
			return;
		}

		for (int i = 0; i < this.dymanicObjects.Length; i++) {
			GameObject obj = this.dymanicObjects [i];
			if (obj == null) {
				continue;
			}

			obj.SetActive(true);
			BaseSpineObjectController mb = obj.GetComponent<BaseSpineObjectController>();
			if (mb != null) {
				mb.OnControllerStart();
			}
		}
	}

	private void InitSkeletonName() {
		Spine.ExposedList<Spine.Bone> temp = this.gameObject.GetComponent<SkeletonAnimation> ().skeleton.Bones;
		this.sklNames = new string[temp.Count];
		for (int i = 0, max = temp.Count; i < max; i++) {
			this.sklNames [i] = temp.Items [i].Data.Name;
		}
	}
}