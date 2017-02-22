using System.Collections;
using Assets.Scripts.Common;
using DG.Tweening;
using FormulaBase;
using HutongGames.PlayMaker.Actions;
using LitJson;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Tools.Managers
{
    public class LoginManager : Singleton<LoginManager>
    {
        // ----------------------------------------------------------------------------------------------------
        // ----------------------------------------------------------------------------------------------------
        // ----------------------------------------------------------------------------------------------------
        public const string Login_Ready = "哔哩哔哩";

        public const string Login_OK = "哔哩哔哩哔哩哔哩";
        public const string Login_Wait = "哔----------";
        public const string Login_Failed = "很遗憾，网络掉线了";
        public const string Load_Data_OK = "今天你吃药了吗";
        public int loadSceneIdx = -1;

        private const float wait = 2f;
        private const float startWait = 0.1f;
        private const string saveFileName = "logininfo";
        private string m_UserName = null;
        private string m_Password = null;
        private string m_VerfiCode = null;

        private Sequence m_WaitSeq;

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
        public void Login(int sceneIdx = -1, LoginResponseDelegate callback = null)
        {
            loadSceneIdx = sceneIdx;
            JsonData loadData = GameLogic.GameGlobal.gConfigLoader.LoadPrefs(saveFileName);

            LoginResponseDelegate loginCallback = new LoginResponseDelegate((r, e, user) =>
            {
                if (callback != null) callback(r, e, user);
                if (!r)
                {
                    // send msg
                    CommonPanel.GetInstance().ShowText(Login_Failed);

                    return;
                }

                DOTweenUtils.Delay(() =>
                {
                    HttpEndResponseDelegate rsp = new HttpEndResponseDelegate(this.LoadAllDataResponse);
                    // Load all data of account from data base or local.
                    FormulaBase.FomulaHostManager.Instance.LoadAllHost(rsp);
                    CommonPanel.GetInstance().ShowText(Login_OK);
                }, startWait);
            });

            if (loadData == null ||
                loadData.GetJsonType() == JsonType.None ||
                !loadData.Keys.Contains("username") ||
                loadData["username"] == null ||
                loadData["username"].ToString() == string.Empty
                )
            {
                this.m_UserName = ObjectId.NewObjectId();
                this.m_Password = ExpandBmobLogin.Instance.password;
                LoginResponseDelegate lrd = new LoginResponseDelegate((r, e, user) =>
                {
                    if (!r)
                    {
                        // send msg
                        CommonPanel.GetInstance().ShowText(Login_Failed);
                        return;
                    }
                    this.SaveSaveData();
                    loginCallback(true, e, user);
                    VerificationScript.Instance.SetVerificationCodeOwner(this.m_UserName);
                });
                ExpandBmobLogin.Instance.Signup(this.m_UserName, this.m_Password, lrd);
            }
            else
            {
                var username = loadData["username"].ToString();

                ExpandBmobLogin.Instance.Login(username, null, loginCallback);
            }

            CommonPanel.GetInstance().ShowText(Login_Ready);

            CommonPanel.GetInstance().ShowText(Login_Wait);
        }

        public string GetVerficationCode()
        {
            return this.m_VerfiCode;
        }

        public void SetVerficationCode(string vcode)
        {
            this.m_VerfiCode = vcode;
            this.SaveSaveData();
        }

        public void LoadSaveData()
        {
            JsonData loadData = GameLogic.GameGlobal.gConfigLoader.LoadPrefs(saveFileName);
            if (loadData == null || loadData.GetJsonType() == JsonType.None)
            {
                this.m_UserName = ExpandBmobLogin.Instance.username;
                this.m_Password = ExpandBmobLogin.Instance.password;
                //this.verfiCode = VerificationScript.Instance.verificateCode;

                return;
            }

            if (loadData.Keys.Contains("username"))
            {
                this.m_UserName = loadData["username"].ToString();
            }

            if (loadData.Keys.Contains("password"))
            {
                this.m_Password = loadData["password"].ToString();
            }

            if (loadData.Keys.Contains("vcode"))
            {
                this.m_VerfiCode = loadData["vcode"].ToString();
            }

            Debug.Log("Load local account : " + this.m_UserName + " / " + this.m_VerfiCode);
        }

        public void LoginSucceed()
        {
            if (loadSceneIdx >= 0)
            {
                SceneManager.LoadScene(1);
            }
        }

        private void SaveSaveData()
        {
            JsonData saveData = new JsonData();

            this.m_UserName = ExpandBmobLogin.Instance.username;
            this.m_Password = ExpandBmobLogin.Instance.password;
            this.m_VerfiCode = VerificationScript.Instance.verificateCode;
            saveData["username"] = this.m_UserName;
            saveData["password"] = this.m_Password;
            saveData["vcode"] = this.m_VerfiCode;
            GameLogic.GameGlobal.gConfigLoader.SavePrefs(saveFileName, saveData);
            Debug.Log("Save local account : " + this.m_UserName + " / " + this.m_VerfiCode);
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
            CommonPanel.GetInstance().ShowText(Load_Data_OK);
            CommonPanel.GetInstance().ShowText(Login_Wait);
        }

        private void OnApplicationQuit()
        {
            PlayerPrefs.SetInt("LoginTimeSF", ExpandBmobLogin.ServerTimeStamp.Get() + (int)(RealTime.time));
            Debug.Log("退出程序");
        }
    }
}