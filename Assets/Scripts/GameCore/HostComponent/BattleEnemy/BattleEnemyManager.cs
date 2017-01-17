using GameLogic;

///自定义模块，可定制模块具体行为
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FormulaBase
{
    public class BattleEnemyManager : CustomComponentBase
    {
        private static BattleEnemyManager instance = null;
        private const int HOST_IDX = 4;

        public static BattleEnemyManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BattleEnemyManager();
                }

                return instance;
            }
        }

        // ----------------------------------- // -----------------------------------
        private int currentGenIdx = -1;

        private int currentPlayIndex = -1;
        private Dictionary<int, FormulaHost> enemy = null;

        public Dictionary<int, FormulaHost> Enemies
        {
            get
            {
                return enemy;
            }
        }

        public BattleEnemyManager()
        {
            this.Init();
            this.enemy = new Dictionary<int, FormulaHost>();
        }

        public void Init()
        {
            this.currentGenIdx = -1;
            this.currentPlayIndex = -1;
        }

        // ----------------------------------- // -----------------------------------
        public FormulaHost GetHost(int idx)
        {
            if (this.enemy == null)
            {
                return null;
            }

            if (!this.enemy.ContainsKey(idx))
            {
                return null;
            }

            return this.enemy[idx];
        }

        public GameObject GetObj(int idx)
        {
            if (this.enemy == null)
            {
                return null;
            }

            if (!this.enemy.ContainsKey(idx))
            {
                return null;
            }

            FormulaHost _enemy = this.enemy[idx];
            return (GameObject)_enemy.GetDynamicObjByKey(SignKeys.GAME_OBJECT);
        }

        public List<int> GetCurrentSceneAliveObjectsIndexes()
        {
            List<int> list = new List<int>();
            for (int i = this.currentGenIdx; i >= 0; i--)
            {
                if (!this.enemy.ContainsKey(i))
                {
                    continue;
                }

                FormulaHost h = this.enemy[i];
                if (h == null)
                {
                    continue;
                }

                object _obj = h.GetDynamicObjByKey(SignKeys.GAME_OBJECT);
                if (_obj == null || _obj.GetType() == typeof(float))
                {
                    break;
                }

                list.Add(i);
            }

            return list;
        }

        public List<GameObject> GetCurrentSceneAliveMimics()
        {
            if (MimicParentController.Instance == null)
            {
                return null;
            }

            List<GameObject> list = new List<GameObject>();
            GameObject mimic = MimicParentController.Instance.GetMimic();
            if (mimic != null)
            {
                list.Add(mimic);
            }

            List<FormulaHost> mimicItems = MimicParentController.Instance.GetItems();
            if (mimicItems != null && mimicItems.Count > 0)
            {
                for (int i = 0; i < mimicItems.Count; i++)
                {
                    FormulaHost host = mimicItems[i];
                    if (host == null)
                    {
                        continue;
                    }

                    GameObject mItem = (GameObject)host.GetDynamicObjByKey(SignKeys.GAME_OBJECT);
                    if (mItem == null || !mItem.activeSelf)
                    {
                        continue;
                    }

                    list.Add(mItem);
                }
            }

            return list;
        }

        public List<GameObject> GetCurrentSceneAliveObjects()
        {
            List<GameObject> list = new List<GameObject>();
            for (int i = this.currentGenIdx; i >= 0; i--)
            {
                if (!this.enemy.ContainsKey(i))
                {
                    continue;
                }

                FormulaHost h = this.enemy[i];
                if (h == null)
                {
                    continue;
                }

                object _obj = h.GetDynamicObjByKey(SignKeys.GAME_OBJECT);
                if (_obj == null || _obj.GetType() == typeof(float))
                {
                    break;
                }

                GameObject obj = (GameObject)_obj;
                list.Add(obj);
            }

            List<GameObject> mimicList = this.GetCurrentSceneAliveMimics();
            if (mimicList != null && mimicList.Count > 0)
            {
                list.AddRange(mimicList);
            }

            return list;
        }

        public uint GetPlayResult(int idx)
        {
            if (this.enemy == null)
            {
                return 0;
            }

            if (!this.enemy.ContainsKey(idx))
            {
                return 0;
            }

            FormulaHost _enemy = this.enemy[idx];
            return (uint)_enemy.GetDynamicDataByKey(SignKeys.PLAY_EVALUATE);
        }

        public int GetDamageRecord()
        {
            if (this.enemy == null)
            {
                return 0;
            }

            int score = 0;
            foreach (FormulaHost _enemy in this.enemy.Values)
            {
                int _score = (int)_enemy.GetDynamicDataByKey(SignKeys.SCORE);
                score += _score;
            }

            return score;
        }

        public bool IsDead(int idx)
        {
            if (this.enemy == null)
            {
                return true;
            }

            if (!this.enemy.ContainsKey(idx))
            {
                return true;
            }

            FormulaHost _enemy = this.enemy[idx];
            return _enemy.GetDynamicDataByKey(SignKeys.BATTLE_HP) <= 0;
        }

        public int GetGenCount()
        {
            return this.currentGenIdx - this.currentPlayIndex;
        }

        public int GetCurrentGenIdx()
        {
            return this.currentGenIdx;
        }

        public int GetCurrentPlayIdx()
        {
            return this.currentPlayIndex;
        }

        public void SetCurrentPlayIndex(int val)
        {
            this.currentPlayIndex = val;
        }

        public int GetDamageValueByIndex(int idx)
        {
            int v = 0;
            FormulaHost host = this.GetHost(idx);
            if (host != null)
            {
                v = (int)host.Result(FormulaKeys.FORMULA_8);
            }

            return v;
        }

        public int GetValueByIndex(int idx)
        {
            int v = 0;
            FormulaHost host = this.GetHost(idx);
            if (host != null)
            {
                v = (int)host.Result(FormulaKeys.FORMULA_22);
            }

            return v;
        }

        public string GetNodeBossAnimationByIdx(int idx)
        {
            MusicData md = StageBattleComponent.Instance.GetMusicDataByIdx(idx);
            return md.nodeData.boss_action;
        }

        public string GetNodeBossHitAnimationByIdx(int idx)
        {
            MusicData md = StageBattleComponent.Instance.GetMusicDataByIdx(idx);
            return md.nodeData.boss_hurt;
        }

        public string GetNodeAudioByIdx(int idx)
        {
            MusicData md = StageBattleComponent.Instance.GetMusicDataByIdx(idx);
            return md.nodeData.key_audio;
        }

        public string GetNodeUidByIdx(int idx)
        {
            MusicData md = StageBattleComponent.Instance.GetMusicDataByIdx(idx);
            return md.nodeData.uid;
        }

        public string GetNodeRoleActKeyByIdx(int idx)
        {
            MusicData md = StageBattleComponent.Instance.GetMusicDataByIdx(idx);
            return md.nodeData.char_action;
        }

        public string GetHitEffectByIdx(int idx)
        {
            MusicData md = StageBattleComponent.Instance.GetMusicDataByIdx(idx);
            if (!md.nodeData.isShowPlayEffect)
            {
                return null;
            }

            return md.nodeData.effect;
        }

        public int GetNodeTypeByIdx(int idx)
        {
            MusicData md = StageBattleComponent.Instance.GetMusicDataByIdx(idx);
            return (int)md.nodeData.type;
        }

        // ----------------------------------- // -----------------------------------

        public void SetDamageRecord(int idx, int damage)
        {
            if (!this.enemy.ContainsKey(idx))
            {
                return;
            }

            FormulaHost _enemy = this.enemy[idx];
            _enemy.SetDynamicData(SignKeys.SCORE, damage);
        }

        public void SetPlayResult(int idx, uint result)
        {
            if (!this.enemy.ContainsKey(idx))
            {
                return;
            }

            FormulaHost _enemy = this.enemy[idx];
            _enemy.SetDynamicData(SignKeys.PLAY_EVALUATE, result);

            StageTeachComponent.Instance.SetPlayResult(idx, result);
            TaskStageTarget.Instance.SetPlayResult(idx, result);

            if (result != GameMusic.NONE)
            {
                this.SetCurrentPlayIndex(idx);
            }

            this.SetPlayResultFireSkill(idx, result);

            this.AddHp(_enemy, -1);
        }

        private void SetPlayResultFireSkill(int idx, uint result)
        {
            // temp value record
            FormulaHost battleRole = BattleRoleAttributeComponent.Instance.GetBattleRole();
            if (battleRole != null)
            {
                battleRole.SetDynamicData(BattleRoleAttributeComponent.SK_TEMP_ADD_SCORE, 0);
            }

            MusicData md = StageBattleComponent.Instance.GetMusicDataByIdx(idx);
            if (result == GameMusic.MISS && GameGlobal.NODE_TYPES_NO_MISS.Contains(md.nodeData.type))
            {
                return;
            }

            if (result == GameMusic.JUMPOVER && !md.nodeData.jump_note)
            {
                return;
            }

            BattleRoleAttributeComponent.Instance.FireSkill(result);
        }

        public void SetLongPressEffect(bool isTo)
        {
            if (BattleEnemyManager.Instance.Enemies.ContainsKey(StageBattleComponent.Instance.curLPSIdx))
            {
                var go = (GameObject)BattleEnemyManager.Instance.Enemies[StageBattleComponent.Instance.curLPSIdx].GetDynamicObjByKey(SignKeys.GAME_OBJECT);
                if (go != null)
                {
                    var sac = go.GetComponent<SpineActionController>();
                    sac.PlayLongPressEffect(isTo);
                }
            }
        }

        public void SetPlayResultAfterTime(decimal tick, uint result)
        {
            ArrayList musicData = StageBattleComponent.Instance.GetMusicData();
            if (musicData == null)
            {
                return;
            }

            decimal _tick = tick + GameGlobal.COMEOUT_TIME_MAX;
            this.SetCurrentPlayIndex(-1);
            for (int i = 0; i < musicData.Count; i++)
            {
                MusicData md = (MusicData)musicData[i];
                if (md.configData.time < _tick)
                {
                    this.SetCurrentPlayIndex(i);
                    continue;
                }

                if (!this.enemy.ContainsKey(i))
                {
                    continue;
                }

                GameObject _obj = this.GetObj(i);
                if (_obj != null)
                {
                    GameObject.Destroy(_obj);
                    GameGlobal.gGameMusicScene.PreLoad(i);
                }

                FormulaHost _enemy = this.enemy[i];
                _enemy.SetDynamicData(SignKeys.PLAY_EVALUATE, result);
                //_enemy.SetDynamicData(SignKeys.BATTLE_HP, _enemy.Result(FormulaKeys.FORMULA_11));
            }

            Debug.Log("Set play results after " + tick + " " + this.GetCurrentPlayIdx());
        }

        public void AddHp(int idx, int value)
        {
            if (this.enemy == null)
            {
                return;
            }

            if (!this.enemy.ContainsKey(idx))
            {
                return;
            }

            FormulaHost _enemy = this.enemy[idx];
            this.AddHp(_enemy, value);
        }

        private void AddHp(FormulaHost _enemy, int value)
        {
            float curHp = _enemy.GetDynamicDataByKey(SignKeys.BATTLE_HP);
            _enemy.SetDynamicData(SignKeys.BATTLE_HP, (int)curHp + value);
        }

        // ----------------------------------- // -----------------------------------

        /// <summary>
        /// Creates the battle enemy.
        ///
        /// 敌人创建接口
        /// 对于host对象中已经有被使用记录(play result)的将不会创建gameobject对象
        /// </summary>
        /// <param name="idx">Index.</param>
        public void CreateBattleEnemy(int idx, bool isVisible = true)
        {
            MusicData md = StageBattleComponent.Instance.GetMusicDataByIdx(idx);
            FormulaHost _enemy = FomulaHostManager.Instance.CreateHost(HOST_IDX);
            this.enemy[idx] = _enemy;

            // 怪物配置等级
            string nodeId = BattleEnemyManager.Instance.GetNodeUidByIdx(idx);
            int cfgIdx = NodeConfigReader.GetNodeIdxByNodeid(nodeId);
            _enemy.SetDynamicData(SignKeys.ID, idx);
            _enemy.SetDynamicData(SignKeys.WHO, nodeId);
            _enemy.SetDynamicData(SignKeys.LEVEL, cfgIdx);
            _enemy.SetDynamicData(SignKeys.LEVEL_STAR, StageBattleComponent.Instance.GetId());
            _enemy.SetDynamicData(SignKeys.BATTLE_HP, 1);

            // Some node already been played will not be gen.
            uint playResult = this.GetPlayResult(idx);
            _enemy.SetDynamicData(SignKeys.GAME_OBJECT, null);
            if (playResult == GameMusic.NONE)
            {
                GameObject _obj = this.CreateObj(idx, !md.isLongPress);
                _enemy.SetDynamicData(SignKeys.GAME_OBJECT, _obj);
            }
            else
            {
                Debug.Log("CreateBattleEnemy " + idx + " under not none result " + playResult);
            }

            this.currentGenIdx = idx;
            if (GameGlobal.gGameMusic.LastOne(idx))
            {
                GirlManager.Instance.StopAutoReduceEnergy();
            }
        }

        private string GetObjPath(int idx)
        {
            string name = GameGlobal.gGameMusic.GetNodeAnimation(idx);
            if (name == Boss.Instance.GetBossName())
            {
                return name;
            }

            return name;
        }

        private GameObject CreateObj(int idx, bool isVisible = true)
        {
            GameObject obj = null;
            string filename = this.GetObjPath(idx);
            if (filename == Boss.Instance.GetBossName())
            {
                obj = Boss.Instance.GetGameObject();
            }
            else
            {
                obj = GameGlobal.gGameMusicScene.GetPreLoadGameObject(idx);
                if (obj == null)
                {
                    obj = GameGlobal.gGameMusicScene.PreLoad(idx);
                    if (obj == null)
                    {
                        if (!string.IsNullOrEmpty(filename))
                        {
                            obj = StageBattleComponent.Instance.AddObj(ref filename);
                        }
                    }
                }
            }

            if (obj == null)
            {
                CommonPanel.GetInstance().ShowText("找不到资源 " + filename);
                return null;
            }

            NodeInitController initController = obj.GetComponent<NodeInitController>();
            if (initController == null)
            {
                initController = obj.AddComponent<NodeInitController>();
            }

            initController.Init(idx);

            SpineActionController sac = obj.GetComponent<SpineActionController>();
            sac.Init(idx);
            obj.SetActive(isVisible);
            //SpineActionController.Pause(obj);
            //Debug.Log(System.Environment.TickCount);
            return obj;
        }
    }
}