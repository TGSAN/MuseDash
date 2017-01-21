using FormulaBase;

/// UI分析工具自动生成代码
/// PnlUnlockSongUI主模块
///
using System;
using Assets.Scripts.Common;
using UnityEngine;

namespace PnlUnlockSong
{
    public class PnlUnlockSong : UIPhaseBase
    {
        private static PnlUnlockSong instance = null;
        private Coroutine m_LoadTxeCoroutine;
        public UITexture texIcon;
        public UILabel txtName, txtAuthor, Txttrophy;
        public GameObject bkg;
        public Action onDisable;

        public static PnlUnlockSong Instance
        {
            get
            {
                return instance;
            }
        }

        public override void OnShow(int idx)
        {
            gameObject.SetActive(true);
            InitLabel(idx);
            InitTex(idx);
        }

        public override void OnShow()
        {
        }

        private void OnDisable()
        {
            if (onDisable != null)
            {
                onDisable();
            }
        }

        private void Start()
        {
            instance = this;
            InitEvent();
        }

        private void InitEvent()
        {
            var animator = GetComponent<Animator>();
            bkg.SetActive(false);
            DOTweenUtils.Delay(() =>
            {
                bkg.SetActive(true);
            }, Time.deltaTime);

            UIEventListener.Get(bkg).onClick += go =>
            {
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
                {
                    return;
                }
                animator.Play("unlock_song_out");
                DOTweenUtils.Delay(() =>
                {
                    Destroy(gameObject);
                }, 0.5f);
            };
        }

        private void InitLabel(int idx)
        {
            var songName = ConfigPool.Instance.GetConfigStringValue("stage", idx.ToString(), "name");
            var author = ConfigPool.Instance.GetConfigStringValue("stage", idx.ToString(), "author");
            var trophy = ConfigPool.Instance.GetConfigStringValue("stage", idx.ToString(), "unlock");
            txtName.text = songName;
            txtAuthor.text = author;
            Txttrophy.text = trophy;
        }

        private void InitTex(int idx)
        {
            var texName = ConfigPool.Instance.GetConfigStringValue("stage", idx.ToString(), "cover");
            this.m_LoadTxeCoroutine = ResourceLoader.Instance.Load(texName, obj =>
            {
                var t = obj as Texture;

                texIcon.mainTexture = t;

                if (this.m_LoadTxeCoroutine != null)
                {
                    ResourceLoader.Instance.StopCoroutine(this.m_LoadTxeCoroutine);
                }
            });
        }
    }
}