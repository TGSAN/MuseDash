﻿using System.Collections.Generic;
using Assets.Scripts.Common;
using UnityEngine;

namespace Assets.Scripts.Tool.FormulaTreeManager
{
    public class FormulaTree : SingletonScriptObject<FormulaTree>
    {
        public List<FormulaNode> nodes = new List<FormulaNode>();
        public FormulaNode head;
        public FormulaObjDictionary formulaObjs;

        public new static FormulaTree instance
        {
            get
            {
                m_Instance = m_Instance ?? (m_Instance = Resources.Load<FormulaTree>(StringCommon.FormulaTreePathInResources));
                return m_Instance ?? ScriptableObject.CreateInstance<FormulaTree>();
            }
        }
    }
}