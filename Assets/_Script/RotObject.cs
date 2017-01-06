//用鼠标点击控制一个对象旋转和停止旋转。

using UnityEngine;
using System.Collections;

public class RotObject : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

	//声明并初始化对象单位时间内的旋转速度。
	public float RotSpeed = 20.0f;

	//声明并初始化对象的旋转状态。
	public bool RotEnable = true;

	//设置物体在不同的设备上因硬件性能产生不同的帧速率也可以保持相近的旋转速度。
	// Update is called once per frame
	void Update () {
		if (RotEnable) {
			transform.Rotate (0, RotSpeed * Time.deltaTime, 0);
		}
	}

	//按下鼠标后目标物体将开始/停止旋转。
	void OnMouseDown()
	{
		RotEnable = !RotEnable;
	}
}
