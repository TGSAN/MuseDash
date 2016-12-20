using UnityEngine;
using UnityEditor;
using System;
using System.Collections;


public static class SA_VersionsManager  {


	public const string SUPPORT_EMAIL = "support@stansassets.com";


	public const string VERSION_INFO_PATH = "Plugins/StansAssets/Versions/";

	public const string AN_VERSION_INFO_PATH 	= VERSION_INFO_PATH + "AN_VersionInfo.txt";
	public const string UM_VERSION_INFO_PATH 	= VERSION_INFO_PATH + "UM_VersionInfo.txt";
	public const string GMA_VERSION_INFO_PATH 	= VERSION_INFO_PATH + "GMA_VersionInfo.txt";
	public const string MSP_VERSION_INFO_PATH 	= VERSION_INFO_PATH + "MSP_VersionInfo.txt";
	public const string ISN_VERSION_INFO_PATH 	= VERSION_INFO_PATH + "ISN_VersionInfo.txt";
	public const string MNP_VERSION_INFO_PATH 	= VERSION_INFO_PATH + "MNP_VersionInfo.txt";

	private const string UM_IOS_INSTALATION_MARK = PluginsInstalationUtil.IOS_DESTANATION_PATH + "UM_IOS_INSTALATION_MARK.txt";


	//--------------------------------------
	// Android Native
	//--------------------------------------

	
	public static bool Is_AN_Installed {
		get { 
			return FileStaticAPI.IsFileExists(AN_VERSION_INFO_PATH);
		}
	}

	public static int AN_Version {
		get {
			return GetVersionCode(AN_VERSION_INFO_PATH);
		}
	}

	public static int AN_MagorVersion {
		get {
			return GetMagorVersionCode(AN_VERSION_INFO_PATH);
		}
	}


	public static string AN_StringVersionId {
		get {
			return GetStringVersionId(AN_VERSION_INFO_PATH);
		}
	}

	//--------------------------------------
	// Mobile Social 
	//--------------------------------------
	

	public static bool Is_MSP_Installed {
		get {
			return FileStaticAPI.IsFileExists(MSP_VERSION_INFO_PATH);
		}
	}

	public static int MSP_Version {

		get {
			return GetVersionCode(MSP_VERSION_INFO_PATH);
		}
	}

	public static int MSP_MagorVersion {
		
		get {
			return GetMagorVersionCode(MSP_VERSION_INFO_PATH);
		}
	}

	public static string MSP_StringVersionId {
		get {
			return GetStringVersionId(MSP_VERSION_INFO_PATH);
		}
	}

	//--------------------------------------
	// Ultimate Mobile  
	//--------------------------------------
	 
	public static bool Is_UM_Installed {
		get {
			return FileStaticAPI.IsFileExists(UM_VERSION_INFO_PATH);
		} 
	}
	
	public static int UM_Version {
		get {
			return GetVersionCode(UM_VERSION_INFO_PATH);
		}
	}

	public static int UM_MagorVersion {
		get {
			return GetMagorVersionCode(UM_VERSION_INFO_PATH);
		}
	}

	public static string UM_StringVersionId {
		get {
			return GetStringVersionId(UM_VERSION_INFO_PATH);
		}
	}


	//--------------------------------------
	// Google Mobile Ad  
	//--------------------------------------

	public static bool Is_GMA_Installed {
		get {
			return FileStaticAPI.IsFileExists(GMA_VERSION_INFO_PATH);
		} 
	}
	
	public static int GMA_Version {
		get {
			return GetVersionCode(GMA_VERSION_INFO_PATH);
		} 
	}

	public static int GMA_MagorVersion {
		get {
			return GetMagorVersionCode(GMA_VERSION_INFO_PATH);
		} 
	}

	public static string GMA_StringVersionId {
		get {
			return GetStringVersionId(GMA_VERSION_INFO_PATH);
		}
	}
	//--------------------------------------
	// Mobile Native Pop Up 
	//--------------------------------------
	public static bool Is_MNP_Installed {
		get {
			return FileStaticAPI.IsFileExists(MNP_VERSION_INFO_PATH);
		} 
	}
	
	public static int MNP_Version {
		get {
			return GetVersionCode(MNP_VERSION_INFO_PATH);
		} 
	}
	
	public static int MNP_MagorVersion {
		get {
			return GetMagorVersionCode(MNP_VERSION_INFO_PATH);
		} 
	}
	
	public static string MNP_StringVersionId {
		get {
			return GetStringVersionId(MNP_VERSION_INFO_PATH);
		}
	}

	//--------------------------------------
	// IOS Native   
	//--------------------------------------

	public static bool Is_ISN_Installed {
		get {
			return FileStaticAPI.IsFileExists(ISN_VERSION_INFO_PATH);
		} 
	}
	
	public static int ISN_Version {
		get {
			return GetVersionCode(ISN_VERSION_INFO_PATH);
		} 
	}

	public static int ISN_MagorVersion {
		get {
			return GetMagorVersionCode(ISN_VERSION_INFO_PATH);
		} 
	}


	public static string ISN_StringVersionId {
		get {
			return GetStringVersionId(ISN_VERSION_INFO_PATH);
		}
	}

	
	//--------------------------------------
	// Utilities
	//--------------------------------------

	public static int ParceMagorVersion(string stringVersionId) {
		string[] versions = stringVersionId.Split (new char[] {'.'});
		int intVersion = Int32.Parse(versions[0]) * 100 + Int32.Parse(versions[1]) * 10;
		return  intVersion;
	} 

	
	private static int GetMagorVersionCode(string versionFilePath) {
		string stringVersionId = FileStaticAPI.Read (versionFilePath);
		return ParceMagorVersion(stringVersionId);
	}



	public static int ParceVersion(string stringVersionId) {
		string[] versions = stringVersionId.Split (new char[] {'.'});
		int intVersion = Int32.Parse(versions[0]) * 100 + Int32.Parse(versions[1]) * 10 + (versions.Length == 3 ? Int32.Parse(versions[2]) : 0);
		return  intVersion;
	} 



	private static int GetVersionCode(string versionFilePath) {
		string stringVersionId = FileStaticAPI.Read (versionFilePath);
		return ParceVersion(stringVersionId);
	}



	private static string GetStringVersionId(string versionFilePath) {
		if(FileStaticAPI.IsFileExists(versionFilePath)) {
			return FileStaticAPI.Read (versionFilePath);
		} else {
			return "0.0";
		}
	}


	public static string InstalledPluginsList {

		get {
			string allPluginsInstalled = "";
			
			if(Is_AN_Installed) {
				allPluginsInstalled = allPluginsInstalled + " (Android Native)" + "\n";
			}
			
			if(Is_ISN_Installed) {
				allPluginsInstalled = allPluginsInstalled + " (IOS Native)" + "\n";
			}
			
			if(Is_UM_Installed) {
				allPluginsInstalled = allPluginsInstalled + " (Ultimate Mobile)" + "\n";
			}
			
			
			if(Is_GMA_Installed) {
				allPluginsInstalled = allPluginsInstalled + " (Google Mobile Ad)" + "\n";
			}
			
			if(Is_MSP_Installed) {
				allPluginsInstalled = allPluginsInstalled + " (Mobile Social)" + "\n";
			}

			if(Is_MNP_Installed) {
				allPluginsInstalled = allPluginsInstalled + " (Mobile Native Pop Up)" + "\n";
			}

			return allPluginsInstalled;
		}
	}



}

















