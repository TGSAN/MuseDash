using UnityEngine;
using System.Collections;

public class welcomeBgController : MonoBehaviour {
	[SerializeField]
	public float Delay;

	public GameObject[] objs;
	// Use this for initialization
	void Start () {
		this.PlayBgAni ();
	}

	public void PlayBgAni() {
		print ("play animation");
		for (int i = 0; i < this.objs.Length; i++) {
			GameObject obj = this.objs [i];

			Animator ani = obj.GetComponent<Animator> ();
			//ani.SetTime((double)(this.Delay * i));
			//ani.Play ("welcome")ssss;

			obj.SetActive (false);

			StartCoroutine (this.PlayAni (this.Delay * i, obj));
		}
	}
	
	// Update is called once per frame
	//void Update () {
	//
	//}

	private IEnumerator PlayAni(float wait, GameObject obj) {
		yield return new WaitForSeconds (wait);

		obj.SetActive (true);
	}
}
