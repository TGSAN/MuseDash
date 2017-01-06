using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class InputManager : MonoBehaviour {

	//JugePoint的判定半径。
	public float maxDistance;

//	[HideInInspector]
	//Note列表，供Note对象加载。
	public List<GameObject> notes;

	//左中右三个判定点的坐标，打算以该坐标位置为中心来对称判定Note是否被玩家击中。
	public Transform judgePointL, judgePointR, judgePointM;

	//指定对应的按钮。
	public Button btnLeft, btnRight, btnMid;

	private GameObject JudgePoint;

	// 在游戏开始时进行初始化。
	void Start () {
		
		notes = new List<GameObject> ();

		btnLeft.onClick.AddListener (() => {
			JudgePoint = judgePointL.gameObject;
			TouchButtonEvent();
		});
		btnRight.onClick.AddListener (() => {
			JudgePoint = judgePointR.gameObject;
			TouchButtonEvent();
		});
		btnMid.onClick.AddListener (() => {
			JudgePoint = judgePointM.gameObject;
			TouchButtonEvent();
		});
	}

	//点击按钮后的事情。
	private void TouchButtonEvent()
	{
		//对所有存在的Note进行排序，以便我们选择那个离判定线最近的Note进行操作。这个貌似对同时出现在两条线上的Note支持不好，待测试。
		notes.Sort ((l, r) => {
			var d1 = Vector3.Distance(JudgePoint.transform.position, l.transform.position);
			var d2 = Vector3.Distance(JudgePoint.transform.position, r.transform.position);
			return Mathf.CeilToInt(d1 - d2);
		});

		//Note的数量至少要大于0。
		if (notes.Count > 0) {	
			//选择离判定点最近的那个Note进行操作。
			var noteToJudge = notes [0];
			//判定方式为计算判定点的坐标和目标Note坐标之间的距离。
			var distance = Vector3.Distance (JudgePoint.transform.position, noteToJudge.transform.position);
			//如果距离小于距离上限，则判定结果为命中。
			if (distance < maxDistance) {
				var parBurst = noteToJudge.GetComponentInChildren<ParticleSystem> ();
				parBurst.transform.SetParent (null);
				//					noteToJudge.SetActive (false);
				GameObject.Destroy (noteToJudge.gameObject);
				parBurst.Play ();
				notes.RemoveAt (0);
			}
		}
	}
	// Update is called once per frame
	void Update () {
		
        JudgePoint = null;

		//按下键盘键后对刚刚初始化的空GameObject进行赋值。
		if (Input.GetKeyDown (KeyCode.F)) {
			JudgePoint = judgePointL.gameObject;
		}
		if (Input.GetKeyDown (KeyCode.J)) {
			JudgePoint = judgePointR.gameObject;
		}
		if (Input.GetKeyDown (KeyCode.Space)) {
			JudgePoint = judgePointM.gameObject;
		}

		if (JudgePoint != null) {
			TouchButtonEvent();
		}
	}
}
