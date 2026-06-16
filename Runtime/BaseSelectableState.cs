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
        private List<StateValue<T>> m_StateValues = new List<StateValue<T>>();

        private string m_CurStateName;
        private StateControllerData m_Data;

        internal override void OnInit(StateControllerMono controllerMono)
        {
            OnStateInit();
            m_Data = controllerMono.GetData(m_DataName);
        }

        internal override void OnRefresh()
        {
            if (m_Data == null || string.IsNullOrEmpty(m_Data.SelectedName))
                return;
            if (m_CurStateName == m_Data.SelectedName)
                return;
            m_CurStateName = m_Data.SelectedName;
            foreach (var stateValue in m_StateValues)
            {
                if (stateValue.StateName == m_CurStateName)
                {
                    OnStateChanged(stateValue.Value);
                    break;
                }
            }
        }

        protected abstract void OnStateInit();
        protected abstract void OnStateChanged(T stateData);
    }
}
