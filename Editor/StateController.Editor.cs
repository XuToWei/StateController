#if UNITY_EDITOR
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace StateController
{
    public partial class StateController
    {
        private readonly List<BaseState> m_EditorStates = new List<BaseState>();
        internal List<BaseState> EditorStates
        {
            get
            {
                m_EditorStates.Clear();
                GetComponentsInChildren<BaseState>(true, m_EditorStates);
                return m_EditorStates;
            }
        }

        private List<StateControllerData> EditorControllerDatas => m_ControllerDatas;

        private void OnValidate()
        {
            EditorRefresh();
        }

        private int m_EditorLastRefreshFrame = -1;
        internal void EditorRefresh()
        {
            if (m_EditorLastRefreshFrame == Time.frameCount)
                return;
            m_EditorLastRefreshFrame = Time.frameCount;
            foreach (var data in EditorControllerDatas)
            {
                data.EditorRefresh(this);
            }
            foreach (var sate in EditorStates)
            {
                sate.EditorRefresh();
            }
            foreach (var data in EditorControllerDatas)
            {
                data.EditorRefreshSelectedName();
            }
        }

        internal StateControllerData EditorGetData(string dateName)
        {
            foreach (var data in EditorControllerDatas)
            {
                if (data.EditorName == dateName)
                {
                    return data;
                }
            }
            return null;
        }

        [BoxGroup("Add Data")]
        [LabelText("New Data Name")]
        [PropertyOrder(10)]
        [ShowInInspector]
        [InfoBox("Data name already exists!", 
            InfoMessageType.Warning,
            "EditorShowNewDataNameInfo")]
        private string m_EditorNewDataName;

        [BoxGroup("Add Data")]
        [GUIColor(0, 1, 0)]
        [Button("Add Data")]
        [PropertyOrder(11)]
        [EnableIf("EditorCheckCanAddData")]
        private void EditorAddNewData()
        {
            if (!EditorCheckCanAddData())
                return;
            StateControllerData data = new StateControllerData();
            data.EditorName = m_EditorNewDataName;
            EditorControllerDatas.Add(data);
            m_EditorSelectedDataName = m_EditorNewDataName;
            m_EditorNewDataName = string.Empty;
            EditorRefresh();
        }

        [BoxGroup("Select Data")]
        [LabelText("Data Name")]
        [PropertyOrder(20)]
        [ShowInInspector]
        [ValueDropdown("EditorGetAllDataNames")]
        [SerializeField]
        private string m_EditorSelectedDataName = string.Empty;

        private StateControllerData EditorSelectedData => GetData(m_EditorSelectedDataName);

        [BoxGroup("Select Data/State")]
        [LabelText("State Name")]
        [PropertyOrder(21)]
        [ShowInInspector]
        [EnableIf("EditorIsSelectedData")]
        [InfoBox("State name already exists!", 
            InfoMessageType.Warning,
            "EditorShowNewStateNameInfo")]
        private string m_EditorNewStateName;

        [BoxGroup("Select Data/State")]
        [GUIColor(0,1,0)]
        [Button("Add State Name")]
        [PropertyOrder(22)]
        [EnableIf("EditorCheckCanAddStateName")]
        private void EditorAddStateName()
        {
            if(!EditorCheckCanAddStateName())
                return;
            EditorSelectedData.EditorStateNames.Add(m_EditorNewStateName);
            m_EditorNewStateName = string.Empty;
            EditorRefresh();
        }

        private readonly List<string> m_EditorEmptyListString = new List<string>();
        [BoxGroup("Select Data/State")]
        [LabelText("State Names")]
        [PropertyOrder(23)]
        [ShowInInspector]
        [ReadOnly]
        [EnableIf("EditorIsSelectedData")]
        [ListDrawerSettings(DefaultExpandedState = true,
            OnBeginListElementGUI = "EditorOnStateNameBeginGUI",
            OnEndListElementGUI = "EditorOnStateNameEndGUI")]
        private List<string> EditorSelectedStateNames
        {
            get
            {
                var data = EditorSelectedData;
                if (data == null)
                {
                    return m_EditorEmptyListString;
                }
                return data.EditorStateNames;
            }
        }
        
        private readonly List<BaseState> m_EditorListStates = new List<BaseState>();
        [BoxGroup("Select Data/State")]
        [LabelText("State Children")]
        [PropertyOrder(24)]
        [ShowInInspector]
        [ReadOnly]
        [ListDrawerSettings(DefaultExpandedState = true)]
        [EnableIf("EditorIsSelectedData")]
        private List<BaseState> EditorSelectedStates
        {
            get
            {
                var data = EditorSelectedData;
                if (data == null)
                {
                    m_EditorListStates.Clear();
                }
                else
                {
                    m_EditorListStates.Clear();
                    foreach (var state in EditorStates)
                    {
                        if (state.EditorCheckIsConnection(data))
                        {
                            m_EditorListStates.Add(state);
                        }
                    }
                }
                return m_EditorListStates;
            }
        }

        [BoxGroup("Select Data/Rename Data")]
        [LabelText("Data Name")]
        [PropertyOrder(25)]
        [ShowInInspector]
        [EnableIf("EditorIsSelectedData")]
        [InfoBox("Data name already exists!", 
            InfoMessageType.Warning,
            "EditorShowRenameDataNameInfo")]
        private string m_EditorRenameDataName;

        [BoxGroup("Select Data/Rename Data")]
        [GUIColor(0,1,0)]
        [Button("Rename Data")]
        [PropertyOrder(26)]
        [EnableIf("EditorCheckCanRenameData")]
        private void EditorRenameSelectedDataName()
        {
            if (string.IsNullOrEmpty(m_EditorRenameDataName))
                return;
            var selectedData = EditorSelectedData;
            if (m_EditorRenameDataName == selectedData.EditorName)
                return;
            foreach (var data in EditorControllerDatas)
            {
                if(data == selectedData)
                    continue;
                if (data.EditorName == m_EditorRenameDataName)
                {
                    return;
                }
            }
            foreach (var state in EditorStates)
            {
                state.EditorOnDataRename(m_EditorSelectedDataName, m_EditorRenameDataName);
            }
            selectedData.EditorName = m_EditorRenameDataName;
            m_EditorSelectedDataName = m_EditorRenameDataName;
        }

        [BoxGroup("Select Data")]
        [GUIColor(1,1,0)]
        [Button("Remove Data")]
        [PropertyOrder(30)]
        [EnableIf("EditorIsSelectedData")]
        private void EditorRemoveSelectedData()
        {
            EditorControllerDatas.Remove(EditorSelectedData);
            m_EditorNewDataName = string.Empty;
            EditorRefresh();
        }

        private bool EditorShowNewDataNameInfo()
        {
            if (string.IsNullOrEmpty(m_EditorNewDataName))
                return false;
            foreach (var subStateController in EditorControllerDatas)
            {
                if (subStateController.EditorName == m_EditorNewDataName)
                {
                    return true;
                }
            }
            return false;
        }

        private bool EditorCheckCanAddData()
        {
            if (string.IsNullOrEmpty(m_EditorNewDataName))
                return false;
            foreach (var data in EditorControllerDatas)
            {
                if (data.EditorName == m_EditorNewDataName)
                    return false;
            }
            return true;
        }

        private bool EditorIsSelectedData()
        {
            if (string.IsNullOrEmpty(m_EditorSelectedDataName))
                return false;
            foreach (var data in EditorControllerDatas)
            {
                if (data.EditorName == m_EditorSelectedDataName)
                    return true;
            }
            return false;
        }

        private readonly List<string> m_EditorControllerNames = new List<string>();
        public List<string> EditorGetAllDataNames()
        {
            m_EditorControllerNames.Clear();
            foreach (var controller in EditorControllerDatas)
            {
                m_EditorControllerNames.Add(controller.EditorName);
            }
            m_EditorControllerNames.Sort();
            return m_EditorControllerNames;
        }

        private bool EditorShowNewStateNameInfo()
        {
            var data = EditorSelectedData;
            if (data == null)
                return false;
            if (string.IsNullOrEmpty(m_EditorNewStateName))
                return false;
            if (data.EditorStateNames.Contains(m_EditorNewStateName))
            {
                return true;
            }
            return false;
        }

        private bool EditorCheckCanAddStateName()
        {
            if (!EditorIsSelectedData())
                return false;
            if(EditorSelectedData.EditorStateNames.Contains(m_EditorNewStateName))
                return false;
            return true;
        }

        private bool EditorShowRenameDataNameInfo()
        {
            var selectedData = EditorSelectedData;
            if (selectedData == null || selectedData.EditorName == m_EditorRenameDataName)
                return false;
            if (string.IsNullOrEmpty(m_EditorRenameDataName))
                return false;
            foreach (var data in EditorControllerDatas)
            {
                if(data == selectedData)
                    continue;
                if (data.EditorName == m_EditorRenameDataName)
                {
                    return true;
                }
            }
            return false;
        }

        private bool EditorCheckCanRenameData()
        {
            if (!EditorIsSelectedData())
                return false;
            var selectedData = EditorSelectedData;
            if (selectedData == null || selectedData.EditorName == m_EditorRenameDataName)
                return false;
            if (string.IsNullOrEmpty(m_EditorRenameDataName))
                return false;
            foreach (var data in EditorControllerDatas)
            {
                if(data == selectedData)
                    continue;
                if (data.EditorName == m_EditorRenameDataName)
                {
                    return false;
                }
            }
            return true;
        }

        private void EditorOnStateNameBeginGUI(int selectionIndex)
        {
            GUILayout.BeginHorizontal();
        }

        private void EditorOnStateNameEndGUI(int selectionIndex)
        {
            GUI.enabled = true;
            var color = GUI.color;
            var selectedData = EditorSelectedData;
            if (selectedData.EditorSelectedName == selectedData.EditorStateNames[selectionIndex])
            {
                GUI.color = new Color(0,1,0);
            }
            if (GUILayout.Button("Apply", GUILayout.Width(80)))
            {
                EditorRefresh();
                selectedData.EditorSelectedName = selectedData.EditorStateNames[selectionIndex];
            }
            GUI.color = color;
            if (GUILayout.Button("X", GUILayout.Width(30)))
            {
                foreach (var state in EditorStates)
                {
                    state.EditorOnDataRemoveState(m_EditorSelectedDataName, selectionIndex);
                }
                if (selectedData.EditorSelectedName == selectedData.EditorStateNames[selectionIndex])
                {
                    selectedData.EditorSelectedName = string.Empty;
                }
                selectedData.EditorStateNames.RemoveAt(selectionIndex);
            }
            GUILayout.EndHorizontal();
        }
    }
}
#endif