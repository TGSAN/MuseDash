using LitJson;
using System.Collections;
using UnityEngine;

namespace GameLogic
{
    public struct NodeConfigData
    {
        public static uint[] NODE_TYPE_IS_ADD_COMBO = new uint[]{
            GameGlobal.NODE_TYPE_MONSTER,
            GameGlobal.NODE_TYPE_HIDE,
            GameGlobal.NODE_TYPE_PRESS,
            GameGlobal.NODE_TYPE_BOSS,
            GameGlobal.NODE_TYPE_AIR_BEAT
        };

        public static uint[] NODE_TYPE_IS_MISS_COMBO = new uint[]{
            GameGlobal.NODE_TYPE_MONSTER,
            GameGlobal.NODE_TYPE_PRESS,
            GameGlobal.NODE_TYPE_BOSS,
            GameGlobal.NODE_TYPE_AIR_BEAT
        };

        public static uint[] NODE_TYPE_IS_SHOW_PLAY_EFFECT = new uint[]{
            GameGlobal.NODE_TYPE_MONSTER,
            GameGlobal.NODE_TYPE_HIDE,
            GameGlobal.NODE_TYPE_PRESS,
            GameGlobal.NODE_TYPE_BOSS,
            GameGlobal.NODE_TYPE_AIR_BEAT
        };

        public const string JUMP_ACTION = "char_jump";

        public string uid;
        public string scene;
        public string des;
        public string prefab_path;
        public uint hit_type;
        public uint type;
        public decimal life;
        // public decimal comeoutTime;

        public string effect;
        public string key_audio;
        public string boss_action;
        public string boss_hurt;
        public string char_action;

        public decimal a_perfect_range;
        public decimal a_great_range;
        public decimal a_cool_range;

        public decimal b_perfect_range;
        public decimal b_great_range;
        public decimal b_cool_range;

        public int damage;
        public int prop_value;
        public float damage_growth;

        public int enable_jump;

        public bool missCombo;
        public bool addCombo;
        public bool jump_note;
        public bool isShowPlayEffect;
        public decimal[] hitRange;
        public decimal hitRangeA;
        public decimal hitRangeB;
    }

    public struct MusicConfigData
    {
        public int id;
        public decimal time;
        public int level;
        public string note_uid;
        public decimal length;
    }

    public struct MusicData
    {
        public int objId;
        public decimal tick;
        public MusicConfigData configData;
        public NodeConfigData nodeData;
        public bool isLongPress;
        private decimal shotPause;
        private float attackRangeRate;
        private int size;       // size 0 is not  anything,1 is big,2 is small
        public bool isLongPressEnd;
        public int longPressPIdx;

        public bool isLongPressStart
        {
            get { return configData.length > 0; }
        }

        public int longPressCount
        {
            get { return (int)(configData.length / GameGlobal.LONG_PRESS_FREQUENCY); }
        }

        public void SetShotPause(decimal _tick)
        {
            if (_tick < 0)
            {
                _tick = 0m;
            }

            this.shotPause = _tick;
        }

        public decimal Tick()
        {
            return this.tick + this.shotPause;
        }

        public bool IsShotPausing()
        {
            return this.shotPause > 0;
        }

        public float GetAttackrangeRate()
        {
            return this.attackRangeRate;
        }

        public int GetSize()
        {
            return this.size;
        }

        public void SetAttackRangeRate(float value)
        {
            this.attackRangeRate = value;
        }

        public bool IsPressTemp()
        {
            return this.nodeData.char_action == "char_press";
        }

        public uint ResultPhaser(decimal passedTick)
        {               //(check) check attack point position here
            /*decimal result = this.Tick() - passedTick;
            decimal perfectRange = this.nodeData.hitRange[GameMusic.PERFECT_RANGE_INDEX];
            decimal greateRange = this.nodeData.hitRange[GameMusic.GREAT_RANGE_INDEX];
            decimal coolRange = this.nodeData.hitRange[GameMusic.COOL_RANGE_INDEX];
            if (result < 0)
            {
                return GameMusic.NONE;
            }

            if (this.nodeData.hitRangeAll != 0)
            {
                this.attackRangeRate = (float)(result / this.nodeData.hitRangeAll);
            }
            //if (result < PERFECTINTERVAL) {
            //	return MISS;
            //}
            if ((this.nodeData.enable_jump == 1) && GameGlobal.gGameTouchPlay.IsJump())
            {
                return GameMusic.JUMPOVER;
            }

            if (result < perfectRange)
            {
                return GameMusic.PERFECT;
            }

            if (result < perfectRange + greateRange)
            {
                return GameMusic.GREAT;
            }

            if (result < perfectRange + greateRange + coolRange)
            {
                return GameMusic.COOL;
            }
*/
            return GameMusic.NONE;
        }
    }

    /// <summary>
    /// node data reader.
    /// 详细说明见基类
    /// </summary>
    public class NodeConfigReader : BaseConfigReader
    {
        private const string CONFIG_NAME = "notedata";

        // 简化版单例
        private NodeConfigReader() { }

        public static NodeConfigReader Instance = new NodeConfigReader();

        private const uint lenHitRange = 6;

        // override Init
        public override void Init()
        {
            if (this.GetDataCount() > 0)
            {
                return;
            }

            // JsonData _data = this.GetJsonConfig (CONFIG_NAME);
            JsonData _data = ConfigPool.Instance.GetConfigByName(CONFIG_NAME);
            for (int i = 0; i <= _data.Count; i++)
            {
                string k = i.ToString();
                NodeConfigData sd = new NodeConfigData();
                if (!_data.Keys.Contains(k))
                {
                    this.Add(sd);
                    continue;
                }

                sd = (NodeConfigData)this.ConfigToObject(_data[k], sd);

                // 从这里以下开始调整属性
                sd.hit_type = GameMusic.TOUCH_ACTION_SIGNLE_PRESS;
                if (sd.type == GameGlobal.NODE_TYPE_NONE)
                {
                    sd.hit_type = GameMusic.TOUCH_ACTION_NONE;
                }

                sd.addCombo = false;
                sd.missCombo = false;
                sd.isShowPlayEffect = false;
                sd.jump_note = (sd.char_action == NodeConfigData.JUMP_ACTION);
                foreach (uint _t in NodeConfigData.NODE_TYPE_IS_ADD_COMBO)
                {
                    if (sd.type == _t)
                    {
                        sd.addCombo = true;
                        break;
                    }
                }

                foreach (uint _t in NodeConfigData.NODE_TYPE_IS_MISS_COMBO)
                {
                    if (sd.type == _t)
                    {
                        sd.missCombo = true;
                        break;
                    }
                }

                foreach (uint _t in NodeConfigData.NODE_TYPE_IS_SHOW_PLAY_EFFECT)
                {
                    if (sd.type == _t)
                    {
                        sd.isShowPlayEffect = true;
                        break;
                    }
                }

                sd.hitRange = new decimal[lenHitRange];
                sd.hitRange[GameMusic.A_PERFECT_RANGE_INDEX] = sd.a_perfect_range;
                sd.hitRange[GameMusic.A_GREAT_RANGE_INDEX] = sd.a_great_range;
                sd.hitRange[GameMusic.A_COOL_RANGE_INDEX] = sd.a_cool_range;
                sd.hitRange[GameMusic.B_PERFECT_RANGE_INDEX] = sd.b_perfect_range;
                sd.hitRange[GameMusic.B_GREAT_RANGE_INDEX] = sd.b_great_range;
                sd.hitRange[GameMusic.B_COOL_RANGE_INDEX] = sd.b_cool_range;
                sd.hitRangeA = sd.a_perfect_range + sd.a_great_range + sd.a_cool_range;
                sd.hitRangeB = sd.b_perfect_range + sd.b_great_range + sd.b_cool_range;
                this.Add(sd);
            }
        }

        public static int GetNodeIdxByNodeid(string uid)
        {
            int len = ConfigPool.Instance.GetConfigByName(CONFIG_NAME).Count;
            foreach (string keyId in ConfigPool.Instance.GetConfigByName(CONFIG_NAME).Keys)
            {
                if (uid == ConfigPool.Instance.GetConfigStringValue(CONFIG_NAME, keyId, "uid"))
                {
                    return int.Parse(keyId);
                }
            }

            return 0;
        }
    }

    /// <summary>
    /// musci data reader.
    /// 详细说明见基类
    /// 这个实现可以作为比较复杂的数据处理的例子
    /// </summary>
    public class MusicConfigReader : BaseConfigReader
    {
        // 简化版单例
        private MusicConfigReader() { }

        public static MusicConfigReader Instance = new MusicConfigReader();

        private const int tickLimit = 0;

        // override Init
        public override void Init(ref string filename)
        {
            if (this.GetDataCount() > 0)
            {
                return;
            }

            NodeConfigReader.Instance.Init();
            ArrayList nodeData = NodeConfigReader.Instance.GetData();
            JsonData _data = ConfigPool.Instance.GetConfigByName(filename);
            // Start from 1
            this.Add(new MusicData());
            var idx = 1;
            for (int i = 0; i < _data.Count; i++)
            {
                MusicConfigData sd = new MusicConfigData();
                sd = (MusicConfigData)this.ConfigToObject(_data[i], sd);
                if (sd.time < tickLimit)
                {
                    Debug.Assert(false, "关卡配置" + filename + "存在Timming少于2秒的node点 : " + i);
                    continue;
                }

                MusicData md = new MusicData();
                md.objId = idx++;
                md.tick = decimal.Round(sd.time, 2);
                md.configData = sd;
                md.SetAttackRangeRate(-1f);
                string nodeId = md.configData.note_uid;
                if (md.objId <= 0 || nodeId == null)
                {
                    Debug.Assert(false, "关卡配置" + filename + "存在无效note_uid : " + i);
                }

                foreach (NodeConfigData _nd in nodeData)
                {
                    if (_nd.uid == nodeId)
                    {
                        md.nodeData = _nd;
                        break;
                    }
                }
                md.isLongPress = false;
                md.isLongPressEnd = false;
                this.Add(md);

                if (md.isLongPressStart)
                {
                    var count = md.longPressCount;
                    for (int j = 1; j <= count; j++)
                    {
                        var longPressMd = new MusicData();
                        longPressMd.objId = idx++;
                        longPressMd.tick = md.tick + GameGlobal.LONG_PRESS_FREQUENCY * j;
                        longPressMd.configData = md.configData;
                        longPressMd.configData.length = 0;
                        longPressMd.isLongPress = true;
                        longPressMd.SetAttackRangeRate(md.GetAttackrangeRate());
                        longPressMd.nodeData = md.nodeData;
                        longPressMd.isLongPressEnd = j == count;
                        longPressMd.longPressPIdx = md.objId;
                        this.Add(longPressMd);
                    }
                }
            }
        }

        public override ArrayList GetData(ref string key)
        {
            this.ClearData();
            this.Init(ref key);
            return this.GetData();
        }

        public override object GetData(int idx)
        {
            if (idx < 0)
            {
                return null;
            }

            ArrayList data = this.GetData();
            if (data == null || idx > data.Count)
            {
                return null;
            }

            return data[idx];
        }
    }
}