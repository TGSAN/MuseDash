using UnityEngine;
using System.Collections;

public class SortingLayerController : MonoBehaviour {
	
	[SerializeField]
	public GameObject target;
	public GameObject parent;
	public int orderId = 0;
	public bool isParent;
	private MeshRenderer render;

	// Use this for initialization
	void Start () {
		if(!isParent){
			render = target.transform.GetComponent<MeshRenderer>();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(!isParent){
			int baseId = parent.transform.GetComponent<SortingLayerController>().orderId;
			if(render.sortingOrder != orderId+baseId){
				render.sortingOrder = orderId+baseId;
				Debug.Log("sortingLayer Order:" + (orderId+baseId));
			}
			Debug.Log("sortingLayer Order:" + render.sortingOrder);
		}
	}
}
