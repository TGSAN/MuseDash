using UnityEngine;
using System.Collections;

public class ISN_NotificationType  {

	//The application may not present any UI upon a notification being received
	public const int None    = 0;   
	//The application may badge its icon upon a notification being received
	public const int Badge   = 1 << 0; 
	//The application may badge its icon upon a notification being received
	public const int Sound   = 1 << 1;
	//The application may display an alert upon a notification being received
	public const int UAlert  = 1 << 2;
}
