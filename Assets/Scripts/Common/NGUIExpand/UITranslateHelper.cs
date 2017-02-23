﻿using UnityEngine;
using System.Collections;
using FormulaBase;
using GameLogic;
using System.Collections.Generic;
using Assets.Scripts.Tools.Managers;

/// <summary>
/// User interface translate helper.
/// UI字体自动语言转换插件
/// </summary>
public class UITranslateHelper : MonoBehaviour
{
    private const string CFG_NAME = "languageconfig";
    private const string DEFAULT_LIST_NAME = "default";

    private static Dictionary<string, string> _languageMap;

    private UILabel _label;

    // Use this for initialization
    private void Start()
    {
        return;
        if (!this.InitLanguageMap())
        {
            return;
        };

        this._label = this.gameObject.GetComponent<UILabel>();
        if (this._label == null)
        {
            return;
        }

        this._label.onPostFill += new UIWidget.OnPostFillCallback(this.OnLabelTextChanged);
    }

    private bool InitLanguageMap()
    {
        if (_languageMap != null)
        {
            return false;
        }

        int l = ConfigManager.instance[CFG_NAME].Count;
        if (l <= 0)
        {
            return false;
        }

        _languageMap = new Dictionary<string, string>();
        for (int i = 0; i < l; i++)
        {
            string _default = ConfigManager.instance.GetConfigStringValue(CFG_NAME, i, DEFAULT_LIST_NAME);
            string _dest = ConfigManager.instance.GetConfigStringValue(CFG_NAME, i, GameGlobal.LANGUAGE_VERSION);
            _languageMap[_default] = _dest;
        }
        return true;
    }

    private void OnLabelTextChanged(UIWidget widget, int bufferOffset, BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color> cols)
    {
        if (this._label.text == null || this._label.text == string.Empty)
        {
            return;
        }

        if (_languageMap == null)
        {
            return;
        }

        if (!_languageMap.ContainsKey(this._label.text))
        {
            Debug.Log(CFG_NAME + "没有配置" + this._label.text);
            return;
        }

        this._label.text = _languageMap[this._label.text];
    }
}