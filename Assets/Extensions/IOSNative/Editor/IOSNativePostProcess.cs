using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;

public class IOSNativePostProcess  {

	#if UNITY_IPHONE
	[PostProcessBuild(50)]
	public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) {


		if(IOSNativeSettings.Instance.EnableInAppsAPI) {

			string StoreKit = "StoreKit.framework";

			if(!ISDSettings.Instance.ContainsFreamworkWithName(StoreKit)) {
				ISD_Framework F =  new ISD_Framework();
				F.Name = StoreKit;
				ISDSettings.Instance.Frameworks.Add(F);
			}

		}

		if(IOSNativeSettings.Instance.EnableGameCenterAPI) {
			
			string GameKit = "GameKit.framework";
			if(!ISDSettings.Instance.ContainsFreamworkWithName(GameKit)) {
				ISD_Framework F =  new ISD_Framework();
				F.Name = GameKit;
				ISDSettings.Instance.Frameworks.Add(F);
			}
			
		}



		if(IOSNativeSettings.Instance.EnableSocialSharingAPI) {
		
			string Accounts = "Accounts.framework";
			if(!ISDSettings.Instance.ContainsFreamworkWithName(Accounts)) {
				ISD_Framework F =  new ISD_Framework();
				F.Name = Accounts;
				ISDSettings.Instance.Frameworks.Add(F);
			}

			
			
			string SocialF = "Social.framework";
			if(!ISDSettings.Instance.ContainsFreamworkWithName(SocialF)) {
				ISD_Framework F =  new ISD_Framework();
				F.Name = SocialF;
				ISDSettings.Instance.Frameworks.Add(F);
			}
			
			string MessageUI = "MessageUI.framework";
			if(!ISDSettings.Instance.ContainsFreamworkWithName(MessageUI)) {
				ISD_Framework F =  new ISD_Framework();
				F.Name = MessageUI;
				ISDSettings.Instance.Frameworks.Add(F);
			}


			ISD_Variable LSApplicationQueriesSchemes =  new ISD_Variable();
			LSApplicationQueriesSchemes.Name = "LSApplicationQueriesSchemes";
			LSApplicationQueriesSchemes.Type = ISD_PlistValueTypes.Array;
			LSApplicationQueriesSchemes.ArrayType = ISD_PlistValueTypes.String;

			ISD_VariableListed instagram =  new ISD_VariableListed();
			instagram.StringValue = "instagram";
			instagram.Type = ISD_PlistValueTypes.String;
			LSApplicationQueriesSchemes.ArrayValue.Add(instagram);

			ISD_VariableListed whatsapp =  new ISD_VariableListed();
			whatsapp.StringValue = "whatsapp";
			whatsapp.Type = ISD_PlistValueTypes.String;
			LSApplicationQueriesSchemes.ArrayValue.Add(whatsapp);


			if(!ISDSettings.Instance.ContainsPlistVarkWithName(LSApplicationQueriesSchemes.Name)) {
				ISDSettings.Instance.PlistVariables.Add(LSApplicationQueriesSchemes);
			}
		}


		if(IOSNativeSettings.Instance.EnableMediaPlayerAPI) {
			string MediaPlayer = "MediaPlayer.framework";
			if(!ISDSettings.Instance.ContainsFreamworkWithName(MediaPlayer)) {
				ISD_Framework F =  new ISD_Framework();
				F.Name = MediaPlayer;
				ISDSettings.Instance.Frameworks.Add(F);
			}
		}
	

		if(IOSNativeSettings.Instance.EnableCameraAPI) {
			string MobileCoreServices = "MobileCoreServices.framework";
			if(!ISDSettings.Instance.ContainsFreamworkWithName(MobileCoreServices)) {
				ISD_Framework F =  new ISD_Framework();
				F.Name = MobileCoreServices;
				ISDSettings.Instance.Frameworks.Add(F);
			}
		}

		if(IOSNativeSettings.Instance.EnableReplayKit) {
			string ReplayKit = "ReplayKit.framework";
			if(!ISDSettings.Instance.ContainsFreamworkWithName(ReplayKit)) {
				ISD_Framework F =  new ISD_Framework();
				F.Name = ReplayKit;
				F.IsOptional = true;
				ISDSettings.Instance.Frameworks.Add(F);
			}
		}


		if(IOSNativeSettings.Instance.EnableCloudKit) {
			string CloudKit = "CloudKit.framework";
			if(!ISDSettings.Instance.ContainsFreamworkWithName(CloudKit)) {
				ISD_Framework F =  new ISD_Framework();
				F.Name = CloudKit;
				F.IsOptional = true;
				ISDSettings.Instance.Frameworks.Add(F);
			}
		}


		if(IOSNativeSettings.Instance.EnableSoomla) {
			string AdSupport = "AdSupport.framework";
			if(!ISDSettings.Instance.ContainsFreamworkWithName(AdSupport)) {
				ISD_Framework F =  new ISD_Framework();
				F.Name = AdSupport;
				ISDSettings.Instance.Frameworks.Add(F);
			}

			string libsqlite3 = "libsqlite3.dylib";
			if(!ISDSettings.Instance.ContainsLibWithName(libsqlite3)) {
				ISD_Lib L =  new ISD_Lib();
				L.Name = libsqlite3;
				ISDSettings.Instance.Libraries.Add(L);
			}



			#if UNITY_5
				string linkerGlag = "-force_load Libraries/Plugins/iOS/libSoomlaGrowLite.a";
			#else
				string linkerGlag = "-force_load Libraries/libSoomlaGrowLite.a";
			#endif

			if(!ISDSettings.Instance.linkFlags.Contains(linkerGlag)) {
				ISDSettings.Instance.linkFlags.Add(linkerGlag);
			}
		}


		Debug.Log("ISN Postprocess Done");

	
	}
	#endif
}
