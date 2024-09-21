#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;

namespace StateController
{
    internal partial class LinkData
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

    public partial class StateControllerData
    {
        private StateControllerMono m_EditorControllerMono;
        internal List<string> EditorStateNames => m_StateNames;

        internal string EditorSelectedName
        {
            get => m_SelectedName;
            set
            {
                int index = m_StateNames.IndexOf(value);
                if (index < 0)
                    throw new Exception($"State name '{value}' is not in data '{m_Name}'.");
                m_SelectedName = value;
                m_SelectedIndex = index;
                foreach (var state in m_EditorControllerMono.EditorStates)
                {
                    state.EditorOnRefresh();
                }
                var linkData = EditorLinkDatas[index];
                var data = m_EditorControllerMono.GetData(linkData.EditorTargetDataName);
                if (data != null && !string.IsNullOrEmpty(linkData.EditorTargetSelectedName))
                {
                    data.EditorSelectedName = linkData.EditorTargetSelectedName;
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

        internal List<LinkData> EditorLinkDatas => m_LinkDatas;
        
        internal int EditorSelectedIndex => EditorStateNames.IndexOf(EditorSelectedName);

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
            foreach (var linkData in EditorLinkDatas)
            {
                if (linkData.EditorTargetDataName == oldDataName)
                {
                    linkData.EditorTargetDataName = newDataName;
                }
            }
        }

        internal void EditorOnStateRename(string dataName, string oldStateName, string newStateName)
        {
            foreach (var linkData in EditorLinkDatas)
            {
                if (linkData.EditorTargetDataName == dataName && linkData.EditorTargetSelectedName == oldStateName)
                {
                    linkData.EditorTargetSelectedName = newStateName;
                }
            }
        }
    }
}
#endif