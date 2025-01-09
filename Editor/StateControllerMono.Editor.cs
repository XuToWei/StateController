#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace StateController
{
    public partial class StateControllerMono
    {
        private const int ConstHorizontalButtonWidth = 110;
        
        private static TextEditor s_TextEditor;
        private static TextEditor TextEditor
        {
            get
            {
                if (s_TextEditor == null)
                {
                    s_TextEditor = new TextEditor();
                }
                return s_TextEditor;
            }
        }

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

        public List<StateControllerData> EditorControllerDatas => m_ControllerDatas;

        private void OnValidate()
        {
            EditorRefresh();
        }

        private int m_EditorLastRefreshFrame = -1;
        internal void EditorRefresh()
        {
            if (m_EditorLastRefreshFrame == Time.frameCount)
                return;
            if (EditorSelectedData == null)
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
        [HorizontalGroup("Add Data/")]
        [LabelText("New Data Name")]
        [PropertyOrder(10)]
        [ShowInInspector]
        [InfoBox("Data name already exists!", 
            InfoMessageType.Warning,
            "EditorShowNewDataNameInfo")]
        private string m_EditorNewDataName;

        [BoxGroup("Add Data")]
        [HorizontalGroup("Add Data/", Width = ConstHorizontalButtonWidth)]
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
        [HorizontalGroup("Select Data/")]
        [LabelText("Data Name")]
        [PropertyOrder(20)]
        [ShowInInspector]
        [ValueDropdown("EditorGetAllDataNames")]
        [OnValueChanged("EditorRefreshSelectedStatesRenameDatas", InvokeOnInitialize = true)]
        [SerializeField]
        private string m_EditorSelectedDataName = string.Empty;

        private StateControllerData EditorSelectedData => GetData(m_EditorSelectedDataName);

        [BoxGroup("Select Data")]
        [HorizontalGroup("Select Data/")]
        [LabelText("Rename")]
        [PropertyOrder(21)]
        [ShowInInspector]
        [EnableIf("EditorIsSelectedData")]
        [InfoBox("Data name already exists!", 
            InfoMessageType.Warning,
            "EditorShowRenameDataNameInfo")]
        private string m_EditorRenameDataName;

        [BoxGroup("Select Data")]
        [HorizontalGroup("Select Data/", Width = ConstHorizontalButtonWidth * 0.5f + 5)]
        [GUIColor(0,1,0)]
        [Button("Rename")]
        [PropertyOrder(22)]
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
            foreach (var data in EditorControllerDatas)
            {
                if(data == selectedData)
                    continue;
                data.EditorOnDataRename(m_EditorSelectedDataName, m_EditorRenameDataName);
            }
            foreach (var state in EditorStates)
            {
                state.EditorOnDataRename(m_EditorSelectedDataName, m_EditorRenameDataName);
            }
            selectedData.EditorName = m_EditorRenameDataName;
            m_EditorSelectedDataName = m_EditorRenameDataName;
        }

        [BoxGroup("Select Data")]
        [HorizontalGroup("Select Data/", Width = ConstHorizontalButtonWidth * 0.5f - 15)]
        [GUIColor(0,1,0)]
        [Button("Copy")]
        [PropertyOrder(23)]
        [EnableIf("EditorIsSelectedData")]
        private void EditorCopySelectedData()
        {
            TextEditor.text = JsonUtility.ToJson(EditorSelectedData);
        }

        [BoxGroup("Select Data")]
        [HorizontalGroup("Select Data/", Width = ConstHorizontalButtonWidth * 0.5f - 10)]
        [GUIColor(1,1,0)]
        [Button("Paste")]
        [PropertyOrder(24)]
        [EnableIf("EditorCanPasteSelectedData")]
        private void EditorPasteSelectedData()
        {
            var selectedData = EditorSelectedData;
            bool hasConnection = false;
            foreach (var state in EditorStates)
            {
                if (state.EditorCheckIsConnection(selectedData))
                {
                    hasConnection = true;
                    break;
                }
            }
            if (hasConnection)
            {
                EditorUtility.DisplayDialog("Paste Error", $"SelectedData({selectedData.EditorName}) has connection by state children, can't paste!", "ok");
                return;
            }
            StateControllerData data = JsonUtility.FromJson<StateControllerData>(TextEditor.text);
            selectedData.EditorStateNames.Clear();
            selectedData.EditorLinkDatas.Clear();
            m_EditorSelectedStatesRenameButtons.Clear();
            m_EditorSelectedStatesRenameNames.Clear();
            m_EditorNewStateName = string.Empty;
            foreach (string stateName in data.EditorStateNames)
            {
                selectedData.EditorStateNames.Add(stateName);
                selectedData.EditorLinkDatas.Add(new LinkData());
                m_EditorSelectedStatesRenameButtons.Add(false);
                m_EditorSelectedStatesRenameNames.Add(stateName);
            }
        }

        [BoxGroup("Select Data")]
        [HorizontalGroup("Select Data/", Width = 20)]
        [GUIColor(1,0,0)]
        [Button("X")]
        [PropertyOrder(25)]
        [EnableIf("EditorIsSelectedData")]
        private void EditorRemoveSelectedData()
        {
            var datas = EditorControllerDatas;
            datas.Remove(EditorSelectedData);
            if (datas.Count > 0)
            {
                m_EditorSelectedDataName = datas[0].EditorName;
            }
            else
            {
                m_EditorSelectedDataName = string.Empty;
            }
            m_EditorRenameDataName = string.Empty;
            m_EditorNewStateName = string.Empty;
            EditorRefresh();
        }

        [BoxGroup("Select Data/State")]
        [HorizontalGroup("Select Data/State/")]
        [LabelText("State Name")]
        [PropertyOrder(30)]
        [ShowInInspector]
        [EnableIf("EditorIsSelectedData")]
        [InfoBox("State name already exists!", 
            InfoMessageType.Warning,
            "EditorShowNewStateNameInfo")]
        private string m_EditorNewStateName;

        [BoxGroup("Select Data/State")]
        [HorizontalGroup("Select Data/State/", Width = ConstHorizontalButtonWidth - 5)]
        [GUIColor(0,1,0)]
        [Button("Add State Name")]
        [PropertyOrder(31)]
        [EnableIf("EditorCheckCanAddStateName")]
        private void EditorAddStateName()
        {
            if(!EditorCheckCanAddStateName())
                return;
            if (string.IsNullOrEmpty(m_EditorNewStateName))
                return;
            var data = EditorSelectedData;
            data.EditorStateNames.Add(m_EditorNewStateName);
            data.EditorLinkDatas.Add(new LinkData());
            m_EditorSelectedStatesRenameButtons.Add(false);
            m_EditorSelectedStatesRenameNames.Add(m_EditorNewStateName);
            m_EditorNewStateName = string.Empty;
            EditorRefresh();
        }

        private readonly List<string> m_EditorEmptyListString = new List<string>();
        [BoxGroup("Select Data/State")]
        [LabelText("State Names")]
        [PropertyOrder(32)]
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
        [PropertyOrder(33)]
        [ShowInInspector]
        [ReadOnly]
        [ListDrawerSettings]
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

        private bool EditorCanPasteSelectedData()
        {
            return EditorIsSelectedData() && !string.IsNullOrEmpty(TextEditor.text);
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
            if (string.IsNullOrEmpty(m_EditorNewStateName))
                return false;
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

            GUI.enabled = true;
            float fixedWidth = EditorStyles.label.fixedWidth;
            GUI.skin.label.fixedWidth = 25;
            GUILayout.Label(selectionIndex.ToString());
            GUI.skin.label.fixedWidth = fixedWidth;
        }

        private readonly string[] m_EditorEmptyStringArray = Array.Empty<string>();
        private readonly List<bool> m_EditorSelectedStatesRenameButtons = new List<bool>();
        private readonly List<string> m_EditorSelectedStatesRenameNames = new List<string>();
        private void EditorOnStateNameEndGUI(int selectionIndex)
        {
            var selectedData = EditorSelectedData;
            if (selectionIndex >= selectedData.EditorStateNames.Count || selectionIndex >= m_EditorSelectedStatesRenameButtons.Count)
            {
                GUILayout.EndHorizontal();
                return;
            }
            var curSateName = selectedData.EditorStateNames[selectionIndex];
            if (m_EditorSelectedStatesRenameButtons[selectionIndex])
            {
                GUI.enabled = true;
                m_EditorSelectedStatesRenameNames[selectionIndex] = EditorGUILayout.TextField(m_EditorSelectedStatesRenameNames[selectionIndex]);
                GUI.enabled = !selectedData.EditorStateNames.Contains(m_EditorSelectedStatesRenameNames[selectionIndex]);
                if (GUILayout.Button("Rename"))
                {
                    Undo.RegisterCompleteObjectUndo(this, "Change State Name");
                    if (selectedData.EditorSelectedName == curSateName)
                    {
                        selectedData.EditorClearSelectedName();
                    }
                    string newStateName = m_EditorSelectedStatesRenameNames[selectionIndex];
                    string oldStateName = selectedData.EditorStateNames[selectionIndex];
                    selectedData.EditorStateNames[selectionIndex] = newStateName;
                    foreach (var data in EditorControllerDatas)
                    {
                        data.EditorOnStateRename(selectedData.EditorName, oldStateName, newStateName);
                    }
                    m_EditorSelectedStatesRenameButtons[selectionIndex] = false;
                    EditorGUI.FocusTextInControl(null);
                }
                GUI.enabled = true;
                if (GUILayout.Button("Cancel"))
                {
                    m_EditorSelectedStatesRenameButtons[selectionIndex] = false;
                    EditorGUI.FocusTextInControl(null);
                }
            }
            else
            {
                GUI.enabled = selectedData.EditorStateNames.Count >= 2 && !m_EditorSelectedStatesRenameButtons.Contains(true);
                if (GUILayout.Button("↑"))
                {
                    int index1 = selectionIndex < 1 ? selectedData.EditorStateNames.Count - 1 : selectionIndex - 1;
                    int index2 = selectionIndex;
                    Undo.RegisterCompleteObjectUndo(this, "Move Up State");
                    (selectedData.EditorStateNames[index1], selectedData.EditorStateNames[index2]) = (selectedData.EditorStateNames[index2], selectedData.EditorStateNames[index1]);
                    (selectedData.EditorLinkDatas[index1], selectedData.EditorLinkDatas[index2]) = (selectedData.EditorLinkDatas[index2], selectedData.EditorLinkDatas[index1]);
                    (m_EditorSelectedStatesRenameButtons[index1], m_EditorSelectedStatesRenameButtons[index2]) = (m_EditorSelectedStatesRenameButtons[index2], m_EditorSelectedStatesRenameButtons[index1]);
                    (m_EditorSelectedStatesRenameNames[index1], m_EditorSelectedStatesRenameNames[index2]) = (m_EditorSelectedStatesRenameNames[index2], m_EditorSelectedStatesRenameNames[index1]);
                    foreach (var state in EditorStates)
                    {
                        state.EditorOnDataSwitchState(m_EditorSelectedDataName, index1, index2);
                    }
                }
                if (GUILayout.Button("↓"))
                {
                    int index1 = selectionIndex >= selectedData.EditorStateNames.Count - 1 ? 0 : selectionIndex + 1;
                    int index2 = selectionIndex;
                    Undo.RegisterCompleteObjectUndo(this, "Move Down State");
                    (selectedData.EditorStateNames[index1], selectedData.EditorStateNames[index2]) = (selectedData.EditorStateNames[index2], selectedData.EditorStateNames[index1]);
                    (selectedData.EditorLinkDatas[index1], selectedData.EditorLinkDatas[index2]) = (selectedData.EditorLinkDatas[index2], selectedData.EditorLinkDatas[index1]);
                    (m_EditorSelectedStatesRenameButtons[index1], m_EditorSelectedStatesRenameButtons[index2]) = (m_EditorSelectedStatesRenameButtons[index2], m_EditorSelectedStatesRenameButtons[index1]);
                    (m_EditorSelectedStatesRenameNames[index1], m_EditorSelectedStatesRenameNames[index2]) = (m_EditorSelectedStatesRenameNames[index2], m_EditorSelectedStatesRenameNames[index1]);
                    foreach (var state in EditorStates)
                    {
                        state.EditorOnDataSwitchState(m_EditorSelectedDataName, index1, index2);
                    }
                }
                GUI.enabled = true;
                var color = GUI.color;
                if (selectedData.EditorSelectedName == curSateName)
                {
                    GUI.color = new Color(0,1,0);
                    GUI.enabled = false;
                }
                if (GUILayout.Button("Apply"))
                {
                    Undo.RegisterCompleteObjectUndo(this, "Apply State");
                    EditorRefresh();
                    selectedData.EditorSelectedName = curSateName;
                }
                GUI.enabled = true;
                GUI.color = new Color(1,0,0);
                if (GUILayout.Button("X"))
                {
                    Undo.RegisterCompleteObjectUndo(this, "Remove State");
                    foreach (var state in EditorStates)
                    {
                        state.EditorOnDataRemoveState(m_EditorSelectedDataName, selectionIndex);
                    }
                    if (selectedData.EditorSelectedName == curSateName)
                    {
                        selectedData.EditorClearSelectedName();
                    }
                    selectedData.EditorStateNames.RemoveAt(selectionIndex);
                    selectedData.EditorLinkDatas.RemoveAt(selectionIndex);
                    m_EditorSelectedStatesRenameButtons.RemoveAt(selectionIndex);
                    m_EditorSelectedStatesRenameNames.RemoveAt(selectionIndex);
                    GUILayout.EndHorizontal();
                    return;
                }
                // link
                GUI.color = color;
                GUILayout.Label("Link");
                var curLinkData = selectedData.EditorLinkDatas[selectionIndex];
                var dataNames = EditorGetCanLinkDataNames(selectedData.EditorName);
                int index = Array.IndexOf(dataNames, curLinkData.EditorTargetDataName);
                index = EditorGUILayout.Popup(index, dataNames);
                if (index != -1)
                {
                    if (curLinkData.EditorTargetDataName != dataNames[index])
                    {
                        Undo.RegisterCompleteObjectUndo(this, "Data Link");
                    }
                    curLinkData.EditorTargetDataName = dataNames[index];
                    GUI.enabled = true;
                }
                else
                {
                    GUI.enabled = false;
                }
                var curData = EditorGetData(curLinkData.EditorTargetDataName);
                if (curData != null)
                {
                    var stateNames = curData.EditorStateNames.ToArray();
                    index = Array.IndexOf(stateNames, curLinkData.EditorTargetSelectedName);
                    index = EditorGUILayout.Popup(index, stateNames);
                    if (index != -1)
                    {
                        if (curLinkData.EditorTargetSelectedName != stateNames[index])
                        {
                            Undo.RegisterCompleteObjectUndo(this, "State Link");
                        }
                        curLinkData.EditorTargetSelectedName = stateNames[index];
                    }
                }
                else
                {
                    EditorGUILayout.Popup(-1, m_EditorEmptyStringArray);
                }
                GUI.enabled = !string.IsNullOrEmpty(curLinkData.EditorTargetDataName);
                if (GUILayout.Button("Clear"))
                {
                    Undo.RegisterCompleteObjectUndo(this, "Clear Link");
                    curLinkData.EditorTargetDataName = string.Empty;
                    curLinkData.EditorTargetSelectedName = string.Empty;
                }
                GUI.enabled = true;
                if (GUILayout.Button("Rename"))
                {
                    Undo.RegisterCompleteObjectUndo(this, "Rename State Button");
                    m_EditorSelectedStatesRenameButtons[selectionIndex] = true;
                    m_EditorSelectedStatesRenameNames[selectionIndex] = selectedData.EditorStateNames[selectionIndex];
                }
            }
            GUILayout.EndHorizontal();
        }

        private string[] EditorGetCanLinkDataNames(string dataName)
        {
            var canLinkDataNames = EditorGetAllDataNames();
            foreach (var data in EditorControllerDatas)
            {
                if(data.EditorName == dataName)
                {
                    canLinkDataNames.Remove(dataName);
                }
                else
                {
                    foreach (var linkData in data.EditorLinkDatas)
                    {
                        if (!string.IsNullOrEmpty(linkData.EditorTargetDataName))
                        {
                            canLinkDataNames.Remove(data.EditorName);
                        }
                    }
                }
            }
            return canLinkDataNames.ToArray();
        }

        private void EditorRefreshSelectedStatesRenameDatas()
        {
            m_EditorSelectedStatesRenameButtons.Clear();
            m_EditorSelectedStatesRenameNames.Clear();
            var selectedData = EditorSelectedData;
            if(selectedData == null)
                return;
            var stateNames = selectedData.EditorStateNames;
            for (int i = 0; i < stateNames.Count; i++)
            {
                m_EditorSelectedStatesRenameButtons.Add(false);
                m_EditorSelectedStatesRenameNames.Add(stateNames[i]);
            }
        }
    }
}
#endif
