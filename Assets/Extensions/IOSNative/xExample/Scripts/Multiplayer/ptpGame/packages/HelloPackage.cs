using UnityEngine;
using System.Collections;

public class HelloPackage : BasePackage {

	public HelloPackage() {
		
		initWriter ();

		writeFloat (1.1f); 

	}
	
	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------
	
	public override int getId() {
		return 2;
	}
}
