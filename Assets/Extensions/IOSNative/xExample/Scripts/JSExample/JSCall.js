#pragma strict

function Start () {
	SendMessage ("InitGameCneter");
}



	
function OnGUI () {
	if (GUI.Button(new Rect(10,10,150,40),"Submit Score")) {
		SendMessage ("SubmitScore", 100);
	}
	
	if (GUI.Button(new Rect(10,60,150,40),"Submit Achievement")) {
	
		 var arr = new Array();
		 arr.push("20.2");
		 arr.push("your.achievement.id1.here");
		 
		 SendMessage ("SubmitAchievement", arr.join("|"));
	}
}