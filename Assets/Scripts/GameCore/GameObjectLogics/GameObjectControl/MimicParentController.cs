using UnityEngine;
using System.Collections;
using GameLogic;
using FormulaBase;

/// <summary>
/// Mimic parent controller.
///
/// 宝箱怪生成器
/// </summary>
using System.Collections.Generic;
using Assets.Scripts.Tools.Managers;

public class MimicParentController : BaseEnemyObjectController
{
    private static MimicParentController instance = null;

    public static MimicParentController Instance
    {
        get
        {
            return instance;
        }
    }

    private GameObject mimic = null;
    private List<FormulaHost> items = null;

    /// <summary>
    /// Init this instance.
    ///
    /// Preload mimic obj if has.
    /// </summary>
    public override void Init()
    {
        instance = this;
        base.Init();

        if (!this.ActiveCheck())
        {
            return;
        }

        this.GenerateMimic();
        this.GenerateItems();
    }

    /// <summary>
    /// Raises the controller start event.
    ///
    /// Start to run mimic if has.
    /// </summary>
    public override void OnControllerStart()
    {
        if (this.mimic == null)
        {
            return;
        }

        MimicEnemyController controller = this.mimic.GetComponent<MimicEnemyController>();
        if (controller == null)
        {
            return;
        }

        this.mimic.SetActive(true);
        controller.OnControllerStart();
        Debug.Log("Mimic start.");
    }

    public override void OnControllerAttacked(int result, bool isDeaded)
    {
    }

    public override bool OnControllerMiss(int idx)
    {
        if (this.mimic != null)
        {
            MimicEnemyController controller = this.mimic.GetComponent<MimicEnemyController>();
            if (controller != null)
            {
                controller.OnControllerMiss(idx);
            }
        }

        return false;
    }

    public GameObject GetMimic()
    {
        return this.mimic;
    }

    public List<FormulaHost> GetItems()
    {
        return this.items;
    }

    public FormulaHost GetNextItem()
    {
        if (this.items == null || this.items.Count <= 0)
        {
            this.items = null;
            return null;
        }

        for (int i = 0; i < this.items.Count; i++)
        {
            FormulaHost host = this.items[i];
            if (host == null)
            {
                continue;
            }

            GameObject obj = (GameObject)host.GetDynamicObjByKey(SignKeys.GAME_OBJECT);
            if (obj == null || obj.activeSelf)
            {
                continue;
            }

            return host;
        }

        return null;
    }

    public FormulaHost GetHostByGameObject(GameObject obj)
    {
        for (int i = 0; i < this.items.Count; i++)
        {
            FormulaHost host = this.items[i];
            if (host == null)
            {
                continue;
            }

            GameObject _obj = (GameObject)host.GetDynamicObjByKey(SignKeys.GAME_OBJECT);
            if (obj != _obj)
            {
                continue;
            }

            return host;
        }

        return null;
    }

    /// <summary>
    /// Actives the check.
    ///
    /// By stage mimic data, if stage has no Mimic active, destory self.
    /// </summary>
    /// <returns><c>true</c>, if check was actived, <c>false</c> otherwise.</returns>
    private bool ActiveCheck()
    {
        return true;
    }

    /// <summary>
    /// Generates the mimic.
    ///
    /// By stage mimic data.
    /// </summary>
    private void GenerateMimic()
    {
        // test with "Note/I00/Box_monster"
        string path = GameGlobal.PREFABS_PATH + "Note/I00/Box_monster";
        this.mimic = StageBattleComponent.Instance.AddObjWithControllerInit(ref path, this.idx);
        if (this.mimic == null)
        {
            Debug.Log("Minic preload has no resource : " + path);
            return;
        }

        this.mimic.SetActive(false);
        Debug.Log("Minic preload ok : " + path);
    }

    /// <summary>
    /// Generates the items.
    ///
    /// Init item pool with pre generated items.
    /// </summary>
    private void GenerateItems()
    {
        if (this.mimic == null)
        {
            return;
        }

        // test with fix node id and time interval.
        int id = 108;
        float tick = 0.5f;
        this.items = new List<FormulaHost>();
        for (int i = 0; i < 3; i++)
        {
            FormulaHost host = FomulaHostManager.Instance.CreateHost(HostKeys.HOST_4);
            string path = GameGlobal.PREFABS_PATH + ConfigManager.instance.GetConfigStringValue("notedata", "id", "animation", id.ToString());
            GameObject _obj = StageBattleComponent.Instance.AddObjWithControllerInit(ref path, this.idx);
            _obj.SetActive(false);
            _obj.transform.parent = this.mimic.transform;

            host.SetDynamicData(SignKeys.ID, id);
            host.SetDynamicData(SignKeys.BATTLE_HP, tick);
            host.SetDynamicData(SignKeys.GAME_OBJECT, _obj);

            this.items.Add(host);
        }
    }
}