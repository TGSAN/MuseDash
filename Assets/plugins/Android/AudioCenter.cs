using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class AudioCenter : MonoBehaviour {
	private static AudioCenter instance;
	private AudioSource audioSource;

	#if UNITY_ANDROID && !UNITY_EDITOR
		public static AndroidJavaClass unityActivityClass ;
		public static AndroidJavaObject activityObj ;
		private static AndroidJavaObject soundObj ;
		
		public static void playSound( int soundId ) {
			soundObj.Call( "playSound", new object[] { soundId } );
		}
		
		public static void playSound( int soundId, float volume ) {
			soundObj.Call( "playSound", new object[] { soundId, volume } );
		}
		
		public static void playSound( int soundId, float leftVolume, float rightVolume, int priority, int loop, float rate  ) {
			soundObj.Call( "playSound", new object[] { soundId, leftVolume, rightVolume, priority, loop, rate } );
		}
		
		public static int loadSound( string soundName ) {
			return soundObj.Call<int>( "loadSound", new object[] { "Resources/Sounds/" +  soundName + ".wav" } );
		}
		
		public static void unloadSound( int soundId ) {
			soundObj.Call( "unloadSound", new object[] { soundId } );
		}
	#else
		private Dictionary<int, AudioClip> audioDic = new Dictionary<int, AudioClip>();
		
		public static void playSound( int soundId ) {
			//AudioCenter.instance.audioSource.clip = AudioCenter.instance.audioDic[soundId];
			AudioCenter.instance.audioSource.PlayOneShot(AudioCenter.instance.audioDic[soundId]);
		}

		public static void playSound( int soundId, float volume ) {
			AudioCenter.instance.audioSource.PlayOneShot(AudioCenter.instance.audioDic[soundId], volume);
		}

		public static void playSound( int soundId, float leftVolume, float rightVolume, int priority, int loop, float rate ) {
			//float panRatio = AudioCenter.instance.audioSource.panStereo;
			//rightVolume = Mathf.Clamp(rightVolume, 0, 1);
			//leftVolume = Mathf.Clamp(leftVolume, 0, 1);
			//AudioCenter.instance.audioSource.panStereo = Mathf.Clamp(rightVolume, 0, 1) - Mathf.Clamp(leftVolume, 0, 1);
			float volume = (leftVolume + rightVolume) / 2;
			AudioCenter.instance.audioSource.PlayOneShot(AudioCenter.instance.audioDic[soundId], volume);
			//AudioCenter.instance.audioSource.panStereo = panRatio;
		}
		
		public static int loadSound( string soundName ) {
			var soundID = soundName.GetHashCode();
			var audioClip = Resources.Load<AudioClip>("Sounds/" + soundName);
			AudioCenter.instance.audioDic[soundID] = audioClip;
			
			return soundID;
		}
		
		public static void unloadSound( int soundId ) {
			var audioClip = AudioCenter.instance.audioDic[soundId];
			Resources.UnloadAsset(audioClip);
			AudioCenter.instance.audioDic.Remove(soundId);
		}
	#endif

	private void Awake() {
		if (instance == null || instance == this) {
			instance = this;
		} else {
			Destroy(this);
			return;
		}
		
		#if !UNITY_ANDROID || UNITY_EDITOR
			audioSource = gameObject.AddComponent<AudioSource>();
			audioSource.hideFlags = HideFlags.HideInInspector;
		#else
			unityActivityClass =  new AndroidJavaClass( "com.unity3d.player.UnityPlayer" );
			activityObj = unityActivityClass.GetStatic<AndroidJavaObject>( "currentActivity" );
			//soundObj = new AndroidJavaObject( "com.catsknead.androidsoundfix.AudioCenter", 1, activityObj, activityObj );
			soundObj = new AndroidJavaObject( "com.catsknead.androidsoundfix.AudioCenter", 5, activityObj );
		#endif
	}
}
