using DG.Tweening;
using DYUnityLib;
using FormulaBase;
using GameLogic;
using System.Collections;
using System.Linq;
using UnityEngine;

public class FightMenuPanel : MonoBehaviour
{
    public TweenFill m_HpNew;         //玩家新的血条移动动画
    public TweenFill m_HpNewBlack;    //玩家血条新的背景
    public TweenAlpha m_HeatLightAlpha;   //心型闪光
    public TweenAlpha m_HpLightAlpha;     //血条闪光

    public TweenFill m_FerverBar;     //Ferver条动画

    public UISlider m_SongSilder;         //音乐进度百分比
    public UILabel m_MusicName;           //音乐名字
    public UILabel m_MusicAuthor;         //音乐作者名字

    public GameObject m_ReGoAni;          //准备开始动画
    public GameObject m_MusicInfo;        //音乐信息
    public TweenPosition m_UpGround;      //抬头信息的设置
    public TweenPosition m_DownGround;    //下方信息栏
    public TweenPosition m_SongInfo;	  //文件信息
    public TweenPosition twnAchievement;
    public UITweener[] twnTrophys;
    public GameObject trophyShow;

    public TweenScale m_FeverScaleLabel;                        //Fever文字的缩放
    public TweenAlpha m_FeverLabelAlpha;                        //Fever文字的透明度

    [HideInInspector]
    public bool isAchieve;

    private bool PlayHeartAnimation = false;

    private const float SONG_PROGRESS_MIN = 0.1f;
    private static FightMenuPanel instance = null;
    private Sequence m_Seq;

    public static FightMenuPanel Instance
    {
        get
        {
            return instance;
        }
    }

    //************************************************
    private IEnumerator ReadygoAniPlay()
    {
        yield return new WaitForSeconds(1.8f);

        m_ReGoAni.SetActive(true);
    }

    //bool tttttt=true;
    private IEnumerator __OnEnable()
    {
        yield return new WaitForSeconds(0.01f);

        SoundEffectComponent.Instance.SayByCurrentRole(GameGlobal.SOUND_TYPE_STAGE_START);
    }

    private IEnumerator ShowSongInfo()
    {
        yield return new WaitForSeconds(4.33f);

        m_SongInfo.ResetToBeginning();
        m_SongInfo.PlayForward();
    }

    private IEnumerator ShowTopInfo()
    {
        yield return new WaitForSeconds(2.66f);

        this.m_UpGround.gameObject.SetActive(true);
        this.m_DownGround.gameObject.SetActive(true);

        this.m_UpGround.ResetToBeginning();
        this.m_UpGround.PlayForward();

        this.m_DownGround.ResetToBeginning();
        this.m_DownGround.PlayForward();
    }

    private IEnumerator HeroComeOut()
    {
        yield return new WaitForSeconds(3.16f);

        GirlManager.Instance.ComeOut();
    }

    private IEnumerator FullHp()
    {
        yield return new WaitForSeconds(3.16f);

        this.PlaySetHpAnimation = true;

        this.m_HpNew.from = 0;
        this.m_HpNew.to = 1f;
        this.m_HpNew.ResetToBeginning();
        this.m_HpNew.Play(true);

        this.m_HpNewBlack.from = 0;
        this.m_HpNewBlack.to = 1f;
        this.m_HpNewBlack.ResetToBeginning();
        this.m_HpNewBlack.Play(true);
    }

    private IEnumerator WaitForFiveS()
    {
        yield return new WaitForSeconds(5f);

        StageBattleComponent.Instance.GameStart(null, 0, null);
    }

    private void OnEnable()
    {
        instance = this;
        //m_pause.enabled = false;
        this.PlayHeartAnimation = false;
        m_MusicInfo.SetActive(true);

        this.StartCoroutine(this.FullHp());
        this.StartCoroutine(this.ReadygoAniPlay());
        this.StartCoroutine(this.ShowTopInfo());
        this.StartCoroutine(this.ShowSongInfo());
        this.StartCoroutine(this.WaitForFiveS());
        this.StartCoroutine(this.HeroComeOut());
        this.StartCoroutine(this.__OnEnable());

        isAchieve = TaskStageTarget.Instance.IsUnLockAllDiff(TaskStageTarget.Instance.Host);
        twnAchievement.gameObject.SetActive(false);
        if (!isAchieve)
        {
            if (m_Seq != null)
            {
                m_Seq.Kill();
            }
            DOTweenUtils.Delay(() =>
            {
                m_Seq = DOTweenUtils.Update(() =>
                {
                    if (twnAchievement != null)
                    {
                        twnAchievement.gameObject.SetActive(true);
                        twnAchievement.Play(true);
                    }
                    twnTrophys.ToList().ForEach(t =>
                    {
                        if (t)
                        {
                            t.gameObject.SetActive(true);
                            t.enabled = true;
                            t.ResetToBeginning();
                            t.Play(true);
                        }
                    });
                }, () => TaskStageTarget.Instance.IsAchieveNow());
            }, 1.0f);
        }
        trophyShow.SetActive(!isAchieve);
    }

    private void Start()
    {
        instance = this;

        this.m_HpNew.ResetToBeginning();
        this.m_HpNewBlack.ResetToBeginning();
        this.SetFerver(0);

        this.m_UpGround.gameObject.SetActive(false);
        this.m_DownGround.gameObject.SetActive(false);
        /*
		this.InitScore ();
		m_MusicName.text = (string)FormulaBase.StageBattleComponent.Instance.GetStageDesName ();
		m_MusicAuthor.text = (string)FormulaBase.StageBattleComponent.Instance.GetStageAuthorName ();
		this.SetScore (0);
		this.SetProgressBar (0);
		this.ChangeSongProgress (0f);
		this.m_SongSilder.onDragFinished = this.OnChangeSongProgress;
		this.m_SongSilder.gameObject.SetActive (GameGlobal.IS_DEBUG);

		showFullHp = false;
		this.m_UpGround.gameObject.SetActive (false);
		this.m_DownGround.gameObject.SetActive (false);

		StartCoroutine ("FullHp");
		*/
    }

    private void OnDestroy()
    {
        m_Seq.Kill();
    }

    public void UnShow()
    {
        this.gameObject.SetActive(false);
    }

    public void OnStageEnd()
    {
        //this.m_pause.enabled = false;
    }

    #region 设置战斗分数

    /// <summary>
    /// 数字滚动时的替换
    /// </summary>
    /// <returns>返回结果.</returns>
    /// <param name="str">需要改变的string.</param>
    /// <param name="index">改变的位置.</param>
    private static string ReplaceChar(string str, int index)
    {
        char[] carr = str.ToCharArray();
        int temp = (int)str[index];
        temp++;
        if (temp >= 58)
            temp = 48;
        carr[index] = (char)temp;
        return new string(carr);
    }

    #endregion 设置战斗分数

    #region 设置Hp

    private bool PlaySetHpAnimation;        //玩家设置血量的HP
    private float desHp;

    //public float tempabs=0.01f;
    private bool HpAddOnce = true;

    public void SetHp(float hpRate, float addValue = 0f)
    {
        if (this.HpAddOnce)
        {
            this.HpAddOnce = false;
            return;
        }

        if (addValue == 0f)
        {
            return;
        }

        if (hpRate < 0)
            hpRate = 0f;
        if (hpRate > 1f)
        {
            hpRate = 1f;
        }

        //扣血
        if (addValue < 0)
        {
            this.m_HpNew.duration = 0.1f;

            this.m_HpNewBlack.from = this.m_HpNewBlack.value;
            this.m_HpNewBlack.to = hpRate;
            this.m_HpNewBlack.ResetToBeginning();
            this.m_HpNewBlack.Play(true);
        }
        else
        {
            this.m_HpNew.duration = 0.2f;
            if (this.PlayHeartAnimation)
            {
                return;
            }

            this.m_HeatLightAlpha.ResetToBeginning();
            this.m_HeatLightAlpha.PlayForward();
            this.m_HpLightAlpha.ResetToBeginning();
            this.m_HpLightAlpha.PlayForward();
        }

        this.m_HpNew.from = this.m_HpNew.value;
        this.m_HpNew.to = hpRate;
        this.m_HpNew.ResetToBeginning();
        this.m_HpNew.Play(true);
    }

    public void FInishHeartAniMation()
    {
        PlayHeartAnimation = false;
    }

    public void SetFerver(float rate)
    {
        this.m_FerverBar.from = this.m_FerverBar.value;
        this.m_FerverBar.to = rate;
        this.m_FerverBar.ResetToBeginning();
        this.m_FerverBar.PlayForward();
    }

    #endregion 设置Hp

    /// <summary>
    /// 改变音乐进度后要改变什么事
    /// </summary>
    /// <param name="_value">_value.(0-1)</param>
    public void ChangeSongProgress(float _value)
    {
        if (this.m_SongSilder == null)
        {
            return;
        }

        float interval = _value - this.m_SongSilder.value;
        if (AudioManager.Instance != null && (interval > SONG_PROGRESS_MIN || interval < -SONG_PROGRESS_MIN))
        {
            Time.timeScale = 0;
            AudioManager.Instance.PauseBackGroundMusic();
        }

        //两种情况一种 调用 需要设置值 一种是自己改变
        m_SongSilder.value = _value;
    }
}