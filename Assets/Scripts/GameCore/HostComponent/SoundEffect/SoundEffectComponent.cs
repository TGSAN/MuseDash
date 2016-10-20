///自定义模块，可定制模块具体行为
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FormulaBase
{
    /// <summary>
    /// Sound effect component.
    ///
    /// 使用音效总配置播放音效模块
    /// </summary>
    public class SoundEffectComponent : CustomComponentBase
    {
        private static SoundEffectComponent instance = null;
        private const int HOST_IDX = 17;

        public static SoundEffectComponent Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SoundEffectComponent();
                    instance._speakerOfType = new Dictionary<string, string>();
                }
                return instance;
            }
        }

        // -------------------------------------------------------
        private const string CFG_NAME = "system_sfx";

        private const string SIGNKEY_EFFECTS = "SIGNKEY_EFFECTS";

        private bool isPause = false;
        private Dictionary<string, string> _speakerOfType = null;

        private FormulaHost GetSpeaker(string speaker)
        {
            if (speaker == null)
            {
                return null;
            }

            if (this.HostList == null)
            {
                this.GetList("SoundEffect");
            }

            FormulaHost speakerHost = this.GetHostByKeyValue(SignKeys.WHO, speaker);
            if (speakerHost != null)
            {
                return speakerHost;
            }

            speakerHost = FomulaHostManager.Instance.CreateHost("SoundEffect");
            Dictionary<string, List<FormulaHost>> _effects = new Dictionary<string, List<FormulaHost>>();
            for (int i = 0; i < ConfigPool.Instance.GetConfigLenght(CFG_NAME); i++)
            {
                string _i = (i + 1).ToString();
                string _speaker = ConfigPool.Instance.GetConfigStringValue(CFG_NAME, _i, "belong");
                if (_speaker != speaker)
                {
                    continue;
                }

                FormulaHost _effectHost = FomulaHostManager.Instance.CreateHost("SoundEffect");
                _effectHost.SetDynamicData(SignKeys.ID, i + 1);

                // init  attr
                _effectHost.Result(FormulaKeys.FORMULA_6);
                // Set path name and file name
                string _mn = _effectHost.GetDynamicStrByKey(SignKeys.MUSIC_NAME);
                if (_mn != null)
                {
                    string _name = string.Empty;
                    if (_mn.Contains("/"))
                    {
                        string[] _ss = _mn.Split('/');
                        _name = _ss[_ss.Length - 1];
                    }
                    else
                    {
                        _name = _mn;
                    }

                    _effectHost.SetDynamicData(SignKeys.NAME, _name);
                }

                // Add to type
                string _effType = _effectHost.GetDynamicStrByKey(SignKeys.TYPE);
                if (!_effects.ContainsKey(_effType))
                {
                    _effects.Add(_effType, new List<FormulaHost>());
                }

                _effects[_effType].Add(_effectHost);
            }

            if (_effects.Count == 0)
            {
                Debug.Log("Speaker " + speaker + " has no config in " + CFG_NAME);
            }

            speakerHost.SetDynamicData(SIGNKEY_EFFECTS, _effects);

            this.HostList[speaker] = speakerHost;

            return speakerHost;
        }

        public void PlayByEffectType(string effectType)
        {
            if (SceneAudioManager.Instance == null)
            {
                return;
            }

            if (SceneAudioManager.defaultTypeSource == null)
            {
                return;
            }

            if (!SceneAudioManager.defaultTypeSource.ContainsKey(effectType))
            {
                return;
            }

            AudioSource ads = SceneAudioManager.defaultTypeSource[effectType];
            if (ads == null)
            {
                return;
            }

            this.isPause = false;
            ads.Play();
        }

        public void PauseByEffectType(string effectType)
        {
            if (SceneAudioManager.Instance == null)
            {
                return;
            }

            if (SceneAudioManager.defaultTypeSource == null)
            {
                return;
            }

            if (!SceneAudioManager.defaultTypeSource.ContainsKey(effectType))
            {
                return;
            }

            AudioSource ads = SceneAudioManager.defaultTypeSource[effectType];
            if (ads == null)
            {
                return;
            }

            this.isPause = true;
            ads.Stop();
        }

        public bool IsPause()
        {
            return this.isPause;
        }

        public bool IsPlaying(string effectType)
        {
            if (SceneAudioManager.Instance == null)
            {
                return false;
            }

            if (SceneAudioManager.defaultTypeSource == null)
            {
                return false;
            }

            if (!SceneAudioManager.defaultTypeSource.ContainsKey(effectType))
            {
                return false;
            }

            AudioSource ads = SceneAudioManager.defaultTypeSource[effectType];
            if (ads == null)
            {
                return false;
            }

            return ads.isPlaying;
        }

        public string SpeakerOfType(string effectType)
        {
            if (!_speakerOfType.ContainsKey(effectType))
            {
                return null;
            }

            return _speakerOfType[effectType];
        }

        /// <summary>
        /// Say the specified speaker and id.
        ///
        /// 通过配置索引值播放音效
        /// </summary>
        /// <param name="id">Identifier.</param>
        public void Say(int id)
        {
            if (SceneAudioManager.Instance == null)
            {
                return;
            }

            string voice = ConfigPool.Instance.GetConfigStringValue(CFG_NAME, id.ToString(), "audio");
            if (voice == null)
            {
                return;
            }

            string effType = ConfigPool.Instance.GetConfigStringValue(CFG_NAME, id.ToString(), "type");

            if (SceneAudioManager.Instance == null)
            {
                return;
            }

            SceneAudioManager.Instance.Play(voice, effType);
        }

        /// <summary>
        /// Say the specified speaker and effectType.
        ///
        /// 通过配置音效类型随机播放相同speaker的该类型音效
        ///
        /// speaker 讲话人名，例如charactor_info配置的角色名字，没有名字的默认用"system"
        /// effectType 音效类型，GameGlobal有定义
        /// </summary>
        /// <param name="speaker">Speaker.</param>
        /// <param name="effectType">Effect type.</param>
        public void Say(string speaker, string effectType)
        {
            if (SceneAudioManager.Instance == null)
            {
                return;
            }

            FormulaHost speakerHost = this.GetSpeaker(speaker);
            if (speakerHost == null)
            {
                Debug.Log("Speaker " + speaker + " has no host data in SoundEffectComponent.");
                return;
            }

            Dictionary<string, List<FormulaHost>> _effects = (Dictionary<string, List<FormulaHost>>)speakerHost.GetDynamicObjByKey(SIGNKEY_EFFECTS);
            if (_effects == null || !_effects.ContainsKey(effectType))
            {
                Debug.Log("Speaker " + speaker + " has no effect data in SoundEffectComponent.");
                return;
            }

            List<FormulaHost> _effectByType = _effects[effectType];
            if (_effectByType == null || _effectByType.Count <= 0)
            {
                Debug.Log("Speaker " + speaker + " has no effect data in SoundEffectComponent.");
                return;
            }

            int idx = UnityEngine.Random.Range(0, _effectByType.Count);
            FormulaHost seHost = _effectByType[idx];
            if (seHost == null)
            {
                Debug.Log("Speaker " + speaker + " has no effect idx " + idx + " in SoundEffectComponent.");
                return;
            }

            string voice = seHost.GetDynamicStrByKey(SignKeys.MUSIC_NAME);
            _speakerOfType[effectType] = speaker;
            Debug.Log(speaker + " with type " + effectType + " say : " + voice);
            SceneAudioManager.Instance.Play(voice, effectType);
        }

        public void SayByCurrentRole(string effectType)
        {
            if (RoleManageComponent.Instance.Host == null)
            {
                Debug.Log("No role data in RoleManageComponent.");
                return;
            }

            string name = RoleManageComponent.Instance.Host.GetDynamicStrByKey(SignKeys.NAME);
            SoundEffectComponent.Instance.Say(name, effectType);
        }

        public void SayByCurrentScene(string effectType)
        {
            if (GameLogic.GameGlobal.gGameMusicScene == null)
            {
                return;
            }

            if (GameLogic.GameGlobal.gGameMusicScene.SecneObject == null)
            {
                return;
            }

            string name = GameLogic.GameGlobal.gGameMusicScene.SecneObject.name;
            SoundEffectComponent.Instance.Say(name, effectType);
        }

        /// <summary>
        /// Say the specified speaker, effectType and containStr.
        ///
        /// 通过配置播放播放该speaker某类型中，音效名称带有某个字符的音效
        /// 例如通过评价、装备等级等关键字"S A B C"选择播放对应音效
        ///
        ///
        /// speaker 讲话人名，例如charactor_info配置的角色名字，没有名字的默认用"system"
        /// effectType 音效类型，GameGlobal有定义
        /// containStr 音效名称包含的关键字，例如根据评级SABCD区分音效，可传入"S" "A" ...
        /// </summary>
        /// <param name="speaker">Speaker.</param>
        /// <param name="effectType">Effect type.</param>
        /// <param name="containStr">Contain string.</param>
        public void Say(string speaker, string effectType, string containStr)
        {
            if (SceneAudioManager.Instance == null)
            {
                return;
            }

            FormulaHost speakerHost = this.GetSpeaker(speaker);
            if (speakerHost == null)
            {
                return;
            }

            Dictionary<string, List<FormulaHost>> _effects = (Dictionary<string, List<FormulaHost>>)speakerHost.GetDynamicObjByKey(SIGNKEY_EFFECTS);
            if (_effects == null || !_effects.ContainsKey(effectType))
            {
                Debug.Log("Speaker " + speaker + " has no sound effect.");
                return;
            }

            List<FormulaHost> _effectByType = _effects[effectType];
            if (_effectByType == null || _effectByType.Count <= 0)
            {
                Debug.Log("Speaker " + speaker + " has no sound effect type " + effectType);
                return;
            }

            FormulaHost seHost = _effectByType.Find(delegate (FormulaHost _se)
            {
                string _n = _se.GetDynamicStrByKey(SignKeys.NAME);
                return (_n != null && _n.Contains(containStr));
            });

            if (seHost == null)
            {
                return;
            }

            string voice = seHost.GetDynamicStrByKey(SignKeys.MUSIC_NAME);
            _speakerOfType[effectType] = speaker;
            SceneAudioManager.Instance.Play(voice, effectType);
        }
    }
}