using UnityEngine;
using System.Collections;

public class InputState : MonoBehaviour {

	//标记设备是否被点击
	public bool ActionButton;

	//标记对象是否站在地面
	public bool standing;

	//游戏对象站立边界阈值
	public float standingThreshold = 1f;

	//Y轴方向的速度绝对值
	public float absVelY = 0f;

	//声明一个刚体组件
	private Rigidbody2D body2d;

	// Use this for initialization
	void Awake () {	
		
		//获取对象Rigidbody2D组件
		body2d = GetComponent<Rigidbody2D> ();

	}

	// Update is called once per frame
	void Update () {

		//检测屏幕的任意输入信息
		ActionButton = Input.anyKeyDown;

	}

	void FixedUpdate () {

		//获取刚体Y轴的当前速度
		absVelY = System.Math.Abs (body2d.velocity.y);

		//Y轴速度小于边界值表明刚体不在地面
		standing = absVelY < standingThreshold;
	}
}