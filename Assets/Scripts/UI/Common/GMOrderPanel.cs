using FormulaBase;
using GameLogic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GMOrderPanel : MonoBehaviour
{
    public GameObject m_GMOrderPanel;
    public GameObject m_SureRestBox;
    public GameObject m_SureUnLockStagesButton;

    public void GetMoney()
    {
        AccountGoldManagerComponent.Instance.ChangeMoney(500);
    }

    public void GetDiamond()
    {
        AccountCrystalManagerComponent.Instance.ChangeCrystal(500);
    }

    public void GetPhysical()
    {
        AccountPhysicsManagerComponent.Instance.ChangeEnergyToMax();
    }

    public void ShowGMOrderPanel()
    {
        m_GMOrderPanel.SetActive(!m_GMOrderPanel.activeSelf);
    }

    public void DeleteAccountData()
    {
        AccountManagerComponent.Instance.DeletePlayerData(new HttpEndResponseDelegate(DeleteAccountCallBack));
    }

    public void SetMusicDelayPlayTime(UILabel label)
    {
        var output = 0.0m;
        if (decimal.TryParse(label.text, out output))
        {
            GameGlobal.DELAY_FOR_MUSIC = output;
        }
    }

    public void SetGameDelayPlayTime(UILabel label)
    {
        var output = 0.0m;
        if (decimal.TryParse(label.text, out output))
        {
            GameGlobal.DELAY_FOR_GAMESTART = output;
        }
    }

    private void DeleteAccountCallBack(cn.bmob.response.EndPointCallbackData<Hashtable> response)
    {
        Debug.Log("删除数据的反馈");

        Application.Quit();
    }

    public void ClearPlayerData()
    {
        PlayerPrefs.DeleteAll();
    }

    public void AddItem()
    {
        Debug.Log("GM命令添加物品");
        ItemManageComponent.Instance.CreateAllItems();
        DOTweenUtils.Delay(() =>
        {
            if (PnlSuitcase.PnlSuitcase.Instance != null)
            {
                PnlSuitcase.PnlSuitcase.Instance.UpdateSuitcase();
            }
        }, 0.5f);
    }

    public void AddMaterials()
    {
        LitJson.JsonData cfg2 = ConfigPool.Instance.GetConfigByName("item");
        Debug.Log("item" + cfg2.Count);
        List<int> TempitemList = new List<int>();
        int tempi = 0;
        foreach (string temp in cfg2.Keys)
        {
            //			tempi++;
            //			if(tempi>=20)
            //				break;
            TempitemList.Add(int.Parse(temp));
        }

        MaterialManageComponent.Instance.CreateItem(TempitemList);
    }

    public void DestroyScene()
    {
        if (SceneObjectController.Instance != null)
        {
            Destroy(SceneObjectController.Instance.gameObject);
        }
    }

    public void AddEquip()
    {
        LitJson.JsonData cfg1 = ConfigPool.Instance.GetConfigByName("Equipment_info");
        Debug.Log("Equipment_info" + cfg1.Count);
        List<int> TempEquipList = new List<int>();
        foreach (string temp in cfg1.Keys)
        {
            TempEquipList.Add(int.Parse(temp));
        }

        EquipManageComponent.Instance.CreateItem(TempEquipList);
    }

    public void AddCharm()
    {
        AccountCharmComponent.Instance.ChangeCharm(50);
    }

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void AddChest()
    {
        Dictionary<string, FormulaHost> _materialdic = FomulaHostManager.Instance.GetHostListByFileName("Material");
        if (_materialdic != null)
        {
            Debug.Log("所有材料的份数" + _materialdic.Count);
        }

        LitJson.JsonData cfg1 = ConfigPool.Instance.GetConfigByName("chest");
        List<int> TempEquipList = new List<int>();
        foreach (string temp in cfg1.Keys)
        {
            TempEquipList.Add(int.Parse(temp));
        }

        ChestManageComponent.Instance.CreateItem(TempEquipList);
    }

    public void UNLockAllStages()
    {
        GameGlobal.IS_UNLOCK_ALL_STAGE = true;
        CommonPanel.GetInstance().ShowText("全关卡解锁", null, false);

        m_SureUnLockStagesButton.SetActive(false);
    }

    public void CloseGMpanel()
    {
        m_GMOrderPanel.SetActive(false);
    }

    //	public GameObject m_SureRestBox;
    //	public GameObject m_SureUnLockStagesButton;
    public void ShowSureRestBox()
    {
        m_SureRestBox.SetActive(true);
    }

    public void ShowSureUnLockStagesBox()
    {
        m_SureUnLockStagesButton.SetActive(true);
    }

    public void ClickCancelInRest()
    {
        m_SureRestBox.SetActive(false);
    }

    public void ClickOkInRest()
    {
        DeleteAccountData();
        m_SureRestBox.SetActive(false);
        Application.Quit();
    }

    public void ClickCancelInUnlockStages()
    {
        m_SureUnLockStagesButton.SetActive(false);
    }

    public void ClickOKInUnlockStages()
    {
        //Debug.Log("暂未开放");
        UNLockAllStages();
    }

    public void GmRobotShow(bool _show)
    {
        m_GMOrderPanel.SetActive(!_show);
        if (_show)
        {
            this.gameObject.layer = 5;
            //CommonPanel.GetInstance().CloseBlur(null);
        }
        else
        {
            this.gameObject.layer = 17;
            //CommonPanel.GetInstance().SetBlurSub(null);
        }
    }

    public void ClickGMOtherPlease(UIToggle m_toggle)
    {
        CloseGMpanel();
        m_toggle.value = !m_toggle.value;
        this.gameObject.layer = 5;
        // CommonPanel.GetInstance().CloseBlur(null);
    }

    public void ClickSetCharacter(UILabel btnLabel)
    {
        if (RoleManageComponent.Instance.HostList == null)
        {
            Debug.Log("Role empty.");
            return;
        }

        int setIdx = 1;
        FormulaHost role = RoleManageComponent.Instance.GetRole(btnLabel.text);
        if (role != null)
        {
            int idx = role.GetDynamicIntByKey(SignKeys.ID);
            setIdx = idx + 1;
            if (setIdx > RoleManageComponent.Instance.HostList.Count)
            {
                setIdx = 1;
            }
        }

        Debug.Log("Gm set battle role " + setIdx);
        FormulaHost setRole = RoleManageComponent.Instance.GetRole(setIdx);
        btnLabel.text = setRole.GetDynamicStrByKey(SignKeys.NAME);
        RoleManageComponent.Instance.SetFightGirlIndex(setIdx);
        CommonPanel.GetInstance().ShowWaittingPanel(false);
    }

    public void ClickSetCloth(UILabel btnLabel)
    {
        if (RoleManageComponent.Instance.HostList == null)
        {
            Debug.Log("Role empty.");
            return;
        }

        int idx = RoleManageComponent.Instance.GetFightGirlIndex();
        if (idx <= 0)
        {
            Debug.Log("No fight role.");
            return;
        }

        FormulaHost role = RoleManageComponent.Instance.GetRole(idx);
        string name = role.GetDynamicStrByKey(SignKeys.NAME);
        int clothId = role.GetDynamicIntByKey(SignKeys.CLOTH);
        LitJson.JsonData jd = ConfigPool.Instance.GetConfigByName("char_cos");
        foreach (string key in jd.Keys)
        {
            int cid = int.Parse(key);
            if (cid <= clothId)
            {
                continue;
            }

            string ownername = ConfigPool.Instance.GetConfigStringValue("char_cos", key, "owner");
            if (ownername != name)
            {
                continue;
            }

            role.SetDynamicData(SignKeys.CLOTH, cid);
            break;
        }

        int setClothId = role.GetDynamicIntByKey(SignKeys.CLOTH);
        if (setClothId != clothId)
        {
            btnLabel.text = ("cloth " + setClothId);
            return;
        }

        clothId = 0;
        foreach (string key in jd.Keys)
        {
            int cid = int.Parse(key);
            if (cid <= clothId)
            {
                continue;
            }

            string ownername = ConfigPool.Instance.GetConfigStringValue("char_cos", key, "owner");
            if (ownername != name)
            {
                continue;
            }

            role.SetDynamicData(SignKeys.CLOTH, cid);
            break;
        }

        setClothId = role.GetDynamicIntByKey(SignKeys.CLOTH);
        if (setClothId != clothId)
        {
            btnLabel.text = ("cloth " + setClothId);
            return;
        }
    }
}