using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameLogic;

public class SceneAudioManager : MonoBehaviour {
	public static Dictionary<string, AudioSource> defaultTypeSource = null;

	private Dictionary<string, AudioClip> buffClips = null;
	
	[SerializeField]
	public AudioSource ferver;
	public AudioSource enemy;
	public AudioSource role;
	public AudioSource boss;
	public AudioSource ui;
	public AudioSource bgm;

	private static SceneAudioManager instance = null;
	public static SceneAudioManager Instance {
		get {
			return instance;
		}
	}

	// Use this for initialization
	void Start () {
		instance = this;

		if (defaultTypeSource != null) {
			defaultTypeSource.Clear ();
			defaultTypeSource = null;
		}

		defaultTypeSource = new Dictionary<string, AudioSource> ();
		defaultTypeSource [GameGlobal.SOUND_TYPE_HURT] = this.role;
		defaultTypeSource [GameGlobal.SOUND_TYPE_FEVER] = this.ferver;
		defaultTypeSource [GameGlobal.SOUND_TYPE_MAIN_BOARD_TOUCH] = this.role;
		defaultTypeSource [GameGlobal.SOUND_TYPE_ENTER_STAGE] = this.ui;
		defaultTypeSource [GameGlobal.SOUND_TYPE_STAGE_START] = this.ferver;
		defaultTypeSource [GameGlobal.SOUND_TYPE_LAST_NODE] = this.ui;
		defaultTypeSource [GameGlobal.SOUND_TYPE_DEAD] = this.role;
		defaultTypeSource [GameGlobal.SOUND_TYPE_ON_EQUIP] = this.ui;
		defaultTypeSource [GameGlobal.SOUND_TYPE_ON_EXP] = this.ui;
		defaultTypeSource [GameGlobal.SOUND_TYPE_ON_CHEST] = this.ui;
		defaultTypeSource [GameGlobal.SOUND_TYPE_ON_CHEST_OPEN] = this.ui;
		defaultTypeSource [GameGlobal.SOUND_TYPE_ON_TEN_COMBO] = this.ui;
		defaultTypeSource [GameGlobal.SOUND_TYPE_UI] = this.ui;
		defaultTypeSource [GameGlobal.SOUND_TYPE_UI_BGM] = this.bgm;
		defaultTypeSource [GameGlobal.SOUND_TYPE_UI_ATTACK_MISS] = this.role;
		defaultTypeSource [GameGlobal.SOUND_TYPE_UI_JUMP_MISS] = this.role;
	}

	void OnDestory() {
		if (this.buffClips == null) {
			return;
		}

		this.buffClips.Clear ();
		this.buffClips = null;
		defaultTypeSource.Clear ();
		defaultTypeSource = null;
	}

	/// <summary>
	/// Play the specified pathname and player.
	/// 
	/// 播放各种音效
	/// player SceneAudioSource里面挂的Audio Source对象
	/// </summary>
	/// <param name="pathname">Pathname.</param>
	/// <param name="player">Player.</param>
	public void Play(string pathname, string effectType, AudioSource player = null) {
		if (pathname == null) {
			return;
		}

		if (player == null) {
			player = defaultTypeSource [effectType];
		}

		if (this.buffClips == null) {
			this.buffClips = new Dictionary<string, AudioClip> ();
		}

		if (this.buffClips.ContainsKey (pathname)) {
			AudioClip ac = this.buffClips [pathname];
			if (ac != null) {
				player.enabled = true;
				player.clip = ac;
				player.time = 0;
				player.Play ();
				Debug.Log (player.name + " : " + player.clip.name);
				return;
			}
		}

		AudioClip _ac = (AudioClip)Resources.Load (pathname);
		if (_ac == null) {
			Debug.Log ("No audio source " + pathname);
			return;
		}

		this.buffClips [pathname] = _ac;
		player.enabled = true;
		player.clip = _ac;
		player.time = 0;
		player.Play ();
		Debug.Log (player.name + " : " + player.clip.name);
	}

	public void Play(AudioSource player) {
		if (player == null) {
			return;
		}

		if (player.clip == null) {
			return;
		}

		player.enabled = true;
		player.Play ();
	}

	public void Load(string pathName, AudioSource ac) {
		this.StartCoroutine (this.__Load (pathName, ac));
	}

	private IEnumerator __Load(string pathName, AudioSource ac) {
		WWW streamAudio = new WWW (pathName);
		yield return streamAudio;

		ac.clip = streamAudio.GetAudioClipCompressed (true, AudioType.OGGVORBIS);
	}
}
