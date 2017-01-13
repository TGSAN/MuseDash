using UnityEngine;
using System.Collections;

public class FeverEffectManager : MonoBehaviour {
	/*
	 * backgrounds are on the bg layer or default layer by sorting less than -1,
	 * chars and nodes on defult layer by sorting 0,
	 * fever is on the default layer by sorting -1, 
	 * clossShot is on the default layer by sorting bigger than 2.
	 */

	private GameObject backGround;
	private SpriteRenderer whitBoardRender;
	private Vector3 centerPosition = new Vector3(2.34f,0.96f,0f);
	private Vector3 outScenePosition= new Vector3(14.08f,0.96f,0f);
	private GameObject[] particles = new GameObject[7];
	private const float COME_IN_DURING_TIME = 0.5f;
	private const float COME_OUT_DURING_TIME = 0.5f;
	private const float DISTANCE = 10.7f;
	private const int CLOSS_SHOT_LAYER_ID = 6;

//	public bool isActivatingFeverEffect;
	private bool isActivatedComeOut;
	private bool ifShow;

	private static FeverEffectManager instance = null;
	public static FeverEffectManager Instance {
		get {
			return instance;
		}
	}

	// Use this for initialization
	void Start () {
		InitFeverEffect();
		instance = this;
		this.gameObject.SetActive(false);
	}

	// Update is called once per frame

	private float duringTime = 0;
	private const float ACCELERATION = 9.8f;

	void Update () {
		if (isActivatedComeOut) {
			if (ifShow) {
				float fadeValue = 1 * (Time.deltaTime / (COME_OUT_DURING_TIME / 2));
				if (whitBoardRender.color.a + fadeValue <= 1) {
					whitBoardRender.color = new Color (1, 1, 1, whitBoardRender.color.a + fadeValue);
				} else {
					whitBoardRender.color = new Color (1, 1, 1, 1);
					backGround.transform.position = outScenePosition;
					backGround.SetActive (false);
					foreach (var particle in particles) {
						if (particle != null) {
							particle.SetActive (false);
						}
					}
					ifShow = false;
				}
			} else {
				float fadeValue = 1 * (Time.deltaTime / (COME_OUT_DURING_TIME / 2));
				if (whitBoardRender.color.a - fadeValue >= 0) {
					whitBoardRender.color = new Color (1, 1, 1, whitBoardRender.color.a - fadeValue);
				} else {
					whitBoardRender.color = new Color (1, 1, 1, 0);
					isActivatedComeOut = false;
					var animator = backGround.GetComponent<Animator> ();
					animator.Rebind ();
					animator.Play ("waiting_outside");
					ifShow = true;
					this.gameObject.SetActive (false);
					var clossShot = GameObject.Find ("ClossShot_layer");
					if (clossShot != null) {
						var renders = clossShot.GetComponentsInChildren<SpriteRenderer> ();
						foreach (var render in renders) {
							render.sortingOrder = 6;
						}
					}
				}
			}
		}
	}

	public void InitFeverEffect() {
//		isActivatingFeverEffect = false;
		isActivatedComeOut = false;
		ifShow = true;
		backGround = GameObject.Find ("bg_S");
		backGround.transform.position = outScenePosition;
		whitBoardRender = GameObject.Find ("whitBoard").GetComponent<SpriteRenderer> ();
		whitBoardRender.color = new Color (1, 1, 1, 0);
		particles [0] = GameObject.Find ("fever_fx_1_start");
		particles [1] = GameObject.Find ("fever_fx_1");
		particles [2] = GameObject.Find ("fever_streamer");
		particles [3] = GameObject.Find ("fever_fx_star_big2");
		particles [4] = GameObject.Find ("fever_fx_star_big1");
		particles [5] = GameObject.Find ("fever_fx_star_small1");
		particles [6] = GameObject.Find ("fever_fx_star_small2");
		foreach (var particle in particles) {
			if (particle != null) {
				particle.SetActive (false);
			}
		}
	}

	public void ActivateFever() {
		if (this.gameObject.activeSelf) {
			return;
		}

		this.gameObject.SetActive (true);
		backGround.SetActive (true);
//		isActivatingFeverEffect = true;
		var animator = backGround.GetComponent<Animator> ();
		animator.Rebind ();
		animator.Play ("come_in");
		isActivatedComeOut = false;
		ifShow = true;
		var clossShot = GameObject.Find ("ClossShot_layer");
		if (clossShot != null) {
			var renders = clossShot.GetComponentsInChildren<SpriteRenderer> ();
			Debug.Log ("renders number is " + renders.Length);
			foreach (var render in renders) {
				render.sortingOrder = -8;
			}
		}

		foreach (var particle in particles) {
			if (particle != null) {
				particle.SetActive (true);
			}
		}
	}

	public void CancelFeverEffect() {
		//TODO: white blank
		isActivatedComeOut = true;
	}
}
