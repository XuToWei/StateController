using System.Collections.Generic;
using UnityEngine;

namespace StateController
{
    public abstract partial class BooleanStateBinding : StateBinding
    {
        [HideInInspector]
        [SerializeField]
        private string m_PrimaryGroupName = string.Empty;

        [HideInInspector]
        [SerializeField]
        private List<StateValueMapping<bool>> m_PrimaryValueMappings = new List<StateValueMapping<bool>>();

        [HideInInspector]
        [SerializeField]
        private BooleanMode m_BooleanMode;

        [HideInInspector]
        [SerializeField]
        private string m_SecondaryGroupName = string.Empty;

        [HideInInspector]
        [SerializeField]
        private List<StateValueMapping<bool>> m_SecondaryValueMappings = new List<StateValueMapping<bool>>();

        private string m_PrimaryCurrentStateName;
        private string m_SecondaryCurrentStateName;
        private StateGroup m_PrimaryGroup;
        private StateGroup m_SecondaryGroup;

        internal override void Initialize(StateController controller)
        {
            InitializeTarget();
            m_PrimaryGroup = controller.GetGroup(m_PrimaryGroupName);
            if (m_BooleanMode != BooleanMode.Single)
            {
                m_SecondaryGroup = controller.GetGroup(m_SecondaryGroupName);
            }
        }

        internal override void RefreshBinding()
        {
            if (m_BooleanMode == BooleanMode.Single)
            {
                if (m_PrimaryGroup == null || string.IsNullOrEmpty(m_PrimaryGroup.CurrentStateName))
                    return;
                if (m_PrimaryGroup.CurrentStateName == m_PrimaryCurrentStateName)
                    return;
                m_PrimaryCurrentStateName = m_PrimaryGroup.CurrentStateName;
                foreach (var valueMapping in m_PrimaryValueMappings)
                {
                    if (valueMapping.StateName == m_PrimaryCurrentStateName)
                    {
                        ApplyValue(valueMapping.Value);
                        break;
                    }
                }
            }
            else
            {
                if (m_PrimaryGroup == null || string.IsNullOrEmpty(m_PrimaryGroup.CurrentStateName))
                    return;
                if (m_SecondaryGroup == null || string.IsNullOrEmpty(m_SecondaryGroup.CurrentStateName))
                    return;
                if (m_PrimaryGroup.CurrentStateName == m_PrimaryCurrentStateName && m_SecondaryGroup.CurrentStateName == m_SecondaryCurrentStateName)
                    return;
                m_PrimaryCurrentStateName = m_PrimaryGroup.CurrentStateName;
                m_SecondaryCurrentStateName = m_SecondaryGroup.CurrentStateName;
                bool primaryValue = false;
                foreach (var valueMapping in m_PrimaryValueMappings)
                {
                    if (valueMapping.StateName == m_PrimaryCurrentStateName)
                    {
                        primaryValue = valueMapping.Value;
                        break;
                    }
                }
                bool secondaryValue = false;
                foreach (var valueMapping in m_SecondaryValueMappings)
                {
                    if (valueMapping.StateName == m_SecondaryCurrentStateName)
                    {
                        secondaryValue = valueMapping.Value;
                        break;
                    }
                }
                bool result = false;
                switch (m_BooleanMode)
                {
                    case BooleanMode.And:
                        result = primaryValue && secondaryValue;
                        break;
                    case BooleanMode.Or:
                        result = primaryValue || secondaryValue;
                        break;
                }
                ApplyValue(result);
            }
        }

        protected abstract void InitializeTarget();
        protected abstract void ApplyValue(bool value);
    }
}
