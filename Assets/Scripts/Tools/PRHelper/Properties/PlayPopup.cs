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

        public float inTime = 0.4f; //面板入场的时长。
        public float inDistance = 200; //面板入场位移距离。
        public Ease moveInEase = Ease.OutElastic; //过渡曲线类型。
        public bool isFadeIn = true; //是否开启淡入。
        public Ease fadeInEase = Ease.Linear; //淡入过渡曲线。

        public float outTime = 0.4f; //面板出场的时长。
        public float outDistance = 200; //面板出场位移距离。
        public Ease moveOutEase = Ease.InExpo; //过渡曲线类型。
        public bool isFadeOut = true; //是否开启淡出。
        public Ease fadeOutEase = Ease.Linear; //淡出过渡曲线。

        public Color color; //Mask的颜色和透明度。
        public bool shut = true; //点击Mask区域是否能关闭面板。
        public string shutButtonName; //关闭按钮指定。

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

            var boards = gameObject.GetComponentsInChildren<Image>(); //获取对象所有子物体上的Image组件。
            var txts = gameObject.GetComponentsInChildren<Text>(); //获取对象所有子物体上的Text组件。

            //延迟0.15秒移动、淡入
            gameObject.transform.DOLocalMoveY(inDistance, inTime).From().SetEase(moveInEase).SetDelay(0.15f);
            if (isFadeIn)
            {
                boards.ToList().ForEach(b => b.DOFade(0, 0.1f).From().SetDelay(0.15f).SetEase(fadeInEase)); //
                txts.ToList().ForEach(t => t.DOFade(0, 0.1f).From().SetDelay(0.15f).SetEase(fadeInEase));
            }

            var btnCancell = btnCancellGO.AddComponent<Button>();

            // 出场动画。
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

            //当 Shut 开关被勾选时，可以通过点击空白区域来关闭面板。
            if (shut)
            {
                if (btnCancell)
                {
                    btnCancell.onClick.AddListener(clickEvent);
                }
            }
            // 检索面版中的按钮，且为指定为 Shut Button 的目标添加关闭面板事件。
            var btn = gameObject.GetComponentsInChildren<Button>().ToList().Find(b => b.gameObject.name == shutButtonName);
            if (btn)
            {
                btn.onClick.AddListener(clickEvent);
            }
        }
    }
}