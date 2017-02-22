using Assets.Scripts.Common;
using FormulaBase;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Tools.Managers
{
    public class GameMain : SingletonMonoBehaviour<GameMain>
    {
        private void Start()
        {
            DontDestroyOnLoad(this.gameObject);

            //For Debug Mode
            if (SceneManager.GetActiveScene().buildIndex != 0)
            {
                Login();
            }
        }

        public void Login(int sceneIdx = -1)
        {
            LoginManager.instance.Login(sceneIdx, (r, e, user) =>
            {
                AccountManagerComponent.Instance.Init();
                AccountPhysicsManagerComponent.Instance.Init();
                //TaskManager.instance.Init();
                StageBattleComponent.Instance.InitById(0);
                AccountManagerComponent.Instance.AddLoginCount(1);
            });
        }

        public void BattleStart(int stageIdx)
        {
            StageBattleComponent.Instance.SetStageId((uint)stageIdx);
            uint diff = StageBattleComponent.Instance.GetDiffcult();
            StageBattleComponent.Instance.Enter((uint)stageIdx, diff);
        }
    }
}