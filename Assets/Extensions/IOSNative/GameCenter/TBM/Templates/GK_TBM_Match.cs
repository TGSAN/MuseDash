using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GK_TBM_Match  {

	public string Id;
	public string Message;
	public GK_TBM_Participant CurrentParticipant;
	public DateTime CreationTimestamp;

	public byte[] Data;

	public GK_TurnBasedMatchStatus Status;
	public List<GK_TBM_Participant> Participants;



	public void SetData(string val) {
		byte[] decodedFromBase64 = System.Convert.FromBase64String(val);
		Data = decodedFromBase64;
	}

	public string UTF8StringData {
		get {
			if(Data != null) {
				return System.Text.Encoding.UTF8.GetString(Data);
			} else {
				return string.Empty;
			}

		}
	}

	public GK_TBM_Participant GetParticipantByPlayerId(string playerId) {

		foreach(GK_TBM_Participant participant in Participants) {
			
			if(participant.Player == null) {
				if(playerId.Length == 0) {
					return participant;
				}
			} else {
				if(playerId.Equals(participant.Player.Id)) {
					return participant;
				}
			}
		}
		
		
		return null;
	}
	

}
