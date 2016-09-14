using UnityEngine;
using System.Collections;

public class UIRayIgnore : MonoBehaviour , ICanvasRaycastFilter{

	public bool ifNotIgnore;

	public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera){
		return ifNotIgnore;
	}
}
