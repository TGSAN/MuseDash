using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EasyEditor;

namespace EasyEditor
{
    public class SoundHandler : MonoBehaviour {

        [Image]
        public string easyEditorImage = "Assets/EasyEditor/Examples/icon.png";
        [Space(20f)]

    	public string ID = "";
    	public List<AudioSource> audioSources;

    	[Inspector(group = "Sound Basic Parameters")]
    	[BeginHorizontal]
    	public float fadeInTime = 0f;
    	[EndHorizontal]
    	public float fadeOutTime = 0f;
    	[Range(0f,1f)]
    	public float volume = 1f;
    	public bool looping = false;
    	public float delayAtStart = 0f;
    	public bool modifyPitch = false;
    	[BeginHorizontal]
    	[Visibility("modifyPitch", true)]
    	public float minPitch = 1f;
    	[EndHorizontal]
    	[Visibility("modifyPitch", true)]
    	public float maxPitch = 1f;
    }
}
