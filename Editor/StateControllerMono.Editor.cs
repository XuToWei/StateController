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
        private const float ConstHorizontalButtonWidth = 110f;

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
                //不是该Mono的子节点就移除
                GameObject selfGo = gameObject;
                for(int i = m_EditorStates.Count - 1; i >= 0; i--)
                {
                    if (m_EditorStates[i].gameObject != selfGo && m_EditorStates[i].GetComponentInParent<StateControllerMono>(true) != this)
                    {
                        m_EditorStates.RemoveAt(i);
                    }
                }
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

        [BoxGroup("Data")]
        [HorizontalGroup("Data/Edit", Width = ConstHorizontalButtonWidth * 0.5f)]
        [GUIColor(0, 1, 0)]
        [Button("Add")]
        [PropertyOrder(25)]
        [EnableIf(nameof(EditorCheckCanAddData))]
        private void EditorAddNewData()
        {
            if (!EditorCheckCanAddData())
                return;
            StateControllerData data = new StateControllerData();
            data.EditorName = m_EditorDataNameInput;
            EditorControllerDatas.Add(data);
            m_EditorSelectedDataName = m_EditorDataNameInput;
            m_EditorDataNameInput = string.Empty;
            EditorRefresh();
        }

        [BoxGroup("Data")]
        [HorizontalGroup("Data/")]
        [LabelText("Data Name")]
        [GUIColor(0.6f, 0.8f, 1f)]
        [PropertyOrder(20)]
        [ShowInInspector]
        [ValueDropdown(nameof(EditorGetAllDataNames))]
        [OnValueChanged(nameof(EditorOnSelectedDataChanged), InvokeOnInitialize = true)]
        [SerializeField]
        private string m_EditorSelectedDataName = string.Empty;

        private StateControllerData EditorSelectedData => GetData(m_EditorSelectedDataName);

        [BoxGroup("Data")]
        [HorizontalGroup("Data/", Width = ConstHorizontalButtonWidth * 0.5f - 15f)]
        [GUIColor(0, 1, 0)]
        [Button("Copy")]
        [PropertyOrder(21)]
        [EnableIf(nameof(EditorIsSelectedData))]
        private void EditorCopySelectedData()
        {
            TextEditor.text = JsonUtility.ToJson(EditorSelectedData);
        }

        [BoxGroup("Data")]
        [HorizontalGroup("Data/", Width = ConstHorizontalButtonWidth * 0.5f - 10f)]
        [GUIColor(1, 1, 0)]
        [Button("Paste")]
        [PropertyOrder(22)]
        [EnableIf(nameof(EditorCanPasteSelectedData))]
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
            selectedData.EditorStates.Clear();
            m_EditorNewStateName = string.Empty;
            // 直接复用反序列化出来的状态对象（含各自的 link），名字与 link 一并粘贴
            selectedData.EditorStates.AddRange(data.EditorStates);
            EditorRefresh();
        }

        [BoxGroup("Data")]
        [HorizontalGroup("Data/", Width = 20f)]
        [GUIColor(1, 0, 0)]
        [Button("X")]
        [PropertyOrder(23)]
        [EnableIf(nameof(EditorIsSelectedData))]
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
            m_EditorDataNameInput = string.Empty;
            m_EditorNewStateName = string.Empty;
            EditorRefresh();
        }

        [BoxGroup("Data")]
        [HorizontalGroup("Data/Edit")]
        [LabelText("Input Data Name")]
        [GUIColor(0.6f, 0.8f, 1f)]
        [PropertyOrder(24)]
        [ShowInInspector]
        [InfoBox("Data name already exists!",
            InfoMessageType.Warning,
            nameof(EditorShowNewDataNameInfo))]
        private string m_EditorDataNameInput;

        [BoxGroup("Data")]
        [HorizontalGroup("Data/Edit", Width = ConstHorizontalButtonWidth * 0.5f)]
        [GUIColor(0, 1, 0)]
        [Button("Rename")]
        [PropertyOrder(26)]
        [EnableIf(nameof(EditorCheckCanRenameData))]
        private void EditorRenameSelectedDataName()
        {
            if (string.IsNullOrEmpty(m_EditorDataNameInput))
                return;
            var selectedData = EditorSelectedData;
            if (m_EditorDataNameInput == selectedData.EditorName)
                return;
            foreach (var data in EditorControllerDatas)
            {
                if(data == selectedData)
                    continue;
                if (data.EditorName == m_EditorDataNameInput)
                {
                    return;
                }
            }
            foreach (var data in EditorControllerDatas)
            {
                if(data == selectedData)
                    continue;
                data.EditorOnDataRename(m_EditorSelectedDataName, m_EditorDataNameInput);
            }
            foreach (var state in EditorStates)
            {
                state.EditorOnDataRename(m_EditorSelectedDataName, m_EditorDataNameInput);
            }
            selectedData.EditorName = m_EditorDataNameInput;
            m_EditorSelectedDataName = m_EditorDataNameInput;
            m_EditorDataNameInput = string.Empty;
        }

        [BoxGroup("Data/State")]
        [HorizontalGroup("Data/State/Edit")]
        [LabelText("New State Name")]
        [GUIColor(1f, 0.82f, 0.45f)]
        [PropertyOrder(30)]
        [ShowInInspector]
        [EnableIf(nameof(EditorIsSelectedData))]
        [InfoBox("State name already exists!",
            InfoMessageType.Warning,
            nameof(EditorShowNewStateNameInfo))]
        private string m_EditorNewStateName;

        [BoxGroup("Data/State")]
        [HorizontalGroup("Data/State/Edit", Width = ConstHorizontalButtonWidth * 0.5f)]
        [GUIColor(0, 1, 0)]
        [Button("Add")]
        [PropertyOrder(31)]
        [EnableIf(nameof(EditorCheckCanAddStateName))]
        private void EditorAddStateName()
        {
            if(!EditorCheckCanAddStateName())
                return;
            if (string.IsNullOrEmpty(m_EditorNewStateName))
                return;
            var data = EditorSelectedData;
            data.EditorStates.Add(new StateControllerState { EditorName = m_EditorNewStateName });
            m_EditorNewStateName = string.Empty;
            EditorRefresh();
        }

        private readonly List<StateControllerState> m_EditorEmptyStates = new List<StateControllerState>();
        [BoxGroup("Data/State")]
        [LabelText("State Names")]
        [PropertyOrder(32)]
        [ShowInInspector]
        [EnableIf(nameof(EditorIsSelectedData))]
        [DisableContextMenu(true, true)]
        [HideReferenceObjectPicker]
        [ListDrawerSettings(DefaultExpandedState = true,
            DraggableItems = true,
            HideAddButton = true,
            HideRemoveButton = true,
            ShowIndexLabels = false,
            OnEndListElementGUI = nameof(EditorOnStateNameGUI))]
        // 直接用 data.EditorStates（真实列表），link 随状态对象一起移动；setter 让 Odin 视为可编辑以启用拖拽排序
        private List<StateControllerState> EditorSelectedStates
        {
            get
            {
                var data = EditorSelectedData;
                return data == null ? m_EditorEmptyStates : data.EditorStates;
            }
            set
            {
                var data = EditorSelectedData;
                if (data == null || ReferenceEquals(value, data.EditorStates))
                    return;
                // Odin 传回的是重排后的列表（复用原状态对象，含各自 link），按其顺序回填
                data.EditorStates.Clear();
                data.EditorStates.AddRange(value);
            }
        }

        private readonly List<BaseState> m_EditorListStates = new List<BaseState>();
        [BoxGroup("Data/State")]
        [LabelText("State Children")]
        [PropertyOrder(33)]
        [ShowInInspector]
        [ReadOnly]
        [DisableContextMenu(true, true)]
        [ListDrawerSettings]
        [EnableIf(nameof(EditorIsSelectedData))]
        private List<BaseState> EditorSelectedStateMonos
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
            if (string.IsNullOrEmpty(m_EditorDataNameInput))
                return false;
            foreach (var subStateController in EditorControllerDatas)
            {
                if (subStateController.EditorName == m_EditorDataNameInput)
                {
                    return true;
                }
            }
            return false;
        }

        private bool EditorCheckCanAddData()
        {
            if (string.IsNullOrEmpty(m_EditorDataNameInput))
                return false;
            foreach (var data in EditorControllerDatas)
            {
                if (data.EditorName == m_EditorDataNameInput)
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
            foreach (var state in data.EditorStates)
            {
                if (state.EditorName == m_EditorNewStateName)
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
            foreach (var state in EditorSelectedData.EditorStates)
            {
                if (state.EditorName == m_EditorNewStateName)
                    return false;
            }
            return true;
        }

        private bool EditorCheckCanRenameData()
        {
            if (!EditorIsSelectedData())
                return false;
            var selectedData = EditorSelectedData;
            if (selectedData == null || selectedData.EditorName == m_EditorDataNameInput)
                return false;
            if (string.IsNullOrEmpty(m_EditorDataNameInput))
                return false;
            foreach (var data in EditorControllerDatas)
            {
                if(data == selectedData)
                    continue;
                if (data.EditorName == m_EditorDataNameInput)
                {
                    return false;
                }
            }
            return true;
        }

        private readonly string[] m_EditorEmptyStringArray = Array.Empty<string>();
        private void EditorOnStateNameGUI(int selectionIndex)
        {
            GUI.enabled = true;
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label(selectionIndex.ToString(), GUILayout.Width(25));
            var selectedData = EditorSelectedData;
            if (selectionIndex >= selectedData.EditorStates.Count)
            {
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                return;
            }
            var curState = selectedData.EditorStates[selectionIndex];
            var curSateName = curState.EditorName;
            if (curState.EditorIsRenaming)
            {
                GUI.enabled = true;
                curState.EditorRenamingText = EditorGUILayout.TextField(curState.EditorRenamingText);
                bool nameExists = false;
                foreach (var s in selectedData.EditorStates)
                {
                    if (s.EditorName == curState.EditorRenamingText)
                    {
                        nameExists = true;
                        break;
                    }
                }
                GUI.enabled = !nameExists;
                if (GUILayout.Button("Rename", GUILayout.ExpandWidth(false)))
                {
                    Undo.RegisterCompleteObjectUndo(this, "Change State Name");
                    if (selectedData.EditorSelectedName == curSateName)
                    {
                        selectedData.EditorClearSelectedName();
                    }
                    string newStateName = curState.EditorRenamingText;
                    curState.EditorName = newStateName;
                    foreach (var data in EditorControllerDatas)
                    {
                        data.EditorOnStateRename(selectedData.EditorName, curSateName, newStateName);
                    }
                    foreach (var state in EditorStates)
                    {
                        state.EditorOnDataStateRename(selectedData.EditorName, curSateName, newStateName);
                    }
                    curState.EditorIsRenaming = false;
                    EditorGUI.FocusTextInControl(null);
                    EditorRefresh();
                }
                GUI.enabled = true;
                if (GUILayout.Button("Cancel", GUILayout.ExpandWidth(false)))
                {
                    curState.EditorIsRenaming = false;
                    EditorGUI.FocusTextInControl(null);
                }
                GUILayout.EndHorizontal();
                // 改名时仍然绘制该状态的 link，保持 link 编辑器内容不变
                EditorDrawStateLinks(selectedData, selectionIndex);
                GUILayout.EndVertical();
                return;
            }

            // 名字只读显示（改名走 Rename 按钮以同步所有关联数据）
            GUI.enabled = false;
            EditorGUILayout.TextField(curSateName);
            GUI.enabled = true;
            var color = GUI.color;
            if (selectedData.EditorSelectedName == curSateName)
            {
                GUI.color = new Color(0, 1, 0);
                GUI.enabled = false;
            }
            if (GUILayout.Button("Apply", GUILayout.ExpandWidth(false)))
            {
                Undo.RegisterCompleteObjectUndo(this, "Apply State");
                EditorRefresh();
                selectedData.EditorSelectedName = curSateName;
            }
            GUI.enabled = true;
            GUI.color = new Color(1, 0, 0);
            if (GUILayout.Button("X", GUILayout.ExpandWidth(false)))
            {
                Undo.RegisterCompleteObjectUndo(this, "Remove State");
                if (selectedData.EditorSelectedName == curSateName)
                {
                    selectedData.EditorClearSelectedName();
                }
                selectedData.EditorStates.RemoveAt(selectionIndex);
                GUI.color = color;
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                EditorRefresh();
                return;
            }
            GUI.color = color;
            if (GUILayout.Button("Rename", GUILayout.ExpandWidth(false)))
            {
                curState.EditorIsRenaming = true;
                curState.EditorRenamingText = curSateName;
            }
            GUILayout.EndHorizontal();

            // 竖排展示该状态的多条 link（一对多）
            EditorDrawStateLinks(selectedData, selectionIndex);

            GUILayout.EndVertical();
        }

        private void EditorDrawStateLinks(StateControllerData selectedData, int selectionIndex)
        {
            var state = selectedData.EditorStates[selectionIndex];
            var links = state.EditorLinks;
            var defColor = GUI.color;
            int linkCount = links.Count;
            for (int i = 0; i < linkCount; i++)
            {
                var curLinkData = links[i];
                GUILayout.BeginHorizontal();
                GUILayout.Space(45);
                GUILayout.Label("Link", GUILayout.Width(35));
                // 目标 Data（蓝青标识）
                GUI.color = new Color(0.6f, 0.8f, 1f);
                GUILayout.Label("Data:", GUILayout.Width(38));
                var dataNames = EditorGetCanLinkDataNames(selectedData.EditorName, state, curLinkData);
                int index = Array.IndexOf(dataNames, curLinkData.EditorTargetDataName);
                index = EditorGUILayout.Popup(index, dataNames);
                GUI.color = defColor;
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
                // 目标 State（橙黄标识）
                GUI.color = new Color(1f, 0.82f, 0.45f);
                GUILayout.Label("State:", GUILayout.Width(42));
                var curData = EditorGetData(curLinkData.EditorTargetDataName);
                if (curData != null)
                {
                    var curStates = curData.EditorStates;
                    var stateNames = new string[curStates.Count];
                    for (int n = 0; n < curStates.Count; n++)
                    {
                        stateNames[n] = curStates[n].EditorName;
                    }
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
                GUI.color = defColor;
                GUI.enabled = true;
                GUI.color = new Color(1, 0, 0);
                if (GUILayout.Button("X", GUILayout.ExpandWidth(false)))
                {
                    Undo.RegisterCompleteObjectUndo(this, "Remove Link");
                    links.RemoveAt(i);
                    GUI.color = defColor;
                    GUILayout.EndHorizontal();
                    break;
                }
                GUI.color = defColor;
                GUILayout.EndHorizontal();
            }
            // 已存在未指定目标 data 的空 link 时，不允许再新增
            bool hasEmptyLink = false;
            foreach (var link in links)
            {
                if (string.IsNullOrEmpty(link.EditorTargetDataName))
                {
                    hasEmptyLink = true;
                    break;
                }
            }
            GUILayout.BeginHorizontal();
            GUILayout.Space(45);
            GUI.enabled = !hasEmptyLink && EditorGetCanLinkDataNames(selectedData.EditorName, state, null).Length > 0;
            GUI.color = new Color(0, 1, 0);
            if (GUILayout.Button("+ Add Link", GUILayout.ExpandWidth(false)))
            {
                Undo.RegisterCompleteObjectUndo(this, "Add Link");
                state.EditorLinks.Add(new StateControllerStateLink());
            }
            GUI.color = defColor;
            GUI.enabled = true;
            GUILayout.EndHorizontal();
        }

        private string[] EditorGetCanLinkDataNames(string dataName, StateControllerState state, StateControllerStateLink exceptLink)
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
                    foreach (var dataState in data.EditorStates)
                    {
                        foreach (var link in dataState.EditorLinks)
                        {
                            if (!string.IsNullOrEmpty(link.EditorTargetDataName))
                            {
                                canLinkDataNames.Remove(data.EditorName);
                            }
                        }
                    }
                }
            }
            // 同一状态下其它 link 已占用的目标 data 不可重复选
            if (state != null)
            {
                foreach (var link in state.EditorLinks)
                {
                    if (link == exceptLink)
                        continue;
                    if (!string.IsNullOrEmpty(link.EditorTargetDataName))
                    {
                        canLinkDataNames.Remove(link.EditorTargetDataName);
                    }
                }
            }
            return canLinkDataNames.ToArray();
        }

        // 切换选中的 Data 时清空各状态的改名编辑态
        private void EditorOnSelectedDataChanged()
        {
            foreach (var data in EditorControllerDatas)
            {
                foreach (var state in data.EditorStates)
                {
                    state.EditorIsRenaming = false;
                }
            }
        }
    }
}
#endif
