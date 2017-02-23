using GameLogic;
using LitJson;

///自定义模块，可定制模块具体行为
using System;
using System.Collections.Generic;
using Assets.Scripts.Tools.Managers;
using UnityEngine;

namespace FormulaBase
{
    public class BattlePetComponent : CustomComponentBase
    {
        private static BattlePetComponent instance = null;
        private const int HOST_IDX = 11;

        public static BattlePetComponent Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BattlePetComponent();
                }
                return instance;
            }
        }

        // ------------------------------------------------------------------------------------
        private int currentPetIndex = -1;

        private FormulaHost currentSkillObject = null;
        private Dictionary<int, FormulaHost> pets = null;

        public void Init()
        {
            this.currentPetIndex = -1;
            this.currentSkillObject = null;
            this.pets = new Dictionary<int, FormulaHost>();
            string[] testpets = new string[] { "51010" }; //, "53010", "52010"};
            for (int i = 0; i < testpets.Length; i++)
            {
                string pid = testpets[i];

                //string perfabName = ConfigManager.instance.GetConfigStringValue("Elfin", "Uid", "Prefab", pid);
#if UNITY_EDITOR || UNITY_EDITOR_OSX || UNITY_EDITOR_64
                if (AdminData.Instance.DefaultPetSkill > 0)
                {
                    //actSkillId = AdminData.Instance.DefaultPetSkill;
                }
#endif
                // Init pet data.
                FormulaHost host = FomulaHostManager.Instance.CreateHost(HOST_IDX);
                host.SetDynamicData(SignKeys.ID, i);
                /*host.SetDynamicData(SkillComponent.SIGN_KEY_ACTIVE_SKILL, actSkillId.ToString());
                host.SetDynamicData(SkillComponent.SIGN_KEY_PASSIVE_SKILL, passiveSkillId.ToString());*/

                //host.SetDynamicData(SignKeys.NAME, perfabName);

                // Init pet skill obj.
                /* FormulaHost actSkillObj = SkillComponent.Instance.CreateSkill(actSkillId, host);
                 FormulaHost passiveSkillObj = SkillComponent.Instance.CreateSkill(passiveSkillId, host);
                 if (actSkillObj != null)
                 {
                     host.SetDynamicData(SkillComponent.SIGN_KEY_ACTIVE_SKILL_OBJ, actSkillObj);
                     SkillComponent.Instance.FireSkill(actSkillObj, SkillComponent.ON_EQUIP);
                 }

                 if (passiveSkillObj != null)
                 {
                     host.SetDynamicData(SkillComponent.SIGN_KEY_PASSIVE_SKILL_OBJ, passiveSkillObj);
                     SkillComponent.Instance.FireSkill(passiveSkillObj, SkillComponent.ON_EQUIP);
                 }*/

                this.pets[i] = host;
            }
        }

        public int Count()
        {
            if (this.pets == null)
            {
                return 0;
            }

            return this.pets.Count;
        }

        public void SetGameObject(int idx, GameObject obj)
        {
            FormulaHost host = this.pets[idx];
            host.SetDynamicData(SignKeys.GAME_OBJECT, obj);
        }

        public string[] GetPetPerfabNames()
        {
            if (this.pets == null || this.pets.Count <= 0)
            {
                return null;
            }

            string[] pnames = new string[this.pets.Count];
            for (int i = 0; i < this.pets.Count; i++)
            {
                if (!this.pets.ContainsKey(i))
                {
                    continue;
                }

                FormulaHost host = this.pets[i];
                pnames[i] = host.GetDynamicStrByKey(SignKeys.NAME);
            }

            return pnames;
        }

        public void SwitchPet()
        {
            this.currentSkillObject = null;
            GameObject[] arms = GirlManager.Instance.Arms;
            if (arms == null || this.currentPetIndex >= arms.Length)
            {
                ArmActionController.Instance = null;
                return;
            }

            this.currentPetIndex += 1;
            if (this.currentPetIndex >= arms.Length)
            {
                ArmActionController.Instance = null;
                return;
            }

            GameObject armObj = arms[this.currentPetIndex];
            if (armObj != null)
            {
                ArmActionController.Instance = armObj.GetComponent<ArmActionController>();
            }

            ArmActionController.Instance.OnControllerStart();
        }

        public float UseSkill()
        {
            float tick = -1f;
            if (this.currentPetIndex < 0)
            {
                return tick;
            }

            if (this.pets == null || this.pets.Count <= this.currentPetIndex)
            {
                return tick;
            }

            if (this.currentSkillObject != null)
            {
                return tick;
            }

            // Set using skill id.
            FormulaHost host = this.pets[this.currentPetIndex];
            int skillId = (int)host.GetDynamicDataByKey(SkillComponent.SIGN_KEY_ACTIVE_SKILL);

            this.currentSkillObject = (FormulaHost)host.GetDynamicObjByKey(SkillComponent.SIGN_KEY_ACTIVE_SKILL_OBJ);
            if (this.currentSkillObject == null)
            {
                return tick;
            }

            SkillComponent.Instance.FireSkill(this.currentSkillObject, SkillComponent.ON_START);
            // Get skill duration.
            tick = this.currentSkillObject.GetDynamicDataByKey(SkillComponent.SIGN_KEY_COUNTDOWN);

            Debug.Log("Arm(Pet) " + this.currentPetIndex + " use skill " + skillId + " with time " + tick);

            return tick;
        }

        public void EndSkill()
        {
            if (this.currentSkillObject == null)
            {
                return;
            }

            SkillComponent.Instance.FireSkill(this.currentSkillObject, SkillComponent.ON_TIME_UP);
        }

        public void FireSkill(uint condiction)
        {
            if (this.currentSkillObject == null)
            {
                return;
            }

            int skillId = this.currentSkillObject.GetDynamicIntByKey(SignKeys.ID);
            SkillComponent.Instance.FireSkill(this.currentSkillObject, condiction);

            //Debug.Log ("Arm(Pet) " + this.currentPetIndex + " fire skill effect " + effectId + " with action key " + actKey);
        }

        public FormulaHost GetCurrentSkill()
        {
            return this.currentSkillObject;
        }

        public FormulaHost GetCurrentPet()
        {
            if (!this.pets.ContainsKey(this.currentPetIndex))
            {
                return null;
            }

            return this.pets[this.currentPetIndex];
        }
    }
}