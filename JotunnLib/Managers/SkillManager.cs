﻿using System.Collections.Generic;
using UnityEngine;

namespace JotunnLib.Managers
{
    public class SkillManager : Manager
    {
        public static SkillManager Instance { get; private set; }

        internal Dictionary<string, Skills.SkillDef> Skills = new Dictionary<string, Skills.SkillDef>();
        private int nextSkillId = 1000;
        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("Error, two instances of singleton: " + this.GetType().Name);
                return;
            }

            Instance = this;
        }

        public Skills.SkillDef RegisterSkill(string id, string name, string description, float increaseStep = 1f, Sprite icon = null)
        {
            LocalizationManager.Instance.RegisterTranslation("skill_" + nextSkillId, name);
            LocalizationManager.Instance.RegisterTranslation("skill_" + nextSkillId + "_description", description);

            Skills.SkillDef skillDef = new Skills.SkillDef()
            {
                m_skill = (Skills.SkillType)nextSkillId,
                m_description = "skill_" + nextSkillId + "_description",
                m_increseStep = increaseStep, // nice they spelled increase wrong 
                m_icon = icon
            };

            Skills.Add(id, skillDef);
            nextSkillId++;

            return skillDef;
        }

        public Skills.SkillDef GetSkill(string id)
        {
            if (Skills.ContainsKey(id))
            {
                return Skills[id];
            }

            return null;
        }
    }
}