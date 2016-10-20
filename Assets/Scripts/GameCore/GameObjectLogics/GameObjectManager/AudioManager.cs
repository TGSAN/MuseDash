using FormulaBase;
using GameLogic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager
{
    private static AudioManager instance = null;

    private AudioSource backGroundMusic;
    private AudioSource girlEffect;
    private AudioSource nodeEffect;
    private AudioClip girlJump;
    private AudioClip girlSlide;

    private AudioClip[] girlAttacks;
    private AudioClip[] girlEffects;

    private Dictionary<string, AudioClip> sceneObjectEffects;
    private AudioClip hurtEffect;
    private Dictionary<string, AudioClip> audioDic;
    private ArrayList audios = new ArrayList();

    private float volume = 1f;
    private string playingMusicName = null;

    public const string MUSIC_PATH = "Audio/Musics/";

    //public static string MUSIC_PATH = AssetBundleFileMangager.FileLoadResPath + "/resources/audio/musics/";
    public const string UI_MUSIC_PATH = "Audio/UIMusic/";

    private const string HIT_EFFECT_PATH = "Audio/GirlEffects/hit_";

    #region getter&setter

    public string PlayingMusic
    {
        get
        {
            return this.playingMusicName;
        }
        set
        {
            this.playingMusicName = value;
        }
    }

    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new AudioManager();
            }
            return instance;
        }
    }

    public float GetBackGroundMusicLength()
    {
        if (this.backGroundMusic == null || this.backGroundMusic.clip == null)
        {
            return -1;
        }

        return this.backGroundMusic.clip.length;
    }

    public int GetBackGroundMusicSample()
    {
        if (this.backGroundMusic == null || this.backGroundMusic.clip == null)
        {
            return 0;
        }

        return this.backGroundMusic.timeSamples;
    }

    public float GetBackGroundMusicTime()
    {
        if (this.backGroundMusic == null || this.backGroundMusic.clip == null)
        {
            return 0;
        }

        return this.backGroundMusic.time;
    }

    public float GetBackGroundMusicLeave()
    {
        return this.GetBackGroundMusicLength() - this.GetBackGroundMusicTime();
    }

    public void SetBackGroundMusic(string fileName)
    {
        //string pathName = (MUSIC_PATH + fileName);
        this.PlayingMusic = fileName;
        this.SetBgmSource();
        if (this.backGroundMusic == null)
        {
            return;
        }

        //this.backGroundMusic.clip.LoadAudioData ();
        if (this.backGroundMusic.clip == null)
        {
            this.backGroundMusic.clip = null;
            //SceneAudioManager.Instance.Load (pathName + ".ogg", this.backGroundMusic);
            this.backGroundMusic.clip = Resources.Load<AudioClip>(this.PlayingMusic);
            Debug.Log("Load battle bgm " + this.PlayingMusic + " succeed.");
        }
    }

    public void SetEffectVolume(float volume)
    {
        this.volume = volume;
        this.girlEffect.volume = this.volume;
    }

    public void SetBgmVolume(float volume)
    {
        this.backGroundMusic.volume = volume;
    }

    public float GetBgmVolume()
    {
        return this.backGroundMusic.volume;
    }

    public void SetBgmTimeScale(float ts)
    {
        if (this.backGroundMusic == null)
        {
            return;
        }

        this.backGroundMusic.pitch = ts;
    }

    #endregion getter&setter

    public AudioManager()
    {
        // this.ReloadAllResource ();
    }

    public void ReloadAllResource()
    {
        if (this.audioDic != null)
        {
            this.audioDic.Clear();
            this.audioDic = null;
        }

        this.audioDic = new Dictionary<string, AudioClip>();
        this.SetBgmSource();

        int lenAtk = 4;
        this.girlAttacks = new AudioClip[lenAtk];
        this.girlEffect = SceneAudioManager.Instance.role;
        for (int i = lenAtk - 1; i >= 0; i--)
        {
            this.girlAttacks[i] = Resources.Load(HIT_EFFECT_PATH + (i + 1)) as AudioClip;
        }

        this.girlJump = Resources.Load("Audio/GirlEffect/jump") as AudioClip;
        this.girlSlide = Resources.Load("Audio/GirlEffect/slide") as AudioClip;

        int lenDmg = 14;
        this.girlEffects = new AudioClip[1 + lenDmg];
        this.girlEffects[0] = Resources.Load("Audio/GirlEffects/hit_notthing") as AudioClip;

        this.nodeEffect = SceneAudioManager.Instance.enemy;
        if (!this.nodeEffect)
        {
            Debug.Log("====== nodeAudioSource is unavaliable ======");
        }

        this.hurtEffect = Resources.Load("Audio/GirlEffects/Hurt") as AudioClip;
        if (hurtEffect == null)
        {
            Debug.Log("Load hurt effect failed!");
        }
        else
        {
            Debug.Log("Load hurt effect success!");
        }
    }

    public void InitSceneObjectAudio()
    {
        this.sceneObjectEffects = new Dictionary<string, AudioClip>();

        // 散乱音效预加载
        this.sceneObjectEffects["Audio/GirlEffects/Coin"] = Resources.Load("Audio/GirlEffects/Coin") as AudioClip;
        this.sceneObjectEffects["Audio/GirlEffects/ball_kill"] = Resources.Load("Audio/GirlEffects/ball_kill") as AudioClip;
    }

    public void PreLoadSceneObjectAudio(string audioName)
    {
        if (this.sceneObjectEffects.ContainsKey(audioName))
        {
            return;
        }

        var clip = Resources.Load(audioName) as AudioClip;
        this.sceneObjectEffects[audioName] = clip;
    }

    public void AddAudioResource(string name)
    {
        if (name == null || name.Length < 2)
        {
            return;
        }
        if (!audioDic.ContainsKey(name))
        {
            var clip = Resources.Load(name) as AudioClip;
            audioDic.Add(name, clip);
            this.girlEffect.PlayOneShot(clip, 0.0f);
        }
    }

    public void PlayBackGroundMusic()
    {
        if (this.backGroundMusic == null)
        {
            return;
        }

        this.backGroundMusic.Play();
        Debug.Log("play background music " + this.backGroundMusic.clip.length);
    }

    public void PlayBackGroundMusicAtTime(float time)
    {
        if (this.backGroundMusic == null)
        {
            return;
        }

        this.ResumeBackGroundMusic();
        this.backGroundMusic.time = time;
    }

    public void PlayBackGroundMusicAt(float rate)
    {
        if (this.backGroundMusic == null)
        {
            return;
        }

        int smp = (int)(this.backGroundMusic.clip.samples * rate);
        this.backGroundMusic.timeSamples = smp;
    }

    public void StopBackGroundMusic()
    {
        this.backGroundMusic.Stop();
        Debug.Log("Stop background music");
    }

    public void ResetBackGroundMusic()
    {
        if (this.backGroundMusic == null)
        {
            this.SetBackGroundMusic(this.playingMusicName);
            return;
        }

        if (this.backGroundMusic == null)
        {
            return;
        }

        this.backGroundMusic.time = 0;
        this.backGroundMusic.Play();
        this.backGroundMusic.Stop();
    }

    public bool IsBackGroundMusicPlaying()
    {
        if (this.backGroundMusic == null || this.backGroundMusic.clip == null)
        {
            return false;
        }

        return this.backGroundMusic.isPlaying;
    }

    public void SetBackGroundMusicProgress(float second)
    {
        if (second < 0)
        {
            return;
        }

        if (second > this.backGroundMusic.clip.length)
        {
            return;
        }

        this.backGroundMusic.time = second;
        this.backGroundMusic.Play();
    }

    public void SetBackGroundMusicTimeScale(float tsc)
    {
        if (this.backGroundMusic == null)
        {
            return;
        }

        this.backGroundMusic.pitch = tsc;
    }

    public void PauseBackGroundMusic()
    {
        if (this.backGroundMusic == null)
        {
            return;
        }

        this.backGroundMusic.Pause();
    }

    public void ResumeBackGroundMusic()
    {
        if (this.backGroundMusic == null)
        {
            return;
        }

        this.backGroundMusic.UnPause();
    }

    // -------------------------------------------------------
    public void PlayGirlHitByName(string name)
    {
        if (name == null || name.Length < 2)
        {
            return;
        }
        Debug.Log(name);
        if (this.audioDic.ContainsKey(name))
        {
            this.girlEffect.PlayOneShot(this.audioDic[name]);
        }
    }

    public void PlayHitNothing()
    {
        if (GameGlobal.gGameTouchPlay.IsJump())
        {
            SoundEffectComponent.Instance.SayByCurrentRole(GameGlobal.SOUND_TYPE_UI_JUMP_MISS);
            return;
        }

        SoundEffectComponent.Instance.SayByCurrentRole(GameGlobal.SOUND_TYPE_UI_ATTACK_MISS);

        //this.girlEffect.clip = this.girlEffects [0];
        //this.girlEffect.Play ();
    }

    public void PlayMissRandom()
    {
        //int randIdx = Random.Range (1, this.girlEffects.Length);
        //this.girlEffect.clip = this.girlEffects [randIdx];
        //this.girlEffect.Play ();
    }

    public void PlayHurtEffect()
    {
        SoundEffectComponent.Instance.SayByCurrentRole(GameGlobal.SOUND_TYPE_HURT);
        //if(this.hurtEffect == null){
        //	return;
        //}else{
        // Debug.Log("Get hurt!");
        //}
        //this.girlEffect.clip = hurtEffect;
        //this.girlEffect.Play();
    }

    private void SetBgmSource()
    {
        if (SceneAudioManager.Instance == null)
        {
            return;
        }

        this.backGroundMusic = SceneAudioManager.Instance.bgm;
        this.girlEffect = SceneAudioManager.Instance.role;
        this.backGroundMusic.loop = false;
    }
}