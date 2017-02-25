using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.NewUI;
using DG.Tweening;
using Smart.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Tools.PRHelper.Properties
{
    [Serializable]
    public class PlayPopup
    {
        public string pnlName;

        public float inTime = 0.4f; //����볡��ʱ����
        public float inDistance = 200; //����볡λ�ƾ��롣
        public Ease moveInEase = Ease.OutElastic; //�����������͡�
        public bool isFadeIn = true; //�Ƿ������롣
        public Ease fadeInEase = Ease.Linear; //����������ߡ�

        public float outTime = 0.4f; //��������ʱ����
        public float outDistance = 200; //������λ�ƾ��롣
        public Ease moveOutEase = Ease.InExpo; //�����������͡�
        public bool isFadeOut = true; //�Ƿ���������
        public Ease fadeOutEase = Ease.Linear; //�����������ߡ�

        public Color color; //Mask����ɫ��͸���ȡ�
        public bool shut = true; //���Mask�����Ƿ��ܹر���塣
        public string shutButtonName; //�رհ�ťָ����

        private Vector3 m_OriginPos = Vector3.zero;
        private bool m_Flag = false;

        public void Play(GameObject go)
        {
            var gameObject = UIManager.instance[pnlName];
            gameObject.SetActive(true);
            DOTween.Kill(gameObject, true);
            if (!m_Flag)
            {
                m_Flag = true;
                m_OriginPos = gameObject.transform.localPosition;
            }
            gameObject.transform.localPosition = m_OriginPos;
            var btnCancellGO = new GameObject("BtnCancel");
            var rectTransform = btnCancellGO.AddComponent<RectTransform>();

            var parent = gameObject.transform.parent;
            btnCancellGO.transform.localScale = new Vector3(1, 1, 1);
            rectTransform.sizeDelta = UIManager.instance.gameObject.GetComponent<RectTransform>().sizeDelta;
            btnCancellGO.transform.SetParent(parent.transform, false);
            btnCancellGO.transform.SetSiblingIndex(0);

            var texBkg = new Texture2D(1, 1);
            texBkg.SetPixel(0, 0, color);
            texBkg.Apply();
            var image = btnCancellGO.AddComponent<Image>();
            var rect = new Rect(0, 0, texBkg.width, texBkg.height);
            var sprite = Sprite.Create(texBkg, rect, new Vector2(0.5f, 0.5f), 1);
            image.material.mainTexture = texBkg;
            image.sprite = sprite;
            image.color = color;
            image.canvasRenderer.SetAlpha(0.0f);
            image.CrossFadeAlpha(1.0f, 0.1f, false);

            var boards = gameObject.GetComponentsInChildren<Image>(); //��ȡ���������������ϵ�Image�����
            var txts = gameObject.GetComponentsInChildren<Text>(); //��ȡ���������������ϵ�Text�����

            //�ӳ�0.15���ƶ�������
            gameObject.transform.DOLocalMoveY(inDistance, inTime).From().SetEase(moveInEase).SetDelay(0.15f);
            if (isFadeIn)
            {
                boards.ToList().ForEach(b => b.DOFade(0, 0.1f).From().SetDelay(0.15f).SetEase(fadeInEase)); //
                txts.ToList().ForEach(t => t.DOFade(0, 0.1f).From().SetDelay(0.15f).SetEase(fadeInEase));
            }

            var btnCancell = btnCancellGO.AddComponent<Button>();

            // ����������
            UnityAction clickEvent = () =>
            {
                gameObject.transform.DOLocalMoveY(outDistance, outTime).SetEase(moveOutEase).SetDelay(0.15f);
                if (isFadeOut)
                {
                    boards.ToList().ForEach(b => b.DOFade(0, 0.05f).SetEase(fadeOutEase));
                    txts.ToList().ForEach(t => t.DOFade(0, 0.05f).SetEase(fadeOutEase));
                }
                image.DOFade(0, 0.2f).SetDelay(0.05f).OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    if (btnCancell)
                    {
                        Object.Destroy(btnCancell.gameObject);
                    }

                    boards.ToList().ForEach(b =>
                    {
                        b.color = new Color(b.color.r, b.color.g, b.color.b, 1.0f);
                    });
                    txts.ToList().ForEach(t =>
                    {
                        t.color = new Color(t.color.r, t.color.g, t.color.b, 1.0f);
                    });
                });
            };

            //�� Shut ���ر���ѡʱ������ͨ������հ��������ر���塣
            if (shut)
            {
                if (btnCancell)
                {
                    btnCancell.onClick.AddListener(clickEvent);
                }
            }
            // ��������еİ�ť����Ϊָ��Ϊ Shut Button ��Ŀ����ӹر�����¼���
            var btn = gameObject.GetComponentsInChildren<Button>().ToList().Find(b => b.gameObject.name == shutButtonName);
            if (btn)
            {
                btn.onClick.AddListener(clickEvent);
            }
        }
    }
}