using UnityEngine;
using System.Collections;
using GameLogic;
using FormulaBase;
using LitJson;

public class WelComeScript : LoginScript {
	private static WelComeScript instance = null;
	public static WelComeScript Instance {
		get {
			return instance;
		}
	}

	[SerializeField]
	public GameObject girls;
	public GameObject girlsbg;

	void Start () {
		this.Init ();
	}

	private void Init() {
		if (instance == this) {
			return;
		}

		instance = this;

		SpineActionController sac = this.girls.GetComponent<SpineActionController> ();
		sac.Init (0);

		this.girls.SetActive (false);
		this.girlsbg.SetActive (false);
	}

	public void OnFinishAp() {
		this.Init ();

		this.girls.SetActive (true);
		this.girlsbg.SetActive (true);
		welcomeBgController c = this.girlsbg.GetComponent<welcomeBgController> ();
		//c.PlayBgAni ();
		
		SpineActionController.Play (ACTION_KEYS.COMEIN, this.girls);

		this.LoadSaveData ();
	}

	public void OnSceneClick() {
		SpineActionController.Play (ACTION_KEYS.COMEOUT, this.girls);
	}

	public override void LoginSucceed () {
		if (this.waitCoroutine != null) {
			this.StopCoroutine (this.waitCoroutine);
		}

		this.OnSceneClick ();
		CommonPanel.GetInstance ().ShowText (LOAD_DATA_OK);
		CommonPanel.GetInstance ().SetMask (true, this.OnLoginSucceed);
	}

	private void OnLoginSucceed() {
		// switch scene
		Application.LoadLevel (1);
		UIManageSystem.g_Instance.RegisterPanel();			//UI资源的注册
	}
}