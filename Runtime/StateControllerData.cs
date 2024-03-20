using System;
using System.Collections.Generic;
using UnityEngine;

namespace StateController
{
    [Serializable]
    internal partial class LinkData
    {
        [SerializeField]
        private string m_TargetDataName = string.Empty;
        [SerializeField]
        private string m_TargetSelectedName = string.Empty;
        public string TargetDataName => m_TargetDataName;
        public string TargetSelectedName => m_TargetSelectedName;
    }
    
    [Serializable]
    public sealed partial class StateControllerData
    { 
        [SerializeField]
        private string m_Name;
        [SerializeField]
        private List<string> m_StateNames = new List<string>();
        [SerializeField]
        private List<LinkData> m_LinkDatas = new List<LinkData>();

        private string m_SelectedName;
        private StateController m_Controller;
        public string Name => m_Name;
        public List<string> StateNames => m_StateNames;

        public string SelectedName
        {
            get => m_SelectedName;
            set
            {
                if(m_SelectedName == value)
                    return;
                int index = m_StateNames.IndexOf(value);
                if (index < 0)
                    throw new Exception($"State name '{value}' is not in data '{m_Name}'.");
                m_SelectedName = value;
                foreach (var state in m_Controller.States)
                {
                    state.OnRefresh();
                }
                var linkData = m_LinkDatas[index];
                var data = m_Controller.GetData(linkData.TargetDataName);
                if (data != null && !string.IsNullOrEmpty(linkData.TargetSelectedName))
                {
                    data.SelectedName = linkData.TargetSelectedName;
                }
            }
        }

        internal void OnInit(StateController controller)
        {
            m_Controller = controller;
        }
    }
}