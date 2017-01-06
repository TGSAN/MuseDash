using UnityEngine;
using System.Collections;

/// <summary>
/// 当对象存在的时间超过设定时间后对其进行摧毁。
/// </summary>
[AddComponentMenu("HaaaqiTool/Time To Destroy")]
public class TimeToDestroy : MonoBehaviour
{
	//延时多久后摧毁对象。
	public float DestroyDelay = 10.0f;

	void Update()
	{
		DestroyDelay -= Time.deltaTime;
		if (DestroyDelay <= 0.0f)
		{
			GameObject.Destroy(gameObject);
		}
	}
}
