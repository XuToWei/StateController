#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;

namespace StateController
{
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
                if (!EditorStateNames.Contains(value))
                    throw new Exception($"State name '{value}' is not in data '{m_Name}'.");
                m_EditorSelectedName = value;
                foreach (var state in m_EditorController.EditorStates)
                {
                    state.EditorOnRefresh();
                }
                EditorApplication.QueuePlayerLoopUpdate();
            }
        }
        
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
    }
}
#endif