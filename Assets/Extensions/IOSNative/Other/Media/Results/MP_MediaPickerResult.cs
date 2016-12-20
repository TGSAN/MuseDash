using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MP_MediaPickerResult : ISN_Result {

	private List<MP_MediaItem> _SelectedmediaItems;


	public MP_MediaPickerResult(List<MP_MediaItem> selectedItems):base(true) {
		_SelectedmediaItems = selectedItems;
	}

	public MP_MediaPickerResult(string errorData):base(errorData) {
		
	}



	public List<MP_MediaItem> SelectedmediaItems {
		get {
			return _SelectedmediaItems;
		}
	}
}
