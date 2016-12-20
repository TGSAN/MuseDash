////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PTPGameController : MonoBehaviour {

	public GameObject pref;

	private DisconnectButton d;
	private ConnectionButton b;
	private ClickManager m;

	public static PTPGameController instance;


	private List<GameObject> spheres =  new List<GameObject>();

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	void Awake() {

		instance = this;


		GameCenterManager.OnAuthFinished += OnAuthFinished;
		GameCenterManager.Init ();



		b = gameObject.AddComponent<ConnectionButton> ();
		b.enabled = false;

		d = gameObject.AddComponent<DisconnectButton> ();
		d.enabled = false;

		m = gameObject.GetComponent<ClickManager> ();
		m.enabled = false;


		GameCenter_RTM.ActionPlayerStateChanged += HandleActionPlayerStateChanged;;
		GameCenter_RTM.ActionMatchStarted += HandleActionMatchStarted;

	}









	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------

	public void createRedSphere(Vector3 pos) {
		GameObject s = Instantiate(pref) as GameObject;
		s.transform.position = pos;

		s.GetComponent<Renderer>().enabled = true;
		s.GetComponent<Renderer>().material = new Material (s.GetComponent<Renderer>().material);
		s.GetComponent<Renderer>().material.color = Color.red;

		spheres.Add (s);

	}

	public void createGreenSphere(Vector3 pos) {
		GameObject s = Instantiate(pref) as GameObject;
		s.transform.position = pos;

		s.GetComponent<Renderer>().enabled = true;
		s.GetComponent<Renderer>().material = new Material (s.GetComponent<Renderer>().material);
		s.GetComponent<Renderer>().material.color = Color.green;

		spheres.Add (s);
	}
	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------
	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------

	void OnAuthFinished (ISN_Result res) {
		if (res.IsSucceeded) {
			IOSNativePopUpManager.showMessage("Player Authed ", "ID: " + GameCenterManager.Player.Id + "\n" + "Name: " + GameCenterManager.Player.DisplayName);
			cleanUpScene ();
		}

	}

	

	void HandleActionPlayerStateChanged (GK_Player player, GK_PlayerConnectionState state, GK_RTM_Match macth) {
		if(state == GK_PlayerConnectionState.Disconnected) {
			IOSNativePopUpManager.showMessage ("Disconnect", "Game finished");
			GameCenter_RTM.Instance.Disconnect();
			cleanUpScene ();
		} else {
			CheckMatchState(macth);
		}
	}
	

	void HandleActionMatchStarted (GK_RTM_MatchStartedResult result) {
		if(result.IsSucceeded) {
			CheckMatchState(result.Match);

		} else {
			IOSNativePopUpManager.showMessage ("Match Start Error", result.Error.Description);
		}
	}

	private void CheckMatchState(GK_RTM_Match macth) {
		IOSNativePopUpManager.dismissCurrentAlert();
		if(macth != null) {
			if(macth.ExpectedPlayerCount == 0) {
				IOSNativePopUpManager.showMessage ("Match Started", "let's play now\n   Macth.ExpectedPlayerCount): " + macth.ExpectedPlayerCount);
				
				
				
				m.enabled = true;
				b.enabled = false;
				d.enabled = true;
				
				
				Debug.Log("Sending HelloPackage ");
				HelloPackage p =  new HelloPackage();
				p.send();
			} else {
				IOSNativePopUpManager.showMessage ("Match Created", "Macth.ExpectedPlayerCount): " + macth.ExpectedPlayerCount);
			}
		}
	}


	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------

	private void cleanUpScene() {
		b.enabled = true;
		m.enabled = false;
		d.enabled = false;

		foreach(GameObject sp in spheres) {
			Destroy (sp);
		}

		spheres.Clear ();
	}
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------

}
