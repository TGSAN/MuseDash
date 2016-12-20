////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GK_ScoreCollection {

	
	public Dictionary<int, GK_Score> AllTimeScores =  new Dictionary<int, GK_Score>();
	public Dictionary<int, GK_Score> WeekScores =  new Dictionary<int, GK_Score>();
	public Dictionary<int, GK_Score> TodayScores =  new Dictionary<int, GK_Score>();

}

