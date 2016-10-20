using GameLogic;
using LitJson;

///自定义模块，可定制模块具体行为
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FormulaBase
{
    public class BattleRoleAttributeComponent : CustomComponentBase
    {
        private static BattleRoleAttributeComponent instance = null;
        private const int HOST_IDX = 2;

        public static BattleRoleAttributeComponent Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BattleRoleAttributeComponent();
                    if (instance.Host == null)
                    {
                        instance.Host = instance.GetBattleRole();
                    }
                }
                return instance;
            }
        }

        // -------------------------------------------------------------------------
        public const string SK_TEMP_ADD_SCORE = "SK_TEMP_ADD_SCORE";

        public void Init()
        {
            this.Host = this.GetBattleRole();
            this.InitSkills();
            GirlManager.Instance.Reset();
            this.FireSkill(SkillComponent.ON_EQUIP_ARM);
        }

        private void InitSkills()
        {
            if (this.Host == null)
            {
                return;
            }

            // test
            List<int> skillIds = new List<int>();
#if UNITY_EDITOR || UNITY_EDITOR_OSX || UNITY_EDITOR_64
            if (skillIds == null || skillIds.Count <= 0)
            {
                skillIds = AdminData.Instance.DefaultSkills;
            }
#endif
            if (skillIds == null || skillIds.Count <= 0)
            {
                return;
            }

            List<FormulaHost> skills = new List<FormulaHost>();
            for (int i = 0; i < skillIds.Count; i++)
            {
                int skId = skillIds[i];
                FormulaHost skillObject = SkillComponent.Instance.CreateSkill(skId, this.Host);
                SkillComponent.Instance.FireSkill(skillObject, SkillComponent.ON_EQUIP);
                // SkillComponent.Instance.FireSkill (skillObject, SkillComponent.ON_START);
                skills.Add(skillObject);
            }

            this.Host.SetDynamicData(SkillComponent.SIGN_KEY_SKILLS, skills);
        }

        public FormulaHost GetBattleRole()
        {
            if (this.Host == null)
            {
                this.Host = FomulaHostManager.Instance.LoadHost(HostKeys.HOST_2);
                this.Revive();
                int HeroIndex = RoleManageComponent.Instance.GetFightGirlIndex();
                if (HeroIndex < 0)
                {
                    HeroIndex = 0;
#if UNITY_EDITOR || UNITY_EDITOR_OSX || UNITY_EDITOR_64
                    HeroIndex = AdminData.Instance.DefaultRoleIdx;
#endif
                }

                this.Host.SetDynamicData(SignKeys.ID, RoleManageComponent.RoleIndexToId(HeroIndex));
                this.Host.Result(FormulaKeys.FORMULA_178);
                this.Host.SetAsUINotifyInstance();
            }

            return this.Host;
        }

        public GameObject GetBattleRoleObject()
        {
            FormulaHost host = this.GetBattleRole();
            if (host == null)
            {
                return null;
            }

            return (GameObject)host.GetDynamicObjByKey(SignKeys.GAME_OBJECT);
        }

        public bool IsComboProtect()
        {
            if (this.Host == null)
            {
                return false;
            }

            if (this.Host.GetDynamicIntByKey("COMBO_PROTECT") > 0)
            {
                return true;
            }

            FormulaHost pet = BattlePetComponent.Instance.GetCurrentPet();
            if (pet == null)
            {
                return false;
            }

            if (pet.GetDynamicIntByKey("COMBO_PROTECT") > 0)
            {
                return true;
            }

            return false;
        }

        public void AddHp(int value, bool showChange = true)
        {
            FormulaHost battleRole = this.GetBattleRole();
            if (battleRole == null)
            {
                return;
            }

            this.AddHp(battleRole, value);
            if (value > 0)
            {
                TaskStageTarget.Instance.AddRecover(value);
            }

            this.FireSkill(SkillComponent.ON_HP_CHANGE);
            if (showChange && CharPanel.Instance != null)
            {
                CharPanel.Instance.SetHpChange(value);
            }
        }

        public void AddHp(FormulaHost host, int value)
        {
            // If is teach stage, hp is lock.
            if (StageTeachComponent.Instance.IsTeachingStage())
            {
                return;
            }

            float hpMax = this.GetHpMax(host);
            float tempHp = host.GetDynamicDataByKey(SignKeys.BATTLE_HP);
            float valuef = (tempHp + value > hpMax) && (value > 0) ? hpMax - tempHp : value;

            host.AddDynamicValue(SignKeys.BATTLE_HP, valuef);

            tempHp = host.GetDynamicDataByKey(SignKeys.BATTLE_HP);
            FightMenuPanel.Instance.SetHp(tempHp / hpMax, valuef);

            if (tempHp > 0)
            {
                return;
            }

            this.Dead();
        }

        public float HpRate(FormulaHost host = null)
        {
            if (host == null)
            {
                host = this.Host;
            }

            if (host == null)
            {
                return 0;
            }

            float hpMax = this.GetHpMax(host);
            float tempHp = host.GetDynamicDataByKey(SignKeys.BATTLE_HP);
            return tempHp / hpMax;
        }

        public int GetHp(FormulaHost host = null)
        {
            if (host == null)
            {
                host = this.Host;
            }

            if (host == null)
            {
                return 0;
            }

            return (int)host.GetDynamicDataByKey(SignKeys.BATTLE_HP);
        }

        public int GetHpMax(FormulaHost host = null)
        {
            if (host == null)
            {
                host = this.Host;
            }

            if (host == null)
            {
                return 0;
            }

            int baseValue = (int)host.Result(FormulaKeys.FORMULA_186);
            int skPlus = (int)this.GetSkillSwitchAddUp(SkillComponent.SING_KEY_SWITCHS_HP, baseValue);

            return baseValue + skPlus;
        }

        public float GetDefence(FormulaHost host = null)
        {
            if (host == null)
            {
                host = this.Host;
            }

            if (host == null)
            {
                return 0;
            }

            float baseValue = host.Result(FormulaKeys.FORMULA_1);
            float skPlus = this.GetSkillSwitchAddUp(SkillComponent.SING_KEY_SWITCHS_DEF, baseValue);

            return baseValue + skPlus;
        }

        public void Hurt(int hurtValue, FormulaHost host = null)
        {
            if (host == null)
            {
                host = this.Host;
            }

            if (host == null)
            {
                return;
            }

            float _def = this.GetDefence(host);
            float _hurtValue = (hurtValue * (1f - _def));
            this.AddHp((int)_hurtValue);
        }

        public bool IsDead()
        {
            return this.GetHp() <= 0;
        }

        public void Dead()
        {
            // If host(player) hp <= 0, is die.
            GirlManager.Instance.StopPhysicDetect();
            GirlManager.Instance.StopAutoReduceEnergy();
            //GirlManager.Instance.PlayGirlDeadAnimation ();

            // If player die, mimic excape.
            if (MimicParentController.Instance != null)
            {
                MimicParentController.Instance.OnControllerMiss(-1);
            }

            StageBattleComponent.Instance.Dead();
            CharPanel.Instance.HideCombo();
            UISceneHelper.Instance.ShowUi("PnlFail");
            //ResurgencePanelScript.Instance.Show ();
        }

        public void Revive(bool ifFirstLoad = false)
        {
            FormulaHost battleRole = this.GetBattleRole();
            if (battleRole == null)
            {
                return;
            }

            float hpMax = battleRole.Result(FormulaKeys.FORMULA_186);
            battleRole.SetDynamicData(SignKeys.BATTLE_HP, hpMax);
            battleRole.SetDynamicData(SignKeys.HP, hpMax);
            battleRole.SetAsUINotifyInstance();
            if (StageTeachComponent.Instance.IsTeachingStage())
            {
                return;
            }
            /*
			if (FightMenuPanel.Instance != null) {
				if (ifFirstLoad == false) {
					FightMenuPanel.Instance.SetHp (1f);
				} else {
					FightMenuPanel.Instance.SetHp (0f);
				}
			}
			*/
        }

        public void FireSkill(uint condiction)
        {
            if (this.Host == null)
            {
                return;
            }

            List<FormulaHost> skills = (List<FormulaHost>)this.Host.GetDynamicObjByKey(SkillComponent.SIGN_KEY_SKILLS);
            SkillComponent.Instance.FireSkill(skills, condiction);
        }

        public void RemoveSkill(int idx)
        {
            if (idx < 0 || this.Host == null)
            {
                return;
            }

            List<FormulaHost> skills = (List<FormulaHost>)this.Host.GetDynamicObjByKey(SkillComponent.SIGN_KEY_SKILLS);
            if (skills == null || skills.Count <= 0)
            {
                return;
            }

            if (idx >= skills.Count)
            {
                return;
            }

            FormulaHost skillObject = skills[idx];
            if (skillObject == null)
            {
                return;
            }

            skills[idx] = null;
            SkillComponent.Instance.RemoveSkill(skillObject);

            this.Host.SetDynamicData(SkillComponent.SIGN_KEY_SKILLS, skills);
        }

        /// <summary>
        /// Gets the skill switch add up.
        ///
        /// 技能引起的动态加成值
        /// </summary>
        /// <returns>The skill switch add up.</returns>
        /// <param name="signKey">Sign key.</param>
        /// <param name="baseValue">Base value.</param>
        private float GetSkillSwitchAddUp(string signKey, float baseValue)
        {
            List<FormulaHost> skills = (List<FormulaHost>)this.Host.GetDynamicObjByKey(SkillComponent.SIGN_KEY_SKILLS);
            if (skills == null || skills.Count <= 0)
            {
                return 0;
            }

            int skPlus = 0;
            foreach (FormulaHost skillObject in skills)
            {
                if (skillObject == null)
                {
                    continue;
                }

                Dictionary<int, float> switchs = (Dictionary<int, float>)skillObject.GetDynamicObjByKey(signKey);
                if (switchs == null || switchs.Keys.Count <= 0)
                {
                    continue;
                }

                foreach (int effectId in switchs.Keys)
                {
                    float _swichvalue = switchs[effectId];
                    if (_swichvalue <= 0)
                    {
                        continue;
                    }

                    skPlus += (int)(baseValue * _swichvalue);
                }
            }

            return skPlus;
        }

        /// <summary>
        /// Calculates the critical.
        ///
        /// 暴击计算
        /// </summary>
        private void CalcCritical()
        {
            float feverValue = 0;
            int criticalFlag = 0;
            float skPlus = this.GetSkillSwitchAddUp(SkillComponent.SING_KEY_SWITCHS_HP, 1);
            this.Host.SetDynamicData(SignKeys.BATTLE_ADD_UP_CTR_DYNAMIC, skPlus);
            if (GameKernel.Instance.IsOnFeverState())
            {
                criticalFlag = 1;
            }
            else
            {
                feverValue = Host.Result(FormulaKeys.FORMULA_87);
                criticalFlag = (int)this.Host.Result(FormulaKeys.FORMULA_46); // xxx is get crt formula
            }

            this.Host.SetDynamicData(SignKeys.CTR, criticalFlag);
            if (criticalFlag > 0)
            {
                TaskStageTarget.Instance.AddCrtCount(1);
                this.FireSkill(SkillComponent.ON_CRITICAL);
            }

            if (feverValue > 0)
            {
                GameKernel.Instance.AddFever(feverValue);
                FightMenuPanel.Instance.SetFerver(GameKernel.Instance.GetFeverRate());
            }
        }

        /// <summary>
        /// Calculates the attack damage.
        ///
        /// 单击伤害
        /// </summary>
        /// <returns>The attack damage.</returns>
        private int CalcAttackDamage()
        {
            if (this.Host == null)
            {
                return 0;
            }

            // Role attack from fromula.
            int attack = (int)this.Host.Result(FormulaKeys.FORMULA_7);

            // Skill dynamic effect to attack.
            int skPlus = (int)this.GetSkillSwitchAddUp(SkillComponent.SING_KEY_SWITCHS_ATK, attack);

            return attack + skPlus;
        }

        /// <summary>
        /// Calculates the damage.
        ///
        /// 打击得分
        /// </summary>
        /// <param name="idx">Index.</param>
        /// <param name="result">Result.</param>
        /// <param name="actType">Act type.</param>
        public void AttackScore(int idx, int result, int actType)
        {
            FormulaHost battleRole = this.GetBattleRole();
            if (battleRole == null)
            {
                return;
            }

            // If is teach stage, score is lock.
            if (StageTeachComponent.Instance.IsTeachingStage())
            {
                return;
            }

            // 跳过普通note不应有分数（enable_jump）
            MusicData md = StageBattleComponent.Instance.GetMusicDataByIdx(idx);
            if (md.nodeData.enable_jump == 1 && GirlManager.Instance.IsJumpingAction())
            {
                return;
            }

            int combo = StageBattleComponent.Instance.GetCombo();
            battleRole.SetDynamicData(SignKeys.PLAY_EVALUATE, result);
            battleRole.SetDynamicData(SignKeys.COMBO, combo);

            this.CalcCritical();

            int score = this.CalcAttackDamage();
            TaskStageTarget.Instance.AddScore(score);

            if (md.nodeData.isShowPlayEffect)
            {
                CharPanel.Instance.SetScore((uint)result, score + this.Host.GetDynamicIntByKey(SK_TEMP_ADD_SCORE));
            }

            //FightMenuPanel.Instance.SetScore (TaskStageTarget.Instance.GetScore ());
            //this.AfterAttackScore (idx, result, actType, score);
        }

        public void AfterAttackScore(int idx, int result, int actType, int score = 0)
        {
            this.Host.SetDynamicData(SignKeys.CTR, 0);

            if (GameGlobal.IS_DEBUG)
            {
                Debug.Log("Hit succ with Damage " + idx + " result/ " + result + " /type " + actType + " dmg/ " + score);
            }

            /*
			// Fire buff event.
			// GameGlobal.gBuffMgr.FireCondiction (BuffManager.CT_ON_ATTACK);

			// boss hp hurt
			if (!md.nodeData.dmgBoss) {
				return;
			}

			if (GameGlobal.gStage.GetBossHp () <= 0) {
				return;
			}

			GameGlobal.gStage.AddBossHp (-score);
			int bosshp = GameGlobal.gStage.GetBossHp ();
			if (bosshp > 0) {
				return;
			}

			GameGlobal.gStage.AfterKillBoss ();
			*/
        }
    }
}