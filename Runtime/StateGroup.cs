using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace StateController
{
    [Serializable]
    internal partial class StateLink
    {
        [SerializeField]
        private string m_TargetGroupName = string.Empty;
        [SerializeField]
        private string m_TargetStateName = string.Empty;
        public string TargetGroupName => m_TargetGroupName;
        public string TargetStateName => m_TargetStateName;
    }

    [Serializable]
    internal partial class State
    {
        // 编辑器里这些字段由 StateController 的列表自行手动绘制，隐藏 Odin 自动绘制
        [HideInInspector]
        [SerializeField]
        private string m_Name = string.Empty;
        [HideInInspector]
        [SerializeField]
        private List<StateLink> m_Links = new List<StateLink>();
        // 状态进入时触发的可视化事件（类似 Button 的 onClick），由 StateController 编辑器手动绘制
        // 用 [SerializeReference] 让 m_OnEnteredEvent 可为 null（普通序列化的 UnityEvent 反序列化后必为非空），null 即表示“未添加事件”
        [HideInInspector]
        [SerializeReference]
        private UnityEvent m_OnEnteredEvent;
        public string Name => m_Name;
        public List<StateLink> Links => m_Links;
        public UnityEvent OnEntered => m_OnEnteredEvent;
    }

    [Serializable]
    public sealed partial class StateGroup
    {
        [SerializeField]
        private string m_Name;
        // 每个状态（名字 + 其 link 列表，一对多）
        [SerializeField]
        private List<State> m_States = new List<State>();

        private string m_CurrentStateName;
        private int m_CurrentStateIndex = -1;
        private StateController m_Controller;

        public string Name => m_Name;
        public Action<string> CurrentStateNameChanged;
        public Action<int> CurrentStateIndexChanged;

        public string CurrentStateName
        {
            get => m_CurrentStateName;
            set
            {
                if (m_CurrentStateName == value)
                    return;
                int index = -1;
                for (int i = 0; i < m_States.Count; i++)
                {
                    if (m_States[i].Name == value)
                    {
                        index = i;
                        break;
                    }
                }
                if (index < 0)
                    throw new Exception($"State name '{value}' is not in group '{m_Name}'.");
                SetCurrentStateInternal(value, index);
            }
        }

        public int CurrentStateIndex
        {
            get => m_CurrentStateIndex;
            set
            {
                if (m_CurrentStateIndex == value)
                    return;
                if (value < 0 || value >= m_States.Count)
                    throw new Exception($"State index '{value}' is not in group '{m_Name}'.");
                SetCurrentStateInternal(m_States[value].Name, value);
            }
        }

        private void SetCurrentStateInternal(string name, int index)
        {
            m_CurrentStateName = name;
            m_CurrentStateIndex = index;
            foreach (var binding in m_Controller.Bindings)
            {
                binding.RefreshBinding();
            }
            var currentState = m_States[index];
            foreach (var link in currentState.Links)
            {
                var targetGroup = m_Controller.GetGroup(link.TargetGroupName);
                if (targetGroup != null && !string.IsNullOrEmpty(link.TargetStateName))
                {
                    targetGroup.CurrentStateName = link.TargetStateName;
                }
            }
            if (currentState.OnEntered != null)
            {
                currentState.OnEntered.Invoke();
            }
            CurrentStateNameChanged?.Invoke(m_CurrentStateName);
            CurrentStateIndexChanged?.Invoke(m_CurrentStateIndex);
        }

        internal List<State> States => m_States;

        internal void Initialize(StateController controller)
        {
            m_Controller = controller;
        }
    }
}
