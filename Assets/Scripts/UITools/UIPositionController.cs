using UnityEngine;
//using UnityEngine.iOS;
using UnityEngine.UI;
using System.Collections;

public enum HorizontalType{
	LEFT = 0,
	RIGHT = 1,
	CENTER = 2
};

public enum VerticalType{
	TOP = 0,
	DOWN = 1,
	CENTER = 2
};

public class UIPositionController{

	private bool isIphone = true;
	public UIPositionController(){
#if Device
		if(Device.generation == DeviceGeneration.iPad1Gen){
			isIphone = false;
		};
		
		if(Device.generation == DeviceGeneration.iPad2Gen){
			isIphone = false;
		};
		
		if(Device.generation == DeviceGeneration.iPad3Gen){
			isIphone = false;
		};
		
		if(Device.generation == DeviceGeneration.iPad4Gen){
			isIphone = false;
		};
		
		if(Device.generation == DeviceGeneration.iPadAir1){
			isIphone = false;
		};
		
		if(Device.generation == DeviceGeneration.iPadAir2){
			isIphone = false;
		};
#else
		return;
#endif
	}

	private static UIPositionController instance = null;

	public static UIPositionController Instance{
		get{
			if(instance == null){
				instance = new UIPositionController();
			}
			return instance;
		}
	}

	public Vector3 AutoFitPosition(Transform transform,GameObject canvas,Vector3 currentPosition,HorizontalType horizontal = HorizontalType.CENTER,VerticalType vertical = VerticalType.CENTER){

		var scaler = canvas.GetComponent<CanvasScaler>();

		if(isIphone){
			// TODO:

			//Debug.Log("currentResolution.width: "+ Screen.width);
			//Debug.Log("currentResolution.height: "+ Screen.height);
			//Debug.Log("current position a: " + currentPosition.ToString());
			//Debug.Log("transform: "+transform);

			var half = Screen.width/2;
			var rate = 750f / Screen.height;

			if(horizontal == HorizontalType.LEFT){
				currentPosition = new Vector3(currentPosition.x - half * rate + 667,currentPosition.y,currentPosition.z);
			}else if(horizontal == HorizontalType.RIGHT){
				currentPosition = new Vector3(currentPosition.x + half * rate - 667,currentPosition.y,currentPosition.z);
			};

			//Debug.Log("current position b: " + currentPosition.ToString());

			return currentPosition;

		}else{

			scaler.matchWidthOrHeight = 0;

//			var half = Screen.width/2;
//			var rate = 750f / Screen.height;
//			
//			if(horizontal == HorizontalType.LEFT){
//				currentPosition = new Vector3(currentPosition.x - half * rate + 667,currentPosition.y,currentPosition.z);
//			}else if(horizontal == HorizontalType.RIGHT){
//				currentPosition = new Vector3(currentPosition.x + half * rate - 667,currentPosition.y,currentPosition.z);
//			};
			
//			Debug.Log("current position b: " + currentPosition.ToString());
			
			return currentPosition;

		}
	}
}
