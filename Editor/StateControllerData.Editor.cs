#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.Events;

namespace StateController
{
    internal partial class StateControllerStateLink
    {
        public string EditorTargetDataName
        {
            set => m_TargetDataName = value;
            get => m_TargetDataName;
        }

        public string EditorTargetSelectedName
        {
            set => m_TargetSelectedName = value;
            get => m_TargetSelectedName;
        }
    }

    internal partial class StateControllerState
    {
        public string EditorName
        {
            get => m_Name;
            set => m_Name = value;
        }
        public List<StateControllerStateLink> EditorLinks => m_Links;
        // 供 SerializedProperty 按字段名导航，避免硬编码字符串路径（重命名时编译期可捕获）
        public const string EditorOnSelectedEventFieldName = nameof(m_OnSelectedEvent);
        // null 表示未添加事件
        public UnityEvent EditorOnSelectedEvent
        {
            get => m_OnSelectedEvent;
            set => m_OnSelectedEvent = value;
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

    public partial class StateControllerData
    {
        private StateControllerMono m_EditorControllerMono;

        internal List<StateControllerState> EditorStates => m_States;
        // 供 SerializedProperty 按字段名导航，避免硬编码字符串路径
        internal const string EditorStatesFieldName = nameof(m_States);

        internal string EditorSelectedName
        {
            get => m_SelectedName;
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
                    throw new Exception($"State name '{value}' is not in data '{m_Name}'.");
                m_SelectedName = value;
                m_SelectedIndex = index;
                foreach (var state in m_EditorControllerMono.EditorStates)
                {
                    state.EditorOnRefresh();
                }
                foreach (var link in m_States[index].EditorLinks)
                {
                    var data = m_EditorControllerMono.GetData(link.EditorTargetDataName);
                    if (data != null && !string.IsNullOrEmpty(link.EditorTargetSelectedName))
                    {
                        data.EditorSelectedName = link.EditorTargetSelectedName;
                    }
                }
                OnSelectedNameChanged?.Invoke(m_SelectedName);
                OnSelectedIndexChanged?.Invoke(m_SelectedIndex);
                EditorApplication.QueuePlayerLoopUpdate();
            }
        }

        internal void EditorClearSelectedName()
        {
            m_SelectedName = string.Empty;
            m_SelectedIndex = -1;
        }

        internal string EditorName
        {
            get => m_Name;
            set => m_Name = value;
        }

        internal void EditorRefresh(StateControllerMono controllerMono)
        {
            m_EditorControllerMono = controllerMono;
        }

        internal void EditorRefreshSelectedName()
        {
            foreach (var state in m_EditorControllerMono.EditorStates)
            {
                state.EditorOnRefresh();
            }
            EditorApplication.QueuePlayerLoopUpdate();
        }

        internal void EditorOnDataRename(string oldDataName, string newDataName)
        {
            foreach (var state in m_States)
            {
                foreach (var link in state.EditorLinks)
                {
                    if (link.EditorTargetDataName == oldDataName)
                    {
                        link.EditorTargetDataName = newDataName;
                    }
                }
            }
        }

        internal void EditorOnStateRename(string dataName, string oldStateName, string newStateName)
        {
            foreach (var state in m_States)
            {
                foreach (var link in state.EditorLinks)
                {
                    if (link.EditorTargetDataName == dataName && link.EditorTargetSelectedName == oldStateName)
                    {
                        link.EditorTargetSelectedName = newStateName;
                    }
                }
            }
        }
    }
}
#endif
