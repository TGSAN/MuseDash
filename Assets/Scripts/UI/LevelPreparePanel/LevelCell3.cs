using UnityEngine;
using System.Collections;

public class LevelCell3 : MonoBehaviour {

	//UILabel m_label=null;
	public UISprite m_sprite=null;
	//public UILabel m_SongNameLabel=null;
	public int m_SongID = -1;

	public SpectrumAnalyzer m_AudioCs = null;

	private AudioClipDefine.ChapterSongData m_data = null;

	public GameObject _LockIcon = null;

	public GameObject _LockTextUI=null;
	/// <summary>
	/// 难度的参数
	/// </summary>
	public LevelCell2002 _DifficultyProgress = null;
	//public UILabel m_PhysicalLabel;
	//public UISprite m_sprite;
	void Awake()
	{
		
		
	}
	// Use this for initialization
	void Start () 
	{
		
	}

	//public Camera m_camera;
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	public void InitUI(AudioClipDefine.ChapterSongData _data)
	{
		//m_SongNameLabel.text = _data.SongName;
		m_sprite.spriteName = _data.IconSpriteName;
		m_data = _data;
		if (m_AudioCs != null) 
		{
			//m_AudioCs._SoureClip = _data._AudioClp;
			m_AudioCs.InitAudioSoure (_data.EndTime,_data.StartTime, _data._AudioClp);
		}
		if (_LockIcon != null) 
		{
			if (_data.IsLock) 
			{
				_LockIcon.SetActive (false);
			} 
			else 
			{
				_LockIcon.SetActive (true);
				UISpriteColor TempCs = _LockIcon.GetComponent<UISpriteColor> ();
				if(TempCs!=null)
					TempCs._UseColor.a = 1.0f;
			}
			_LockIcon.SetActive (_data.IsLock);
		}
	}
	public UILabel _TipTextUI=null;
	public void PlayAudio()
	{
		if (m_AudioCs == null)
			return;
		
		if (m_data.IsLock) 
		{
			_TipTextUI.gameObject.SetActive (false);
			_TipTextUI.color = Color.red;
			_LockIcon.SetActive (true);
			UISpriteColor TempCs = _LockIcon.GetComponent<UISpriteColor> ();
			if(TempCs!=null)
				TempCs._UseColor.a = 1.0f;
			m_AudioCs.PlayAudio ();

			///在歌曲在解锁过程中 音乐的处理
		} 
		else 
		{
			m_AudioCs.PlayAudio ();
			_TipTextUI.gameObject.SetActive (true);
			_TipTextUI.text = "TOUCH TO START";
			_TipTextUI.color = Color.white;
			_LockIcon.SetActive (false);
		}
		UIEventListener.Get (gameObject).onClick = ButtonOnclick;
		UIEventListener.Get (gameObject).onDrag = DragButtonOnclick;
		UIEventListener.Get (gameObject).onDragEnd = DragOverOnclick;

		_DifficultyProgress.PlayAnimaiton (m_data.SongDiffcult);
	}

	public void StopAudio()
	{
		if (m_AudioCs == null)
			return;
		m_AudioCs.StopAudio ();

		UIEventListener.Get (gameObject).onClick = null;
		UIEventListener.Get (gameObject).onDrag = null;
		UIEventListener.Get (gameObject).onDragOver = null;
	}

	public void SetAudioVolume(float value)
	{
		if (m_AudioCs == null)
			return;
		m_AudioCs.Setvolume (value);
	}

	public void ButtonOnclick(GameObject button)
	{
		if (m_data.IsLock) 
		{
			///开启解锁CDUI
		} 
		else
		{
			
			FormulaBase.StageBattleComponent.Instance.SetStageId ((uint)m_data.SongID);
			//UIManageSystem.g_Instance.SetTopPanel(false);
			SetAudioVolume(0.5f);
			//UIManageSystem.g_Instance.AddUI(UIManageSystem.UILEVELGOALSPANEL,1,FormulaBase.StageBattleComponent.Instance.GetStage((int)m_data.SongID));
			///进入战斗界面
		}
		Debug.Log ("点击了长篇");
	}
	public LevelChangeChapter _RootCs=null;
	private Vector2 _DraData=Vector2.zero;
	/// <summary>
	/// 正在拖拽
	/// </summary>
	/// <param name="button">Button.</param>
	/// <param name="_dragv2">Dragv2.</param>
	public void DragButtonOnclick(GameObject button,Vector2 _dragv2)
	{
		
		_DraData = _dragv2;
	
	}
	/// <summary>
	/// 拖拽完成
	/// </summary>
	/// <param name="button">Button.</param>
	public void DragOverOnclick(GameObject button)
	{
		if (_DraData.x > 0)
		{
			_RootCs.AddButtonOnclick (null);
		} 
		else if (_DraData.x < 0) 
		{
			_RootCs.CubeButtonOnclick (null);
		} else
			return;
	}


	public Animation _HandleAnimation = null;
	public GameObject _Star1=null;
	public void PlayLockAnimation()
	{
		if (_HandleAnimation != null) 
		{
			_HandleAnimation.Stop ();
			_HandleAnimation.Play();
			_Star1.SetActive (false);
		}
	}
	/// <summary>
	/// 开启完歌曲后 提示文本的显示+歌曲的播放
	/// </summary>
	public void OpenSongEnd()
	{
	}
}
