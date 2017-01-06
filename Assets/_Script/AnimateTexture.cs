using UnityEngine;
using System.Collections;

public class AnimateTexture : MonoBehaviour {

	//声明一个二维向量Speed，并初始化为0，0；
	public Vector2 Speed = Vector2.zero;

	//声明一个二维向量Offset，并初始化为0，0；
	private Vector2 Offset = Vector2.zero;

	//声明一个材质Material；
	public Material _Material;


	// Use this for initialization
	void Start () {
		
		//初始化mterial，从组件Renderer获取material；
		_Material = GetComponent<Renderer> ().material;

	}


	// Update is called once per frame
	void Update () {
		
		Offset += Speed * Time.deltaTime;
		_Material.SetTextureOffset ("_MainTex", Offset);

	}
}
