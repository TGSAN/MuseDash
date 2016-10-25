using DG.Tweening;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class UIScrollViewDragBack : MonoBehaviour
    {
        public UIScrollView scrollView;
        public UIPanel panel;
        private Vector2 m_OriginOffset;
        public bool isVertical = true;
        public float duration = 1.0f;
        public AnimationCurve curve;
        public UIScrollView.DragEffect originDragEffect;
        private Tweener m_MoveTwner1, m_MoveTwner2;
        public bool isEnable = true;

        public void ResetBack(Vector2 offset)
        {
            scrollView.transform.position = offset;
            panel.clipOffset = offset;
        }

        private void Awake()
        {
            if (isEnable)
            {
                scrollView = GetComponent<UIScrollView>();
                panel = GetComponent<UIPanel>();
                originDragEffect = scrollView.dragEffect;
                m_OriginOffset = scrollView.transform.position;
                scrollView.onDragFinished += OnDragEvent;
                scrollView.onStoppedMoving += OnDragEvent;
                scrollView.onDragStarted += OnDragStart;
            }
        }

        private void OnDragStart()
        {
            if (m_MoveTwner1 != null)
            {
                m_MoveTwner1.Kill();
            }

            if (m_MoveTwner2 != null)
            {
                m_MoveTwner2.Kill();
            }
        }

        private void OnDragEvent()
        {
            if (isVertical)
            {
                if (scrollView.transform.position.y <= panel.GetViewSize().y)
                {
                    scrollView.dragEffect = UIScrollView.DragEffect.Momentum;
                    m_MoveTwner1 = scrollView.transform.DOMoveY(m_OriginOffset.y, duration).SetEase(curve);
                    m_MoveTwner2 = DOTween.To(() => panel.clipOffset, x => panel.clipOffset = x, m_OriginOffset, duration)
                        .SetEase(curve);
                }
                else
                {
                    scrollView.dragEffect = originDragEffect;
                }
            }
            else
            {
                if (scrollView.transform.position.x <= panel.GetViewSize().x)
                {
                    scrollView.dragEffect = UIScrollView.DragEffect.Momentum;
                    m_MoveTwner1 = scrollView.transform.DOMoveX(m_OriginOffset.x, duration).SetEase(curve);
                    m_MoveTwner2 = DOTween.To(() => panel.clipOffset, x => panel.clipOffset = x, m_OriginOffset, duration)
                        .SetEase(curve);
                }
                else
                {
                    scrollView.dragEffect = originDragEffect;
                }
            }
        }
    }
}