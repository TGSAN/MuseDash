using UnityEngine;
using System.Collections;

public class PlanetCell : MonoBehaviour {

	public ushort m_LayerPos=1;
	public int m_id;
	public float m_Angel=5;
	public PlanetData m_PlanetData; 



	//m_TweenScal
	public void InitPlanetCell(PlanetData _data)
	{

		if(_data==null)
		{
		//	Debug.Log("ScrollView is nULL");
			this.gameObject.SetActive(false);
		}
		else{
			m_PlanetData=_data;
			m_id=_data.m_id;
			m_LayerPos=_data.m_Layer;
			m_Angel=5;
			this.gameObject.SetActive(true);
		}
		
	}
	public void OnTriggerEnter(Collider other)
	{
		Debug.Log("jinru");

	}

	public void MovePlanet()
	{
		
	}
	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void Update () {
//		this.transform.Rotate(new Vector3(0,0,-m_Angel*Time.deltaTime));
	}
//	void On
}
