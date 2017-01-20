﻿using Assets.Scripts.Common.Manager;
using FormulaBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskBox : MonoBehaviour
{
    public UISprite sprIcon;
    public UILabel txtDes, txtAwardNum;
    public GameObject coin, crystal;
    public UIButton btnGet;
    private UIGrid m_Grid;

    private void Awake()
    {
        m_Grid = transform.parent.GetComponent<UIGrid>();
        var idx = transform.GetSiblingIndex();
        var dailyTask = TaskManager.instance.awardTaskList[idx];
        var host = TaskManager.instance.GetFormulaHost(int.Parse(dailyTask.uid));
        var target = host.GetDynamicIntByKey(SignKeys.DT_TARGET);
        sprIcon.spriteName = dailyTask.icon;
        txtDes.text = dailyTask.description.Replace("(N)", target.ToString());
        txtAwardNum.text = dailyTask.coinAward > 0 ? dailyTask.coinAward.ToString() : dailyTask.crystalAward.ToString();
        coin.SetActive(dailyTask.coinAward > 0);
        crystal.SetActive(!coin.activeSelf);
        UIEventListener.Get(btnGet.gameObject).onClick = go =>
        {
            Destroy(gameObject);
            m_Grid.enabled = true;
            TaskManager.instance.AwardDailyTask(int.Parse(dailyTask.uid));
        };
    }
}