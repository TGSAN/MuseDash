using System;
using Assets.Scripts.Common;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.Tools.PRHelper.Properties
{
    [Serializable]
    public class PlayScrollRect
    {
        public float maxScale, minScale;
        public Transform parent;
        public GameObject scrollSpace;
        public float midX;
        public float changeDistance;
        public float speedtThreshold;
        public float gap;
        public float curveTime;
        public Ease curve;
        public float moveLeftRightTime;
        public Ease moveLeftRightCurve;

        private Transform[] m_Childs;
        private bool m_IsDragging = false;
        private float m_XOffset = 0;
        private Tweener m_MoveTwner;
        private Tweener m_ScrollTwner;

        public void Play(GameObject go)
        {
            m_Childs = new Transform[parent.childCount];
            for (int i = 0; i < parent.childCount; i++)
            {
                m_Childs[i] = parent.GetChild(i);
            }

            var scrollRect = scrollSpace.AddComponent<ScrollRect>();
            scrollRect.onValueChanged.AddListener(offset =>
            {
                Debug.Log("=========1");
                var speed = Mathf.Abs(offset.x - m_XOffset);
                m_XOffset = offset.x;
                if (speed < speedtThreshold && m_IsDragging == false)
                {
                    scrollRect.StopMovement();
                    var targetX = Mathf.RoundToInt((scrollRect.content.transform.localPosition.x + gap / 2) / gap) * gap - gap / 2;

                    if (m_ScrollTwner == null)
                    {
                        m_ScrollTwner = scrollRect.content.transform.DOLocalMoveX(targetX, curveTime).SetEase(curve);
                    }
                }
            });

            UIEventUtils.OnEvent(scrollSpace, EventTriggerType.BeginDrag, eventData =>
            {
                m_IsDragging = true;
                if (m_ScrollTwner != null)
                {
                    m_ScrollTwner.Kill();
                    m_ScrollTwner = null;
                }
            });

            UIEventUtils.OnEvent(scrollSpace, EventTriggerType.EndDrag, eventData =>
            {
                m_IsDragging = false;
                if (m_ScrollTwner != null)
                {
                    m_ScrollTwner.Kill();
                    m_ScrollTwner = null;
                }
            });
        }
    }
}