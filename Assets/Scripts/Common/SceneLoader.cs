using DYUnityLib;
using GameLogic;
using System.Collections;
using UnityEngine;

/*
 * How to use:
 * SceneLoader.SetLoadInfo("SceneName");
 * Application.LoadLevel("LoadingScene");
 *
 * then, SceneName will be load async and will be show after LoadingScene
 */

public class SceneLoader : SingletonMonoBehaviour<SceneLoader>
{
    // Load end value, unity say that 90% is already save as 100% ... wtf
    // If want to 100%, allowSceneActivation must be true.
    private const float LOAD_FIN = 0.9f;

    // Load percentage count down timer
    private const float LOAD_TIME_MAX = 10.0f;

    private const int WAIT_COUNTER_LIM = 100;

    private static string __sceneName = null;
    private static string[] _str = { "interlude_01", "interlude_02", "interlude_03", "interlude_04", "interlude_05", "interlude_06" };

    public oTimer loadTimer = null;
    private Hashtable loadedMap = new Hashtable();

    // 场景切换等待异常计数器
    private int waitCounter = 0;

    // private static ArrayList __resoucesList = new ArrayList();
    private AsyncOperation asyncOper = null;

    //public UISprite m_sprite;
    public UITexture m_Texture;

    public TweenAlpha twAlpha;
    public Camera camera;

    private void Start()
    {
        var mainTexture = Resources.Load("ui_resource/loading/atlas/" + _str[Random.Range(0, _str.Length - 1)]) as Texture;
        m_Texture.mainTexture = mainTexture;
        this.LoadSceneAsync();
        ScreenFit.CameraFit(this.camera);
    }

    public static void SetLoadInfo(ref string sceneName)
    {
        __sceneName = sceneName;
    }

    private void TimerStepTrigger(object sender, uint triggerId, params object[] args)
    {
        if (this.loadTimer == null)
        {
            return;
        }

        if (this.asyncOper == null)
        {
            return;
        }

        // args [0] is passed tick of this timer
        // decimal ts = (decimal)args [0];

        // Debug.Log ("Load progress " + this.asyncOper.progress);
        if (this.asyncOper.progress < LOAD_FIN)
        {
            return;
        }

        if (ExpandBmobHeartBeat.Instance != null && ExpandBmobHeartBeat.Instance.IsUpdateWaitting())
        {
            this.waitCounter += 1;
            return;
        }

        if (this.waitCounter == WAIT_COUNTER_LIM)
        {
            // tips
            Debug.Log("数据保存等待时间过长");
        }

        this.loadTimer.Cancle();
        FormulaBase.FomulaHostManager.Instance.ClearNotifyUiHelper();
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
        /*
		twAlpha.from = 1f;
		twAlpha.to = 0f;
		twAlpha.enabled = true;
		twAlpha.ResetToBeginning ();
		twAlpha.onFinished.Clear ();
		twAlpha.onFinished.Add (new EventDelegate (this.OnLoaded));
		twAlpha.Play ();
		*/
    }

    private void OnLoaded()
    {
        this.asyncOper.allowSceneActivation = true;
    }

    private void LoadSceneAsync()
    {
        if (__sceneName == null)
        {
            return;
        }

        Resources.UnloadUnusedAssets();
        //System.GC.Collect (0, System.GCCollectionMode.Forced);

        this.InitTimer();
        this.asyncOper = Application.LoadLevelAsync(__sceneName);
        this.asyncOper.allowSceneActivation = !this.loadedMap.ContainsKey(__sceneName);
        if (!this.asyncOper.allowSceneActivation)
        {
            this.loadedMap.Add(string.Copy(__sceneName), 1);
        }

        this.loadTimer.Run();
        __sceneName = null;
    }

    private void InitTimer()
    {
        this.waitCounter = 0;
        this.loadTimer = null;
        this.loadTimer = this.gameObject.AddComponent<oTimer>();
        this.loadTimer.ClearTickEvent();
        this.loadTimer.Init(0, LOAD_TIME_MAX, oTimer.TIMER_TYPE_STEP_ARRAY);
        this.loadTimer.AddTickEvent(0, GameGlobal.LOADING);

        // Timer event of step
        EventTrigger stPress = gTrigger.RegEvent(GameGlobal.LOADING);
        stPress.Trigger += TimerStepTrigger;
    }

    /*
	void OnEnable()
	{
		Time.timeScale=1;
	}

	void OnDisable()
	{
		Time.timeScale=0;
	}
	*/
}