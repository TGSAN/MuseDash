using UnityEngine;
using System;
[RequireComponent(typeof(AudioSource))]
public class SpectrumAnalyzer : MonoBehaviour 
{
	public int resolution = 1024;
	public float lowFreqThreshold = 14700;
	public float lowEnhance = 1f;
	
	public  AudioSource audio_;
	public  string _SoureClip=null;
	public  float     _StarSample=10;
	public  float     _EndTSample=20;
	private bool _Play=false;
	public  GameObject _LeftUI=null;
	public GameObject  _RightUI=null;

	public float _MinSize=1.0f;
	public float _ChangeValue=50f;

	public LevelAudioQuan _QuanCs=null;

	void Start() {
		//InitAudioSoure (audio_);
		//PlayAudio ();
	}
	
	void Update()  {
		if (_Play)
			LogicAudio ();
	}

	public void LogicAudio() {
		float low = 0f;
		var spectrum = audio_.GetSpectrumData(resolution, 0, FFTWindow.Triangle);
		var deltaFreq = AudioSettings.outputSampleRate / resolution;
		for (var i = 0; i < resolution; ++i) {
			var freq = deltaFreq * i;
			if (freq <= lowFreqThreshold)  
				low  += spectrum[i];
		}

		low  *= lowEnhance;
		float _HandleValue = GetReturnLow (low);
		gameObject.transform.localScale = _HandleValue * Vector3.one;
		_LeftUI.transform.localScale = (0.9f + 0.3f * (low / (low + 2f))) * Vector3.one;
		_RightUI.transform.localScale = (0.9f + 0.3f * (low / (low + 2f))) * Vector3.one;

		if (_QuanCs != null) {
			_QuanCs.SetAudioValue (low);
		}
		//TweenScale.Begin (gameObject, _TweenTime,GetReturnLow(low)*Vector3.one);
		//_CDtime = _TweenTime;
	}

	public float GetReturnLow(float _value) {
		float RetrunValue = _MinSize + (_value / (_value + _ChangeValue));
		return RetrunValue;
	}

	public void InitAudioSoure(float EndTSample,float StarSample,string Soure) {
		if (audio_ != null) {
			_SoureClip = Soure;
			_EndTSample = EndTSample;
			_StarSample = StarSample;
//			int KeepSample = (int)((_EndTSample-_StarSample)*_SoureClip.frequency);
//			AudioClip _CreateSoure = AudioClip.Create ("Use", KeepSample, _SoureClip.channels, _SoureClip.frequency,false);
//			float[] samples = new float[_CreateSoure.samples*_SoureClip.channels];
//			_SoureClip.GetData(samples, (int)(_StarSample*_SoureClip.frequency*_SoureClip.channels));
//			_CreateSoure.SetData (samples, 0);
//			if (audio_.clip != null) {
//				audio_.clip.UnloadAudioData ();
//				audio_.clip = null;
//			}
			//Resources.UnloadUnusedAssets ();
		}
	}

	public void PlayAudio() {
		if (audio_ != null) {
			AudioClip SoureClip = Resources.Load<AudioClip> (_SoureClip);
			audio_.clip = SoureClip;
			audio_.Play ();
			_Play = true;
		}
	}

	public void StopAudio() {
		if (audio_ != null) {
			_Play = false;
	
			audio_.Stop ();
		}
	}

	/// <summary>
	/// 设置音乐大小
	/// </summary>
	/// <param name="value">Value.</param>
	public void Setvolume(float value) {
		if (audio_ != null) {
			audio_.volume = value;
		}
	}
}