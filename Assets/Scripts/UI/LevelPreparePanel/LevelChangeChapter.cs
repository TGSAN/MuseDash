using UnityEngine;
using System.Collections;

public class LevelChangeChapter : MonoBehaviour {
	public LevelCell2001 _MoveChapter = null;

	public GameObject _CubButton = null;

	public GameObject _AddButton=null;

	public LevelCell2001 _CDLogic=null;

	public GameObject _EventButton=null;
	/// <summary>
	/// 歌曲的index
	/// </summary>
	public int _index = 1;
	/// <summary>
	/// 章节的index
	/// </summary>
	public int _ChapterIndex=1;
	public UILabel _TipTextLabel=null;
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
//		if (Input.GetKeyDown (KeyCode.C)) 
//		{
//			OpenNextSong ();
//		}
	}

	public void CubeButtonOnclick(GameObject button)
	{
		
		_index++;

		_CDLogic.CubInitUI(AudioClipDefine.AudioClipManager.Get().GetChapterSongs(0,ref _index));
		Debug.Log ("歌曲INDEX="+_index.ToString());
		_MoveChapter.PlayCubeAudio ();
		_TipTextLabel.gameObject.SetActive (false);
	}
	public void AddButtonOnclick(GameObject button)
	{
		
		_index--;

		_CDLogic.AddInitUI(AudioClipDefine.AudioClipManager.Get().GetChapterSongs(0,ref _index));
		Debug.Log ("歌曲INDEX="+_index.ToString());

		_MoveChapter.PlayAddAudio ();
		_TipTextLabel.gameObject.SetActive (false);
	}

	public void OpenUI()
	{
		if (_CubButton != null)
			_CubButton.SetActive (true);
		if (_AddButton != null)
			_AddButton.SetActive (true);
		
		_index = LevelPrepaerPanel.NowLevel;
		Debug.Log ("歌曲INDEX="+_index.ToString());
		///返回当前历史的使用歌曲
		_CDLogic.InitUI(AudioClipDefine.AudioClipManager.Get().GetChapterSongs(0,ref _index));
		//AudioClipDefine.AudioClipManager.Get ().SetUseSongID (_index);
		ButtonShowUI (false);
		_TipTextLabel.gameObject.SetActive (false);
		_MoveChapter.OpenUI ();

		UIEventListener.Get (_EventButton).onClick = EventButtonOnclick;
		//_CDLogic.
	}
	public GameObject _HandleButtonUI=null;
	private bool IsListenButtonOnclick=false;
	/// <summary>
	/// 显示增加和减少按钮的显示效果
	/// </summary>
	/// <param name="Show">If set to <c>true</c> show.</param>
	public void ButtonShowUI(bool Show)
	{
		if (_HandleButtonUI != null) {
			_HandleButtonUI.SetActive (Show);
			if (!IsListenButtonOnclick) 
			{
				UIEventListener.Get (_CubButton).onClick = CubeButtonOnclick;
				UIEventListener.Get (_AddButton).onClick = AddButtonOnclick;
			}
		} else 
		{
			_HandleButtonUI.SetActive (false);
//			UIEventListener.Get (_CubButton).onClick=null;
//			UIEventListener.Get (_AddButton).onClick = null;
		}
	}

	/// <summary>
	/// 显示提示文本对象
	/// </summary>
	public void ShowTipText(string text)
	{
		if (_TipTextLabel != null)
			_TipTextLabel.text = text;
	}
	/// <summary>
	/// 退出对应的UI
	/// </summary>
	public void ExitUI()
	{
		_MoveChapter.ExitUI ();
		if (_TipTextLabel != null)
			_TipTextLabel.gameObject.SetActive (false);
		if (_CubButton != null)
			_CubButton.SetActive (false);
		if (_AddButton != null)
			_AddButton.SetActive (false);
	}
	/// <summary>
	/// 开启下一首歌曲
	/// </summary>
	public void OpenNextSong()
	{
		///加一个歌曲然后进行 判断是否需要进行解锁
		CubeButtonOnclick (null);
		_MoveChapter.SetOpenLock (true);

	}

	public void SetVolume(float value)
	{
		_MoveChapter.SetVolme (value);

	}
	/// <summary>
	/// 事件按钮的点击处理
	/// </summary>
	/// <param name="Button">Button.</param>
	public void EventButtonOnclick(GameObject Button)
	{
		Debug.Log ("EventButton Onclick");

	}
}
