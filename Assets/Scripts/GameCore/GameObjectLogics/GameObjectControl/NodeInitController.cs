using UnityEngine;
using System.Collections;
using DYUnityLib;
using FormulaBase;
using GameLogic;

public class NodeInitController : MonoBehaviour
{
    private int idx = -1;
    private decimal countdown = -1m;

    private void FixedUpdate()
    {
        if (FixUpdateTimer.IsPausing())
        {
            return;
        }

        if (this.countdown < 0)
        {
            return;
        }
        this.countdown -= FixUpdateTimer.dInterval;
        if (this.countdown != 0)
        {
            return;
        }

        this.Run();
    }

    public void Init(int idx)
    {
        if (idx < 0 || this.idx >= 0)
        {
            return;
        }

        this.idx = idx;
        this.enabled = true;
        SpineActionController sac = this.gameObject.GetComponent<SpineActionController>();
        if (sac == null)
        {
            return;
        }

        if (sac.startDelay <= 0)
        {
            this.Run();
            return;
        }

        this.countdown = GameGlobal.COMEOUT_TIME_MAX - decimal.Round((decimal)sac.startDelay, 2);
        if (this.countdown <= 0m)
        {
            this.countdown = FixUpdateTimer.dInterval;
            Debug.Log("Node " + this.gameObject.name + " start delay now.");
            return;
        }

        MeshRenderer meshRender = this.gameObject.GetComponent<MeshRenderer>();
        if (meshRender != null)
        {
            meshRender.enabled = false;
        }
    }

    public void Run()
    {
        MeshRenderer meshRender = this.gameObject.GetComponent<MeshRenderer>();
        if (meshRender != null)
        {
            meshRender.enabled = true;
        }

        SpineActionController sac = this.gameObject.GetComponent<SpineActionController>();
        sac.OnControllerStart();

        GameGlobal.gGameMusicScene.OnObjRun(this.idx);
        this.countdown = -1;
        // this.enabled = false;
    }

    public void AddCoundDown(decimal value)
    {
        this.countdown += value;
    }
}