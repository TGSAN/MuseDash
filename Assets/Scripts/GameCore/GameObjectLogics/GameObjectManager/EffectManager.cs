using FormulaBase;
using GameLogic;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Tools.Managers;
using UnityEngine;
using UnityEngine.UI;

public class EffectManager : MonoBehaviour
{
    private const int EX_IDX = 999;
    public const int COMBO_CHANGE_LIM = 20;

    private GameObject[] playResult;
    private float[] playResultOffset;
    private GameObject pressEffect;
    private GameObject ferverScene;

    private Dictionary<string, GameObject> effectPool = null;

    private static EffectManager instance = null;

    public GameObject comboAnimationNormal;
    public GameObject comboAnimationHigh;

    public EffectManager()
    {
        instance = this;
    }

    public static EffectManager Instance
    {
        get
        {
            return instance;
        }
    }

    private GameObject InitComboAnimationByName(string name)
    {
        GameObject _comboAnimation = this.transform.Find(name).gameObject;
        NodeInitController initController = _comboAnimation.GetComponent<NodeInitController>();
        if (initController == null)
        {
            initController = _comboAnimation.AddComponent<NodeInitController>();
        }

        SpineActionController sac = _comboAnimation.GetComponent<SpineActionController>();
        sac.Init(EX_IDX);

        _comboAnimation.SetActive(false);
        return _comboAnimation;
    }

    // Use this for initialization
    private void Start()
    {
        this.comboAnimationNormal.SetActive(false);
        this.comboAnimationHigh.SetActive(false);
        this.Preload();
    }

    public void Preload()
    {
        this.effectPool = new Dictionary<string, GameObject>();
        // 零散特效预加载
        string path = "Prefabs/skill/Skill_hp";
        //(check) use this function to create new object
        this.effectPool[path] = Resources.Load(path) as GameObject;

        path = "Prefabs/arms/Arm_item";
        this.effectPool[path] = Resources.Load(path) as GameObject;
    }

    public void SetEffectByCharact(int heroIndex)
    {
        // Load press succeed effect.
        string path = GirlManager.GetCharactPath(name);

        // Fever scene
        string feverScene = ConfigManager.instance.GetConfigStringValue("char_info", "id", "fever", heroIndex);
        this.ferverScene = GameObject.Instantiate(Resources.Load(feverScene)) as GameObject;

        string atkFxGreat = ConfigManager.instance.GetConfigStringValue("char_info", "id", "fx_atk_great", heroIndex);
        string atkFxPerfect = ConfigManager.instance.GetConfigStringValue("char_info", "id", "fx_atk_perfect", heroIndex);
        string atkFxCrit = ConfigManager.instance.GetConfigStringValue("char_info", "id", "fx_atk_crit", heroIndex);
        // Load attack succeed effect.
        this.playResult = new GameObject[(int)GameMusic.CRITICAL + 1];
        this.playResult[GameMusic.COOL] = GameObject.Instantiate(Resources.Load(atkFxGreat)) as GameObject;
        this.playResult[GameMusic.GREAT] = GameObject.Instantiate(Resources.Load(atkFxGreat)) as GameObject;
        this.playResult[GameMusic.PERFECT] = GameObject.Instantiate(Resources.Load(atkFxPerfect)) as GameObject;
        this.playResult[GameMusic.CRITICAL] = GameObject.Instantiate(Resources.Load(atkFxCrit)) as GameObject;
        for (int i = 0; i < this.playResult.Length; i++)
        {
            GameObject _playResult = this.playResult[i];
            if (_playResult == null)
            {
                continue;
            }

            NodeInitController _initController = _playResult.GetComponent<NodeInitController>();
            if (_initController == null)
            {
                _initController = _playResult.AddComponent<NodeInitController>();
            }

            SpineActionController _sac = this.comboAnimationNormal.GetComponent<SpineActionController>();
            _sac.Init(EX_IDX + i);

            SpineActionController _sac2 = this.comboAnimationHigh.GetComponent<SpineActionController>();
            _sac2.Init(EX_IDX + i);

            _playResult.SetActive(false);
        }

        this.InitAttackFxOffset();
    }

    public void ShowPressGirlEffect(bool isShow)
    {
        if (this.pressEffect == null)
        {
            return;
        }
        this.pressEffect.SetActive(isShow);
        Animator ani = this.pressEffect.GetComponent<Animator>();
        if (ani == null)
        {
            return;
        }

        ani.Stop();
        ani.Rebind();
        ani.Play("press_succeed_fx");
    }

    public void ShowCombo(int number, bool isPlayComboEffect = true)
    {
        GameObject _comboAnimation = this.comboAnimationNormal;
        if (number >= COMBO_CHANGE_LIM)
        {
            if (number == COMBO_CHANGE_LIM)
            {
                // SpineActionController.Play (ACTION_KEYS.COMEOUT, this.comboAnimationNormal);
                this.comboAnimationNormal.SetActive(false);
            }

            _comboAnimation = this.comboAnimationHigh;
        }

        if (!_comboAnimation.activeSelf)
        {
            _comboAnimation.SetActive(true);
            SpineActionController.Play(ACTION_KEYS.COMEIN, _comboAnimation);
        }

        CharPanel.Instance.ShowCombo(number, isPlayComboEffect);
    }

    public void StopCombo()
    {
        this.StopCombo(this.comboAnimationNormal);
        this.StopCombo(this.comboAnimationHigh);
        CharPanel.Instance.IsShowingCombo = false;
        CharPanel.Instance.StopCombo();
    }

    private void StopCombo(GameObject comboAni)
    {
        string actname = SpineActionController.CurrentAnimationName(comboAni);
        if (comboAni.activeSelf && actname != "end")
        {   //
            SpineActionController.Play(ACTION_KEYS.COMEOUT, comboAni);
        }

        if (comboAni.activeSelf && actname == null)
        {
            comboAni.SetActive(false);
        }
    }

    private void InitAttackFxOffset()
    {
        Vector3 pos = GirlManager.Instance.GetCurrentGirlPositon();
        this.playResultOffset = new float[(int)GameMusic.CRITICAL + 1];
        for (int i = 0; i < this.playResultOffset.Length; i++)
        {
            GameObject fx = this.playResult[i];
            if (fx == null)
            {
                continue;
            }

            this.playResultOffset[i] = pos.y - fx.transform.position.y;
        }
    }

    public void ShowPlayResult(uint resultCode)
    {
        for (int i = 0; i < this.playResult.Length; i++)
        {
            GameObject _playResult = this.playResult[i];
            if (_playResult == null)
            {
                continue;
            }

            _playResult.SetActive(false);
        }

        int ctr = BattleRoleAttributeComponent.Instance.Host.GetDynamicIntByKey(SignKeys.CTR);
        if (ctr > 0)
        {
            resultCode = GameMusic.CRITICAL;
            //暴击的事件在此
        }

        int _result = (int)resultCode;
        GameObject pResult = this.playResult[_result];
        if (pResult == null)
        {
            return;
        }

        pResult.SetActive(true);
        SpineActionController.Play(ACTION_KEYS.COMEIN, pResult);

        // Attack effect follow y of girl.
        float offset = this.playResultOffset[_result];
        Vector3 pos = GirlManager.Instance.GetCurrentGirlPositon();
        pResult.transform.position = new Vector3(pResult.transform.position.x, pos.y - offset, pResult.transform.position.z);
    }
}