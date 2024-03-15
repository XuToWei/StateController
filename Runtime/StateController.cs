using System.Collections.Generic;
using UnityEngine;

namespace StateController
{
    [DisallowMultipleComponent]
    public sealed partial class StateController : MonoBehaviour
    {
        [HideInInspector]
        [SerializeField]
        private List<StateControllerData> m_ControllerDatas = new List<StateControllerData>();

        private readonly List<BaseState> m_States = new List<BaseState>();
        public List<BaseState> States => m_States;

        private void Awake()
        {
            m_States.Clear();
            GetComponentsInChildren<BaseState>(true, m_States);
            foreach (var data in m_ControllerDatas)
            {
                data.OnInit(this);
            }
            foreach (var state in m_States)
            {
                state.OnInit(this);
            }
        }

        public void SelectedName(string dataName, string stateName)
        {
            var data = GetData(dataName);
            if (data != null)
            {
                data.SelectedName = stateName;
            }
        }

        public string GetSelectedName(string dataName)
        {
            var data = GetData(dataName);
            if (data != null)
            {
                return data.SelectedName;
            }
            return null;
        }

        public List<string> GetStateNames(string dataName)
        {
            var data = GetData(dataName);
            if (data != null)
            {
                return data.StateNames;
            }
            return null;
        }
        
        public StateControllerData GetData(string dateName)
        {
            foreach (var data in m_ControllerDatas)
            {
                if (data.Name == dateName)
                    return data;
            }
            return null;
        }
    }
}