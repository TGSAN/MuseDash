using System;
using DG.Tweening;
using UnityEngine;

namespace Assets.Scripts.Tool.PRHelper.Properties
{
    [Serializable]
    public class PlayTween
    {
        public TweenType tweenType;

        public void Play()
        {
            Debug.Log("Play Tween");
        }

        public enum TweenType
        {
            None,
            Move,
            Rotate,
            Scale,
            Color,
            Fade,
            UIWidthHeight,
        }
    }
}