using DG.Tweening;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class UIScrollViewDragBack : MonoBehaviour
    {
        public UIScrollView scrollView;
        public UIPanel panel;
        private Vector2 m_OriginOffset;
        public float limitOffset;
        public bool isVertical = true;
        public float duration = 1.0f;
        public AnimationCurve curve;
        public UIScrollView.DragEffect originDragEffect;

        private void Awake()
        {
            scrollView = GetComponent<UIScrollView>();
            panel = GetComponent<UIPanel>();
            originDragEffect = scrollView.dragEffect;
            m_OriginOffset = scrollView.transform.position;
            scrollView.onDragFinished += () =>
            {
                if (isVertical)
                {
                    if (scrollView.transform.position.y <= limitOffset)
                    {
                        scrollView.dragEffect = UIScrollView.DragEffect.Momentum;
                        scrollView.transform.DOMoveY(m_OriginOffset.y, duration).SetEase(curve);
                        DOTween.To(() => panel.clipOffset, x => panel.clipOffset = x, m_OriginOffset, duration)
                            .SetEase(curve);
                    }
                    else
                    {
                        scrollView.dragEffect = originDragEffect;
                    }
                }
                else
                {
                    if (scrollView.transform.position.x <= limitOffset)
                    {
                        scrollView.dragEffect = UIScrollView.DragEffect.Momentum;
                        scrollView.DisableSpring();
                        scrollView.transform.DOMoveX(m_OriginOffset.x, duration).SetEase(curve);
                        DOTween.To(() => panel.clipOffset, x => panel.clipOffset = x, m_OriginOffset, duration)
                            .SetEase(curve);
                    }
                    else
                    {
                        scrollView.dragEffect = originDragEffect;
                    }
                }
            };
        }

        // Use this for initialization
        private void Start()
        {
        }
    }
}