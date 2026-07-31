using System.Collections.Generic;
using UnityEngine;

namespace StateController
{
    public abstract partial class BooleanStateBinding : StateBinding
    {
        [HideInInspector]
        [SerializeField]
        private string m_GroupName1 = string.Empty;

        [HideInInspector]
        [SerializeField]
        private List<StateValueMapping<bool>> m_ValueMappings1 = new List<StateValueMapping<bool>>();

        [HideInInspector]
        [SerializeField]
        private BooleanMode m_BooleanMode;

        [HideInInspector]
        [SerializeField]
        private string m_GroupName2 = string.Empty;

        [HideInInspector]
        [SerializeField]
        private List<StateValueMapping<bool>> m_ValueMappings2 = new List<StateValueMapping<bool>>();

        private string m_CurrentStateName1;
        private string m_CurrentStateName2;
        private StateGroup m_Group1;
        private StateGroup m_Group2;

        internal override void Initialize(StateController controller)
        {
            InitializeTarget();
            m_Group1 = controller.GetGroup(m_GroupName1);
            if (m_BooleanMode != BooleanMode.Single)
            {
                m_Group2 = controller.GetGroup(m_GroupName2);
            }
        }

        internal override void RefreshBinding()
        {
            if (m_BooleanMode == BooleanMode.Single)
            {
                if (m_Group1 == null || string.IsNullOrEmpty(m_Group1.CurrentStateName))
                    return;
                if (m_Group1.CurrentStateName == m_CurrentStateName1)
                    return;
                m_CurrentStateName1 = m_Group1.CurrentStateName;
                foreach (var valueMapping in m_ValueMappings1)
                {
                    if (valueMapping.StateName == m_CurrentStateName1)
                    {
                        ApplyValue(valueMapping.Value);
                        break;
                    }
                }
            }
            else
            {
                if (m_Group1 == null || string.IsNullOrEmpty(m_Group1.CurrentStateName))
                    return;
                if (m_Group2 == null || string.IsNullOrEmpty(m_Group2.CurrentStateName))
                    return;
                if (m_Group1.CurrentStateName == m_CurrentStateName1 && m_Group2.CurrentStateName == m_CurrentStateName2)
                    return;
                m_CurrentStateName1 = m_Group1.CurrentStateName;
                m_CurrentStateName2 = m_Group2.CurrentStateName;
                bool value1 = false;
                foreach (var valueMapping in m_ValueMappings1)
                {
                    if (valueMapping.StateName == m_CurrentStateName1)
                    {
                        value1 = valueMapping.Value;
                        break;
                    }
                }
                bool value2 = false;
                foreach (var valueMapping in m_ValueMappings2)
                {
                    if (valueMapping.StateName == m_CurrentStateName2)
                    {
                        value2 = valueMapping.Value;
                        break;
                    }
                }
                bool result = false;
                switch (m_BooleanMode)
                {
                    case BooleanMode.And:
                        result = value1 && value2;
                        break;
                    case BooleanMode.Or:
                        result = value1 || value2;
                        break;
                }
                ApplyValue(result);
            }
        }

        protected abstract void InitializeTarget();
        protected abstract void ApplyValue(bool value);
    }
}
