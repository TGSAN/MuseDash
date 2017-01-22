using System;
using System.Collections;
using System.Linq;
using Assets.Scripts.Common;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Tool.PRHelper.Properties
{
    [Serializable]
    public class PlayPopup
    {
        public string pnlName;
        public Color bkgColor;
        public float speed;
        public float distance;
        public float moveInDt;
        public float moveOutDt;
        public Ease moveInEase;
        public Ease moveOutEase;

        public void Play()
        {
            var gameObject = UIManager.instance[pnlName];
            gameObject.SetActive(true);
            var btnCancellGO = new GameObject("BtnCancel");
            var rectTransform = btnCancellGO.AddComponent<RectTransform>();

            var parent = UIManager.instance.gameObject;
            btnCancellGO.transform.localScale = new Vector3(1, 1, 1);
            rectTransform.sizeDelta = parent.GetComponent<RectTransform>().sizeDelta;
            btnCancellGO.transform.SetParent(parent.transform, false);
            btnCancellGO.transform.SetSiblingIndex(parent.transform.GetSiblingIndex());

            var texBkg = new Texture2D(1, 1);
            texBkg.SetPixel(0, 0, bkgColor);
            texBkg.Apply();
            var image = btnCancellGO.AddComponent<Image>();
            var rect = new Rect(0, 0, texBkg.width, texBkg.height);
            var sprite = Sprite.Create(texBkg, rect, new Vector2(0.5f, 0.5f), 1);
            image.material.mainTexture = texBkg;
            image.sprite = sprite;
            image.color = bkgColor;
            image.canvasRenderer.SetAlpha(0.0f);
            image.CrossFadeAlpha(1.0f, 0.1f, false);

            var boards = gameObject.GetComponentsInChildren<Image>();
            var txts = gameObject.GetComponentsInChildren<Text>();
            gameObject.transform.DOLocalMoveY(distance, speed).From().SetEase(moveInEase).SetDelay(moveInDt);

            boards.ToList().ForEach(b => b.DOFade(0, 0.1f).From().SetDelay(moveInDt));
            txts.ToList().ForEach(t => t.DOFade(0, 0.1f).From().SetDelay(moveInDt));

            var btnCancell = btnCancellGO.AddComponent<Button>();
            btnCancell.onClick.AddListener(() =>
            {
                boards.ToList().ForEach(b => b.DOFade(0, 0.05f).From());
                txts.ToList().ForEach(t => t.DOFade(0, 0.05f).From());

                image.DOFade(0, 0.2f).SetDelay(0.05f);
                DOTweenUtils.Delay(() =>
                {
                    gameObject.SetActive(false);
                    Object.Destroy(btnCancell.gameObject);
                }, 0.25f);
            });
        }
    }
}