using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace StateController
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public sealed class StateController : MonoBehaviour
    {
        [HideInInspector]
        [SerializeField]
        private List<StateControllerData> m_ControllerDatas = new List<StateControllerData>();

        private readonly List<BaseState> m_States = new List<BaseState>();
        public List<BaseState> States => m_States;
        private readonly Dictionary<string, StateControllerData> m_ControllerDataDict = new Dictionary<string, StateControllerData>();

        private void Awake()
        {
#if UNITY_EDITOR
            if (GetComponentsInChildren<StateController>(true).Length > 1)
            {
                throw new Exception("A StateController can't contains an other StateController.");
            }
            if (GetComponentsInParent<StateController>(true).Length > 1)
            {
                throw new Exception("A StateController can't be under an other StateController.");
            }
#endif
            GetComponentsInChildren<BaseState>(true, m_States);
            foreach (var data in m_ControllerDatas)
            {
                m_ControllerDataDict.Add(data.Name, data);
                data.OnInit(this);
            }
            foreach (var state in m_States)
            {
                state.OnInit(this);
            }
        }

        public void SelectedName(string dateName, string stateName)
        {
            m_ControllerDataDict[dateName].SelectedName = stateName;
        }

        public string GetSelectedName(string dateName)
        {
            return m_ControllerDataDict[dateName].SelectedName;
        }
        
        public StateControllerData GetData(string dateName)
        {
            return m_ControllerDataDict[dateName];
        }

#if UNITY_EDITOR
        internal List<BaseState> Editor_States
        {
            get
            {
                m_States.Clear();
                GetComponentsInChildren<BaseState>(true, m_States);
                return m_States;
            }
        }

        private void Editor_Refresh()
        {
            foreach (var sate in Editor_States)
            {
                sate.Editor_OnRefresh();
            }
        }

        internal StateControllerData Editor_GetData(string dateName)
        {
            foreach (var data in m_ControllerDatas)
            {
                if (data.Name == dateName)
                {
                    return data;
                }
            }
            return null;
        }
        
        
        [PropertyOrder(1)]
        [NonSerialized]
        [NonReorderable]
        [ShowInInspector]
        private string m_NewDataName2;

        [BoxGroup("Add Data")]
        [LabelText("New Data Name")]
        [PropertyOrder(10)]
        [ShowInInspector]
        [InfoBox("Data name already exists!", 
            InfoMessageType.Warning,
            "ShowNewDataNameInfo")]
        private string m_NewDataName;

        [BoxGroup("Add Data")]
        [GUIColor(0, 1, 0)]
        [Button("Add Data")]
        [PropertyOrder(11)]
        [EnableIf("CheckCanAddData")]
        private void AddNewData()
        {
            if (!CheckCanAddData())
                return;
            StateControllerData data = new StateControllerData();
            data.Name = m_NewDataName;
            m_ControllerDatas.Add(data);
            m_SelectedDataName = m_NewDataName;
            OnSelectedData();
            m_NewDataName = string.Empty;
            data.OnInit(this);
        }

        [BoxGroup("Select Data")]
        [LabelText("Data Name")]
        [PropertyOrder(20)]
        [ShowInInspector]
        [ValueDropdown("GetAllDataNames")]
        [OnValueChanged("OnSelectedData")]
        private string m_SelectedDataName = string.Empty;
        private StateControllerData m_SelectedData;

        [BoxGroup("Select Data/State")]
        [LabelText("State Name")]
        [PropertyOrder(21)]
        [ShowInInspector]
        [EnableIf("IsSelectedData")]
        [InfoBox("State name already exists!", 
            InfoMessageType.Warning,
            "ShowNewStateNameInfo")]
        private string m_NewStateName;

        [BoxGroup("Select Data/State")]
        [GUIColor(0,1,0)]
        [Button("Add State Name")]
        [PropertyOrder(22)]
        [EnableIf("IsSelectedData")]
        private void AddStateName()
        {
            if (string.IsNullOrEmpty(m_NewStateName))
                return;
            if(m_SelectedData.Editor_StateNames.Contains(m_NewStateName))
                return;
            m_SelectedData.Editor_StateNames.Add(m_NewStateName);
            m_NewStateName = string.Empty;
            Editor_Refresh();
        }

        private readonly List<string> m_EmptyListString = new List<string>();
        [BoxGroup("Select Data/State")]
        [LabelText("State Names")]
        [PropertyOrder(23)]
        [ShowInInspector]
        [ReadOnly]
        [EnableIf("IsSelectedData")]
        [ListDrawerSettings(DefaultExpandedState = true,
            OnBeginListElementGUI = "OnStateNameBeginGUI",
            OnEndListElementGUI = "OnStateNameEndGUI")]
        private List<string> m_SelectedStateNames
        {
            get
            {
                if (m_SelectedData == null)
                {
                    return m_EmptyListString;
                }
                return m_SelectedData.Editor_StateNames;
            }
        }
        
        private readonly List<BaseState> m_ListState = new List<BaseState>();
        [BoxGroup("Select Data/State")]
        [LabelText("State Children")]
        [PropertyOrder(24)]
        [ShowInInspector]
        [ReadOnly]
        [ListDrawerSettings(DefaultExpandedState = true)]
        [EnableIf("IsSelectedData")]
        private List<BaseState> m_SelectedStates
        {
            get
            {
                if (m_SelectedData == null)
                {
                    m_ListState.Clear();
                }
                else
                {
                    m_ListState.Clear();
                    foreach (var state in Editor_States)
                    {
                        if (state.Editor_CheckIsConnection(m_SelectedData))
                        {
                            m_ListState.Add(state);
                        }
                    }
                }
                return m_ListState;
            }
        }

        [BoxGroup("Select Data/Rename Data")]
        [LabelText("Data Name")]
        [PropertyOrder(25)]
        [ShowInInspector]
        [EnableIf("IsSelectedData")]
        [InfoBox("Data name already exists!", 
            InfoMessageType.Warning,
            "ShowRenameDataNameInfo")]
        private string m_RenameDataName;

        [BoxGroup("Select Data/Rename Data")]
        [GUIColor(0,1,0)]
        [Button("Rename Data")]
        [PropertyOrder(26)]
        [EnableIf("CheckCanRenameData")]
        private void RenameSelectedDataName()
        {
            if (string.IsNullOrEmpty(m_RenameDataName))
                return;
            if (m_RenameDataName == m_SelectedData.Name)
                return;
            foreach (var data in m_ControllerDatas)
            {
                if(data == m_SelectedData)
                    continue;
                if (data.Name == m_RenameDataName)
                {
                    return;
                }
            }
            foreach (var state in Editor_States)
            {
                state.Editor_OnDataRename(m_SelectedDataName, m_RenameDataName);
            }
            m_SelectedData.Name = m_RenameDataName;
            m_SelectedDataName = m_RenameDataName;
        }

        [BoxGroup("Select Data")]
        [GUIColor(1,1,0)]
        [Button("Remove Data")]
        [PropertyOrder(30)]
        [EnableIf("IsSelectedData")]
        private void RemoveSelectedData()
        {
            m_ControllerDatas.Remove(m_SelectedData);
            m_SelectedData = null;
            m_NewDataName = string.Empty;
            m_RenameDataName = string.Empty;
            Editor_Refresh();
        }

        private bool ShowNewDataNameInfo()
        {
            if (string.IsNullOrEmpty(m_NewDataName))
                return false;
            foreach (var subStateController in m_ControllerDatas)
            {
                if (subStateController.Name == m_NewDataName)
                {
                    return true;
                }
            }
            return false;
        }

        private bool CheckCanAddData()
        {
            if (string.IsNullOrEmpty(m_NewDataName))
                return false;
            foreach (var data in m_ControllerDatas)
            {
                if (data.Name == m_NewDataName)
                    return false;
            }
            return true;
        }

        private bool IsSelectedData()
        {
            if (string.IsNullOrEmpty(m_SelectedDataName))
                return false;
            foreach (var data in m_ControllerDatas)
            {
                if (data.Name == m_SelectedDataName)
                    return true;
            }
            return false;
        }

        private readonly List<string> m_ControllerNames = new List<string>();
        public List<string> GetAllDataNames()
        {
            m_ControllerNames.Clear();
            foreach (var controller in m_ControllerDatas)
            {
                m_ControllerNames.Add(controller.Name);
            }
            m_ControllerNames.Sort();
            return m_ControllerNames;
        }

        private void OnSelectedData()
        {
            m_SelectedData = null;
            m_RenameDataName = string.Empty;
            foreach (var data in m_ControllerDatas)
            {
                if (data.Name == m_SelectedDataName)
                {
                    m_SelectedData = data;
                    m_RenameDataName = m_SelectedDataName;
                    break;
                }
            }
        }

        private bool ShowNewStateNameInfo()
        {
            if (m_SelectedData == null)
                return false;
            if (string.IsNullOrEmpty(m_NewStateName))
                return false;
            if (m_SelectedData.Editor_StateNames.Contains(m_NewStateName))
            {
                return true;
            }
            return false;
        }

        private bool ShowRenameDataNameInfo()
        {
            if (m_SelectedData == null || m_SelectedData.Name == m_RenameDataName)
                return false;
            if (string.IsNullOrEmpty(m_RenameDataName))
                return false;
            foreach (var data in m_ControllerDatas)
            {
                if(data == m_SelectedData)
                    continue;
                if (data.Name == m_RenameDataName)
                {
                    return true;
                }
            }
            return false;
        }

        private bool CheckCanRenameData()
        {
            if (!IsSelectedData())
                return false;
            if (m_SelectedData == null || m_SelectedData.Name == m_RenameDataName)
                return false;
            if (string.IsNullOrEmpty(m_RenameDataName))
                return false;
            foreach (var data in m_ControllerDatas)
            {
                if(data == m_SelectedData)
                    continue;
                if (data.Name == m_RenameDataName)
                {
                    return false;
                }
            }
            return true;
        }

        private void OnStateNameBeginGUI(int selectionIndex)
        {
            GUILayout.BeginHorizontal();
        }

        private void OnStateNameEndGUI(int selectionIndex)
        {
            GUI.enabled = m_SelectedData.SelectedName != m_SelectedData.Editor_StateNames[selectionIndex];
            if (GUILayout.Button("Apply"))
            {
                m_SelectedData.SelectedName = m_SelectedData.Editor_StateNames[selectionIndex];
            }
            GUI.enabled = true;
            if (GUILayout.Button("X"))
            {
                foreach (var state in Editor_States)
                {
                    state.Editor_OnDataRemoveState(m_SelectedData.Name, selectionIndex);
                }
                m_SelectedData.Editor_StateNames.RemoveAt(selectionIndex);
            }
            GUILayout.EndHorizontal();
        }
#endif
    }
}