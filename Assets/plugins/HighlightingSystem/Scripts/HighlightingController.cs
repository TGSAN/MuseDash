using UnityEngine;
using System.Collections;

public class HighlightingController : MonoBehaviour {
	// no use high light
	public bool openHightLight = false;

	public Color objColor;
	protected HighlightableObject ho;
	
	void Awake()
	{
		if (!this.openHightLight) {
			return;
		}

		ho = gameObject.AddComponent<HighlightableObject>();
		// ho.ConstantOnImmediate (this.objColor);
		this.objColor.a = 1f;
		ho.ConstantParams (this.objColor);
		ho.ConstantSwitchImmediate();
	}

	public void ReinitMaterials() {
		if (!this.openHightLight) {
			this.enabled = false;
			return;
		}

		if (this.ho == null) {
			return;
		}

		ho.ReinitMaterials ();
	}
	/*
	void Update()
	{
		// ho.ReinitMaterials ();
		// Fade in/out constant highlighting with 'Tab' button
		if (Input.GetKeyDown(KeyCode.Tab)) 
		{
			ho.ConstantSwitch();
		}
		// Turn on/off constant highlighting with 'Q' button
		else if (Input.GetKeyDown(KeyCode.Q))
		{
			ho.ConstantSwitchImmediate();
		}
		
		// Turn off all highlighting modes with 'Z' button
		if (Input.GetKeyDown(KeyCode.Z)) 
		{
			ho.Off();
		}
		
		AfterUpdate();
	}
	
	protected virtual void AfterUpdate() {}
	*/
}