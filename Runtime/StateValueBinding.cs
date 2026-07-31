using System.Collections.Generic;
using UnityEngine;

namespace StateController
{
    public abstract partial class StateValueBinding<T> : StateBinding
    {
        [HideInInspector]
        [SerializeField]
        private string m_GroupName = string.Empty;

        [HideInInspector]
        [SerializeField]
        private List<StateValueMapping<T>> m_ValueMappings = new List<StateValueMapping<T>>();

        private string m_CurrentStateName;
        private StateGroup m_Group;

        internal override void Initialize(StateController controller)
        {
            InitializeTarget();
            m_Group = controller.GetGroup(m_GroupName);
        }

        internal override void RefreshBinding()
        {
            if (m_Group == null || string.IsNullOrEmpty(m_Group.CurrentStateName))
                return;
            if (m_CurrentStateName == m_Group.CurrentStateName)
                return;
            m_CurrentStateName = m_Group.CurrentStateName;
            foreach (var valueMapping in m_ValueMappings)
            {
                if (valueMapping.StateName == m_CurrentStateName)
                {
                    ApplyValue(valueMapping.Value);
                    break;
                }
            }
        }

        protected abstract void InitializeTarget();
        protected abstract void ApplyValue(T value);
    }
}
