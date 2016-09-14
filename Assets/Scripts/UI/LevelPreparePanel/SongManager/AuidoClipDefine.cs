using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FormulaBase;
using GameLogic;
using System.Threading;

namespace AudioClipDefine 
{
	[System.Serializable]
	public class ChapterSongData
	{
		/// <summary>
		/// 歌名的ID
		/// </summary>
		public int SongID=-1 ;
		/// <summary>
		/// 歌曲的章节顺序
		/// </summary>
		public int SongIndex = 0;
		/// <summary>
		/// 歌曲的章节ID
		/// </summary>
		public int SongChapterID = 0;
		/// <summary>
		/// 歌名
		/// </summary>
		public string SongName=string.Empty;
		/// <summary>
		/// 作者
		/// </summary>
		public string SongText=string.Empty ;
		/// <summary>
		/// 音频文件资源
		/// </summary>
		public string _AudioClp=null;
		/// <summary>
		/// IOCN
		/// </summary>
		public int    IconID=-1;
		/// <summary>
		/// 音频的开启时间
		/// </summary>
		public float StartTime = 0f;
		/// <summary>
		/// 音频的结束时间
		/// </summary>
		public float EndTime=60f;
		/// <summary>
		/// CD图标的名字
		/// </summary>
		public  string IconSpriteName = string.Empty;
		/// <summary>
		/// 是否解锁了
		/// </summary>
		public bool  IsLock=false;
		/// <summary>
		/// 歌的难度
		/// </summary>
		public int SongDiffcult = 0;
		/// <summary>
		/// 消耗的体力值
		/// </summary>
		public int CostTili=10;

	}

	public class AudioClipManager
	{
		static AudioClipManager S_Instance = null;

		static public AudioClipManager Get()
		{
			if (S_Instance == null) 
			{
				S_Instance = new AudioClipManager ();
			}
			return S_Instance;
		}
		private List<AudioClipDefine.ChapterSongData> _ChapterList = new List<ChapterSongData> ();
		private Dictionary<int,AudioClipDefine.ChapterSongData> _ChapterDic = new Dictionary<int, ChapterSongData> ();
		public  bool AddSongData(AudioClipDefine.ChapterSongData[] _datas)
		{
			_ChapterList.Clear ();
			for (int i = 0; i < _datas.Length; i++)
			{
				_ChapterList.Add(_datas[i]);
			}

			return true;
		}
		private GameObject _ui=null;
		/// <summary>
		/// 初始化函数  传入该章节的数据
		/// </summary>
		public void InitUI(GameObject ui=null,List<AudioClipDefine.ChapterSongData> datas=null)
		{
			_ChapterDic.Clear ();
			_ChapterList.Clear ();
			Resources.UnloadUnusedAssets ();
			for(int i=0;i<datas.Count;i++)
			{
				_ChapterList.Add(datas[i]);
				_ChapterDic.Add(datas[i].SongID,datas[i]);
			}

			///加载资源 线程加载
			LoadChapterAudio ();
			//UseSongID = datas [LevelPrepaerPanel.NowLevel].SongID;
			//SetUseSongID(1);
			//UseSongID = GetHostrySongID ();
			_ui=ui;
		}
		/// <summary>
		/// 添加一个音乐数据
		/// </summary>
		/// <param name="AudioData">Audio data.</param>
		public void AddSongData(AudioClipDefine.ChapterSongData AudioData)
		{
			if (_ChapterDic.ContainsKey (AudioData.SongID)) 
			{
				Debug.LogError ("歌曲ID 数据已经 重复："+AudioData.SongID.ToString());
				return;
			}
			_ChapterDic.Add (AudioData.SongID, AudioData);
			_ChapterList.Add (AudioData);
		}
		/// <summary>
		/// 移除对应音乐数据
		/// </summary>
		/// <param name="SongID">Song I.</param>
		public bool RemoveSongData(int SongID)
		{
			if (_ChapterDic.ContainsKey (SongID)) 
			{
				_ChapterList.Remove (_ChapterDic [SongID]);
				_ChapterDic.Remove (SongID);
				return true;
			}
			return false;
		}
		/// <summary>
		/// 获得对应音乐ID的数据
		/// </summary>
		/// <param name="SongID">Song I.</param>
		public AudioClipDefine.ChapterSongData  GetSongData(int SongID)
		{
			if (_ChapterDic.ContainsKey (SongID)) 
			{
				return _ChapterDic[SongID];
			}
			return null;
		}
		/// <summary>
		/// 改变对应ID的音乐数据
		/// </summary>
		/// <returns><c>true</c>, if song data was changed, <c>false</c> otherwise.</returns>
		/// <param name="SongID">Song I.</param>
		/// <param name="AudioData">Audio data.</param>
		public bool ChangeSongData(int SongID,AudioClipDefine.ChapterSongData AudioData)
		{
			if (_ChapterDic.ContainsKey (SongID)) 
			{
				_ChapterDic [SongID].CostTili = AudioData.CostTili;
				_ChapterDic [SongID].EndTime = AudioData.EndTime;
				_ChapterDic [SongID].IconID = AudioData.IconID;
				_ChapterDic [SongID].IconSpriteName = AudioData.IconSpriteName;
				_ChapterDic [SongID].IsLock = AudioData.IsLock;
				_ChapterDic [SongID].SongDiffcult = AudioData.SongDiffcult;
				_ChapterDic [SongID].SongID = AudioData.SongID;
				_ChapterDic [SongID].SongName = AudioData.SongName;
				_ChapterDic [SongID].SongText = AudioData.SongText;
				_ChapterDic [SongID].StartTime = AudioData.StartTime;
				_ChapterDic [SongID]._AudioClp = AudioData._AudioClp;
				_ChapterDic [SongID].SongIndex = AudioData.SongIndex;
				_ChapterDic [SongID].SongChapterID = AudioData.SongChapterID;
				return true;
			}
			return false;
		}
		/// <summary>
		/// 返回5首歌 参数为章节ID + 首歌INDEX
		/// </summary>
		/// <param name="ChapterID">Chapter I.</param>
		/// <param name="SongCenterIndex">Song center index.</param>
		public AudioClipDefine.ChapterSongData[] GetChapterSongs(int ChapterID,ref int SongStartIndex)
		{
			
			
			if (SongStartIndex == -1) {
				///重置顺序
				SongStartIndex = _ChapterList.Count - 1;
				//到达了歌曲的开头
				AudioClipDefine.ChapterSongData[] _ReturnData = new ChapterSongData[6];
				_ReturnData [0] = _ChapterList [SongStartIndex-2];
				_ReturnData [1] = _ChapterList [SongStartIndex-1];
				_ReturnData [2] = _ChapterList [SongStartIndex];
				_ReturnData [3] = _ChapterList [0];
				_ReturnData [4] = _ChapterList [1];
				_ReturnData [5] = _ChapterList [2];
				return _ReturnData;
			} 
			else if (SongStartIndex == _ChapterList.Count) 
			{
				SongStartIndex = 0;
				AudioClipDefine.ChapterSongData[] _ReturnData = new ChapterSongData[6];
				_ReturnData [0] = _ChapterList [_ChapterList.Count-2];
				_ReturnData [1] = _ChapterList [_ChapterList.Count-1];
				_ReturnData [2] = _ChapterList [0];
				_ReturnData [3] = _ChapterList [1];
				_ReturnData [4] = _ChapterList [2];
				_ReturnData [5] = _ChapterList [3];
				return _ReturnData;

			} else 
			{
				AudioClipDefine.ChapterSongData[] _ReturnData = new ChapterSongData[6];
				_ReturnData [2] = _ChapterList [SongStartIndex];
				///左边的CD
				if (SongStartIndex - 1 >= 0) 
				{
					_ReturnData [1] = _ChapterList [SongStartIndex - 1];
				} 
				else 
				{
					_ReturnData [1] = _ChapterList [_ChapterList.Count - 1];
				}

				if (SongStartIndex - 2 >= 0) 
				{
					_ReturnData [0] = _ChapterList [SongStartIndex - 2];
				} 
				else 
				{
					_ReturnData [0] = _ChapterList [_ChapterList.Count - 2];
				}
				///右边的CD
				if (SongStartIndex+1 < _ChapterList.Count) 
				{
					_ReturnData [3] = _ChapterList [SongStartIndex + 1];
				} 
				else 
				{
					_ReturnData [3] = _ChapterList [SongStartIndex + 1 - _ChapterList.Count];
				}
				///右边的CD
				if (SongStartIndex+2 < _ChapterList.Count) 
				{
					_ReturnData [4] = _ChapterList [SongStartIndex + 2];
				} 
				else 
				{
					_ReturnData [4] = _ChapterList [SongStartIndex + 2 - _ChapterList.Count];
				}

				///右边的CD
				if (SongStartIndex+3 < _ChapterList.Count) 
				{
					_ReturnData [5] = _ChapterList [SongStartIndex + 3];
				} 
				else 
				{
					_ReturnData [5] = _ChapterList [SongStartIndex + 3 - _ChapterList.Count];
				}
				LevelPrepaerPanel.NowLevel = _ReturnData [2].SongIndex;
				return _ReturnData;
			}
		}
		/// <summary>
		/// 返回历史的选择歌曲ID
		/// </summary>
		/// <returns>The hostry songs.</returns>
//		public int GetHostrySongID()
//		{
//			return UseSongID;
//		}

		public int GetHostySongIndex()
		{
			return LevelPrepaerPanel.NowLevel;
		}
		/// <summary>
		/// 获得历史的选择的章节的INDEX
		/// </summary>
		/// <returns>The hostry chapter index.</returns>
//		public int GetHostryChapterID()
//		{
//			return 1;
//		}
		/// <summary>
		/// 当前UI的
		/// </summary>
		//private int UseSongID=1;

//		public int GetUseSongID()
//		{
//			return LevelPrepaerPanel.NowLevel;
//		}

//		public void SetUseSongID(int ID)
//		{
//			UseSongID = ID;
//		}

		/// <summary>
		/// 弹出动画CD
		/// </summary>
		public void OpenUI()
		{
			if (_ui != null) {	
				_ui.GetComponent<LevelChangeChapter> ().OpenUI ();
				SoundEffectComponent.Instance.PauseByEffectType (GameGlobal.SOUND_TYPE_UI_BGM);
			}
		}
		/// <summary>
		/// 退出动画CD
		/// </summary>
		public void ExitUI()
		{
			if (_ui != null) 
			{
				_ui.GetComponent<LevelChangeChapter> ().ExitUI ();
				SoundEffectComponent.Instance.PlayByEffectType (GameGlobal.SOUND_TYPE_UI_BGM);
			}
		}
		/// <summary>
		/// 开启下一首歌曲
		/// </summary>
		/// <param name="SongID">Song I.</param>
		public void OpenNextSong()
		{
			if (_ui != null) 
			{
				_ui.GetComponent<LevelChangeChapter> ().OpenNextSong ();
			}
		}
		/// <summary>
		/// she zhi xuanzhe guanka de yingyue daxiao 
		/// </summary>
		/// <param name="value">Value.</param>
		public void SetAudioVolme(float value=0.5f)
		{
			if (_ui != null) 
			{
				_ui.GetComponent<LevelChangeChapter> ().SetVolume (value);
			}
		}
		/// <summary>
		/// 加载该章节的音乐歌曲
		/// </summary>
		public void LoadChapterAudio()
		{
			///清理历史卸载歌曲
			///
//			foreach (AudioClip T in _AudioClipDic.Values) 
//			{
//				T.UnloadAudioData ();
//			}
			//Thread _t = new Thread (LoadAudioThread);
			//_t.Start ();
			//LoadAudioThread ();

		}
		private Dictionary<string,AudioClip> _AudioClipDic = new Dictionary<string, AudioClip> ();
		public AudioClip GetAudioClip(string Path)
		{
			if (_AudioClipDic.ContainsKey (Path))
			{
				return _AudioClipDic [Path];
			}
			return null;
		}
		private void LoadAudioThread()
		{
//			for (int i = 0, max = _ChapterList.Count; i < max; i++) 
//			{
//				
//				if (_AudioClipDic.ContainsKey (_ChapterList [i]._AudioClp)) 
//				{
//					Debug.Log ("歌曲="+_ChapterList[i]._AudioClp+"已经存在");
//				} else 
//				{
//					AudioClip _Clip = Resources.Load<AudioClip> (_ChapterList [i]._AudioClp);
//					_AudioClipDic.Add (_ChapterList [i]._AudioClp, _Clip);
//				}
//			}
		}

		public AudioClip GetChapterAudio(string Path)
		{
			if (_AudioClipDic.ContainsKey (Path)) 
			{
				return _AudioClipDic[Path];

			} else 
			{
				return null;
			}
		}
	}
}
