using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Text.RegularExpressions;
using LitJson;

/// <summary>
/// Formula edit.
/// </summary>
using System;
using Assets.Scripts.Tools.Managers;
using FormulaBase;
using cn.bmob.json;
using cn.bmob.io;
using cn.bmob.api;

public class TestAssetEdit : EditorWindow
{
    private static string SAVE_BUTTON = "Save";

    private const string DES_0 = "测试辅助工具";
    private const string DES_1 = "默认战斗关卡ID";
    private const string DES_2 = "角色测试技能ID";
    private const string DES_3 = "宠物测试技能ID";
    private const string DES_4 = "角色血量百分比";
    private const string DES_5 = "游戏速度";
    private const string DES_6 = "角色数据id";
    private const string DES_7 = "角色形象uid";

    private float debugTimeScale = 1f;
    private float debugHpRate = 1f;

    [MenuItem("MD/测试辅助工具")]
    private static void Init()
    {
        TestAssetEdit window = (TestAssetEdit)EditorWindow.GetWindow(typeof(TestAssetEdit));
        window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        this.MkTitle();
        this.MkSkills();
        this.MkOther();
        EditorGUILayout.EndVertical();
    }

    private void MkOther()
    {
        /*
		if (GUILayout.Button ("XXX")) {
			//DataPhaser dp = new DataPhaser ();
			//TimerComponent.Instance.SetAccountPhysicalTimer (false);

			AccountGoldManagerComponent.Instance.ChangeMoney (60, true, new HttpResponseDelegate (((bool result) => {
				Debug.Log ("On Change Money : " + result);
			})));
		}
		*/
    }

    private void MkTitle()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(DES_0);
        EditorGUILayout.EndHorizontal();

        AdminData.Instance.DefaultRoleIdx = int.Parse(EditorGUILayout.TextField(DES_6, AdminData.Instance.DefaultRoleIdx.ToString()));
        AdminData.Instance.DefaultClothUid = int.Parse(EditorGUILayout.TextField(DES_7, AdminData.Instance.DefaultClothUid.ToString()));
        EditorGUILayout.BeginHorizontal();
        AdminData.Instance.DefaultStage = int.Parse(EditorGUILayout.TextField(DES_1, AdminData.Instance.DefaultStage.ToString()));
        if (GUILayout.Button(SAVE_BUTTON))
        {
            this.Save();
            return;
        }
        EditorGUILayout.EndHorizontal();

        if (Application.isEditor)
        {
            this.debugTimeScale = EditorGUILayout.Slider(DES_5, this.debugTimeScale, 0f, 1f);

            if (!BattleRoleAttributeComponent.Instance.IsDead())
            {
                Time.timeScale = this.debugTimeScale;
                AudioManager.Instance.SetBgmTimeScale(this.debugTimeScale);
            }
        }

        this.MkRoleHPRate();

        EditorGUILayout.LabelField("角色/关卡/node配置检查");
        if (GUILayout.Button("配置检查"))
        {
            this.StageConfigCheck();
            this.CharacterConfigCheck();
            Resources.UnloadUnusedAssets();
            Debug.Log("角色/关卡/node配置检查完毕");
        }

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("自动战斗"))
        {
            StageBattleComponent.Instance.SetAutoPlay(!StageBattleComponent.Instance.IsAutoPlay());
        }

        if (GUILayout.Button("Fever On"))
        {
            GameKernel.Instance.AddFever(200f);
            FightMenuPanel.Instance.SetFerver(GameKernel.Instance.GetFeverRate());
        }

        if (GUILayout.Button("关卡强制结束"))
        {
            StageBattleComponent.Instance.End();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("log角色数据"))
        {
            FormulaHost battleRole = BattleRoleAttributeComponent.Instance.GetBattleRole();
            if (battleRole != null)
            {
                Debug.Log("Role sign data : " + battleRole.SignToJson().ToJson());
            }
        }

        if (GUILayout.Button("开闭node debug"))
        {
            GameLogic.GameGlobal.IS_NODE_DEBUG = !GameLogic.GameGlobal.IS_NODE_DEBUG;
            Debug.Log("Set node debug : " + GameLogic.GameGlobal.IS_NODE_DEBUG);
        }

        if (GUILayout.Button("Clear All Player Data"))
        {
            PlayerPrefs.DeleteAll();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void MkSkills()
    {
        int len = 3;
        if (AdminData.Instance.DefaultSkills == null || AdminData.Instance.DefaultSkills.Count < len)
        {
            for (int i = 0; i < len; i++)
            {
                AdminData.Instance.DefaultSkills.Add(0);
            }
        }

        EditorGUILayout.LabelField(DES_2);
        EditorGUILayout.BeginHorizontal();
        for (int i = 0; i < len; i++)
        {
            AdminData.Instance.DefaultSkills[i] = int.Parse(EditorGUILayout.TextField(AdminData.Instance.DefaultSkills[i].ToString()));
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField(DES_3);
        EditorGUILayout.BeginHorizontal();
        AdminData.Instance.DefaultPetSkill = int.Parse(EditorGUILayout.TextField(AdminData.Instance.DefaultPetSkill.ToString()));
        EditorGUILayout.EndHorizontal();
    }

    private void MkRoleHPRate()
    {
        EditorGUILayout.BeginHorizontal();
        this.debugHpRate = EditorGUILayout.Slider(DES_4, this.debugHpRate, 0f, 1f);
        if (GUILayout.Button("Set"))
        {
            try
            {
                int value = (int)(BattleRoleAttributeComponent.Instance.GetHpMax() * this.debugHpRate);
                int hp = BattleRoleAttributeComponent.Instance.GetHp();
                BattleRoleAttributeComponent.Instance.AddHp((value - hp));
            }
            catch (Exception)
            {
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    // --------------------------------------------------------------------------------------------------
    private void Save()
    {
        GameObject dataObject = AdminData.GetDataObject();
        if (dataObject == null)
        {
            Debug.Log("Editor Data prefab is null.");
            return;
        }

        //PrefabUtility.ReplacePrefab (this.gameObject, dataObject);
        PropertyModification[] temp = new PropertyModification[1];
        temp[0] = new PropertyModification();
        temp[0].target = dataObject;
        PrefabUtility.SetPropertyModifications(dataObject, temp);
        AssetDatabase.SaveAssets();
        AdminData.Instance.AfterSave();
    }

    private void CharacterConfigCheck()
    {
        JsonData jd = ConfigManager.instance["char_info"];
        if (jd == null)
        {
            Debug.Log("没有char_info配置表");
            return;
        }

        for (int i = 0; i < jd.Count; i++)
        {
            var charName = ConfigManager.instance.GetConfigStringValue("char_info", i, "name");
            var imgVictory = ConfigManager.instance.GetConfigStringValue("char_info", i, "image_victory");
            var imgFail = ConfigManager.instance.GetConfigStringValue("char_info", i, "image_fail");
            //string _characterPrefab = ConfigPool.Instance.GetConfigStringValue ("character", iid, "character");
            var fxgreat = ConfigManager.instance.GetConfigStringValue("char_info", i, "fx_atk_great");
            var fxperfect = ConfigManager.instance.GetConfigStringValue("char_info", i, "fx_atk_perfect");
            var fxcrit = ConfigManager.instance.GetConfigStringValue("char_info", i, "fx_atk_crit");
            var fever = ConfigManager.instance.GetConfigStringValue("char_info", i, "fever");

            if (charName == null)
            {
                Debug.Log("char_info " + i + "没有name配置");
                continue;
            }

            if (imgVictory == null)
            {
                Debug.Log("char_info " + i + "没有 image_victory 配置");
            }
            else
            {
                if (Resources.Load(imgVictory) == null)
                {
                    Debug.Log("char_info配置表" + i + "没有 image_victory 资源 : " + imgVictory);
                }
            }

            if (imgFail == null)
            {
                Debug.Log("char_info " + i + "没有 image_fail 配置");
            }
            else
            {
                if (Resources.Load(imgFail) == null)
                {
                    Debug.Log("char_info 配置表" + i + "没有 image_fail 资源 : " + imgFail);
                }
            }
            /*
			if (_characterPrefab == null) {
				Debug.Log ("character" + iid + "没有 character 配置");
			} else {
				if (Resources.Load (_characterPrefab) == null) {
					Debug.Log ("character配置表" + iid + "没有 character 资源 : " + _characterPrefab);
				}
			}
*/
            if (fxgreat == null)
            {
                Debug.Log("char_info " + i + "没有 fx_atk_great 配置");
            }
            else
            {
                if (Resources.Load(fxgreat) == null)
                {
                    Debug.Log("char_info 配置表" + i + "没有 fx_atk_great 资源 : " + fxgreat);
                }
            }

            if (fxperfect == null)
            {
                Debug.Log("char_info " + i + "没有 fx_atk_perfect 配置");
            }
            else
            {
                if (Resources.Load(fxperfect) == null)
                {
                    Debug.Log("char_info 配置表" + i + "没有 fx_atk_perfect 资源 : " + fxperfect);
                }
            }

            if (fxcrit == null)
            {
                Debug.Log("char_info " + i + "没有 fx_atk_crit 配置");
            }
            else
            {
                if (Resources.Load(fxcrit) == null)
                {
                    Debug.Log("char_info 配置表" + i + "没有 fx_atk_crit 资源 : " + fxcrit);
                }
            }

            if (fever == null)
            {
                Debug.Log("char_info " + i + "没有 fever 配置");
            }
            else
            {
                if (Resources.Load(fever) == null)
                {
                    Debug.Log("char_info配置表" + i + "没有 fever 资源 : " + fever);
                }
            }
        }

        var clothjd = ConfigManager.instance["char_cos"];
        if (clothjd == null)
        {
            Debug.Log("没有char_cos配置表");
            return;
        }

        for (var i = 0; i < jd.Count; i++)
        {
            var path = ConfigManager.instance.GetConfigStringValue("char_cos", i, "path");
            if (path == null)
            {
                Debug.Log("char_cos" + i + "没有 path 配置");
            }
            else
            {
                if (Resources.Load(path) == null)
                {
                    Debug.Log("char_cos配置表" + i + "没有 path 资源 : " + path);
                }
            }
        }
    }

    private void StageConfigCheck()
    {
        JsonData jd = ConfigManager.instance["stage"];
        if (jd == null)
        {
            Debug.Log("没有stage配置表");
            return;
        }

        for (int i = 0; i < jd.Count; i++)
        {
            string stgCfgName = ConfigManager.instance.GetConfigStringValue("stage", i, "note_json");
            string sceneCfgName = ConfigManager.instance.GetConfigStringValue("stage", i, "scene");
            string iconCfgName = ConfigManager.instance.GetConfigStringValue("stage", i, "cover");
            // 歌曲检查
            for (int diff = 1; diff <= 3; diff++)
            {
                string songCfgName = ConfigManager.instance.GetConfigStringValue("stage", i, "cover");
                if (songCfgName == null)
                {
                    Debug.Log("stage配置表" + i + "没有file name " + diff + "配置");
                }
                else
                {
                    string songPathName = songCfgName;
                    //AssetBundle.LoadFromFile
                    if (Resources.Load(songPathName) == null)
                    {
                        Debug.Log("stage配置表" + i + "没有file name " + diff + "资源 : " + songPathName);
                    }
                }
            }

            // 场景检查
            if (sceneCfgName == null)
            {
                Debug.Log("stage配置表" + i + "没有scene配置");
            }
            else
            {
                if (Resources.Load(sceneCfgName) == null)
                {
                    Debug.Log("stage配置表" + i + "没有scene资源 : " + sceneCfgName);
                }
            }

            // 图标检查
            if (iconCfgName == null)
            {
                Debug.Log("stage配置表" + i + "没有icon配置");
            }
            else
            {
                string sceneFilename = sceneCfgName;
                if (Resources.Load(iconCfgName) == null)
                {
                    Debug.Log("stage配置表" + i + "没有icon资源 : " + sceneCfgName);
                }
            }

            // 关卡检查
            this.StageConfigNodeCheck(i, stgCfgName + GameLogic.GameGlobal.DIFF_LEVEL_NORMAL);
            this.StageConfigNodeCheck(i, stgCfgName + GameLogic.GameGlobal.DIFF_LEVEL_HARD);
            this.StageConfigNodeCheck(i, stgCfgName + GameLogic.GameGlobal.DIFF_LEVEL_SUPER);
        }
    }

    private void StageConfigNodeCheck(int sid, string name)
    {
        if (name == null || name.Length < 2)
        {
            return;
        }

        JsonData jd = ConfigManager.instance[name];
        if (jd == null)
        {
            Debug.Log("stage配置表" + sid + "没有对应配置表" + name);
            return;
        }

        int clen = ConfigManager.instance["notedata"].Count;
        for (int i = 0; i < jd.Count; i++)
        {
            int nIdx = -1;
            string nodeIdx = ConfigManager.instance.GetConfigStringValue(name, i, "note_uid");
            for (int ii = 0; ii < clen; ii++)
            {
                string nid = ConfigManager.instance.GetConfigStringValue("notedata", ii, "id");
                if (nid == nodeIdx)
                {
                    nIdx = ii;
                    break;
                }
            }

            if (nIdx < 0)
            {
                Debug.Log(name + " nodedata配置表没有配置 : " + nodeIdx);
                continue;
            }

            string prefabPath = ConfigManager.instance.GetConfigStringValue("notedata", nIdx, "prefab_path");
            if (string.IsNullOrEmpty(prefabPath))
            {
                Debug.Log("nodedata的 " + nodeIdx + " 行没有配置或者没有note_prefab配置");
                continue;
            }

            if (Resources.Load(prefabPath) == null)
            {
                Debug.Log("nodedata的 " + nodeIdx + " 没有note_prefab资源 : " + prefabPath);
            }
        }
    }
}