using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
namespace StateController
{
    public partial class StateController
    {
        private readonly List<BaseState> m_EditorStates = new List<BaseState>();
        private List<BaseState> EditorStates
        {
            get
            {
                m_EditorStates.Clear();
                GetComponentsInChildren<BaseState>(m_EditorStates);
                return m_States;
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
            foreach (var sate in EditorStates)
            {
                sate.EditorOnRefresh();
            }
        }

        internal void EditorOnInit()
        {
            foreach (var data in EditorControllerDatas)
            {
                data.OnInit(this);
            }
            foreach (var state in EditorStates)
            {
                state.OnInit(this);
            }
        }

        internal StateControllerData EditorGetData(string dateName)
        {
            foreach (var data in EditorControllerDatas)
            {
                if (data.Name == dateName)
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
            data.Name = m_EditorNewDataName;
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

        private StateControllerData m_EditorSelectedData => GetData(m_EditorSelectedDataName);

        [BoxGroup("Select Data/State")]
        [LabelText("State Name")]
        [PropertyOrder(21)]
        [ShowInInspector]
        [EnableIf("EditorIsSelectedData")]
        [InfoBox("State name already exists!", 
            InfoMessageType.Warning,
            "EditorShowNewStateNameInfo")]
        private string m_NewStateName;

        [BoxGroup("Select Data/State")]
        [GUIColor(0,1,0)]
        [Button("Add State Name")]
        [PropertyOrder(22)]
        [EnableIf("EditorCheckCanAddStateName")]
        private void AddStateName()
        {
            if(!EditorCheckCanAddStateName())
                return;
            m_EditorSelectedData.StateNames.Add(m_NewStateName);
            m_NewStateName = string.Empty;
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
        private List<string> m_EditorSelectedStateNames
        {
            get
            {
                var data = m_EditorSelectedData;
                if (data == null)
                {
                    return m_EditorEmptyListString;
                }
                return data.StateNames;
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
        private List<BaseState> m_EditorSelectedStates
        {
            get
            {
                var data = m_EditorSelectedData;
                if (data == null)
                {
                    m_EditorListStates.Clear();
                }
                else
                {
                    m_EditorListStates.Clear();
                    foreach (var state in EditorStates)
                    {
                        if (state.Editor_CheckIsConnection(data))
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
            var selectedData = m_EditorSelectedData;
            if (m_EditorRenameDataName == selectedData.Name)
                return;
            foreach (var data in EditorControllerDatas)
            {
                if(data == selectedData)
                    continue;
                if (data.Name == m_EditorRenameDataName)
                {
                    return;
                }
            }
            foreach (var state in EditorStates)
            {
                state.Editor_OnDataRename(m_EditorSelectedDataName, m_EditorRenameDataName);
            }
            selectedData.Name = m_EditorRenameDataName;
            m_EditorSelectedDataName = m_EditorRenameDataName;
        }

        [BoxGroup("Select Data")]
        [GUIColor(1,1,0)]
        [Button("Remove Data")]
        [PropertyOrder(30)]
        [EnableIf("EditorIsSelectedData")]
        private void RemoveSelectedData()
        {
            EditorControllerDatas.Remove(m_EditorSelectedData);
            m_EditorNewDataName = string.Empty;
            EditorRefresh();
        }

        private bool EditorShowNewDataNameInfo()
        {
            if (string.IsNullOrEmpty(m_EditorNewDataName))
                return false;
            foreach (var subStateController in EditorControllerDatas)
            {
                if (subStateController.Name == m_EditorNewDataName)
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
                if (data.Name == m_EditorNewDataName)
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
                if (data.Name == m_EditorSelectedDataName)
                    return true;
            }
            return false;
        }

        private readonly List<string> m_ControllerNames = new List<string>();
        public List<string> EditorGetAllDataNames()
        {
            m_ControllerNames.Clear();
            foreach (var controller in EditorControllerDatas)
            {
                m_ControllerNames.Add(controller.Name);
            }
            m_ControllerNames.Sort();
            return m_ControllerNames;
        }

        private bool EditorShowNewStateNameInfo()
        {
            var data = m_EditorSelectedData;
            if (data == null)
                return false;
            if (string.IsNullOrEmpty(m_NewStateName))
                return false;
            if (data.StateNames.Contains(m_NewStateName))
            {
                return true;
            }
            return false;
        }

        private bool EditorCheckCanAddStateName()
        {
            if (!EditorIsSelectedData())
                return false;
            if(m_EditorSelectedData.StateNames.Contains(m_NewStateName))
                return false;
            return true;
        }

        private bool EditorShowRenameDataNameInfo()
        {
            var selectedData = m_EditorSelectedData;
            if (selectedData == null || selectedData.Name == m_EditorRenameDataName)
                return false;
            if (string.IsNullOrEmpty(m_EditorRenameDataName))
                return false;
            foreach (var data in EditorControllerDatas)
            {
                if(data == selectedData)
                    continue;
                if (data.Name == m_EditorRenameDataName)
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
            var selectedData = m_EditorSelectedData;
            if (selectedData == null || selectedData.Name == m_EditorRenameDataName)
                return false;
            if (string.IsNullOrEmpty(m_EditorRenameDataName))
                return false;
            foreach (var data in EditorControllerDatas)
            {
                if(data == selectedData)
                    continue;
                if (data.Name == m_EditorRenameDataName)
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
            var selectedData = m_EditorSelectedData;
            if (selectedData.SelectedName == selectedData.StateNames[selectionIndex])
            {
                GUI.color = new Color(0,1,0);
            }
            if (GUILayout.Button("Apply", GUILayout.Width(80)))
            {
                EditorRefresh();
                EditorOnInit();
                selectedData.SelectedName = selectedData.StateNames[selectionIndex];
            }
            GUI.color = color;
            if (GUILayout.Button("X", GUILayout.Width(30)))
            {
                foreach (var state in m_States)
                {
                    state.Editor_OnDataRemoveState(m_EditorSelectedDataName, selectionIndex);
                }
                if (selectedData.SelectedName == selectedData.StateNames[selectionIndex])
                {
                    selectedData.Editor_SelectedName = string.Empty;
                }
                selectedData.StateNames.RemoveAt(selectionIndex);
            }
            GUILayout.EndHorizontal();
        }
    }
}
#endif