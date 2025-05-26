using System.Collections.Generic;
using UnityEngine;

namespace StateController
{
    [DisallowMultipleComponent]
    public sealed partial class StateControllerMono : MonoBehaviour
    {
        [HideInInspector]
        [SerializeField]
        private List<StateControllerData> m_ControllerDatas = new List<StateControllerData>();

        private readonly List<BaseState> m_States = new List<BaseState>();
        public List<BaseState> States => m_States;

        private bool m_IsInit;

        private void Awake()
        {
            TryInit();
        }

        private void TryInit()
        {
            if(m_IsInit)
                return;
            m_IsInit = true;
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

        public void SetSelectedName(string dataName, string selectedName)
        {
            var data = GetData(dataName);
            if (data != null)
            {
                data.SelectedName = selectedName;
            }
        }

        public void SetSelectedIndex(string dataName, int selectedIndex)
        {
            var data = GetData(dataName);
            if (data != null)
            {
                data.SelectedIndex = selectedIndex;
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

        public int GetSelectedIndex(string dataName)
        {
            var data = GetData(dataName);
            if (data != null)
            {
                return data.SelectedIndex;
            }
            return -1;
        }

        public string[] GetStateNames(string dataName)
        {
            var data = GetData(dataName);
            if (data != null)
            {
                return data.StateNames.ToArray();
            }
            return null;
        }

        public void GetStateNames(string dataName, List<string> results)
        {
            if (results == null)
            {
                throw new System.ArgumentNullException(nameof(results));
            }
            results.Clear();
            var data = GetData(dataName);
            if (data != null)
            {
                results.AddRange(data.StateNames);
            }
        }

        public StateControllerData GetData(string dateName)
        {
            TryInit();
            foreach (var data in m_ControllerDatas)
            {
                if (data.Name == dateName)
                    return data;
            }
            return null;
        }
    }
}