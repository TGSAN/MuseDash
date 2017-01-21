using System;
using Smart.Types;
using UnityEngine;

namespace Assets.Scripts.Tool.PRHelper.Properties
{
    [Serializable]
    public class PlayAnimation
    {
        public enum AnimtionType
        {
            Forward,
            Reverse,
            PingPong,
        }

        public enum ActionAfterFinish
        {
            KeepCurrent,
            Resume,
        }

        public Animator animator;

        public AnimtionType animationType;
        public ActionAfterFinish afterAction;
        public string stateName;
        public bool hasStart = false, hasPlaying = false, hasFinish = false;
        public UnityEventGameObject onAnimationStart, onAnimationPlaying, onAnimationFinish;

        public void Play()
        {
            if (animator != null && !string.IsNullOrEmpty(stateName))
            {
                animator.Play(stateName);
            }
            if (afterAction == ActionAfterFinish.Resume)
            {
                //animator.
            }
        }
    }
}