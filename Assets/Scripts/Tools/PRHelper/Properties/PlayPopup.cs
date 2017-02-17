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

        public float inTime;
        public float inDistance;
        public Ease moveInEase;
        public bool isFadeIn;
        public Ease fadeInEase;

        public float outTime;
        public float outDistance;
        public Ease moveOutEase;
        public bool isFadeOut;
        public Ease fadeOutEase;

        public Color color;
        public bool shut;
        public string shutButtonName;

        public void Play(GameObject go)
        {
            var gameObject = UIManager.instance[pnlName];
            gameObject.SetActive(true);
            var originPos = gameObject.transform.localPosition;
            var btnCancellGO = new GameObject("BtnCancel");
            var rectTransform = btnCancellGO.AddComponent<RectTransform>();

            var parent = UIManager.instance.gameObject;
            btnCancellGO.transform.localScale = new Vector3(1, 1, 1);
            rectTransform.sizeDelta = parent.GetComponent<RectTransform>().sizeDelta;
            btnCancellGO.transform.SetParent(parent.transform, false);
            btnCancellGO.transform.SetSiblingIndex(parent.transform.GetSiblingIndex());

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

            var boards = gameObject.GetComponentsInChildren<Image>();
            var txts = gameObject.GetComponentsInChildren<Text>();
            //延迟0.15秒移动、淡入
            gameObject.transform.DOLocalMoveY(inDistance, inTime).From().SetEase(moveInEase).SetDelay(0.15f);
            if (isFadeIn)
            {
                boards.ToList().ForEach(b => b.DOFade(0, 0.1f).From().SetDelay(0.15f).SetEase(fadeInEase));
                txts.ToList().ForEach(t => t.DOFade(0, 0.1f).From().SetDelay(0.15f).SetEase(fadeInEase));
            }

            var btnCancell = btnCancellGO.AddComponent<Button>();

            UnityAction clickEvent = () =>
            {
                gameObject.transform.DOLocalMoveY(outDistance, outTime).SetEase(moveOutEase).SetDelay(0.15f).OnComplete(
                    () =>
                    {
                        gameObject.transform.localPosition = originPos;
                    });
                if (isFadeOut)
                {
                    boards.ToList().ForEach(b => b.DOFade(0, 0.05f).SetEase(fadeOutEase));
                    txts.ToList().ForEach(t => t.DOFade(0, 0.05f).SetEase(fadeOutEase));
                }
                image.DOFade(0, 0.2f).SetDelay(0.05f).OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    Object.Destroy(btnCancell.gameObject);
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
            //按下关闭按钮时淡出
            if (shut)
            {
                btnCancell.onClick.AddListener(clickEvent);
            }

            var btn = gameObject.GetComponentsInChildren<Button>().ToList().Find(b => b.gameObject.name == shutButtonName);
            if (btn != null)
            {
                btn.onClick.AddListener(clickEvent);
            }
        }
    }
}