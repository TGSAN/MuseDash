using FormulaBase;
using LitJson;

/// UI分析工具自动生成代码
/// PnlWelcomeUI主模块
///
using System;
using System.Collections;
using UnityEngine;

namespace PnlWelcome_new
{
    public class PnlWelcome_new : UIPhaseBase
    {
        private static PnlWelcome_new instance = null;

        public static PnlWelcome_new Instance
        {
            get
            {
                return instance;
            }
        }

        private void Start()
        {
            instance = this;
            //StartCoroutine (this.__Login ());
        }

        public override void OnShow()
        {
        }

        public override void OnHide()
        {
        }

        // ----------------------------------------------------------------------------------------------------
        // ----------------------------------------------------------------------------------------------------
        // ----------------------------------------------------------------------------------------------------
        public const string LOGIN_READY = "哔哩哔哩";

        public const string LOGIN_OK = "哔哩哔哩哔哩哔哩";
        public const string LOGIN_WAIT = "哔----------";
        public const string LOGIN_FAILED = "很遗憾，网络掉线了";
        public const string LOAD_DATA_OK = "今天你吃药了吗";

        private const float WAIT = 2f;
        private const float startWait = 0.1f;
        private const string SAVE_FILE_NAME = "logininfo";
        private string userName = null;
        private string passWord = null;
        private string verfiCode = null;
        private string phoneNumber = null;

        public Coroutine waitCoroutine = null;

        private IEnumerator __Login()
        {
            yield return new WaitForSeconds(startWait);

            this.Login();
        }

        /// <summary>
        /// Login this instance.
        ///
        /// If no local user info, use random info to signup.
        /// 登陆/注册
        ///
        /// 如果在Bmob perfabs中没有填写账号密码或者外部传入账号密码，
        /// 则默认使用随机生成账号 登陆/注册
        ///
        /// 登陆/注册成功后，会自动加载该账户全部数据
        /// </summary>
        public void Login()
        {
            /*JsonData loadData = GameLogic.GameGlobal.gConfigLoader.LoadPrefs(SAVE_FILE_NAME);
            if (loadData == null ||
                loadData.GetJsonType() == JsonType.None ||
                !loadData.Keys.Contains("username") ||
                loadData["username"] == null ||
                loadData["username"].ToString() == string.Empty
            )
            {
                this.userName = ObjectId.NewObjectId();
                this.passWord = ExpandBmobLogin.Instance.password;
                LoginResponseDelegate lrd = new LoginResponseDelegate(this.SingupResponse);
                ExpandBmobLogin.Instance.Signup(this.userName, this.passWord, lrd);
            }
            else
            {
                string username = loadData["username"].ToString();
                string passworld = loadData["password"].ToString();

                LoginResponseDelegate lrd = new LoginResponseDelegate(this.LoginResponse);
                ExpandBmobLogin.Instance.Login(username, null, lrd);
            }

            CommonPanel.GetInstance().ShowText(LOGIN_READY);

            if (this.waitCoroutine != null)
            {
                this.StopCoroutine(this.waitCoroutine);
            }

            CommonPanel.GetInstance().ShowText(LOGIN_WAIT, () =>
            {
                this.waitCoroutine = this.StartCoroutine(this.OnLoginWait());
                return;
            });*/
        }

        public string GetVerficationCode()
        {
            return this.verfiCode;
        }

        public void SetVerficationCode(string vcode)
        {
            this.verfiCode = vcode;
            this.SaveSaveData();
        }

        public void LoadSaveData()
        {
            JsonData loadData = GameLogic.GameGlobal.gConfigLoader.LoadPrefs(SAVE_FILE_NAME);
            if (loadData == null || loadData.GetJsonType() == JsonType.None)
            {
                this.userName = ExpandBmobLogin.Instance.username;
                this.passWord = ExpandBmobLogin.Instance.password;
                //this.verfiCode = VerificationScript.Instance.verificateCode;

                return;
            }

            if (loadData.Keys.Contains("username"))
            {
                this.userName = loadData["username"].ToString();
            }

            if (loadData.Keys.Contains("password"))
            {
                this.passWord = loadData["password"].ToString();
            }

            if (loadData.Keys.Contains("vcode"))
            {
                this.verfiCode = loadData["vcode"].ToString();
            }

            Debug.Log("Load local account : " + this.userName + " / " + this.verfiCode);
        }

        public void LoginSucceed()
        {
            Application.LoadLevel(1);
        }

        private void SaveSaveData()
        {
            JsonData saveData = new JsonData();

            this.userName = ExpandBmobLogin.Instance.username;
            this.passWord = ExpandBmobLogin.Instance.password;
            this.verfiCode = VerificationScript.Instance.verificateCode;
            saveData["username"] = this.userName;
            saveData["password"] = this.passWord;
            saveData["vcode"] = this.verfiCode;
            GameLogic.GameGlobal.gConfigLoader.SavePrefs(SAVE_FILE_NAME, saveData);
            Debug.Log("Save local account : " + this.userName + " / " + this.verfiCode);
        }

        /// <summary>
        /// Logins the response.
        /// </summary>
        /// <param name="user">User.</param>
        private void LoginResponse(bool result, cn.bmob.exception.BmobException excp, ExpandBmobUser user)
        {
            if (!result)
            {
                // send msg
                // CommonPanel.GetInstance ().ShowText (excp.Message);}
                CommonPanel.GetInstance().ShowText(LOGIN_FAILED, () =>
                {
                    if (this.waitCoroutine != null)
                    {
                        this.StopCoroutine(this.waitCoroutine);
                    }

                    this.StartCoroutine(this.OnLoginFailed());
                    return;
                });

                return;
            }

            if (this == null)
            {
                return;
            }

            StartCoroutine(this.LoadLevel());
        }

        /// <summary>
        /// Singups the response.
        /// </summary>
        /// <param name="user">User.</param>
        private void SingupResponse(bool result, cn.bmob.exception.BmobException excp, ExpandBmobUser user)
        {
            if (!result)
            {
                // send msg
                CommonPanel.GetInstance().ShowText(LOGIN_FAILED, () =>
                {
                    this.StartCoroutine(this.OnLoginFailed());
                    return;
                });

                return;
            }

            this.SaveSaveData();
            this.LoginResponse(result, excp, user);
            VerificationScript.Instance.SetVerificationCodeOwner(this.userName);
        }

        private IEnumerator LoadLevel()
        {
            yield return new WaitForSeconds(startWait);

            HttpEndResponseDelegate rsp = new HttpEndResponseDelegate(this.LoadAllDataResponse);
            // Load all data of account from data base or local.
            FormulaBase.FomulaHostManager.Instance.LoadAllHost(rsp);
            CommonPanel.GetInstance().ShowText(LOGIN_OK);
        }

        /// <summary>
        /// Loads all data response.
        /// 登陆、数据加载完成，切换场景
        /// </summary>
        /// <param name="response">Response.</param>
        private void LoadAllDataResponse(cn.bmob.response.EndPointCallbackData<Hashtable> response)
        {
            this.LoginSucceed();
            if (PlayerPrefs.HasKey("LoginTimeSF"))
            {  //有值
                int temp = PlayerPrefs.GetInt("LoginTimeSF");

                UIPerfabsManage.g_Instan.SetOffLineTime(ExpandBmobLogin.ServerTimeStamp.Get() - temp);

                //Debug.LogWarning("use the Time:"+(-temp));
            }
            else
            {
                PlayerPrefs.SetInt("LoginTimeSF", ExpandBmobLogin.ServerTimeStamp.Get());
            }

            //		Debug.Log("加载成功"+ExpandBmobLogin.ServerTime+"int Time"+);
            CommonPanel.GetInstance().ShowText(LOAD_DATA_OK);
            if (this == null)
            {
                return;
            }

            if (this.waitCoroutine != null)
            {
                this.StopCoroutine(this.waitCoroutine);
            }

            CommonPanel.GetInstance().ShowText(LOGIN_WAIT, () =>
            {
                this.waitCoroutine = this.StartCoroutine(this.OnLoginWait());
                return;
            });
        }

        private IEnumerator OnLoginFailed()
        {
            yield return new WaitForSeconds(WAIT);

            //LoginPanel.Instance.ClickCancelButton ();
        }

        private IEnumerator OnLoginWait()
        {
            yield return new WaitForSeconds(WAIT);

            CommonPanel.GetInstance().ShowText(LOGIN_WAIT, () =>
            {
                this.waitCoroutine = this.StartCoroutine(this.OnLoginWait());
                return;
            });
        }

        private void OnApplicationQuit()
        {
            PlayerPrefs.SetInt("LoginTimeSF", ExpandBmobLogin.ServerTimeStamp.Get() + (int)(RealTime.time));
            Debug.Log("退出程序");
        }
    }
}