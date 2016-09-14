using UnityEngine;
using System.Collections;

public class LevelCell2001 : MonoBehaviour {
	public Animation  _HandleAnimation=null;
	public string _OpenAnimationStr = string.Empty;
	public string _ExitAnimationStr=string.Empty;
	public string _CubeAnimationStr = string.Empty;
	public string _AddAnimationStr=string.Empty;

	public LevelCell3[] _ItemUIs = null;

	private LevelCell3 _ItemUIsPlaying=null;
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void OpenUI()
	{
		if(_OpenAnimationStr!=string.Empty)
			_HandleAnimation.Play (_OpenAnimationStr);
	}

	public void ExitUI()
	{
		if(_ExitAnimationStr!=string.Empty)
			_HandleAnimation.Play (_ExitAnimationStr);
	}

	public void SetVolme(float value)
	{
		if (_ItemUIsPlaying != null) 
		{
			_ItemUIsPlaying.SetAudioVolume (value);
		}
	}
	public void PlayAudio(int CDIndex)
	{
		if (_ItemUIsPlaying!=null) 
		{
			_ItemUIsPlaying.StopAudio ();
		}
		_ItemUIsPlaying = _ItemUIs [CDIndex];
		_ItemUIsPlaying.PlayAudio ();
	}
	/// <summary>
	/// 停止音乐
	/// </summary>
	public void StopAudio()
	{
		if (_ItemUIsPlaying != null)
			_ItemUIsPlaying.StopAudio ();
	}
	public void PlayCubeAudio()
	{
		_HandleAnimation.Stop ();
		_HandleAnimation.Play (_CubeAnimationStr);
	}
	public void PlayAddAudio()
	{
		_HandleAnimation.Stop ();
		_HandleAnimation.Play (_AddAnimationStr);
	}

	private bool IsOpen=false;
	public void SetOpenLock(bool Open)
	{
		IsOpen = Open;
	}

	public void OpenSong()
	{
		if (IsOpen) 
		{
			if (_ItemUIsPlaying != null)
			{
				_ItemUIsPlaying.PlayLockAnimation ();
				_ItemUIsPlaying.StopAudio ();
				IsOpen = false;
			}
		} 
		else 
		{
			return;
		}
	}

	public void InitUI(AudioClipDefine.ChapterSongData[] _data)
	{
		//_ItemUIs
		_ItemUIs[0].InitUI(_data[1]);
		_ItemUIs[1].InitUI(_data[2]);
		_ItemUIs[2].InitUI(_data[3]);
		_ItemUIs[3].InitUI(_data[4]);
		_ItemUIs[4].InitUI(_data[5]);
		_ItemUIs[5].InitUI(_data[0]);

		m_OldSongUI.SetActive (true);
		m_NewSongUI.SetActive (false);
		if (_data [2].IsLock)
		{
			m_NewSongNameLabel.text = "???";
			m_NewSongAuthorNameLabel.text = "???";
			m_NewSongTiliLabel.text = "???";
		} 
		else 
		{
			m_NewSongNameLabel.text = _data [2].SongName;
			m_NewSongAuthorNameLabel.text = _data [2].SongText;
			m_NewSongTiliLabel.text = _data [2].CostTili.ToString ();
		}

		if (_data [2].IsLock) 
		{
			m_OldSongNameLabel	.text = "???";
			m_OldSongAuthorNameLabel	.text = "???";
			m_OldSongTiliLabel	.text = "???";
		} 
		else 
		{
			m_OldSongNameLabel	.text = _data [2].SongName;
			m_OldSongAuthorNameLabel	.text = _data [2].SongText;
			m_OldSongTiliLabel	.text = _data [2].CostTili.ToString ();
		}
		//LevelPrepaerPanel.NowLevel = _data [2].SongIndex;
		//AudioClipDefine.AudioClipManager.Get ().SetUseSongID (_data[2].SongID);

	}

	public void CubInitUI(AudioClipDefine.ChapterSongData[] _data)
	{
		_ItemUIs[0].InitUI(_data[0]);
		_ItemUIs[1].InitUI(_data[1]);
		_ItemUIs[2].InitUI(_data[2]);
		_ItemUIs[3].InitUI(_data[3]);
		_ItemUIs[4].InitUI(_data[4]);
		_ItemUIs[5].InitUI(_data[5]);
		m_OldSongUI.SetActive (true);
		m_NewSongUI.SetActive (false);

		if (_data [2].IsLock)
		{
			m_NewSongNameLabel.text = "???";
			m_NewSongAuthorNameLabel.text = "???";
			m_NewSongTiliLabel.text = "???";
		} 
		else 
		{
			m_NewSongNameLabel.text = _data [2].SongName;
			m_NewSongAuthorNameLabel.text = _data [2].SongText;
			m_NewSongTiliLabel.text = _data [2].CostTili.ToString ();
		}



		if (_data [1].IsLock) 
		{
			m_OldSongNameLabel	.text = "???";
			m_OldSongAuthorNameLabel	.text = "???";
			m_OldSongTiliLabel	.text = "???";
		} 
		else 
		{
			m_OldSongNameLabel	.text = _data [1].SongName;
			m_OldSongAuthorNameLabel	.text = _data [1].SongText;
			m_OldSongTiliLabel	.text = _data [1].CostTili.ToString ();
		}
		LevelPrepaerPanel.NowLevel = _data [2].SongIndex;
		//AudioClipDefine.AudioClipManager.Get ().SetUseSongID (_data[2].SongID);
	}
	public void AddInitUI(AudioClipDefine.ChapterSongData[] _data)
	{
		_ItemUIs[0].InitUI(_data[2]);
		_ItemUIs[1].InitUI(_data[3]);
		_ItemUIs[2].InitUI(_data[4]);
		_ItemUIs[3].InitUI(_data[5]);
		_ItemUIs[4].InitUI(_data[0]);
		_ItemUIs[5].InitUI(_data[1]);
		m_OldSongUI.SetActive (true);
		m_NewSongUI.SetActive (false);

		if (_data [2].IsLock)
		{
			m_NewSongNameLabel.text = "???";
			m_NewSongAuthorNameLabel.text = "???";
			m_NewSongTiliLabel.text = "???";
		} 
		else 
		{
			m_NewSongNameLabel.text = _data [2].SongName;
			m_NewSongAuthorNameLabel.text = _data [2].SongText;
			m_NewSongTiliLabel.text = _data [2].CostTili.ToString ();
		}

		if (_data [3].IsLock) 
		{
			m_OldSongNameLabel	.text = "???";
			m_OldSongAuthorNameLabel	.text = "???";
			m_OldSongTiliLabel	.text = "???";
		} 
		else 
		{
			m_OldSongNameLabel	.text = _data [3].SongName;
			m_OldSongAuthorNameLabel	.text = _data [3].SongText;
			m_OldSongTiliLabel	.text = _data [3].CostTili.ToString ();
		}
		LevelPrepaerPanel.NowLevel = _data [2].SongIndex;
		//AudioClipDefine.AudioClipManager.Get ().SetUseSongID (_data[2].SongID);
	}
	public LevelChangeChapter _RootCs=null;
	/// <summary>
	/// 显示处理的按钮
	/// </summary>
	/// <param name="Show">If set to <c>true</c> show.</param>
	public void ShowHandleButtonUi()
	{
		if (_RootCs != null) 
		{
			_RootCs.ButtonShowUI (true);
		}
	}

	/// <summary>
	/// 旧的歌曲UI
	/// </summary>
	public GameObject m_OldSongUI=null;
	/// <summary>
	/// 旧歌的文本对象
	/// </summary>
	public UILabel   m_OldSongNameLabel=null;
	/// <summary>
	/// 旧歌的作者名字
	/// </summary>
	public UILabel   m_OldSongAuthorNameLabel=null;
	/// <summary>
	/// 旧歌的体力消耗
	/// </summary>
	public UILabel   m_OldSongTiliLabel=null;

	public void SetOldSongInfor(bool Showui,string OldSongName,string AuthorName,int TiliValue)
	{
		if (m_OldSongUI != null)
			m_OldSongUI.SetActive (Showui);
		if (m_OldSongNameLabel != null)
			m_OldSongNameLabel.text = OldSongName;
		if (m_OldSongAuthorNameLabel != null)
			m_OldSongAuthorNameLabel.text = AuthorName;
		if (m_OldSongTiliLabel != null)
			m_OldSongTiliLabel.text = TiliValue.ToString ();
	}
	/// <summary>
	/// 新的歌曲UI
	/// </summary>
	public GameObject m_NewSongUI=null;
	/// <summary>
	/// 新歌的文本对象
	/// </summary>
	public UILabel   m_NewSongNameLabel=null;
	/// <summary>
	/// 新歌的作者名字
	/// </summary>
	public UILabel   m_NewSongAuthorNameLabel=null;
	/// <summary>
	/// 新歌歌消耗的体力
	/// </summary>
	public UILabel m_NewSongTiliLabel = null;
	public void SetNewSongInfor(bool Showui,string NewSongName,string AuthorName,int TiliValue)
	{
		if (m_NewSongUI != null)
			m_NewSongUI.SetActive (Showui);
		if (m_NewSongNameLabel != null)
			m_NewSongNameLabel.text = NewSongName;
		if (m_NewSongAuthorNameLabel != null)
			m_NewSongAuthorNameLabel.text = AuthorName;
		if (m_NewSongTiliLabel != null)
			m_NewSongTiliLabel.text = TiliValue.ToString ();
	}

	void OnEnable()
	{
		if (_ItemUIsPlaying != null)
			_ItemUIsPlaying.PlayAudio ();
	}

	void OnDisable()
	{
		if (_ItemUIsPlaying != null)
			_ItemUIsPlaying.StopAudio ();
	}
}
