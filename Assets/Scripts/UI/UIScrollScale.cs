using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.UI;
using DG.Tweening;

public class UIScrollScale : MonoBehaviour
{
    //public UIScrollRect scrollRect;
    public float maxScale, minScale;

    public Transform parent;
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

    private void Awake()
    {
        m_Childs = new Transform[parent.childCount];
        for (int i = 0; i < parent.childCount; i++)
        {
            m_Childs[i] = parent.GetChild(i);
        }
        //return;
        /*scrollRect.onValueChanged.AddListener (offset => {
			var speed = Mathf.Abs(offset.x - m_XOffset);
			m_XOffset = offset.x;
			if (speed < speedtThreshold && m_IsDragging == false) {
				scrollRect.StopMovement();
				var targetX = Mathf.RoundToInt((scrollRect.content.transform.localPosition.x + gap / 2) / gap) * gap - gap / 2;

				if (m_ScrollTwner == null) {
					m_ScrollTwner = scrollRect.content.transform.DOLocalMoveX(targetX, curveTime).SetEase(curve);
				}
			}
		});
		scrollRect.onEndDrag += eventData =>
		{
			m_IsDragging = false;
			if (m_ScrollTwner != null) {
				m_ScrollTwner.Kill();
				m_ScrollTwner = null;
			}
		};
		scrollRect.onBeginDrag += eventData =>
		{
			m_IsDragging = true;
			if (m_ScrollTwner != null) {
				m_ScrollTwner.Kill();
				m_ScrollTwner = null;
			}
		};*/
    }

    private void Update()
    {
        foreach (var item in m_Childs)
        {
            item.transform.localScale = Mathf.Lerp(maxScale, minScale, Mathf.Abs(item.transform.position.x - midX) / changeDistance) * Vector3.one;
        }
    }

    public void MoveNext(bool isRight)
    {
        /*if (m_MoveTwner != null)
        {
            m_MoveTwner.Kill();
        }
        if (isRight) {
			m_MoveTwner = scrollRect.content.transform.DOLocalMoveX (Mathf.FloorToInt ((scrollRect.content.transform.localPosition.x - 1 + gap / 2) / gap) * gap * 1f - gap / 2, moveLeftRightTime).SetEase (moveLeftRightCurve);
		} else {
			m_MoveTwner = scrollRect.content.transform.DOLocalMoveX (Mathf.CeilToInt ((scrollRect.content.transform.localPosition.x + 1 + gap / 2) / gap) * gap * 1f - gap / 2, moveLeftRightTime).SetEase (moveLeftRightCurve);
		}*/
    }
}