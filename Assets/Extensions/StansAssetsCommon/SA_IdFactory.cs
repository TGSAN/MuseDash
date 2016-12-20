using UnityEngine;
using System.Collections;
using System.Linq;

public class SA_IdFactory  {
	
	private const string PP_ID_KEY = "SA_IdFactory_Key";
	
	
	public static int NextId {
		get {
			int id = 1;
			if(PlayerPrefs.HasKey(PP_ID_KEY)) {
				id = PlayerPrefs.GetInt(PP_ID_KEY);
				id++;
			} 
			
			PlayerPrefs.SetInt(PP_ID_KEY, id);
			return id;
		}
		
	}

	public static string RandomString {
		get {
			var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			var random = new System.Random();
			string result = new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());
			
			return result;
		}
	}
}
