using UnityEngine;
using System.Collections;

public class WholeLayerControl : MonoBehaviour {

	struct SearchNode{
		public ArrayList parent;
		public Transform self;
		public int flag;
		public ArrayList children;
	}

	public GameObject instance = null;
	public int layer = 0;
	private int lastTimeLayer = 0;
	private ArrayList meshRenderers;

	// Use this for initialization
	void Start () {
		lastTimeLayer = -1;
		meshRenderers = new ArrayList();
		searchRenderers(instance.transform);
	}
	
	// Update is called once per frame
	void Update () {
		if(lastTimeLayer != layer){

			Debug.Log("Layer has been sort");
			Debug.Log("value of lastTimeLayer :"+lastTimeLayer);
			Debug.Log("count of renders : "+ meshRenderers.Count);

			for(int i=meshRenderers.Count-1; i>=0; i--){
				MeshRenderer render = meshRenderers[i] as MeshRenderer;
				if(render != null){
					Debug.Log("give order");
					render.sortingOrder = layer;
				}
			}
			lastTimeLayer = layer;
		}
	}

	private void searchRenderers(Transform transform){

		SearchNode rootNode;
		rootNode.parent = null;
		rootNode.self = transform;
		rootNode.flag = 0;
		rootNode.children = new ArrayList();

		Transform[] tmp = transform.GetComponentsInChildren<Transform>(true);

		for(int i=tmp.Length-1; i>=0; i--){
			MeshRenderer renderer = tmp[i].GetComponent<MeshRenderer>();
			if(renderer != null){
				meshRenderers.Add(renderer);
			}
		}

		Debug.Log("layer " +layer+ " has " + meshRenderers.Count);

//		Debug.Log("layer " +layer+ " has " + tmp.Length);

//		root.Add();
//
//		while(root.Count!=0){
//
//
//
//		}
	}

}
