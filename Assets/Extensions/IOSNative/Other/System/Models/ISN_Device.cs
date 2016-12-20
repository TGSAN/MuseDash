using UnityEngine;
using System.Collections;



#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
using System.Runtime.InteropServices;
#endif


public class ISN_Device  {
	#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
	[DllImport ("__Internal")]
	private static extern string _ISN_RetriveDeviceData();
	#endif


	private static ISN_Device _CurrentDevice = null;

	private string _Name = "Test Name";
	private string _SystemName = "iPhone OS";
	private string _Model = "iPhone";
	private string _LocalizedModel = "iPhone";
	

	private string _SystemVersion = "9.0.0";
	private int _MajorSystemVersion = 9;
	

	private ISN_InterfaceIdiom _InterfaceIdiom = ISN_InterfaceIdiom.Phone;
	private ISN_DeviceGUID _GUID =  new ISN_DeviceGUID(string.Empty);


	public ISN_Device() {
		
	}
	

	public ISN_Device(string deviceData) {

		string[] dataArray 		= deviceData.Split(IOSNative.DATA_SPLITTER);

		_Name = dataArray[0];
		_SystemName = dataArray[1];
		_Model = dataArray[2];
		_LocalizedModel = dataArray[3];

		_SystemVersion = dataArray[4];
		_MajorSystemVersion = System.Convert.ToInt32(dataArray[5]);

		_InterfaceIdiom = (ISN_InterfaceIdiom) System.Convert.ToInt32(dataArray[6]);
		_GUID = new ISN_DeviceGUID(dataArray[7]);

	}

	public string Name {
		get {
			return _Name;
		}
	}

	public string SystemName {
		get {
			return _SystemName;
		}
	}

	public string Model {
		get {
			return _Model;
		}
	}

	public string LocalizedModel {
		get {
			return _LocalizedModel;
		}
	}



	public string SystemVersion {
		get {
			return _SystemVersion;
		}
	}

	public int MajorSystemVersion {
		get {
			return _MajorSystemVersion;
		}
	}


	public ISN_InterfaceIdiom InterfaceIdiom {
		get {
			return _InterfaceIdiom;
		}
	}

	public ISN_DeviceGUID GUID {
		get {
			return _GUID;
		}
	}




	public static ISN_Device CurrentDevice {
		get {

			if(_CurrentDevice == null) {

			#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
				_CurrentDevice =  new ISN_Device(_ISN_RetriveDeviceData());
			#else
				_CurrentDevice =  new ISN_Device();
			#endif

			}

			return _CurrentDevice;
		}
	}


}
