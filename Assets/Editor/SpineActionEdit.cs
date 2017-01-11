using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;

[CustomEditor(typeof(SpineActionController))]
public class SpineActionEdit : Editor
{
    private const string TITLE_DES1 = "SpineActionEdit(编辑完毕后Apply保存)";
    private const string LIST_DES4 = "行为模式";
    private const string LIST_DES3 = "动作保护级别";

    private const string BTN_DES1 = "新增动作序列";
    private const string BTN_DES2 = "删除序列";
    private const string BTN_DES3 = "+动作";
    private const string BTN_DES4 = "-动作";

    private const string SB_DES2 = "末尾动作循环";
    private const string SB_DES3 = "该序列是否随机";
    private const string SB_DES4 = "动作自保护";
    private const string SLIDER_DES1 = "出场延迟";

    private static string[] PROTECT_LEVEL = new string[] { "0 自由切换", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
    private string[] sklActionNames;
    private GameObject obj;

    public override void OnInspectorGUI()
    {
        this.obj = null;
        if (target != null)
        {
            this.obj = ((SpineActionController)target).gameObject;
        }

        if (this.obj == null)
        {
            return;
        }

        SpineActionController spa = this.obj.GetComponent<SpineActionController>();
        if (spa == null)
        {
            return;
        }

        this.FieldSkeletionActions();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(TITLE_DES1, EditorStyles.boldLabel);
        if (GUILayout.Button(BTN_DES1))
        {
            this.AddArray();
            return;
        }

        EditorGUILayout.EndHorizontal();

        spa.actionMode = EditorGUILayout.Popup(LIST_DES4, spa.actionMode, EditorData.Instance.SpineModeDes);
        spa.startDelay = EditorGUILayout.Slider(SLIDER_DES1, spa.startDelay, 0f, (float)GameLogic.GameGlobal.COMEOUT_TIME_MAX);
        if (spa.actionMode == 12)
        {
            spa.duration = EditorGUILayout.FloatField(new GUIContent("长按节点穿越时间"), spa.duration);
            spa.lengthRate = EditorGUILayout.FloatField(new GUIContent("长度比例"), spa.lengthRate);
            spa.rendererPreb = (GameObject)EditorGUILayout.ObjectField(new GUIContent("渲染预制"), spa.rendererPreb, typeof(GameObject));
            return;
        }
        for (int j = 0; j < spa.DataCount(); j++)
        {
            SkeletActionData _d = spa.GetData(j);
            if (_d.name == null)
            {
                continue;
            }

            EditorGUILayout.BeginHorizontal();
            _d.collapsed = EditorGUILayout.Foldout(_d.collapsed, _d.name);
            // _d.name = EditorGUILayout.TextField ("", _d.name);
            _d.spineActionKeyIndex = EditorGUILayout.Popup("", _d.spineActionKeyIndex, EditorData.Instance.SpineActionDes);
            if (GUILayout.Button(BTN_DES3))
            {
                this.AddActionIdx(j, ref _d);
                this.Repaint();
                return;
            }

            if (GUILayout.Button(BTN_DES2))
            {
                this.DelArray(j);
                this.Repaint();
                return;
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical();
            if (_d.collapsed)
            {
                EditorGUILayout.BeginVertical();
                _d.isEndLoop = EditorGUILayout.Toggle(SB_DES2, _d.isEndLoop);
                _d.isRandomSequence = EditorGUILayout.Toggle(SB_DES3, _d.isRandomSequence);
                _d.isSelfProtect = EditorGUILayout.Toggle(SB_DES4, _d.isSelfProtect);
                EditorGUILayout.EndVertical();
                _d.protectLevel = EditorGUILayout.Popup(LIST_DES3, _d.protectLevel, PROTECT_LEVEL);
                GUILayoutOption opt = GUILayout.MaxWidth(150);
                for (int i = 0; i < _d.arrayCount; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    _d.actionIdx[i] = EditorGUILayout.Popup("", _d.actionIdx[i], this.sklActionNames, opt);
                    if (_d.actionEventIdx == null || _d.actionEventIdx.Length == 0)
                    {
                        _d.actionEventIdx = new int[10];
                    }

                    _d.actionEventIdx[i] = EditorGUILayout.Popup("", _d.actionEventIdx[i], EditorData.Instance.SpineEventDes, opt);
                    if (GUILayout.Button(BTN_DES4, opt))
                    {
                        this.DelActionIdx(j, ref _d, i);
                        return;
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndVertical();

            spa.SetData(j, _d);
        }
    }

    private void AddArray()
    {
        SpineActionController spa = this.obj.GetComponent<SpineActionController>();
        if (spa == null)
        {
            return;
        }

        SkeletActionData d = new SkeletActionData();
        d.name = "df";
        d.actionIdx = new int[10];
        d.actionEventIdx = new int[10];
        spa.AddData(d);
        this.Repaint();
    }

    private void DelArray(int idx)
    {
        SpineActionController spa = this.obj.GetComponent<SpineActionController>();
        if (spa == null)
        {
            return;
        }

        spa.DelData(idx);
        this.Repaint();
    }

    private void AddActionIdx(int idx, ref SkeletActionData data)
    {
        SpineActionController spa = this.obj.GetComponent<SpineActionController>();
        if (spa == null)
        {
            return;
        }

        data.arrayCount += 1;
        spa.SetData(idx, data);
    }

    private void DelActionIdx(int idx, ref SkeletActionData data, int actionIdx)
    {
        SpineActionController spa = this.obj.GetComponent<SpineActionController>();
        if (spa == null)
        {
            return;
        }

        data.actionIdx[actionIdx] = 0;
        data.arrayCount -= 1;
        for (int ii = actionIdx; ii < data.actionIdx.Length; ii++)
        {
            int _idx = 0;
            if (ii + 1 < data.actionIdx.Length)
            {
                _idx = data.actionIdx[ii + 1];
            }

            data.actionIdx[ii] = _idx;
        }

        spa.SetData(idx, data);
    }

    private void FieldSkeletionActions()
    {
        if (target == null)
        {
            this.sklActionNames = new string[0];
            return;
        }

        GameObject obj = ((SpineActionController)target).gameObject;
        if (obj == null)
        {
            this.sklActionNames = new string[0];
            return;
        }

        SkeletonAnimation sklAni = obj.GetComponent<SkeletonAnimation>();
        if (sklAni == null)
        {
            this.sklActionNames = new string[0];
            return;
        }

        Spine.ExposedList<Spine.Animation> list = sklAni.skeletonDataAsset.GetAnimationStateData().SkeletonData.Animations;
        this.sklActionNames = new string[list.Count];
        for (int i = 0; i < list.Count; i++)
        {
            Spine.Animation ani = list.Items[i];
            if (ani == null)
            {
                continue;
            }

            this.sklActionNames[i] = ani.Name;
        }
    }
}