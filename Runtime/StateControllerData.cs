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
        private int m_SelectedIndex = -1;
        private StateControllerMono m_ControllerMono;

        public string Name => m_Name;
        public List<string> StateNames => m_StateNames;
        public Action<string> OnSelectedNameChanged;
        public Action<int> OnSelectedIndexChanged;

        public string SelectedName
        {
            get => m_SelectedName;
            set
            {
                if (m_SelectedName == value)
                    return;
                int index = m_StateNames.IndexOf(value);
                if (index < 0)
                    throw new Exception($"State name '{value}' is not in data '{m_Name}'.");
                SetSelectedInternal(value, index);
            }
        }

        public int SelectedIndex
        {
            get => m_SelectedIndex;
            set
            {
                if (m_SelectedIndex == value)
                    return;
                if (value < 0 || value >= m_StateNames.Count)
                    throw new Exception($"State index '{value}' is not in data '{m_Name}'.");
                SetSelectedInternal(m_StateNames[value], value);
            }
        }

        private void SetSelectedInternal(string name, int index)
        {
            m_SelectedName = name;
            m_SelectedIndex = index;
            foreach (var state in m_ControllerMono.States)
            {
                state.OnRefresh();
            }
            var linkData = m_LinkDatas[index];
            var targetData = m_ControllerMono.GetData(linkData.TargetDataName);
            if (targetData != null && !string.IsNullOrEmpty(linkData.TargetSelectedName))
            {
                targetData.SelectedName = linkData.TargetSelectedName;
            }
            OnSelectedNameChanged?.Invoke(m_SelectedName);
            OnSelectedIndexChanged?.Invoke(m_SelectedIndex);
        }

        internal void OnInit(StateControllerMono controllerMono)
        {
            m_ControllerMono = controllerMono;
        }
    }
}