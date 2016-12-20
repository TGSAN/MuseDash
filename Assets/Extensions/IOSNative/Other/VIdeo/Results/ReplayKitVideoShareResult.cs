using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReplayKitVideoShareResult  {

	private string[] _Sources =  new string[0];


	public ReplayKitVideoShareResult(string[] sourcesArray) {
		_Sources = sourcesArray;
	}
	

	public string[] Sources {
		get {
			return _Sources;
		}
	}
}
