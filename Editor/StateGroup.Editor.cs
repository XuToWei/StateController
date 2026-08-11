#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.Events;

namespace StateController
{
    internal partial class StateLink
    {
        public string EditorTargetGroupName
        {
            set => m_TargetGroupName = value;
            get => m_TargetGroupName;
        }

        public string EditorTargetStateName
        {
            set => m_TargetStateName = value;
            get => m_TargetStateName;
        }
    }

    internal partial class State
    {
        public string EditorName
        {
            get => m_Name;
            set => m_Name = value;
        }
        public List<StateLink> EditorLinks => m_Links;
        // 供 SerializedProperty 按字段名导航，避免硬编码字符串路径（重命名时编译期可捕获）
        public const string EditorOnEnteredEventFieldName = nameof(m_OnEnteredEvent);
        // null 表示未添加事件
        public UnityEvent EditorOnEnteredEvent
        {
            get => m_OnEnteredEvent;
            set => m_OnEnteredEvent = value;
        }

        // 改名编辑态（仅编辑器内 UI 用，不序列化）
        private bool m_EditorRenaming;
        private string m_EditorRenamingText;
        public bool EditorIsRenaming
        {
            get => m_EditorRenaming;
            set => m_EditorRenaming = value;
        }
        public string EditorRenamingText
        {
            get => m_EditorRenamingText;
            set => m_EditorRenamingText = value;
        }
    }

    public partial class StateGroup
    {
        private StateController m_EditorController;

        internal List<State> EditorStates => m_States;
        // 供 SerializedProperty 按字段名导航，避免硬编码字符串路径
        internal const string EditorStatesFieldName = nameof(m_States);

        internal string EditorCurrentStateName
        {
            get => m_CurrentStateName;
            set
            {
                int index = -1;
                for (int i = 0; i < m_States.Count; i++)
                {
                    if (m_States[i].EditorName == value)
                    {
                        index = i;
                        break;
                    }
                }
                if (index < 0)
                    throw new Exception($"State name '{value}' is not in group '{m_Name}'.");
                m_CurrentStateName = value;
                m_CurrentStateIndex = index;
                foreach (var binding in m_EditorController.EditorBindings)
                {
                    binding.EditorApplyCurrentState();
                }
                foreach (var link in m_States[index].EditorLinks)
                {
                    var group = m_EditorController.GetGroup(link.EditorTargetGroupName);
                    if (group != null && !string.IsNullOrEmpty(link.EditorTargetStateName))
                    {
                        group.EditorCurrentStateName = link.EditorTargetStateName;
                    }
                }
                CurrentStateNameChanged?.Invoke(m_CurrentStateName);
                CurrentStateIndexChanged?.Invoke(m_CurrentStateIndex);
                EditorApplication.QueuePlayerLoopUpdate();
            }
        }

        internal void EditorClearCurrentState()
        {
            m_CurrentStateName = string.Empty;
            m_CurrentStateIndex = -1;
        }

        internal string EditorName
        {
            get => m_Name;
            set => m_Name = value;
        }

        internal void EditorRefresh(StateController controller)
        {
            m_EditorController = controller;
        }

        internal void EditorApplyCurrentState()
        {
            foreach (var binding in m_EditorController.EditorBindings)
            {
                binding.EditorApplyCurrentState();
            }
            EditorApplication.QueuePlayerLoopUpdate();
        }

        internal void EditorOnGroupRename(string oldGroupName, string newGroupName)
        {
            foreach (var state in m_States)
            {
                foreach (var link in state.EditorLinks)
                {
                    if (link.EditorTargetGroupName == oldGroupName)
                    {
                        link.EditorTargetGroupName = newGroupName;
                    }
                }
            }
        }

        internal void EditorOnStateRename(string groupName, string oldStateName, string newStateName)
        {
            foreach (var state in m_States)
            {
                foreach (var link in state.EditorLinks)
                {
                    if (link.EditorTargetGroupName == groupName && link.EditorTargetStateName == oldStateName)
                    {
                        link.EditorTargetStateName = newStateName;
                    }
                }
            }
        }
    }
}
#endif
