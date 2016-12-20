using UnityEngine;
using System;
using System.Reflection;
using System.Collections;




public class SA_ModulesInfo  {

	public const int VERSION_UNDEFINED = 0;
	public const string VERSION_UNDEFINED_STRING = "Undefined";



	public static string FB_SDK_VersionCode {
		get {

			string versionCode = VERSION_UNDEFINED_STRING;
			foreach (System.Reflection.Assembly a in System.AppDomain.CurrentDomain.GetAssemblies()) {


				Type FBBuildVersionAttribute_type 	= a.GetType("Facebook.FBBuildVersionAttribute");
				Type IFacebook_type 				= a.GetType("Facebook.IFacebook");


				if(IFacebook_type != null && FBBuildVersionAttribute_type != null) {
				

					MethodInfo method  = FBBuildVersionAttribute_type.GetMethod("GetVersionAttributeOfType", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

					if(method != null) {
						object  MethodValue =  method.Invoke(null, new object[] { IFacebook_type } );
						PropertyInfo SdkVersionProp = FBBuildVersionAttribute_type.GetProperty("SdkVersion");
						if(MethodValue != null && SdkVersionProp != null) {
							String vc =   SdkVersionProp.GetValue(MethodValue, null)  as String;
							if(vc != null) {
								versionCode = vc;
							}
						}
					}
					
					break;

				}
			}

		
			Type FacebookSdkVersion_type 	= Type.GetType("Facebook.Unity.FacebookSdkVersion");
			if(FacebookSdkVersion_type != null) {
				System.Reflection.PropertyInfo propert  = FacebookSdkVersion_type.GetProperty("Build", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
				if(propert != null) {
					versionCode = (string) propert.GetValue(null, null);
				}
			}


		


			return versionCode;

		}
	}


	public static int FB_SDK_MajorVersionCode {
		get {
			string versionCode = FB_SDK_VersionCode;
			int MajorVersion = VERSION_UNDEFINED;

			if(versionCode.Equals(VERSION_UNDEFINED_STRING)) {
				return MajorVersion;
			}

			try {
				string[] SplittedVersionCode = versionCode.Split (new char[] {'.'});
				MajorVersion = System.Convert.ToInt32(SplittedVersionCode[0]);
			} catch (Exception ex) {
				Debug.LogWarning("FB_SDK_MajorVersionCode failed: " + ex.Message);
			}

			return MajorVersion;
		}
	}





}
