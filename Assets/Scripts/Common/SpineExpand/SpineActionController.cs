using FormulaBase;
using GameLogic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DYUnityLib;
using UnityEngine;

[Serializable]
public struct SkeletActionData
{
    public bool collapsed;
    public bool isEndLoop;
    public bool isRandomSequence;
    public bool isSelfProtect;
    public string name;
    public int protectLevel;
    public int arrayCount;
    public int spineActionKeyIndex;
    public int[] actionIdx;
    public int[] actionEventIdx;
}

public class SpineActionController : MonoBehaviour
{
    // private static int EVENT_INDEX_NONE = 0;
    public static Type[] TYPE_POLL;

    public BaseSpineObjectController objController;
    private int protectLevel;
    private string currentActionName;
    private SpineEventFactory eventFactory;
    private SkeletonAnimation skAnimation;
    private Dictionary<string, Spine.Bone> bones;
    private string[] skAnimationsName;
    private static System.Reflection.Assembly assembly = System.Reflection.Assembly.Load("Assembly-CSharp");

    private List<GameObject> synchroObjects;

    [SerializeField]
    private SkeletActionData[] actionData;

    // public int startEventIndex;
    public int actionMode;

    public float startDelay;
    public float duration;
    public float lengthRate;
    public GameObject rendererPreb;
    public GameObject detroyEffect;
    public GameObject clipEffect;
    public float rotateRuration = 1.0f;
    private Material m_Mtrl;
    private Renderer m_Renderer;
    private decimal m_Length = 0m;
    private int m_Idx = 0;

    public static void InitTypePoll()
    {
        if (TYPE_POLL != null && TYPE_POLL.Length > 0)
        {
            return;
        }

        TYPE_POLL = new Type[EditorData.Instance.SpineModeName.Length];
        for (int i = 0; i < EditorData.Instance.SpineModeName.Length; i++)
        {
            string moduleName = EditorData.Instance.SpineModeName[i];
            TYPE_POLL[i] = assembly.GetType(moduleName);
        }
    }

    // -----------------------------  Inst about -------------------------------------------
    private void Start()
    {
#if UNITY_EDITOR || UNITY_EDITOR_OSX || UNITY_EDITOR_64
        SkeletonAnimation ska = this.gameObject.GetComponent<SkeletonAnimation>();
        MeshRenderer meshRender = this.gameObject.GetComponent<MeshRenderer>();
        if (ska == null || meshRender == null || ska.skeletonDataAsset == null)
        {
            return;
        }

        if (meshRender != null && ska.skeletonDataAsset.atlasAssets != null)
        {
            foreach (AtlasAsset _aa in ska.skeletonDataAsset.atlasAssets)
            {
                foreach (Material _m in _aa.materials)
                {
                    Shader _sd = Shader.Find(_m.shader.name);
                    _m.shader = _sd;
                    _m.color = Color.white;
                }
            }
        }
#endif
    }

    public void Init(int idx)
    {
        m_Idx = idx;
        if (this.eventFactory == null)
        {
            this.eventFactory = this.gameObject.AddComponent<SpineEventFactory>();
        }

        InitTypePoll();
        this.eventFactory.SetIdx(idx);

        if (this.objController == null)
        {
            Type compType = TYPE_POLL[this.actionMode];
            this.objController = (BaseSpineObjectController)this.gameObject.AddComponent(compType);
        }

        this.objController.SetIdx(idx);
        this.objController.Init();

        SkeletonAnimation ska = this.gameObject.GetComponent<SkeletonAnimation>();
        if (ska != null && ska.skeleton != null)
        {
            this.bones = new Dictionary<string, Spine.Bone>();
            for (int i = 0, max = ska.skeleton.bones.Count; i < max; i++)
            {
                string _n = ska.skeleton.bones.Items[i].Data.Name;
                this.bones[_n] = ska.skeleton.bones.Items[i];
            }
        }

        SpineSynchroObjects sso = this.gameObject.GetComponent<SpineSynchroObjects>();
        if (sso != null)
        {
            this.SetSynchroObjects(sso.synchroObjects);
        }

        var md = StageBattleComponent.Instance.GetMusicDataByIdx(idx);
        if (md.isLongPressStart)
        {
            SetLength(md.configData.length);
        }
    }

    public void Clip(int startIdx, int idx)
    {
        var md = StageBattleComponent.Instance.GetMusicDataByIdx(m_Idx);
        //Ïû³ýÐÇÐÇ
        if (startIdx <= (md.nodeData.a_great_range * (decimal)lengthRate) / FixUpdateTimer.dInterval)
        {
            startIdx = 0;
            var stars = gameObject.transform.GetChild(0).GetComponentsInChildren<SpriteRenderer>();
            var startStarIdx = stars.ToList().FindIndex(s => s.gameObject.name.Contains("_top"));
            if (startStarIdx != -1)
            {
                detroyEffect = GameObject.Instantiate(detroyEffect);
                Destroy(stars[startStarIdx].gameObject);
            }
        }
        if (idx >= (m_Length - md.nodeData.a_great_range) / FixUpdateTimer.dInterval && idx <= (m_Length + md.nodeData.b_great_range) / FixUpdateTimer.dInterval)
        {
            var stars = gameObject.transform.GetChild(0).GetComponentsInChildren<SpriteRenderer>();
            var endIdx = stars.ToList().FindIndex(s => s.gameObject.name.Contains("_end"));
            if (endIdx != -1)
            {
                detroyEffect = GameObject.Instantiate(detroyEffect);
                Destroy(stars[endIdx].gameObject);
            }
        }

        var idxLength = Mathf.FloorToInt((float)(10m / FixUpdateTimer.dInterval));
        if (idx > idxLength)
        {
            idx = idxLength;
        }

        var tex = (Texture2D)m_Mtrl.GetTexture("_ClipTex");
        var colors = tex.GetPixels();
        for (int i = startIdx; i < idx; i++)
        {
            if (i < idxLength)
            {
                colors[i] = new Color(1f, 0f, 0f, 1f);
            }
        }
        tex.SetPixels(colors);
        tex.Apply();
    }

    public void PlayLongPressEffect(bool isTo)
    {
        clipEffect.GetComponentsInChildren<ParticleSystem>().ToList().ForEach(p =>
        {
            var emit = p.emission;
            emit.enabled = isTo;
        });

        if (!isTo)
        {
            if (AudioManager.Instance.girlEffect.loop)
            {
                AudioManager.Instance.girlEffect.Stop();
            }
        }
        AudioManager.Instance.girlEffect.loop = isTo;
    }

    public void SetLength(decimal time)
    {
        m_Length = time * (decimal)lengthRate;
        var shader = Resources.Load("shaders/SkeleClip") as Shader;
        var r = GameObject.Instantiate(rendererPreb, transform);
        r.SetActive(false);
        r.transform.localPosition = Vector3.zero;
        m_Mtrl = new Material(shader);
        m_Renderer = r.GetComponent<Renderer>();
        var originMtrl = m_Renderer.material;
        m_Mtrl.mainTexture = originMtrl.mainTexture;

        var idxLength = Mathf.FloorToInt((float)(10m / FixUpdateTimer.dInterval));
        var endIdx = Mathf.FloorToInt((float)(m_Length / FixUpdateTimer.dInterval));
        var tex = new Texture2D(idxLength, 1);
        var colors = tex.GetPixels();
        for (int i = 0; i < idxLength; i++)
        {
            if (i > endIdx)
            {
                colors[i] = new Color(1f, 0f, 0f, 1f);
            }
            else
            {
                colors[i] = new Color(0f, 0f, 0f, 1f);
            }
        }
        tex.SetPixels(colors);
        tex.Apply();
        m_Mtrl.SetTexture("_ClipTex", tex);
        var originLength = m_Mtrl.GetFloat("_Length");

        var startStart = transform.GetChild(0).GetChild(0).gameObject;
        startStart.transform.DOLocalRotate(new Vector3(0, 0, 360), rotateRuration, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).SetLoops(-1);
        var endStart = GameObject.Instantiate(startStart, startStart.transform.parent);
        endStart.transform.DOLocalRotate(new Vector3(0, 0, 360), rotateRuration, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).SetLoops(-1);
        endStart.transform.localPosition = startStart.transform.localPosition + Vector3.right * originLength * (float)(m_Length / 10m);
        endStart.name = endStart.name.Replace("top", "end");
        clipEffect = GameObject.Instantiate(clipEffect);
        clipEffect.GetComponentsInChildren<ParticleSystem>().ToList().ForEach(p =>
        {
            var emit = p.emission;
            emit.enabled = false;
        });
    }

    public decimal GetLength()
    {
        return m_Length;
    }

    private void Update()
    {
        if (m_Renderer && m_Mtrl)
        {
            m_Renderer.material = m_Mtrl;
        }
    }

    public void OnControllerStart()
    {
        if (this.objController == null)
        {
            return;
        }
        if (actionMode == 12)
        {
            var size = 6.67f;
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                child.gameObject.SetActive(true);
            }
            transform.position = new Vector3(size, transform.position.y, transform.position.z);
            transform.DOMoveX(-size * 19f, duration * 10f).SetEase(Ease.Linear).OnComplete(() =>
            {
                GameObject.Destroy(gameObject);
            });
            return;
        }
        this.objController.OnControllerStart();
    }

    private SkeletonAnimation GetSkeletonAnimation()
    {
        if (this.skAnimation != null)
        {
            return this.skAnimation;
        }

        this.skAnimation = this.gameObject.transform.GetComponent<SkeletonAnimation>();
        if (this.skAnimation == null)
        {
            Debug.Log("Animation is null");
        }

        this.InitAnimationsName();

        return this.skAnimation;
    }

    private void ClearTracks()
    {
        if (this.skAnimation == null)
        {
            return;
        }

        if (this.skAnimation.state == null)
        {
            return;
        }

        this.skAnimation.state.ClearTracks();
    }

    private void InitAnimationsName()
    {
        if (this.skAnimationsName != null)
        {
            return;
        }

        if (this.skAnimation == null || this.skAnimation.skeletonDataAsset == null)
        {
            return;
        }

        Spine.ExposedList<Spine.Animation> list = this.skAnimation.skeletonDataAsset.GetAnimationStateData().SkeletonData.Animations;
        this.skAnimationsName = new string[list.Count];
        for (int i = 0; i < list.Count; i++)
        {
            Spine.Animation ani = list.Items[i];
            if (ani == null)
            {
                continue;
            }

            this.skAnimationsName[i] = ani.Name;
        }
    }

    // -----------------------------  play about -------------------------------------------
    //
    //(check) play animation by spine or unity
    //
    private static void PlayAnimator(string actionKey, GameObject obj)
    {
        if (obj == null)
        {
            return;
        }

        Animator ani = obj.GetComponent<Animator>();
        if (ani == null)
        {
            return;
        }

        /*
		AnimatorClipInfo[] clipInfo = ani.GetCurrentAnimatorClipInfo (0);
		if (clipInfo != null && clipInfo.Length > 0) {
			AnimatorClipInfo cInfo = clipInfo [0];
			Debug.Log ("------>>> " + cInfo.clip.name);
		}

		// action key is conflict stop
		AnimatorStateInfo sInfo = ani.GetCurrentAnimatorStateInfo (0);
		for (int i = 0; i < EditorData.Instance.SpineActionKeys.Length; i++) {
			string _k = EditorData.Instance.SpineActionKeys [i];
			if (sInfo.IsName (_k)) {
				// ani.Stop ();
				ani.speed = 0f;
			}
		}
		*/

        if (!ani.HasState(0, Animator.StringToHash(actionKey)))
        {
            return;
        }

        ani.speed = 1f;
        ani.Stop();
        ani.Rebind();
        ani.Play(actionKey);
    }

    /// <summary>
    /// Play the specified action key, obj and onComplete.
    /// </summary>
    /// <param name="actionKey">Action key.</param>
    /// <param name="obj">Object.</param>
    /// <param name="onComplete">On complete.</param>
    public static void Play(string actionKey, GameObject obj, float tick = 0)
    {
        PlayAnimator(actionKey, obj);

        if (obj == null)
        {
            return;
        }

        SpineActionController sac = obj.GetComponent<SpineActionController>();
        if (sac == null)
        {
            return;
        }

        if (sac.actionData == null || sac.actionData.Length == 0)
        {
            return;
        }

        //sac.ResumeCurrentAnimation ();
        sac.PlayByKey(actionKey);
        if (tick > 0)
        {
            sac.SetCurrentAnimationTime(tick);
        }

        if (sac.synchroObjects != null && sac.synchroObjects.Count > 0)
        {
            foreach (GameObject so in sac.synchroObjects)
            {
                Play(actionKey, so, tick);
            }
        }
    }

    public static void Pause(GameObject obj)
    {
        if (obj == null)
        {
            return;
        }

        SpineActionController sac = obj.GetComponent<SpineActionController>();
        if (sac == null)
        {
            return;
        }

        sac.PauseCurrentAnimation();
    }

    public static void Reset(GameObject obj)
    {
        SpineActionController sac = obj.GetComponent<SpineActionController>();
        if (sac == null)
        {
            return;
        }

        sac.ResetAnimation();
    }

    public static string CurrentAnimationName(GameObject obj)
    {
        if (obj == null)
        {
            return null;
        }

        SpineActionController sac = obj.GetComponent<SpineActionController>();
        if (sac == null)
        {
            return null;
        }

        return sac.GetCurrentAnimationName();
    }

    public static int CurrentProtectLevel(GameObject obj)
    {
        if (obj == null)
        {
            return 0;
        }

        SpineActionController sac = obj.GetComponent<SpineActionController>();
        if (sac == null)
        {
            return 0;
        }

        return sac.GetProtectLevel();
    }

    public static void ClearProtectLevel(GameObject obj)
    {
        if (obj == null)
        {
            return;
        }

        SpineActionController sac = obj.GetComponent<SpineActionController>();
        if (sac == null)
        {
            return;
        }

        sac.SetProtectLevel(0);
    }

    public static void SetSkeletonOrder(int order, GameObject obj)
    {
        if (obj == null)
        {
            return;
        }

        SpineActionController sac = obj.GetComponent<SpineActionController>();
        if (sac == null)
        {
            return;
        }

        if (sac.skAnimation == null)
        {
            return;
        }

        MeshRenderer rd = sac.skAnimation.GetComponent<MeshRenderer>();
        if (rd)
        {
            rd.sortingOrder = order;
        }
    }

    public static Vector3 GetBoneRealPosition(string boneName, GameObject obj)
    {
        if (obj == null)
        {
            return Vector3.zero;
        }

        SpineActionController sac = obj.GetComponent<SpineActionController>();
        if (sac == null)
        {
            return Vector3.zero;
        }

        if (sac.bones == null || !sac.bones.ContainsKey(boneName))
        {
            return Vector3.zero;
        }

        Spine.Bone bone = sac.bones[boneName];
        if (bone == null)
        {
            return Vector3.zero;
        }

        float wx, wy;
        bone.localToWorld(bone.x, bone.y, out wx, out wy);
        return new Vector3(wx, wy, 0);
    }

    public static void SetSynchroObjectsActive(GameObject obj, bool val)
    {
        if (obj == null)
        {
            return;
        }

        SpineActionController sac = obj.GetComponent<SpineActionController>();
        if (sac == null)
        {
            return;
        }

        if (sac.synchroObjects == null)
        {
            return;
        }

        foreach (GameObject so in sac.synchroObjects)
        {
            so.SetActive(val);
        }
    }

    private void ResumeCurrentAnimation()
    {
        if (this.skAnimation == null)
        {
            return;
        }

        this.skAnimation.state.TimeScale = 1f;
    }

    private void PauseCurrentAnimation()
    {
        if (this.skAnimation == null)
        {
            return;
        }

        this.skAnimation.state.TimeScale = 0;
    }

    private void ResetAnimation()
    {
        if (this.skAnimation == null)
        {
            return;
        }

        this.skAnimation.Reset();
    }

    private void SetCurrentAnimationTime(float tick)
    {
        if (this.skAnimation == null)
        {
            return;
        }

        this.skAnimation.state.Update(tick * 0.5f);
    }

    private Spine.TrackEntry AddAnimation(string name, bool isLoop, float delay)
    {
        if (name == null)
        {
            return null;
        }

        if (this.skAnimation == null || this.skAnimation.state == null)
        {
            return null;
        }

        if (!this.CheckHasAction(name))
        {
            Debug.Log("SAC Add Ani No such skeleton : " + name);
            return null;
        }

        return this.skAnimation.state.AddAnimation(0, name, isLoop, delay);
    }

    public void PlayByKey(string actionKey)
    {
        for (int i = 0; i < this.actionData.Length; i++)
        {
            SkeletActionData sad = this.GetData(i);
            if (sad.name == actionKey)
            {
                this.PlayByData(sad);
                return;
            }
        }
    }

    public void PlayByData(SkeletActionData data)
    {
        if (this.GetSkeletonAnimation() == null)
        {
            return;
        }

        if (this.skAnimationsName == null)
        {
            return;
        }

        if (this.CheckActionProtect(data))
        {
            return;
        }

        this.ClearTracks();
        this.currentActionName = data.name;
        this.protectLevel = data.protectLevel;
        if (data.isRandomSequence)
        {
            int i = UnityEngine.Random.Range(0, data.arrayCount);
            int idx = data.actionIdx[i];
            int eventIdx = data.actionEventIdx[i];
            string name = this.skAnimationsName[idx];
            Spine.AnimationState.CompleteDelegate comDlg = SpineEventFactory.GetFunction(this.gameObject, eventIdx);
            Spine.TrackEntry entry = this.AddAnimation(name, data.isEndLoop, 0);
            if (entry != null && comDlg != null)
            {
                entry.Complete += comDlg;
            }
        }
        else
        {
            int endIdx = data.arrayCount - 1;
            for (int i = 0; i < data.arrayCount; i++)
            {
                int idx = data.actionIdx[i];
                string name = this.skAnimationsName[idx];
                bool isLoop = (data.isEndLoop && !(i < endIdx));
                Spine.TrackEntry entry = this.AddAnimation(name, isLoop, 0);
                if (entry == null)
                {
                    continue;
                }

                int eventIdx = data.actionEventIdx[i];
                Spine.AnimationState.CompleteDelegate comDlg = SpineEventFactory.GetFunction(this.gameObject, eventIdx);
                if (comDlg == null)
                {
                    continue;
                }

                // Debug.Log (" animation be added " + name + " -/- " + comDlg.Target.GetType().Name);

                entry.Complete += comDlg;
            }
        }
    }

    public int GetProtectLevel()
    {
        return this.protectLevel;
    }

    public void SetProtectLevel(int level)
    {
        this.protectLevel = level;
    }

    public void SetCurrentActionName(string name)
    {
        this.currentActionName = name;
    }

    public string GetCurrentActionName()
    {
        return this.currentActionName;
    }

    public void SetSynchroObjects(List<GameObject> objs)
    {
        if (this.synchroObjects != null)
        {
            return;
        }

        this.synchroObjects = objs;
        if (this.synchroObjects == null || this.synchroObjects.Count <= 0)
        {
            return;
        }

        for (int i = 0; i < this.synchroObjects.Count; i++)
        {
            GameObject so = this.synchroObjects[i];
            SpineActionController sac = so.GetComponent<SpineActionController>();
            if (sac == null)
            {
                continue;
            }

            so = GameObject.Instantiate(so);
            so.transform.parent = this.gameObject.transform.parent;
            sac = so.GetComponent<SpineActionController>();
            sac.Init(-1);
            this.synchroObjects[i] = so;
        }
    }

    private string GetCurrentAnimationName()
    {
        if (this.GetSkeletonAnimation() == null)
        {
            return null;
        }

        return this.skAnimation.AnimationName;
    }

    private bool CheckHasAction(string name)
    {
        if (this.skAnimation == null)
        {
            return false;
        }

        Spine.Animation ani = this.skAnimation.skeletonDataAsset.GetAnimationStateData().skeletonData.FindAnimation(name);
        return ani != null;
    }

    private bool CheckActionProtect(SkeletActionData data)
    {
        if (this.skAnimation == null)
        {
            return true;
        }

        if (data.isSelfProtect && this.currentActionName == data.name)
        {
            return true;
        }

        return this.protectLevel > data.protectLevel;
    }

    // ------------------------------ data about ------------------------------------------
    public void AddData(SkeletActionData d)
    {
        List<SkeletActionData> _list;
        if (this.actionData != null && this.actionData.Length > 0)
        {
            _list = this.actionData.ToList();
        }
        else
        {
            _list = new List<SkeletActionData>();
        }

        _list.Add(d);

        this.actionData = _list.ToArray();
    }

    public void DelData(int idx)
    {
        if (this.actionData == null)
        {
            return;
        }

        if (this.actionData.Length <= idx)
        {
            return;
        }

        List<SkeletActionData> _list = this.actionData.ToList();
        _list.RemoveAt(idx);
        this.actionData = _list.ToArray();
    }

    public void SetData(int idx, SkeletActionData d)
    {
        if (this.actionData == null)
        {
            return;
        }

        if (this.actionData.Length <= idx)
        {
            return;
        }

        d.name = EditorData.Instance.SpineActionKeys[d.spineActionKeyIndex];

        this.actionData[idx] = d;
    }

    public SkeletActionData GetData(int idx)
    {
        return this.actionData[idx];
    }

    public int DataCount()
    {
        if (this.actionData == null)
        {
            return 0;
        }

        return this.actionData.Length;
    }
}