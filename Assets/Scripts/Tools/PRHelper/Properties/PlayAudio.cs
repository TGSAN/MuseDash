using System;
using UnityEngine;

namespace Assets.Scripts.Tools.PRHelper.Properties
{
    [Serializable]
    public class PlayAudio
    {
        public AudioClip audioClip;
        public bool isPlay;

        public void Play(GameObject go)
        {
            Debug.Log("Play Audio");
            //doTweenAnimation.DOPlay();
        }
    }
}