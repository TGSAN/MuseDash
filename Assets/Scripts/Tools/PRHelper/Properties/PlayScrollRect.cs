using System;
using System.Collections.Generic;
using System.Linq;
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
        public GameObject scrollSpace;
        public float midX;
        public float changeDistance;
        public float speedThreshold;
        public float moveBackSpeed;
        public Ease curve;
        public float moveLeftRightSpeed;
        public Ease moveLeftRightCurve;
        public GameObject btnLeft, btnRight;
        public GameObject prefab;
        public int count;

        private List<Transform> m_Children = new List<Transform>();
        private bool m_IsDragging = false;
        private float m_XOffset = 0;
        private Tweener m_MoveTwner;
        private Tweener m_ScrollTwner;

        public void Play(GameObject go)
        {
            var width = prefab.GetComponent<RectTransform>().rect.width;
            var height = prefab.GetComponent<RectTransform>().rect.height;

            var layoutGO = new GameObject("Content");
            layoutGO.transform.SetParent(scrollSpace.transform, false);
            var layoutRT = layoutGO.AddComponent<RectTransform>();
            layoutRT.sizeDelta = new Vector2(width * (count + 2), height);
            layoutRT.localPosition = new Vector3(layoutRT.sizeDelta.x / 2, 0.0f, 0.0f);

            var layoutGroup = layoutGO.AddComponent<HorizontalLayoutGroup>();
            layoutGroup.padding = new RectOffset((int)width, (int)width, 0, 0);
            layoutGroup.childControlWidth = false;
            layoutGroup.childControlHeight = false;
            layoutGroup.childAlignment = TextAnchor.MiddleLeft;
            for (int i = 0; i < count; i++)
            {
                var p = UnityEngine.Object.Instantiate(prefab, layoutGroup.transform).transform;
                p.localScale = Vector3.one;
                m_Children.Add(p);
            }

            var scrollRect = scrollSpace.AddComponent<ScrollRect>();
            scrollRect.content = layoutRT;
            scrollRect.viewport = scrollRect.gameObject.GetComponent<RectTransform>();
            scrollRect.vertical = false;
            scrollRect.onValueChanged.AddListener(offset =>
            {
                var speed = Mathf.Abs(offset.x - m_XOffset);
                m_XOffset = offset.x;
                var curX = scrollRect.content.transform.localPosition.x;
                if (speed < speedThreshold && m_IsDragging == false && curX <= Mathf.Abs((count - 2) * width) / 2f)
                {
                    scrollRect.StopMovement();
                    var targetX = Mathf.RoundToInt(curX / width) * width;
                    if (m_ScrollTwner == null)
                    {
                        var time = Mathf.Abs(curX - targetX) / moveBackSpeed;

                        m_ScrollTwner = scrollRect.content.transform.DOLocalMoveX(targetX, time).SetEase(curve).OnUpdate(
                            () =>
                            {
                                if (Vector3.Magnitude(scrollRect.velocity) > 0.0f)
                                {
                                    m_ScrollTwner.Kill();
                                    m_ScrollTwner = null;
                                }
                            });
                    }
                }
                m_Children.ToList().ForEach(item =>
                {
                    item.transform.localScale = Mathf.Lerp(maxScale, minScale, Mathf.Abs(item.transform.position.x - midX) / changeDistance) * Vector3.one;
                });
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

            Action<bool> page = isRight =>
            {
                {
                    if (m_MoveTwner != null)
                    {
                        m_MoveTwner.Kill();
                    }
                    var targetX = Mathf.FloorToInt((scrollRect.content.transform.localPosition.x - 1 + width / 2) / width) * width *
                                  1f - width / 2;
                    var moveLeftRightTime = Mathf.Abs(targetX - scrollRect.content.transform.localPosition.x) /
                                            moveLeftRightSpeed;
                    m_MoveTwner = isRight
                        ? scrollRect.content.transform.DOLocalMoveX(
                            Mathf.FloorToInt((scrollRect.content.transform.localPosition.x - 1 + width / 2) / width) * width * 1f -
                            width / 2, moveLeftRightTime).SetEase(moveLeftRightCurve)
                        : scrollRect.content.transform.DOLocalMoveX(
                            Mathf.CeilToInt((scrollRect.content.transform.localPosition.x + 1 + width / 2) / width) * width * 1f -
                            width / 2, moveLeftRightTime).SetEase(moveLeftRightCurve);
                }
            };
            UIEventUtils.OnEvent(btnLeft, EventTriggerType.PointerClick, eventData =>
            {
                page(false);
            });

            UIEventUtils.OnEvent(btnRight, EventTriggerType.PointerClick, eventData =>
            {
                page(true);
            });
        }
    }
}