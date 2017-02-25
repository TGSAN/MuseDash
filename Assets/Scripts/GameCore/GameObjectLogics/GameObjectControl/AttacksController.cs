using FormulaBase;
using GameLogic;
using System.Collections;
using UnityEngine;

public class AttacksController : MonoBehaviour
{
    public const int FAIL_PLAY_IDX1 = -1;
    public const int FAIL_PLAY_IDX2 = -2;

    private const string JUMP_AUDIO = "Audio/GirlEffects/jump";

    private static AttacksController instance = null;

    public static AttacksController Instance
    {
        get
        {
            return instance;
        }
    }

    private Vector3 sceneCameraOrgPosition;

    [SerializeField]
    public EffectManager attackEffectManager;

    public GameObject sceneCamera;
    public Animator sceneCameraAnimator;
    public MobileBloom sceneHurtBloom;
    public AudioSource girlAudioSource;

    // Use this for initialization
    private void Start()
    {
        instance = this;
        this.sceneCameraOrgPosition = this.sceneCameraAnimator.transform.position;
        SoundEffectComponent.Instance.SayByCurrentRole(GameGlobal.SOUND_TYPE_ENTER_STAGE);
    }

    public void ShowAttack(int id, uint resultCode, uint actionType, bool isContinue = false)
    {
        switch (resultCode)
        {
            case GameMusic.MISS:
                this.BeAttacked();
                break;

            case GameMusic.COOL:
            case GameMusic.GREAT:
            case GameMusic.PERFECT:
                this.ShowAttackEffect(id, resultCode, actionType, isContinue);
                break;

            case GameMusic.JUMPOVER:
                this.PlayJumpAnim();
                break;

            default:
                if (actionType == GameMusic.TOUCH_ACTION_SIGNLE_PRESS)
                {
                    this.PlayRandomHitNothingAnim();
                    if (id == FAIL_PLAY_IDX1)
                    {
                        TaskStageTarget.Instance.AddHitEmptyCount(1);
                        BattleRoleAttributeComponent.Instance.FireSkill(GameMusic.NONE);
                    }
                }
                else
                {
                    this.PlayAttackAnim(-1, actionType, isContinue, resultCode);
                }

                AudioManager.Instance.PlayHitNothing();
                break;
        }
    }

    public void OnShowAttack(int idx, uint result)
    {
        GameObject obj = BattleEnemyManager.Instance.GetObj(idx);
        if (obj == null)
        {
            // Debug.Log("Game music scene remove null obj at " + idx);
            return;
        }

        ArrayList musicData = StageBattleComponent.Instance.GetMusicData();
        MusicData md = (MusicData)musicData[idx];
        bool isBreak = BattleEnemyManager.Instance.IsDead(idx);
        var cubeController = obj.GetComponent<BaseEnemyObjectController>();
        if (cubeController != null)
        {
            decimal shotPauseTime = 0m;
            if (!isBreak)
            {
                shotPauseTime = md.nodeData.life;
            }

            md.SetShotPause(shotPauseTime);
            musicData[idx] = md;

            cubeController.SetShotPause(shotPauseTime);
            cubeController.AttackedSuccessful(result, isBreak);
            if (ArmActionController.Instance != null)
            {
                ArmActionController.Instance.OnControllerAttacked((int)result, isBreak);
            }
        }
    }

    public void BeAttacked()
    {
        // show hurt camera shake animation
        this.sceneCameraAnimator.gameObject.transform.position = this.sceneCameraOrgPosition;
        this.sceneCameraAnimator.Stop();
        this.sceneCameraAnimator.Rebind();
        //this.sceneCameraAnimator.Play("Shake");

        //AudioManager.Instance.PlayMissRandom ();
    }

    public void ShowHurtBloom(bool isShow)
    {
        /*
		if (this.sceneHurtBloom == null) {
			return;
		}

		//if (!this.sceneHurtBloom.Supported ()) {
		//	return;
		//}

		this.sceneHurtBloom.enabled = isShow;
		if (!isShow) {
			return;
		}

		this.ShowRadialBlur(false);
		this.ShowPressGirl (false);
		this.sceneHurtBloom.OnDamage ();
		*/
    }

    public void ShowRadialBlur(bool isShow)
    {
        /*
		if (this.sceneCamera != null) {
			this.sceneCamera.SetActive (!isShow);
		}

		if (this.sceneCameraRb != null) {
			this.sceneCameraRb.SetActive (isShow);
		}

*/
        if (isShow)
        {
            this.ShowHurtBloom(false);
        }
    }

    public void ShowPressGirl(bool isShow)
    {
        this.attackEffectManager.ShowPressGirlEffect(isShow);
    }

    private void ShowAttackEffect(int id, uint resultCode, uint actionType, bool isContinue)
    {
        string effect = BattleEnemyManager.Instance.GetHitEffectByIdx(id);
        if (effect == "1" && this.attackEffectManager != null)
        {
            this.attackEffectManager.ShowPlayResult(resultCode);
        }

        this.PlayAttackAnim(id, actionType, isContinue, resultCode);

        string audioName = BattleEnemyManager.Instance.GetNodeAudioByIdx(id);
        AudioManager.Instance.PlayGirlHitByName(audioName);
    }

    private void PlayAttackAnim(int id, uint actionType, bool isContinue, uint result)
    {
        string actKey = BattleEnemyManager.Instance.GetNodeRoleActKeyByIdx(id);
        actKey = AtkScore(actKey, result);
        if (isContinue)
        {
            GirlManager.Instance.AttacksWithoutExchange(result, actKey);
        }
        else
        {
            GirlManager.Instance.AttackWithExchange(result, actKey);
        }
    }

    private string AtkScore(string actKey, uint result)
    {
        if (actKey == "char_atk")
        {
            actKey = ACTION_KEYS.ATTACK_PERFECT;
            if (result == GameMusic.GREAT)
            {
                actKey = ACTION_KEYS.ATTACK_GREAT;
            }
        }
        return actKey;
    }

    private void PlayRandomHitNothingAnim()
    {
        string actKey = ACTION_KEYS.ATTACK_MISS;
        if (GameGlobal.gGameTouchPlay.IsJump() && !GirlManager.Instance.IsJumpingAction())
        {
            actKey = ACTION_KEYS.JUMP;
        }

        GirlManager.Instance.AttackWithExchange(GameMusic.NONE, actKey);
    }

    public void PlayJumpAnim()
    {
        GirlManager.Instance.AttacksWithoutExchange(GameMusic.JUMPOVER, ACTION_KEYS.JUMP);
        AudioManager.Instance.PlayGirlHitByName(JUMP_AUDIO);
    }
}