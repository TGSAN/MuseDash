using UnityEngine;
using System.Collections;

public class Jump : MonoBehaviour {

	//对象起跳的速度
	public float jumpSpeed = 20f;

	//声明一个Rigidbody2D变量
	private Rigidbody2D body2d;

	//声明一个InputState变量
	private InputState inputState;

	// Use this for initialization
	void Awake () {
		
		//获取对象的Rigidbody2D组件
		body2d = GetComponent<Rigidbody2D> ();
		//获取对象的InputState组件
		inputState = GetComponent<InputState> ();
	}
	
	// Update is called once per frame
	void Update () {
		//执行跳跃行为的判定逻辑
//		if (inputState.standing) {
			if (inputState.ActionButton) {
				body2d.velocity = new Vector2 (0, jumpSpeed);
//			}
		}
	}
}
