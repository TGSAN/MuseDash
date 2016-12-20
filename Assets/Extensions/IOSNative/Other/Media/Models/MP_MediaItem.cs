using UnityEngine;
using System.Collections;

public class MP_MediaItem {

	private string _Id;
	private string _Title;
	private string _Artist;
	private string _AlbumTitle;
	private string _AlbumArtist;
	private string _Genre;
	private string _Composer;


	//--------------------------------------
	// Initialize
	//--------------------------------------

	public MP_MediaItem(string id, string title, string artist, string albumTitle, string albumArtist, string genre, string composer) {
		_Id = id;
		_Title = title;
		_Artist = artist;
		_AlbumTitle = albumTitle;
		_AlbumArtist = albumArtist;
		_Genre = genre;
		_Composer = composer;

	}


	//--------------------------------------
	// Get / Set
	//--------------------------------------

	public string Id {
		get {
			return _Id;
		}
	}

	public string Title {
		get {
			return _Title;
		}
	}

	public string Artist {
		get {
			return _Artist;
		}
	}

	public string AlbumTitle {
		get {
			return _AlbumTitle;
		}
	}

	public string AlbumArtist {
		get {
			return _AlbumArtist;
		}
	}

	public string Genre {
		get {
			return _Genre;
		}
	}

	public string Composer {
		get {
			return _Composer;
		}
	}
}
