using System;
using System.Collections.Generic;
using UnityEngine;

namespace StateController
{
    [Serializable]
    public sealed partial class StateControllerData
    { 
        [SerializeField]
        private string m_Name;
        [SerializeField]
        private List<string> m_StateNames = new List<string>();

        private string m_SelectedName;
        private StateController m_Controller;
        public string Name => m_Name;
        public List<string> StateNames => m_StateNames;

        public string SelectedName
        {
            get => m_SelectedName;
            set
            {
                if (!m_StateNames.Contains(value))
                    throw new Exception($"State name '{value}' is not in data '{m_Name}'.");
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
    }
}