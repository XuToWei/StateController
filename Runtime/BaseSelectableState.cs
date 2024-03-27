using System.Collections.Generic;
using UnityEngine;

namespace StateController
{
    public abstract partial class BaseSelectableState<T> : BaseState
    {
        [HideInInspector]
        [SerializeField]
        private string m_DataName = string.Empty;

        [HideInInspector]
        [SerializeField]
        private List<T> m_StateDatas = new List<T>();

        private string m_CurStateName;
        private Dictionary<string, T> m_StateDataDict = new Dictionary<string, T>();
        private StateControllerData m_Data;

        internal override void OnInit(StateController controller)
        {
            OnStateInit();
            m_Data = controller.GetData(m_DataName);
            if (m_Data != null)
            {
                m_StateDataDict.Clear();
                for (int i = 0; i < m_Data.StateNames.Count; i++)
                {
                    m_StateDataDict.Add(m_Data.StateNames[i], m_StateDatas[i]);
                }
            }
        }

        internal override void OnRefresh()
        {
            if (m_Data == null || string.IsNullOrEmpty(m_Data.SelectedName))
                return;
            if (m_CurStateName == m_Data.SelectedName)
                return;
            m_CurStateName = m_Data.SelectedName;
            OnStateChanged(m_StateDataDict[m_Data.SelectedName]);
        }

        protected abstract void OnStateInit();
        protected abstract void OnStateChanged(T stateData);
    }
}