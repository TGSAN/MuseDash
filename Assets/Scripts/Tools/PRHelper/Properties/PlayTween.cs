using System;
using UnityEngine;

namespace Assets.Scripts.Tools.PRHelper.Properties
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