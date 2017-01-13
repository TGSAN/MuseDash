using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScrollScale : MonoBehaviour {

	public float maxScale, minScale;
	public UIGrid grid;
	public float midX;
	public float changeDistance;
	private Transform[] m_Childs;

	private void Awake()
	{
		m_Childs = new Transform[grid.transform.childCount];
		for (int i = 0; i < grid.transform.childCount; i++) {
			m_Childs [i] = grid.transform.GetChild (i);
		}
	}
	private void Update()
	{
		foreach (var item in m_Childs) {
			item.transform.localScale = Mathf.Lerp (maxScale, minScale, Mathf.Abs (item.transform.position.x - midX) / changeDistance) * Vector3.one;
		}
	}

}
