using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Common;
using Assets.Scripts.NGUI;
using Assets.Scripts.Tools.Managers;
using Assets.Scripts.UI;
using DG.Tweening;
using FormulaBase;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.NewUI.Panels
{
    public class PnlStage : SingletonMonoBehaviour<PnlStage>
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

        [Header("文字信息")]
        public float txtOffsetX;

        public Vector2 maxMinAlpha;
        public float eneryAnimDurationEnter, eneryAnimDurationLeave;
        public AnimationCurve energyCurve;
        public float btnFadeTime = 0.3f;

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

        [Header("难度")]
        public float animDT;

        [Header("对象绑定")]
        public Transform pivot;

        public GameObject scrollRect;
        public GameObject cell;
        public Button btnLeft, btnRight;
        public Text txtNameNext, txtAuthorNext;
        public Text txtNameLast, txtAuthorLast, txtEnergyLast;
        public Image imgEnergy;
        public GameObject energy, difficulty;
        public GameObject btnStart, btnTip;
        public Image imgSongProgress;
        public GameObject goPnlUnlockSong;

        [Header("音频")]
        public float loadDelay = 0.5f;

        public int resolution = 1024;

        public float lowFreqThreshold = 14700;
        public float lowEnhance = 1f;
        public float minSize = 1.0f;
        public float changeValue = 5.0f;
        public Action<int> onSongChange;

        [Header("背景")]
        public List<GradientKVP> gradients;

        public List<UIGradient> bkg, cube;
        private int m_CurGradientIdx = 0;

        private Tweener m_SlideTweener;
        private Tweener m_NextTweener;
        private Tweener m_EnergyTweener1, m_EnergyTweener2;
        private Sequence m_SlideSeq;
        private Sequence m_DlySeq;
        private Sequence m_DiffSeq;
        private int m_PreDiff = 30;
        private int m_TrophySum = 0;
        private float m_ZAngle = 0.0f;
        private int m_CurrentIdx = 0;
        private bool m_IsSliding = false;
        private ResourceRequest m_Request;
        private Coroutine m_Coroutine;
        private AudioClip m_CatchClip;
        private bool m_FinishEnter = true;
        private readonly Dictionary<int, GameObject> m_CellGroup = new Dictionary<int, GameObject>();
        private readonly List<StageInfo> m_StageInfos = new List<StageInfo>();
        private readonly List<float> m_Angles = new List<float>();
        private readonly Dictionary<float, GradientKVP> m_GradientDics = new Dictionary<float, GradientKVP>();

        public float offsetX { get; private set; }

        public AudioClip catchClip
        {
            get { return this.m_CatchClip; }
        }

        public float scale
        {
            get { return UIManager.instance.GetComponent<RectTransform>().lossyScale.x; }
        }

        public bool FinishEnter
        {
            get
            {
                return this.m_FinishEnter;
            }

            set
            {
                this.m_FinishEnter = value;
            }
        }

        public int currentSongIdx
        {
            get { return m_CurrentIdx + 1; }
        }

        private void Awake()
        {
            InitInfo();
            InitUI();
            InitEvent();
        }

        private void Update()
        {
            OnScrolling();
            //OnSongInfoChange();
            UpdatePos();
        }

        #region 初始化

        private void InitEvent()
        {
            UnityAction<BaseEventData> onBeginDrag = (eventData) =>
            {
                if (!m_FinishEnter)
                {
                    return;
                }
                m_IsSliding = true;
                if (m_SlideTweener != null)
                {
                    m_SlideTweener.Complete();
                }
            };

            UnityAction<BaseEventData> onDrag = eventData =>
            {
                var ed = eventData as PointerEventData;
                if (!m_FinishEnter)
                {
                    return;
                }
                offsetX = ed.delta.x * -sensitivity;
                offsetX = offsetX < 0
                    ? Mathf.Clamp(offsetX, -minMaxSlide.y, -minMaxSlide.x)
                    : Mathf.Clamp(offsetX, minMaxSlide.x, minMaxSlide.y);

                pivot.localEulerAngles += new Vector3(0, 0, offsetX);
                m_ZAngle += offsetX;
            };
            UnityAction<BaseEventData> onEndDrag = eventData =>
            {
                if (!m_FinishEnter)
                {
                    return;
                }
                var endAngles = pivot.localEulerAngles +
                                new Vector3(0, 0, offsetX * Mathf.Max(maxMoveDistance, minMoveDistance));
                endAngles = new Vector3(endAngles.x, endAngles.y, Mathf.RoundToInt(endAngles.z / angle) * angle);
                var up = pivot.up;
                var isPositive = offsetX > 0 ? 1 : -1;
                var angleBetween = Mathf.Abs(pivot.localEulerAngles.z - endAngles.z);
                var t = angleBetween / elasticSpeed;
                var originZAngle = m_ZAngle;
                m_SlideTweener = pivot.DORotate(endAngles, t, RotateMode.FastBeyond360)
                    .SetEase(Ease.OutSine)
                    .OnUpdate(() =>
                    {
                        m_ZAngle += Vector3.Angle(up, pivot.up) * isPositive;
                        up = pivot.up;
                        m_IsSliding = true;
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

            UIEventUtils.OnEvent(scrollRect, EventTriggerType.BeginDrag, onBeginDrag);
            UIEventUtils.OnEvent(scrollRect, EventTriggerType.Drag, onDrag);
            UIEventUtils.OnEvent(scrollRect, EventTriggerType.EndDrag, onEndDrag);

            UIEventUtils.OnEvent(btnStart, EventTriggerType.BeginDrag, onBeginDrag);
            UIEventUtils.OnEvent(btnStart, EventTriggerType.Drag, onDrag);
            UIEventUtils.OnEvent(btnStart, EventTriggerType.EndDrag, onEndDrag);

            UIEventUtils.OnEvent(btnLeft.gameObject, EventTriggerType.PointerClick, eventData =>
            {
                if (!m_FinishEnter)
                {
                    return;
                }
                if (m_IsSliding)
                {
                    return;
                }
                OnChangeOffset(new Vector3(0, 0, angle * -1), nextPageTime);
            });

            UIEventUtils.OnEvent(btnRight.gameObject, EventTriggerType.PointerClick, eventData =>
            {
                if (!m_FinishEnter)
                {
                    return;
                }
                if (m_IsSliding)
                {
                    return;
                }
                OnChangeOffset(new Vector3(0, 0, angle * 1), nextPageTime);
            });

            onSongChange += PlayMusic;
            //onSongChange += OnInfoChange;
        }

        public void UpdateInfo()
        {
            m_StageInfos.Clear();
            InitInfo();
        }

        private void InitInfo()
        {
            var count = StageBattleComponent.Instance.GetStageCount();
            var lockList = TaskStageTarget.Instance.GetLockList();
            var stageJData = ConfigManager.instance["stage"];
            for (int i = 0; i < count; i++)
            {
                var iconPath = stageJData[i]["cover"].ToString();
                var musicPath = stageJData[i]["music"].ToString();
                var musicName = stageJData[i]["name"].ToString();
                var authorName = stageJData[i]["author"].ToString();
                var unlockNum = (int)stageJData[i]["unlock_level"];
                var isLock = lockList[i];
                m_StageInfos.Add(new StageInfo(i, iconPath, musicPath, musicName, authorName, 0, 0, isLock, unlockNum));
            }
#if UNITY_IPHONE || UNITY_ANDROID
            minMaxSlide.y *= 2;
#endif
            m_TrophySum = TaskStageTarget.Instance.GetTotalTrophy();
            var idx = 0;
            /* if (m_GradientDics.Count == 0)
             {
                 for (int i = 0; i < m_StageInfos.Count; i++)
                 {
                     var theAngle = i * 40f;
                     m_GradientDics.Add(theAngle, gradients[idx]);
                     idx++;
                     if (idx >= gradients.Count)
                     {
                         idx = 0;
                     }
                 }
             }*/
        }

        private void InitUI()
        {
            // 读取关卡数量生成disk元件
            for (int i = 0; i < m_StageInfos.Count; i++)
            {
                GameObject item = GameObject.Instantiate(cell) as GameObject;
                item.transform.SetParent(pivot.transform);
                /*StageDisc.StageDisc sd = item.GetComponent<StageDisc.StageDisc>();
                if (sd != null)
                {
                    sd.SetStageId(i + 1);
                    sd.Lock(m_StageInfos[i].isLock);
                }*/
                m_CellGroup.Add(i, item);
            }
            //txtTrophySum.text = m_TrophySum.ToString();
        }

        #endregion 初始化

        #region 事件

        private void OnEnable()
        {
            ResetPos();
            //StageDisc.StageDisc.LoadAllDiscCover();
            //SceneAudioManager.Instance.bgm.clip = null;

            JumpToSong(currentSongIdx);
            UpdateInfo();
            /*enabled = false;
            DOTweenUtils.Delay(() =>
            {
                enabled = true;
            }, Time.deltaTime);*/
            // 新歌曲解锁
            if (TaskStageTarget.isNextUnlock)
            {
                var pnl = GameObject.Instantiate<GameObject>(goPnlUnlockSong);
                pnl.GetComponent<PnlUnlockSong.PnlUnlockSong>().OnShow(TaskStageTarget.nextUnlockIdx);
                TaskStageTarget.isNextUnlock = false;
            }
        }

        private void OnScrollEnd()
        {
            m_IsSliding = false;
            //PnlStageOld.PnlStageOld.Instance.OnSongChanged(m_CurrentIdx + 1);
            onSongChange(m_CurrentIdx);
        }

        private void OnInfoChange(int idx)
        {
            OnEnergyInfoChange(true);
            OnTrophyChange();
            m_CurGradientIdx = m_CurGradientIdx + 1 >= gradients.Count ? 0 : m_CurGradientIdx + 1;
        }

        private void OnTrophyChange()
        {
            var trophyNum = TaskStageTarget.Instance.GetXMax(TaskStageTarget.TASK_SIGNKEY_STAGE_EVLUATE);
            var isLock = m_StageInfos[m_CurrentIdx].isLock;

            btnTip.gameObject.SetActive(isLock);
        }

        private void OnEnergyInfoChange(bool change)
        {
            if (m_EnergyTweener1 != null)
            {
                m_EnergyTweener1.Kill();
            }
            if (m_EnergyTweener2 != null)
            {
                m_EnergyTweener2.Kill();
            }
            if (change)
            {
                m_EnergyTweener1 = DOTween.To(() => imgEnergy.fillAmount, x => imgEnergy.fillAmount = x, 1.0f,
                    eneryAnimDurationEnter).SetEase(energyCurve);
                m_EnergyTweener2 = energy.transform.DOScale(1.0f, eneryAnimDurationEnter).SetEase(energyCurve);
                var cost = 1f;
                var diff = 1;

                if (StageBattleComponent.Instance.Host != null)
                {
                    diff = StageBattleComponent.Instance.Host.GetDynamicIntByKey(SignKeys.DIFFCULT);

                    if (diff > 0)
                    {
                        cost = StageBattleComponent.Instance.Host.Result(FormulaKeys.FORMULA_20);
                    }
                }
                txtEnergyLast.text = cost.ToString();

                if (m_DiffSeq != null)
                {
                    m_DiffSeq.Complete();
                }
                m_DiffSeq = DOTween.Sequence();
                //var dt = animDT / difficulty.transform.childCount;
                var dt = 10;
                if (m_PreDiff < diff)
                {
                    for (int i = m_PreDiff; i < difficulty.transform.childCount; i++)
                    {
                        var isVisiable = i < diff;
                        var idx = i;
                        m_DiffSeq.AppendCallback(() =>
                        {
                            var child = difficulty.transform.GetChild(idx);
                            child.gameObject.SetActive(isVisiable);
                        });
                        m_DiffSeq.AppendInterval(dt);
                    }
                }
                else if (m_PreDiff > diff)
                {
                    for (int i = m_PreDiff; i >= 0; i--)
                    {
                        var isVisiable = i < diff;
                        var idx = i;
                        m_DiffSeq.AppendCallback(() =>
                        {
                            /*
                        if (difficulty.transform.childCount > idx)
                        {
                            var child = difficulty.transform.GetChild(idx);
                            child.gameObject.SetActive(isVisiable);
                        }
                        */
                        });
                        m_DiffSeq.AppendInterval(dt);
                    }
                }
                m_DiffSeq.Play().OnComplete(() =>
                {
                    m_PreDiff = diff;
                });
            }
            else
            {
                /* m_EnergyTweener1 = DOTween.To(() => imgEnergy.fillAmount, x => imgEnergy.fillAmount = x, 0.0f,
                     eneryAnimDurationLeave);
                 m_EnergyTweener2 = energy.transform.DOScale(0.0f, eneryAnimDurationLeave);*/
            }
        }

        private void OnMusicPlayAction(GameObject go)
        {
            if (!SceneAudioManager.Instance.bgm || !SceneAudioManager.Instance.bgm.isPlaying || m_IsSliding)
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

        private Tweener OnChangeOffset(Vector3 offset, float dt)
        {
            if (m_NextTweener != null)
            {
                if (m_NextTweener.Elapsed(false) != 0)
                {
                    return null;
                }
            }
            var endValue = pivot.localEulerAngles + offset;
            var up = pivot.up;
            var isPositive = offset.z > 0 ? 1 : -1;
            m_NextTweener = pivot.transform.DORotate(endValue, dt, RotateMode.FastBeyond360).OnUpdate(() =>
            {
                m_ZAngle += Vector3.Angle(up, pivot.up) * isPositive;
                up = pivot.up;
                m_IsSliding = true;
            }).OnComplete(() =>
            {
                m_ZAngle = Mathf.RoundToInt(m_ZAngle / angle) * angle;
                pivot.transform.localEulerAngles = new Vector3(0, 0, m_ZAngle);
                OnScrollEnd();
            }).SetEase(Ease.OutSine);
            return m_NextTweener;
        }

        private void OnSongInfoChange()
        {
            if (m_CurrentIdx < m_StageInfos.Count)
            {
                var offsetForInfo = new Vector3(offsetX < 0 ? txtOffsetX : -txtOffsetX, 220, 0);
                txtNameLast.text = m_StageInfos[m_CurrentIdx].idx + " " + m_StageInfos[m_CurrentIdx].musicName;
                txtAuthorLast.text = "Music by " + m_StageInfos[m_CurrentIdx].musicAuthor;
                var lerpNumLast = 1 - scale * Mathf.Abs(pivot.transform.position.x - m_CellGroup[m_CurrentIdx].transform.position.x) / ((Mathf.Sin(angle * Mathf.Deg2Rad) * radius));
                txtNameLast.color = new Color(txtNameLast.color.r, txtNameLast.color.b, txtNameLast.color.g, lerpNumLast);
                txtAuthorLast.color = new Color(txtAuthorLast.color.r, txtAuthorLast.color.b, txtAuthorLast.color.g, lerpNumLast);
                txtNameLast.transform.parent.localPosition = Vector3.Lerp(offsetForInfo, new Vector3(0, 220, 0), lerpNumLast);

                var startX = -570;
                var endX = 535;
                var progressPercent = ((m_ZAngle / angle + 2) % (m_CellGroup.Count)) / (m_CellGroup.Count - 1);
                if (m_ZAngle < -80.0f)
                {
                    progressPercent = 1 + ((m_ZAngle / angle + 3) % (m_CellGroup.Count)) / (m_CellGroup.Count - 1);
                }
                var pos = Vector3.Lerp(new Vector3(startX, 12, 0), new Vector3(endX, 12, 0), progressPercent);
                imgSongProgress.transform.localPosition = Vector3.Lerp(imgSongProgress.transform.localPosition, pos, Time.deltaTime * 5.0f);
            }
        }

        private void OnScrolling()
        {
            if (m_IsSliding)
            {
                //OnEnergyInfoChange(false);
                SceneAudioManager.Instance.bgm.Stop();
            }

            var theAngle = Mathf.Abs(Mathf.Abs(m_ZAngle) % (m_CellGroup.Count * 40f) - 80f);
            for (int i = 0; i < m_GradientDics.Count; i++)
            {
                var idx = i;
                var pair = m_GradientDics.ToList()[idx];
                idx++;
                if (idx >= m_GradientDics.Count)
                {
                    idx = 0;
                }
                var nextPair = m_GradientDics.ToList()[idx];
                var curAngle = pair.Key;
                var nextAngle = nextPair.Key;
                if (i == m_CellGroup.Count - 1)
                {
                    nextAngle = curAngle + 40f;
                }
                if (theAngle >= curAngle && theAngle < nextAngle)
                {
                    var curGradientKvp = pair.Value;
                    var nextGradientKvp = nextPair.Value;
                    var percent = (theAngle - curAngle) / (nextAngle - curAngle);
                    var cubeGradient = GradientUtils.BlendGradient(curGradientKvp.key, nextGradientKvp.key, percent);
                    var bkgGradient = GradientUtils.BlendGradient(curGradientKvp.value, nextGradientKvp.value, percent);
                    cube.ForEach(g => g.color = cubeGradient);
                    bkg.ForEach(g => g.color = bkgGradient);
                }
            }

            var alphaTo = m_StageInfos[m_CurrentIdx].isLock ? 0.0f : 1.0f;
            //DOTweenUtils.TweenAllAlphaTo(btnStart, alphaTo, btnFadeTime, 0.1f);
            //btnStart.GetComponent<TweenAlpha>().enabled = !m_StageInfos[m_CurrentIdx].isLock;

            var midIdx = Mathf.RoundToInt(m_ZAngle / angle + 2);
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
            var maxCellScaleX = m_CellGroup[m_CurrentIdx].transform.localScale.x;
            foreach (var pair in m_CellGroup)
            {
                var go = pair.Value;
                var idx = pair.Key;
                var xOffset = Mathf.Abs(go.transform.position.x - pivot.transform.position.x) / scale;
                if (go.transform.localScale.x > maxCellScaleX)
                {
                    m_CurrentIdx = m_StageInfos[pair.Key].idx;
                }
                if (m_FinishEnter)
                {
                    if (idx == midIdx || idx == first || idx == second || idx == fourth || idx == fifth)
                    {
                        go.SetActive(true);
                    }
                    else
                    {
                        go.SetActive(false);
                    }
                }
                go.transform.localScale = Vector3.Lerp(Vector3.one * minScale, Vector3.one * maxScale,
                    1 - xOffset / distanceToChangeScale);
                var texs = go.GetComponentsInChildren<UITexture>();
                foreach (var tex in texs)
                {
                    var color = Color.Lerp(min, max,
                        1 - xOffset / distanceToChangeColor);
                    if (m_StageInfos[idx].isLock)
                    {
                        color = min;
                    }
                    tex.color = color;
                }
                if (xOffset <= distanceToChangeScale && go.activeSelf)
                {
                    if (go.transform.localScale.x >= maxScale - 0.01f)
                    {
                        OnMusicPlayAction(go);
                    }
                }
                else
                {
                    go.transform.localScale = Vector3.one * minScale;
                }

                if (go.activeSelf)
                {
                    var x = (go.transform.position.x - pivot.transform.position.x) / scale;
                    var absX = Mathf.Abs(x);

                    var myAngleOffset = 90;
                    var x0 = radius * Mathf.Abs(Mathf.Cos(Mathf.Deg2Rad * (myAngleOffset)));
                    var x1 = radius * Mathf.Abs(Mathf.Cos(Mathf.Deg2Rad * (myAngleOffset - angle)));
                    var x2 = radius * Mathf.Abs(Mathf.Cos(Mathf.Deg2Rad * (myAngleOffset - angle * 2)));
                    var distance0 = x1 - x0;
                    var ditance1 = x2 - x1;
                    var distance2 = radius - x2;
                    var offset = Vector3.zero;
                    if (absX <= x1)
                    {
                        offset = Vector3.Lerp(offset0, offset1, (absX - x0) / distance0);
                    }
                    else if (absX <= x2)
                    {
                        offset = Vector3.Lerp(offset1, offset2, (absX - x1) / ditance1);
                    }
                    else if (absX <= radius)
                    {
                        offset = Vector3.Lerp(offset2, Vector3.zero, (absX - radius) / distance2);
                    }
                    if (x < 0)
                    {
                        offset = new Vector3(-offset.x, offset.y, offset.z);
                    }
                    offset = go.transform.InverseTransformVector(offset) * scale;

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

        private IEnumerator LoadCoroutine(float wait)
        {
            yield return new WaitForSeconds(wait);
            if (m_IsSliding)
            {
                yield return null;
            }

            string musicPath = m_StageInfos[m_CurrentIdx].musicPath;
            musicPath = musicPath.Replace("music/", "demo/") + "_demo";
            Debug.Log("Stage select load music : " + musicPath);
            this.m_Coroutine = ResourceLoader.Instance.Load(musicPath, this.LoadSync);
        }

        private void LoadSync(UnityEngine.Object res)
        {
            AudioClip newClip = res as AudioClip;
            var audioSource = SceneAudioManager.Instance.bgm;
            if (audioSource.clip != newClip)
            {
                Resources.UnloadAsset(audioSource.clip);
            }
            m_CatchClip = newClip;
            audioSource.clip = newClip;
            audioSource.Play();
            audioSource.loop = true;
        }

        #endregion 资源加载

        #region 操作

        private void UpdatePos()
        {
            var currentIdx = m_CurrentIdx;
            if (currentIdx >= m_Angles.Count)
            {
                return;
            }
            var startAngle = m_Angles[currentIdx];
            for (int i = currentIdx; i < (m_CellGroup.Count) / 2 + currentIdx; i++)
            {
                var idx = i > m_CellGroup.Count - 1 ? i - m_CellGroup.Count : i;
                var item = m_CellGroup[idx];
                var myAngle = startAngle - (i - currentIdx) * angle;
                item.transform.localPosition = radius * new Vector3(Mathf.Cos(myAngle * Mathf.Deg2Rad),
                    Mathf.Sin(myAngle * Mathf.Deg2Rad), 0.0f);
                item.transform.up = Vector3.Normalize(item.transform.position - pivot.transform.position);
                m_Angles[idx] = myAngle;
            }
            for (int i = currentIdx; i > currentIdx - (m_CellGroup.Count) / 2; i--)
            {
                var idx = i < 0 ? i + m_CellGroup.Count : i;
                var item = m_CellGroup[idx];
                var myAngle = startAngle + (currentIdx - i) * angle;
                item.transform.localPosition = radius * new Vector3(Mathf.Cos(myAngle * Mathf.Deg2Rad),
                    Mathf.Sin(myAngle * Mathf.Deg2Rad), 0.0f);
                item.transform.up = Vector3.Normalize(item.transform.position - pivot.transform.position);
                m_Angles[idx] = myAngle;
            }
        }

        public void PlayMusic(int idx)
        {
            if (m_Coroutine != null)
            {
                StopCoroutine(m_Coroutine);
            }

            m_Coroutine = StartCoroutine(LoadCoroutine(this.loadDelay));
        }

        public void ResetPos()
        {
            m_Angles.Clear();
            m_ZAngle = 0;
            pivot.localEulerAngles = new Vector3(0, 0, m_ZAngle);
            for (int i = 0; i < m_CellGroup.Count; i++)
            {
                var item = m_CellGroup[i];
                var myAngle = angleOffset - i * angle;
                item.transform.localPosition = radius * new Vector3(Mathf.Cos(myAngle * Mathf.Deg2Rad),
                    Mathf.Sin(myAngle * Mathf.Deg2Rad), 0.0f);
                item.transform.up = Vector3.Normalize(item.transform.position - pivot.transform.position);
                m_Angles.Add(myAngle);
            }
        }

        public void JumpToSong(int idx)
        {
            m_FinishEnter = false;
            idx--;
            m_CurrentIdx = idx;
            m_ZAngle += angle * (idx - 2 - numFrom);
            var angleAxis = new Vector3(0, 0, m_ZAngle);
            pivot.transform.localEulerAngles = angleAxis;
            var offset = new Vector3(0, 0, numFrom * angle);
            OnChangeOffset(offset, animDuration);
            DOTweenUtils.Delay(() =>
            {
                m_FinishEnter = true;
                //CommonPanel.GetInstance().ShowWaittingPanel(false);
            }, animDuration + 0.5f);

            var first = idx - 2;
            first = first < 0 ? first + m_CellGroup.Count : first;
            var second = idx - 1;
            second = second < 0 ? m_CellGroup.Count + second : second;
            var fourth = idx + 1;
            fourth = fourth > m_CellGroup.Count - 1 ? fourth - m_CellGroup.Count : fourth;
            var fifth = idx + 2;
            fifth = fifth > m_CellGroup.Count - 1 ? fifth - m_CellGroup.Count : fifth;
            foreach (var cell in m_CellGroup)
            {
                var i = cell.Key;
                if (i == first || i == second || i == fourth || i == fifth || i == idx)
                {
                    cell.Value.SetActive(true);
                }
                else
                {
                    cell.Value.SetActive(false);
                }
            }
        }

        #endregion 操作
    }
}