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
        private string m_EditorSelectedName;
        private StateController m_EditorController;
        internal List<string> EditorStateNames => m_StateNames;

        internal string EditorSelectedName
        {
            get => m_EditorSelectedName;
            set
            {
                int index = m_StateNames.IndexOf(value);
                if (index < 0)
                    throw new Exception($"State name '{value}' is not in data '{m_Name}'.");
                m_EditorSelectedName = value;
                foreach (var state in m_EditorController.EditorStates)
                {
                    state.EditorOnRefresh();
                }
                var linkData = EditorLinkDatas[index];
                var data = m_EditorController.GetData(linkData.EditorTargetDataName);
                if (data != null && !string.IsNullOrEmpty(linkData.EditorTargetSelectedName))
                {
                    data.EditorSelectedName = linkData.EditorTargetSelectedName;
                }
                EditorApplication.QueuePlayerLoopUpdate();
            }
        }
        
        internal List<LinkData> EditorLinkDatas => m_LinkDatas;
        
        internal int EditorSelectedIndex => EditorStateNames.IndexOf(EditorSelectedName);

        internal string EditorName
        {
            get => m_Name;
            set => m_Name = value;
        }

        internal void EditorRefresh(StateController controller)
        {
            m_EditorController = controller;
        }

        internal void EditorRefreshSelectedName()
        {
            foreach (var state in m_EditorController.EditorStates)
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
    }
}
#endif