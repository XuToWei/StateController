using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace StateController
{
    [Serializable]
    public sealed class StateControllerData
    { 
        [SerializeField]
        private string m_Name;
        [SerializeField, ReadOnly]
        private List<string> m_StateNames = new List<string>();

        private string m_SelectedName;
        private StateController m_Controller;

        public List<string> StateNames => m_StateNames;

        public string SelectedName
        {
            get
            {
                return m_SelectedName;
            }
            set
            {
                if (!m_StateNames.Contains(value))
                {
                    throw new Exception($"State name '{value}' is not in data '{m_Name}'.");
                }
                if(m_SelectedName == value)
                    return;
                m_SelectedName = value;
                foreach (var state in m_Controller.States)
                {
                    state.OnRefresh();
                }
            }
        }

        internal void OnInit(StateController controller)
        {
            m_Controller = controller;
        }

#if UNITY_EDITOR
        internal string Name
        {
            get => m_Name;
            set => m_Name = value;
        }
        internal List<string> Editor_StateNames => m_StateNames;
#endif
    }
}