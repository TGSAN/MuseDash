using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.NGUI
{
    public class StageInfo
    {
        public string iconPath;
        public string musicPath;

        public StageInfo(string icon, string music)
        {
            iconPath = icon;
            musicPath = music;
        }
    }

    public class PnlScrollCircle : SingletonMonoBehaviour<PnlScrollCircle>
    {
        public float radius;
        public float angle;
        public float angleOffset;

        [Header("滑动")]
        public float sensitivity;

        public float elasticSpeed;
        public float maxMoveDistance, minMoveDistance;
        public float nextPageTime;
        public float slideSpeed;
        public float delaySlideTime;
        public Vector2 minMaxSlide;
        public int numFrom;
        public float animDuration;

        [Header("缩放")]
        public float distanceToChangeScale;

        public float minScale;
        public float maxScale;

        [Header("偏移")]
        public Vector3 offset0;

        public Vector3 offset1;
        public Vector3 offset2;

        [Header("颜色变化")]
        public float distanceToChangeColor;

        public Color min, max;

        [Header("对象")]
        public Transform pivot;

        public GameObject cell;
        public GameObject leftButton, rightButton;

        [Header("音频")]
        public int resolution = 1024;

        public float lowFreqThreshold = 14700;
        public float lowEnhance = 1f;
        public float minSize = 1.0f;
        public float changeValue = 5.0f;
        public float scale = 1.0f;
        public Action<int> onSongChange;

        private Tweener m_SlideTweener;
        private Tweener m_NextTweener;
        private Sequence m_SlideSeq;
        private Sequence m_DlySeq;
        private float m_ZAngle = 0.0f;
		private static int m_CurrentIdx = 0;
        private bool m_IsSliding = false;
        private ResourceRequest m_Request;
        private Coroutine m_Coroutine;

        private readonly Dictionary<int, GameObject> m_CellGroup = new Dictionary<int, GameObject>();
        private readonly List<StageInfo> m_StageInfos = new List<StageInfo>();

        public float offsetX { get; private set; }

		public static int currentSongIdx
        {
            get { return m_CurrentIdx + 1; }
        }

        private void Awake()
        {
            InitInfo();
            InitUI();
            InitEvent();
            InitResoruce();
        }

        private void Update()
        {
            OnScrolling();
        }

        #region 初始化

        private void InitResoruce()
        {
            for (int i = 0; i < m_StageInfos.Count; i++)
            {
                var item = m_StageInfos[i];
                var musicPath = item.musicPath;
            }
            this.onSongChange += PlayMusic;
        }

        private void InitInfo()
        {
            var jData = ConfigPool.Instance.GetConfigByName("stage");
            for (int i = 0; i < jData.Count; i++)
            {
                var iconPath = jData[i]["icon"].ToString();
                var musicPath = jData[i]["FileName_1"].ToString();
                m_StageInfos.Add(new StageInfo(iconPath, musicPath));
            }
        }

        private void InitEvent()
        {
            UIEventListener.VoidDelegate onDragEnd = go =>
            {
                var endAngles = pivot.localEulerAngles +
                                new Vector3(0, 0, offsetX * Mathf.Max(maxMoveDistance, minMoveDistance));
                endAngles = new Vector3(endAngles.x, endAngles.y, Mathf.RoundToInt(endAngles.z / angle) * angle);
                var up = pivot.up;
                var isPositive = offsetX > 0 ? 1 : -1;
                var angleBetween = Mathf.Abs(pivot.localEulerAngles.z - endAngles.z);
                var t = angleBetween / elasticSpeed;
                var originZAngle = m_ZAngle;
                m_SlideTweener = pivot.DORotate(endAngles, t, RotateMode.FastBeyond360).SetEase(Ease.OutSine).OnUpdate(() =>
                {
                    m_ZAngle += Vector3.Angle(up, pivot.up) * isPositive;
                    up = pivot.up;
                }).OnComplete(() =>
                {
                    if (angleBetween / angle < 0.5f)
                    {
                        m_ZAngle = originZAngle;
                    }
                    m_ZAngle = Mathf.RoundToInt(m_ZAngle / angle) * angle;
                    var angleAxis = new Vector3(0, 0, m_ZAngle);
                    pivot.transform.localEulerAngles = angleAxis;
                    OnScrollEnd();
                });
            };

            Action<GameObject, bool, float> onSlide = (go, isPressing, mul) =>
            {
                if (isPressing)
                {
                    m_IsSliding = true;
                    m_DlySeq = DOTween.Sequence();
                    m_DlySeq.AppendInterval(delaySlideTime);
                    m_DlySeq.AppendCallback(() =>
                    {
                        m_SlideSeq.Play();
                    });
                    m_SlideSeq = DOTween.Sequence();
                    m_SlideSeq.AppendInterval(float.MaxValue).OnUpdate(() =>
                    {
                        var offset = slideSpeed * Time.deltaTime * mul;
                        pivot.localEulerAngles += new Vector3(0, 0, offset);
                        m_ZAngle += offset;
                    });
                    m_SlideSeq.Pause();
                }
                else
                {
                    var clickTime = m_DlySeq.Elapsed(false) * m_DlySeq.Duration(false);
                    if (clickTime < 0.2f && clickTime != 0)
                    {
                        OnChangeOffset(new Vector3(0, 0, angle * mul), nextPageTime);
                    }
                    else
                    {
                        offsetX = 0;
                        onDragEnd(null);
                    }
                    if (m_SlideSeq != null)
                    {
                        m_SlideSeq.Kill();
                        m_SlideSeq = null;
                    }

                    if (m_DlySeq != null)
                    {
                        m_DlySeq.Kill();
                        m_DlySeq = null;
                    }
                }
            };
            UIEventListener.Get(gameObject).onDragStart = go =>
            {
                m_IsSliding = true;
                if (m_SlideTweener != null)
                {
                    m_SlideTweener.Complete();
                }
            };
            UIEventListener.Get(gameObject).onDrag = (go, delta) =>
            {
                offsetX = delta.x * -sensitivity;
                offsetX = offsetX < 0
                    ? Mathf.Clamp(offsetX, -minMaxSlide.y, -minMaxSlide.x)
                    : Mathf.Clamp(offsetX, minMaxSlide.x, minMaxSlide.y);

                pivot.localEulerAngles += new Vector3(0, 0, offsetX);
                m_ZAngle += offsetX;
            };
            UIEventListener.Get(gameObject).onDragEnd = onDragEnd;

            UIEventListener.Get(leftButton).onPress = (go, isPressing) =>
            {
                onSlide(go, isPressing, 1);
            };
            UIEventListener.Get(rightButton).onPress = (go, isPressing) =>
            {
                onSlide(go, isPressing, -1);
            };
        }

        private void InitUI()
        {
            var numPerRound = 360f / angle;
            var count = Mathf.CeilToInt(m_StageInfos.Count / numPerRound) * numPerRound;
            // 读取关卡数量生成disk元件
            for (int i = 0; i < count; i++)
            {
                GameObject item = GameObject.Instantiate(cell) as GameObject;
                item.transform.parent = pivot.transform;
                item.transform.localScale = Vector3.one;
                StageDisc.StageDisc sd = item.GetComponent<StageDisc.StageDisc>();
                if (sd != null)
                {
                    sd.SetStageId(i + 1);
                }

                var myAngle = angleOffset + angle * i;
                item.transform.localPosition = radius * new Vector3(Mathf.Cos(myAngle * Mathf.Deg2Rad),
                    Mathf.Sin(myAngle * Mathf.Deg2Rad), 0.0f);
                item.transform.up = Vector3.Normalize(item.transform.position - pivot.transform.position);
                m_CellGroup.Add(i, item);
            }
        }

        #endregion 初始化

        #region 事件

        private void OnScrollEnd()
        {
            m_IsSliding = false;
            onSongChange(m_CurrentIdx + 1);
        }

        private void OnMusicPlayAction(GameObject go)
        {
            if (!SceneAudioManager.Instance.bgm || !SceneAudioManager.Instance.bgm.isPlaying)
            {
                return;
            }
            var spectrum = SceneAudioManager.Instance.bgm.GetSpectrumData(resolution, 0, FFTWindow.Triangle);

            var deltaFreq = AudioSettings.outputSampleRate / resolution;
            float low = 0f;

            for (var i = 0; i < resolution; ++i)
            {
                var freq = deltaFreq * i;
                if (freq <= lowFreqThreshold)
                    low += spectrum[i];
            }

            low *= lowEnhance;
            float handleValue = minSize + (low / (low + changeValue));
            go.transform.localScale = handleValue * Vector3.one * maxScale;
        }

        private void OnChangeOffset(Vector3 offset, float dt)
        {
            if (m_NextTweener != null)
            {
                if (m_NextTweener.Elapsed(false) != 0)
                {
                    return;
                }
            }
            var endValue = pivot.localEulerAngles + offset;
            var up = pivot.up;
            var isPositive = offset.z > 0 ? 1 : -1;
            m_NextTweener = pivot.transform.DORotate(endValue, dt, RotateMode.FastBeyond360).OnUpdate(() =>
            {
                m_ZAngle += Vector3.Angle(up, pivot.up) * isPositive;
                up = pivot.up;
            }).OnComplete(() =>
            {
                m_ZAngle = Mathf.RoundToInt(m_ZAngle / angle) * angle;
                pivot.transform.localEulerAngles = new Vector3(0, 0, m_ZAngle);
                OnScrollEnd();
            }).SetEase(Ease.OutSine);
        }

        private void OnScrolling()
        {
            if (m_IsSliding)
            {
                SceneAudioManager.Instance.bgm.Stop();
            }
            var midIdx = Mathf.RoundToInt(-m_ZAngle / angle + 2);
            midIdx %= m_CellGroup.Count;
            midIdx = midIdx < 0
                ? midIdx + m_CellGroup.Count
                : midIdx > m_CellGroup.Count - 1 ? midIdx - m_CellGroup.Count : midIdx;
            var first = midIdx - 2;
            first = first < 0 ? first + m_CellGroup.Count : first;
            var second = midIdx - 1;
            second = second < 0 ? m_CellGroup.Count + second : second;
            var fourth = midIdx + 1;
            fourth = fourth > m_CellGroup.Count - 1 ? fourth - m_CellGroup.Count : fourth;
            var fifth = midIdx + 2;
            fifth = fifth > m_CellGroup.Count - 1 ? fifth - m_CellGroup.Count : fifth;

            foreach (var pair in m_CellGroup)
            {
                var go = pair.Value;
                var idx = pair.Key;
                var xOffset = Mathf.Abs(go.transform.position.x - pivot.transform.position.x) * scale;
                if (idx == midIdx || idx == first || idx == second || idx == fourth || idx == fifth)
                {
                    go.SetActive(true);
                }
                else
                {
                    go.SetActive(false);
                }
                go.transform.localScale = Vector3.Lerp(Vector3.one * minScale, Vector3.one * maxScale,
                1 - xOffset / distanceToChangeScale);
                var texs = go.GetComponentsInChildren<UITexture>();
                foreach (var tex in texs)
                {
                    tex.color = Color.Lerp(min, max,
                        1 - xOffset / distanceToChangeColor);
                }
                if (xOffset <= distanceToChangeScale && go.activeSelf)
                {
                    if (go.transform.localScale.x >= maxScale - 0.01f)
                    {
                        OnMusicPlayAction(go);
                    }

                    m_CurrentIdx = idx;
                }
                else
                {
                    go.transform.localScale = Vector3.one * minScale;
                }

                if (go.activeSelf)
                {
                    var x = (go.transform.position.x - pivot.transform.position.x) * scale;
                    var absX = Mathf.Abs(x);
                    var myAngleOffset = angleOffset + 2 * angle;
                    var x0 = radius * Mathf.Abs(Mathf.Cos(Mathf.Deg2Rad * (myAngleOffset)));
                    var x1 = radius * Mathf.Abs(Mathf.Cos(Mathf.Deg2Rad * (myAngleOffset - angle)));
                    var x2 = radius * Mathf.Abs(Mathf.Cos(Mathf.Deg2Rad * (myAngleOffset - angle * 2)));
                    var distance0 = x1 - x0;
                    var ditance1 = x2 - x1;
                    var distance2 = radius - x2;
                    var offset = Vector3.zero;
                    if (absX < x1)
                    {
                        offset = Vector3.Lerp(offset0, offset1, (absX - x0) / distance0);
                    }
                    else if (absX < x2)
                    {
                        offset = Vector3.Lerp(offset1, offset2, (absX - x1) / ditance1);
                    }
                    else if (absX < radius)
                    {
                        offset = Vector3.Lerp(offset2, Vector3.zero, (absX - radius) / distance2);
                    }
                    if (x < 0)
                    {
                        offset = new Vector3(-offset.x, offset.y, offset.z);
                    }
                    offset = go.transform.InverseTransformVector(offset) / scale;
                    for (int i = 0; i < go.transform.childCount; i++)
                    {
                        var child = go.transform.GetChild(i);
                        child.localPosition = offset;
                    }
                }
            }
        }

        #endregion 事件

        #region 资源加载

        private IEnumerator LoadCoroutine()
        {
            while (m_Request.isDone)
            {
                yield return null;
            }
            var clip = m_Request.asset as AudioClip;
            while (!clip.isReadyToPlay)
            {
                yield return null;
            }
            var percent = 15.0f / clip.length;
            var length = Mathf.RoundToInt((float)(clip.channels * clip.samples) * percent);
            var data = new float[length];
            var name = clip.name;
            clip.GetData(data, 0);
            Resources.UnloadAsset(clip);
            var newClip = AudioClip.Create(name, data.Length, 2, 44100, false);
            newClip.SetData(data, 0);
            while (!newClip.isReadyToPlay)
            {
                yield return null;
            }
            var audioSource = SceneAudioManager.Instance.bgm;
            if (audioSource.clip != newClip)
            {
                Destroy(audioSource.clip);
            }
            audioSource.clip = newClip;
            audioSource.Play();
            audioSource.loop = true;
        }

        #endregion 资源加载

        #region 操作

        public void PlayMusic(int idx)
        {
            idx -= 1;
            if (m_Coroutine != null)
            {
                StopCoroutine(m_Coroutine);
            }
            m_Request = Resources.LoadAsync(m_StageInfos[idx].musicPath) as ResourceRequest;
            m_Coroutine = StartCoroutine(LoadCoroutine());
        }

        public void ResetPos()
        {
            m_ZAngle = 0;
            pivot.localEulerAngles = Vector3.zero;
        }

        public void JumpToSong(int idx, float dt = 0.8f)
        {
            idx -= 1;
            m_ZAngle -= angle * (idx - 2 - numFrom);
            var angleAxis = new Vector3(0, 0, m_ZAngle);
            pivot.transform.localEulerAngles = angleAxis;
            var offset = new Vector3(0, 0, -numFrom * angle);
            OnChangeOffset(offset, animDuration);
        }

        #endregion 操作
    }
}