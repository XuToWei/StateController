using System.Collections.Generic;
using UnityEngine;

namespace StateController
{
    public abstract partial class BaseBooleanLogicState : BaseState
    {
        [HideInInspector]
        [SerializeField]
        private string m_DataName1 = string.Empty;

        [HideInInspector]
        [SerializeField]
        private List<StateValue<bool>> m_StateValues1 = new List<StateValue<bool>>();

        [HideInInspector]
        [SerializeField]
        private BooleanLogicType m_BooleanLogicType;

        [HideInInspector]
        [SerializeField]
        private string m_DataName2 = string.Empty;

        [HideInInspector]
        [SerializeField]
        private List<StateValue<bool>> m_StateValues2 = new List<StateValue<bool>>();

        private string m_CurStateName1;
        private string m_CurStateName2;
        private StateControllerData m_Data1;
        private StateControllerData m_Data2;

        internal override void OnInit(StateControllerMono controllerMono)
        {
            OnStateInit();
            m_Data1 = controllerMono.GetData(m_DataName1);
            if (m_BooleanLogicType != BooleanLogicType.None)
            {
                m_Data2 = controllerMono.GetData(m_DataName2);
            }
        }

        internal override void OnRefresh()
        {
            if (m_BooleanLogicType == BooleanLogicType.None)
            {
                if (m_Data1 == null || string.IsNullOrEmpty(m_Data1.SelectedName))
                    return;
                if (m_Data1.SelectedName == m_CurStateName1)
                    return;
                m_CurStateName1 = m_Data1.SelectedName;
                foreach (var stateValue in m_StateValues1)
                {
                    if (stateValue.StateName == m_CurStateName1)
                    {
                        OnStateChanged(stateValue.Value);
                        break;
                    }
                }
            }
            else
            {
                if (m_Data1 == null || string.IsNullOrEmpty(m_Data1.SelectedName))
                    return;
                if (m_Data2 == null || string.IsNullOrEmpty(m_Data2.SelectedName))
                    return;
                if (m_Data1.SelectedName == m_CurStateName1 && m_Data2.SelectedName == m_CurStateName2)
                    return;
                m_CurStateName1 = m_Data1.SelectedName;
                m_CurStateName2 = m_Data2.SelectedName;
                bool value1 = false;
                foreach (var stateValue in m_StateValues1)
                {
                    if (stateValue.StateName == m_CurStateName1)
                    {
                        value1 = stateValue.Value;
                        break;
                    }
                }
                bool value2 = false;
                foreach (var stateValue in m_StateValues2)
                {
                    if (stateValue.StateName == m_CurStateName2)
                    {
                        value2 = stateValue.Value;
                        break;
                    }
                }
                bool logicResult = false;
                switch (m_BooleanLogicType)
                {
                    case BooleanLogicType.And:
                        logicResult = value1 && value2;
                        break;
                    case BooleanLogicType.Or:
                        logicResult = value1 || value2;
                        break;
                }
                OnStateChanged(logicResult);
            }
        }

        protected abstract void OnStateInit();
        protected abstract void OnStateChanged(bool logicResult);
    }
}