using Assets.Scripts.NGUI;
using FormulaBase;
using GameLogic;

/// UI分析工具自动生成代码
/// PnlStageUI主模块
///
using System;
using System.Collections;
using UnityEngine;

namespace PnlStage
{
	public class PnlStage : UIPhaseBase
	{
		private static PnlStage instance = null;

		public static PnlStage Instance
		{
			get
			{
				return instance;
			}
		}

		private Coroutine _loadMusicCoroutine;
		public AudioSource diskAudioSource;

		private void Start()
		{
			instance = this;
		}

		public override void OnShow()
		{
			StageDisc.StageDisc.LoadAllDiscCover();
			this.OnSongChanged(PnlScrollCircle.currentSongIdx);
			SceneAudioManager.Instance.bgm.clip = null;
			PnlScrollCircle.instance.ResetPos();
			PnlScrollCircle.instance.JumpToSong(PnlScrollCircle.currentSongIdx);
		}

		public override void OnHide()
		{
			//print ("rinima");
		}

		public void OnSongChanged(int idx)
		{
			Debug.Log("Stage selected " + idx);
			if (idx <= 0)
			{
				Debug.Log("Unlaw stage : " + idx);
				return;
			}

			int len = ConfigPool.Instance.GetConfigLenght("stage");
			if (idx >= len)
			{
				Debug.Log("Unlaw stage : " + idx);
				return;
			}

			StageBattleComponent.Instance.InitById(idx);
			StageBattleComponent.Instance.Host.SetAsUINotifyInstance();
			TaskStageTarget.Instance.Host.SetAsUINotifyInstance();
			/*
			SceneAudioManager.Instance.bgm.clip = null;
			PnlScrollCircle.instance.ResetPos();
			PnlScrollCircle.instance.JumpToSong(idx);
			if (this.diskAudioSource == null) {
				this.diskAudioSource = SceneAudioManager.Instance.bgm;
			}

			if (this.diskAudioSource == null) {
				return;
			}

			AudioClip oldClip = this.diskAudioSource.clip;
			this.diskAudioSource.clip = null;
			if (oldClip != null) {
				Resources.UnloadAsset (oldClip);
			}

			if (this._loadMusicCoroutine != null) {
				ResourceLoader.Instance.StopCoroutine (this._loadMusicCoroutine);
				this._loadMusicCoroutine = null;
			}

			//StageDisc.StageDisc.OnSongChanged (idx);

			string musicPath = ConfigPool.Instance.GetConfigStringValue ("stage", idx.ToString (), "FileName_1");
			this._loadMusicCoroutine = ResourceLoader.Instance.Load (musicPath, this.__OnLoadMusic, ResourceLoader.RES_FROM_LOCAL);*/
		}

		/*private void __OnLoadMusic(UnityEngine.Object musicRes) {
			AudioClip clip = musicRes as AudioClip;
			this.diskAudioSource.loop = true;
			this.diskAudioSource.clip = clip;
			this.diskAudioSource.Play ();
		}*/
	}
}