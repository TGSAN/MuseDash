using System;
using UnityEngine;

namespace Assets.Scripts.Tool.PRHelper.Properties
{
    [Serializable]
    public class PlayAudio
    {
        public AudioClip audioClip;
        public bool isPlay;

        public void Play()
        {
            Debug.Log("Play Audio");
            //doTweenAnimation.DOPlay();
        }
    }
}