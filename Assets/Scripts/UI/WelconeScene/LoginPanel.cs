using UnityEngine;
using System.Collections;

public class LoginPanel : MonoBehaviour {
	/// <summary>
	/// The ADMI n_ COD.
	/// 
	/// 测试专用通过码
	/// </summary>
	private const string ADMIN_CODE = "233233";
	private static LoginPanel instance = null;
	public static LoginPanel Instance {
		get {
			return instance;
		}
	}

	public GameObject m_Bg;
	public GameObject LoginBox;
	public GameObject m_Title;
	public GameObject m_Cover;
	public GameObject m_StudioLogo;
	public GameObject m_PreEnter;
	public GameObject m_Followus;
	public UIInput m_InPut;
	public UILabel m_ContinueLable;
	private string localVerificateCode = null;
	void Awake()
	{
		this.m_Title.SetActive (false);
		//UIEventListener.Get (m_Bg).onClick = ClickScreen;
	}

	void ClickScreen(GameObject _ob) {
		UIEventListener.Get (m_Bg).onClick = null;

		this.TryLogin ();
		/*
		Debug.Log ("Click Screen");
		UIEventListener.Get (m_Bg).onClick = null;

		m_Cover.SetActive (true);
		TweenAlpha ta = m_Cover.GetComponent<TweenAlpha> ();
		ta.enabled = true;
		ta.ResetToBeginning ();

		m_ContinueLable.gameObject.SetActive (false);

		WelComeScript.Instance.OnSceneClick ();
		*/
	}

	public void TryLogin() {
		//this.m_Followus.SetActive (false);
		LoginBox.SetActive (true);
		this.localVerificateCode = WelComeScript.Instance.GetVerficationCode ();
		if (this.localVerificateCode != null && this.localVerificateCode.Length == 6) {
			m_InPut.defaultText = this.localVerificateCode;
			m_InPut.value = this.localVerificateCode;

			LoginBox.SetActive (false);
			WelComeScript.Instance.Login ();
		}

		/*
		// Try auto login.
		if (vcode == null || vcode.Length < 6) {
			LoginBox.SetActive (true);
		} else {
			WelComeScript.Instance.Login ();
			LoginBox.SetActive (false);
		}
		*/
	}

	public void ClickSignInButton()
	{
		LoginBox.SetActive (false);
		string vcode = SubmitKey ();
		if (vcode != ADMIN_CODE) {
			if (vcode == null || vcode.Length != 6) {
				LoginBox.SetActive (true);
				return;
			}

			if (vcode == this.localVerificateCode) {
				WelComeScript.Instance.Login ();
			} else {
				VerificationScript.Instance.GetVerificationCode (vcode);
			}
		} else {
			WelComeScript.Instance.Login ();
			CommonPanel.GetInstance ().ShowText ("已使用管理员验证码");
		}
	}

	public string SubmitKey()
	{
		string temp = m_InPut.text;
		if (temp.Length != 6) {
			CommonPanel.GetInstance ().ShowText ("请输入正确的6位验证码，没错，是6位");
			m_InPut.text = "";
			return "";
		} else {
			return temp;
		}
	}

	public void FinishAni()
	{
		WelComeScript.Instance.Login();
	}

	public void ClickCancelButton()
	{
		LoginBox.SetActive (false);
		m_ContinueLable.gameObject.SetActive (true);
		m_Cover.SetActive (false);
		this.FinishAp ();
		//Application.LoadLevel (Application.loadedLevel);
	}

	// Use this for initialization
	void Start () {
		instance = this;
	}
	
	// Update is called once per frame
	//void Update () {
	//
	//}

	public void FinshStudioLogo() {
		this.m_StudioLogo.SetActive (false);
		this.m_PreEnter.SetActive (true);
	}

	public void FinishAp()
	{
		Handheld.PlayFullScreenMovie ("op.mp4", Color.black, FullScreenMovieControlMode.CancelOnInput);
		this.StartCoroutine (this.ContFinishAp ());
	}

	private IEnumerator ContFinishAp() {
		yield return new WaitForSeconds (0.01f);

		this.m_Followus.SetActive (true);
		this.m_Title.SetActive (true);
		this.m_ContinueLable.gameObject.SetActive (true);
		UIEventListener.Get (m_Bg).onClick = ClickScreen;
		m_Bg.SetActive (true);

		WelComeScript.Instance.OnFinishAp ();
	}

	public void OnClickWeibo() {
	}

	public void OnClickWechat() {
	}

	public void OnClickQQ() {
	}
}
