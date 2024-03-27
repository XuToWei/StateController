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
        private List<bool> m_StateDatas1 = new List<bool>();

        [HideInInspector]
        [SerializeField]
        private BooleanLogicType m_BooleanLogicType;

        [HideInInspector]
        [SerializeField]
        private string m_DataName2 = string.Empty;

        [HideInInspector]
        [SerializeField]
        private List<bool> m_StateDatas2 = new List<bool>();

        private string m_CurStateName1;
        private string m_CurStateName2;
        private readonly Dictionary<string, bool> m_StateDataDict1 = new Dictionary<string, bool>();
        private readonly Dictionary<string, bool> m_StateDataDict2 = new Dictionary<string, bool>();
        private StateControllerData m_Data1;
        private StateControllerData m_Data2;

        internal override void OnInit(StateController controller)
        {
            OnStateInit();
            m_Data1 = controller.GetData(m_DataName1);
            if (m_Data1 != null)
            {
                m_StateDataDict1.Clear();
                for (int i = 0; i < m_Data1.StateNames.Count; i++)
                {
                    m_StateDataDict1.Add(m_Data1.StateNames[i], m_StateDatas1[i]);
                }
            }
            if (m_BooleanLogicType != BooleanLogicType.None)
            {
                m_Data2 = controller.GetData(m_DataName2);
                if (m_Data2 != null)
                {
                    m_StateDataDict2.Clear();
                    for (int i = 0; i < m_Data2.StateNames.Count; i++)
                    {
                        m_StateDataDict2.Add(m_Data2.StateNames[i], m_StateDatas2[i]);
                    }
                }
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
                OnStateChanged(m_StateDataDict1[m_CurStateName1]);
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
                bool logicResult = false;
                switch (m_BooleanLogicType)
                {
                    case BooleanLogicType.And:
                        logicResult = m_StateDataDict1[m_CurStateName1] && m_StateDataDict2[m_CurStateName2];
                        break;
                    case BooleanLogicType.Or:
                        logicResult = m_StateDataDict1[m_CurStateName1] || m_StateDataDict2[m_CurStateName2];
                        break;
                }
                OnStateChanged(logicResult);
            }
        }

        protected abstract void OnStateInit();
        protected abstract void OnStateChanged(bool logicResult);
    }
}