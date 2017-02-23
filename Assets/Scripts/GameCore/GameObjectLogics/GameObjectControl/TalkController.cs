using UnityEngine;
using System.Collections;
using DYUnityLib;
using GameLogic;
using FormulaBase;
using System.Collections.Generic;
using Assets.Scripts.Tools.Managers;

public class TalkController : BaseEnemyObjectController
{
    public override void OnControllerStart()
    {
        TalkConfigController tcc = this.gameObject.GetComponent<TalkConfigController>();
        if (tcc == null)
        {
            return;
        }

        string message = ConfigManager.instance.GetConfigStringValue("talk", tcc.talkId, "message");
        Debug.Log("Talk : " + message);
        CommonPanel.GetInstance().ShowText(message);
    }

    public override bool OnControllerMiss(int idx)
    {
        return false;
    }
}