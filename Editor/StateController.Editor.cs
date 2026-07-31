#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace StateController
{
    public partial class StateController
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

        private readonly List<StateBinding> m_EditorBindings = new List<StateBinding>();
        public List<StateBinding> EditorBindings
        {
            get
            {
                m_EditorBindings.Clear();
                GetComponentsInChildren<StateBinding>(true, m_EditorBindings);
                //不是该 Controller 的子节点就移除
                GameObject selfGo = gameObject;
                for(int i = m_EditorBindings.Count - 1; i >= 0; i--)
                {
                    if (m_EditorBindings[i].gameObject != selfGo && m_EditorBindings[i].GetComponentInParent<StateController>(true) != this)
                    {
                        m_EditorBindings.RemoveAt(i);
                    }
                }
                return m_EditorBindings;
            }
        }

        public List<StateGroup> EditorGroups => m_Groups;

        private void OnValidate()
        {
            EditorRefresh();
        }

        private int m_EditorLastRefreshFrame = -1;
        internal void EditorRefresh()
        {
            if (m_EditorLastRefreshFrame == Time.frameCount)
                return;
            if (EditorSelectedGroup == null)
                return;
            m_EditorLastRefreshFrame = Time.frameCount;
            foreach (var group in EditorGroups)
            {
                group.EditorRefresh(this);
            }
            foreach (var binding in EditorBindings)
            {
                binding.EditorRefresh();
            }
            foreach (var group in EditorGroups)
            {
                group.EditorApplyCurrentState();
            }
        }

        internal StateGroup EditorGetGroup(string groupName)
        {
            foreach (var group in EditorGroups)
            {
                if (group.EditorName == groupName)
                {
                    return group;
                }
            }
            return null;
        }

        [BoxGroup("Group")]
        [HorizontalGroup("Group/Edit", Width = ConstHorizontalButtonWidth * 0.5f)]
        [GUIColor(0, 1, 0)]
        [Button("Add")]
        [PropertyOrder(25)]
        [EnableIf(nameof(EditorCheckCanAddGroup))]
        private void EditorAddNewGroup()
        {
            if (!EditorCheckCanAddGroup())
                return;
            StateGroup group = new StateGroup();
            group.EditorName = m_EditorGroupNameInput;
            EditorGroups.Add(group);
            m_EditorSelectedGroupName = m_EditorGroupNameInput;
            m_EditorGroupNameInput = string.Empty;
            EditorRefresh();
        }

        [BoxGroup("Group")]
        [HorizontalGroup("Group/")]
        [LabelText("Group Name")]
        [GUIColor(0.6f, 0.8f, 1f)]
        [PropertyOrder(20)]
        [ShowInInspector]
        [ValueDropdown(nameof(EditorGetAllGroupNames))]
        [OnValueChanged(nameof(EditorOnSelectedGroupChanged), InvokeOnInitialize = true)]
        [SerializeField]
        private string m_EditorSelectedGroupName = string.Empty;

        private StateGroup EditorSelectedGroup => GetGroup(m_EditorSelectedGroupName);

        [BoxGroup("Group")]
        [HorizontalGroup("Group/", Width = ConstHorizontalButtonWidth * 0.5f - 15f)]
        [GUIColor(0, 1, 0)]
        [Button("Copy")]
        [PropertyOrder(21)]
        [EnableIf(nameof(EditorIsSelectedGroup))]
        private void EditorCopySelectedGroup()
        {
            TextEditor.text = JsonUtility.ToJson(EditorSelectedGroup);
        }

        [BoxGroup("Group")]
        [HorizontalGroup("Group/", Width = ConstHorizontalButtonWidth * 0.5f - 10f)]
        [GUIColor(1, 1, 0)]
        [Button("Paste")]
        [PropertyOrder(22)]
        [EnableIf(nameof(EditorCanPasteSelectedGroup))]
        private void EditorPasteSelectedGroup()
        {
            var selectedGroup = EditorSelectedGroup;
            bool hasConnection = false;
            foreach (var binding in EditorBindings)
            {
                if (binding.EditorIsBoundTo(selectedGroup))
                {
                    hasConnection = true;
                    break;
                }
            }
            if (hasConnection)
            {
                EditorUtility.DisplayDialog("Paste Error", $"Selected group ({selectedGroup.EditorName}) has connected state bindings and cannot be pasted.", "OK");
                return;
            }
            StateGroup group = JsonUtility.FromJson<StateGroup>(TextEditor.text);
            selectedGroup.EditorStates.Clear();
            m_EditorNewStateName = string.Empty;
            // 直接复用反序列化出来的状态对象（含各自的 link），名字与 link 一并粘贴
            selectedGroup.EditorStates.AddRange(group.EditorStates);
            EditorRefresh();
        }

        [BoxGroup("Group")]
        [HorizontalGroup("Group/", Width = 20f)]
        [GUIColor(1, 0, 0)]
        [Button("X")]
        [PropertyOrder(23)]
        [EnableIf(nameof(EditorIsSelectedGroup))]
        private void EditorRemoveSelectedGroup()
        {
            var groups = EditorGroups;
            groups.Remove(EditorSelectedGroup);
            if (groups.Count > 0)
            {
                m_EditorSelectedGroupName = groups[0].EditorName;
            }
            else
            {
                m_EditorSelectedGroupName = string.Empty;
            }
            m_EditorGroupNameInput = string.Empty;
            m_EditorNewStateName = string.Empty;
            EditorRefresh();
        }

        [BoxGroup("Group")]
        [HorizontalGroup("Group/Edit")]
        [LabelText("Input Group Name")]
        [GUIColor(0.6f, 0.8f, 1f)]
        [PropertyOrder(24)]
        [ShowInInspector]
        [InfoBox("Group name already exists!",
            InfoMessageType.Warning,
            nameof(EditorShowNewGroupNameInfo))]
        private string m_EditorGroupNameInput;

        [BoxGroup("Group")]
        [HorizontalGroup("Group/Edit", Width = ConstHorizontalButtonWidth * 0.5f)]
        [Button("Rename")]
        [PropertyOrder(26)]
        [EnableIf(nameof(EditorCheckCanRenameGroup))]
        private void EditorRenameSelectedGroupName()
        {
            if (string.IsNullOrEmpty(m_EditorGroupNameInput))
                return;
            var selectedGroup = EditorSelectedGroup;
            if (m_EditorGroupNameInput == selectedGroup.EditorName)
                return;
            foreach (var group in EditorGroups)
            {
                if(group == selectedGroup)
                    continue;
                if (group.EditorName == m_EditorGroupNameInput)
                {
                    return;
                }
            }
            foreach (var group in EditorGroups)
            {
                if(group == selectedGroup)
                    continue;
                group.EditorOnGroupRename(m_EditorSelectedGroupName, m_EditorGroupNameInput);
            }
            foreach (var binding in EditorBindings)
            {
                binding.EditorOnGroupRename(m_EditorSelectedGroupName, m_EditorGroupNameInput);
            }
            selectedGroup.EditorName = m_EditorGroupNameInput;
            m_EditorSelectedGroupName = m_EditorGroupNameInput;
            m_EditorGroupNameInput = string.Empty;
        }

        [BoxGroup("Group/State")]
        [HorizontalGroup("Group/State/Edit")]
        [LabelText("Input State Name")]
        [GUIColor(1f, 0.82f, 0.45f)]
        [PropertyOrder(30)]
        [ShowInInspector]
        [EnableIf(nameof(EditorIsSelectedGroup))]
        [InfoBox("State name already exists!",
            InfoMessageType.Warning,
            nameof(EditorShowNewStateNameInfo))]
        private string m_EditorNewStateName;

        [BoxGroup("Group/State")]
        [HorizontalGroup("Group/State/Edit", Width = ConstHorizontalButtonWidth * 0.5f)]
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
            var group = EditorSelectedGroup;
            group.EditorStates.Add(new State { EditorName = m_EditorNewStateName });
            m_EditorNewStateName = string.Empty;
            EditorRefresh();
        }

        private readonly List<State> m_EditorEmptyStates = new List<State>();
        [BoxGroup("Group/State")]
        [LabelText("State Names")]
        [PropertyOrder(32)]
        [ShowInInspector]
        [EnableIf(nameof(EditorIsSelectedGroup))]
        [DisableContextMenu(true, true)]
        [HideReferenceObjectPicker]
        [ListDrawerSettings(DefaultExpandedState = true,
            DraggableItems = true,
            HideAddButton = true,
            HideRemoveButton = true,
            ShowIndexLabels = false,
            OnEndListElementGUI = nameof(EditorOnStateNameGUI))]
        // 直接用 group.EditorStates（真实列表），link 随状态对象一起移动；setter 让 Odin 视为可编辑以启用拖拽排序
        private List<State> EditorSelectedStates
        {
            get
            {
                var group = EditorSelectedGroup;
                return group == null ? m_EditorEmptyStates : group.EditorStates;
            }
            set
            {
                var group = EditorSelectedGroup;
                if (group == null || ReferenceEquals(value, group.EditorStates))
                    return;
                // Odin 传回的是重排后的列表（复用原状态对象，含各自 link），按其顺序回填
                group.EditorStates.Clear();
                group.EditorStates.AddRange(value);
                EditorRefresh();
            }
        }

        private readonly List<StateBinding> m_EditorConnectedBindings = new List<StateBinding>();
        [BoxGroup("Group/State")]
        [LabelText("State Bindings")]
        [PropertyOrder(33)]
        [ShowInInspector]
        [ReadOnly]
        [DisableContextMenu(true, true)]
        [ListDrawerSettings]
        [EnableIf(nameof(EditorIsSelectedGroup))]
        private List<StateBinding> EditorSelectedBindings
        {
            get
            {
                var group = EditorSelectedGroup;
                if (group == null)
                {
                    m_EditorConnectedBindings.Clear();
                }
                else
                {
                    m_EditorConnectedBindings.Clear();
                    foreach (var binding in EditorBindings)
                    {
                        if (binding.EditorIsBoundTo(group))
                        {
                            m_EditorConnectedBindings.Add(binding);
                        }
                    }
                }
                return m_EditorConnectedBindings;
            }
        }

        private bool EditorShowNewGroupNameInfo()
        {
            if (string.IsNullOrEmpty(m_EditorGroupNameInput))
                return false;
            foreach (var group in EditorGroups)
            {
                if (group.EditorName == m_EditorGroupNameInput)
                {
                    return true;
                }
            }
            return false;
        }

        private bool EditorCheckCanAddGroup()
        {
            if (string.IsNullOrEmpty(m_EditorGroupNameInput))
                return false;
            foreach (var group in EditorGroups)
            {
                if (group.EditorName == m_EditorGroupNameInput)
                    return false;
            }
            return true;
        }

        private bool EditorIsSelectedGroup()
        {
            if (string.IsNullOrEmpty(m_EditorSelectedGroupName))
                return false;
            foreach (var group in EditorGroups)
            {
                if (group.EditorName == m_EditorSelectedGroupName)
                    return true;
            }
            return false;
        }

        private bool EditorCanPasteSelectedGroup()
        {
            return EditorIsSelectedGroup() && !string.IsNullOrEmpty(TextEditor.text);
        }

        private readonly List<string> m_EditorGroupNames = new List<string>();
        public List<string> EditorGetAllGroupNames()
        {
            m_EditorGroupNames.Clear();
            foreach (var group in EditorGroups)
            {
                m_EditorGroupNames.Add(group.EditorName);
            }
            m_EditorGroupNames.Sort();
            return m_EditorGroupNames;
        }

        private bool EditorShowNewStateNameInfo()
        {
            var group = EditorSelectedGroup;
            if (group == null)
                return false;
            if (string.IsNullOrEmpty(m_EditorNewStateName))
                return false;
            foreach (var state in group.EditorStates)
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
            if (!EditorIsSelectedGroup())
                return false;
            foreach (var state in EditorSelectedGroup.EditorStates)
            {
                if (state.EditorName == m_EditorNewStateName)
                    return false;
            }
            return true;
        }

        private bool EditorCheckCanRenameGroup()
        {
            if (!EditorIsSelectedGroup())
                return false;
            var selectedGroup = EditorSelectedGroup;
            if (selectedGroup == null || selectedGroup.EditorName == m_EditorGroupNameInput)
                return false;
            if (string.IsNullOrEmpty(m_EditorGroupNameInput))
                return false;
            foreach (var group in EditorGroups)
            {
                if(group == selectedGroup)
                    continue;
                if (group.EditorName == m_EditorGroupNameInput)
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
            var selectedGroup = EditorSelectedGroup;
            if (selectionIndex >= selectedGroup.EditorStates.Count)
            {
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                return;
            }
            var currentState = selectedGroup.EditorStates[selectionIndex];
            var currentStateName = currentState.EditorName;
            if (currentState.EditorIsRenaming)
            {
                GUI.enabled = true;
                currentState.EditorRenamingText = EditorGUILayout.TextField(currentState.EditorRenamingText);
                bool nameExists = false;
                foreach (var s in selectedGroup.EditorStates)
                {
                    if (s.EditorName == currentState.EditorRenamingText)
                    {
                        nameExists = true;
                        break;
                    }
                }
                GUI.enabled = !nameExists;
                if (GUILayout.Button("Rename", GUILayout.ExpandWidth(false)))
                {
                    Undo.RegisterCompleteObjectUndo(this, "Change State Name");
                    if (selectedGroup.EditorCurrentStateName == currentStateName)
                    {
                        selectedGroup.EditorClearCurrentState();
                    }
                    string newStateName = currentState.EditorRenamingText;
                    currentState.EditorName = newStateName;
                    foreach (var group in EditorGroups)
                    {
                        group.EditorOnStateRename(selectedGroup.EditorName, currentStateName, newStateName);
                    }
                    foreach (var binding in EditorBindings)
                    {
                        binding.EditorOnGroupStateRename(selectedGroup.EditorName, currentStateName, newStateName);
                    }
                    currentState.EditorIsRenaming = false;
                    EditorGUI.FocusTextInControl(null);
                    EditorRefresh();
                }
                GUI.enabled = true;
                if (GUILayout.Button("Cancel", GUILayout.ExpandWidth(false)))
                {
                    currentState.EditorIsRenaming = false;
                    EditorGUI.FocusTextInControl(null);
                }
                GUILayout.EndHorizontal();
                // 改名时仍然绘制该状态的 link、event，保持编辑器内容不变
                EditorDrawStateLinks(selectedGroup, selectionIndex);
                EditorDrawStateOnSelectedEvent(selectedGroup, selectionIndex);
                GUILayout.EndVertical();
                return;
            }

            // 名字只读显示（改名走 Rename 按钮以同步所有关联数据）
            GUI.enabled = false;
            EditorGUILayout.TextField(currentStateName);
            GUI.enabled = true;
            var color = GUI.color;
            if (selectedGroup.EditorCurrentStateName == currentStateName)
            {
                GUI.color = new Color(0, 1, 0);
                GUI.enabled = false;
            }
            if (GUILayout.Button("Apply", GUILayout.ExpandWidth(false)))
            {
                Undo.RegisterCompleteObjectUndo(this, "Apply State");
                EditorRefresh();
                selectedGroup.EditorCurrentStateName = currentStateName;
            }
            GUI.enabled = true;
            // + Add Link 放在 Apply 和 X 之间，与状态名同一行（不换行）
            // 已存在未指定目标 Group 的空 link 时，不允许再新增
            bool hasEmptyLink = false;
            foreach (var link in currentState.EditorLinks)
            {
                if (string.IsNullOrEmpty(link.EditorTargetGroupName))
                {
                    hasEmptyLink = true;
                    break;
                }
            }
            GUI.enabled = !hasEmptyLink && EditorGetLinkableGroupNames(selectedGroup.EditorName, currentState, null).Length > 0;
            GUI.color = new Color(0.6f, 0.82f, 0.8f);
            if (GUILayout.Button("Add Link", GUILayout.ExpandWidth(false)))
            {
                Undo.RegisterCompleteObjectUndo(this, "Add Link");
                currentState.EditorLinks.Add(new StateLink());
            }
            GUI.color = color;
            GUI.enabled = true;
            // Add Event 按钮始终显示；已添加事件（m_OnSelectedEvent 非 null）后置灰不可点击，事件在下方绘制并提供删除按钮（类似 Button 的 onClick）
            GUI.enabled = currentState.EditorOnSelectedEvent == null;
            GUI.color = new Color(0.95f, 0.7f, 0.65f);
            if (GUILayout.Button("Add Event", GUILayout.ExpandWidth(false)))
            {
                Undo.RegisterCompleteObjectUndo(this, "Add Event");
                currentState.EditorOnSelectedEvent = new UnityEvent();
            }
            GUI.color = color;
            GUI.enabled = true;
            GUI.color = new Color(1, 0, 0);
            if (GUILayout.Button("X", GUILayout.ExpandWidth(false)))
            {
                Undo.RegisterCompleteObjectUndo(this, "Remove State");
                if (selectedGroup.EditorCurrentStateName == currentStateName)
                {
                    selectedGroup.EditorClearCurrentState();
                }
                selectedGroup.EditorStates.RemoveAt(selectionIndex);
                GUI.color = color;
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                EditorRefresh();
                return;
            }
            GUI.color = color;
            if (GUILayout.Button("Rename", GUILayout.ExpandWidth(false)))
            {
                currentState.EditorIsRenaming = true;
                currentState.EditorRenamingText = currentStateName;
            }
            GUILayout.EndHorizontal();

            // 竖排展示该状态的多条 link（一对多）
            EditorDrawStateLinks(selectedGroup, selectionIndex);

            // 展示该状态的可视化 event（单个，类似 Button 的 onClick）
            EditorDrawStateOnSelectedEvent(selectedGroup, selectionIndex);

            GUILayout.EndVertical();
        }

        private void EditorDrawStateLinks(StateGroup selectedGroup, int selectionIndex)
        {
            var state = selectedGroup.EditorStates[selectionIndex];
            var links = state.EditorLinks;
            var defColor = GUI.color;
            int linkCount = links.Count;
            for (int i = 0; i < linkCount; i++)
            {
                var currentLink = links[i];
                // 校验 link 的目标 Group 与 State 是否合法，任一非法则 Link 文字变红
                var linkTargetGroup = EditorGetGroup(currentLink.EditorTargetGroupName);
                bool linkValid = linkTargetGroup != null;
                if (linkValid)
                {
                    linkValid = false;
                    foreach (var s in linkTargetGroup.EditorStates)
                    {
                        if (s.EditorName == currentLink.EditorTargetStateName)
                        {
                            linkValid = true;
                            break;
                        }
                    }
                }
                GUILayout.BeginHorizontal();
                // 非法 link 在前面加一个黄色感叹号图标
                if (!linkValid)
                {
                    GUILayout.Space(25);
                    GUILayout.Label(EditorGUIUtility.IconContent("console.warnicon.sml"), GUILayout.Width(20), GUILayout.Height(18));
                }
                else
                {
                    GUILayout.Space(45);
                }
                GUILayout.Label("Link", GUILayout.Width(35));
                // 目标 Group（蓝青标识）
                GUI.color = new Color(0.6f, 0.8f, 1f);
                GUILayout.Label("Group:", GUILayout.Width(38));
                var groupNames = EditorGetLinkableGroupNames(selectedGroup.EditorName, state, currentLink);
                int index = Array.IndexOf(groupNames, currentLink.EditorTargetGroupName);
                index = EditorGUILayout.Popup(index, groupNames);
                GUI.color = defColor;
                if (index != -1)
                {
                    if (currentLink.EditorTargetGroupName != groupNames[index])
                    {
                        Undo.RegisterCompleteObjectUndo(this, "Group Link");
                    }
                    currentLink.EditorTargetGroupName = groupNames[index];
                    GUI.enabled = true;
                }
                else
                {
                    GUI.enabled = false;
                }
                // 目标 State（橙黄标识）
                GUI.color = new Color(1f, 0.82f, 0.45f);
                GUILayout.Label("State:", GUILayout.Width(42));
                var currentGroup = EditorGetGroup(currentLink.EditorTargetGroupName);
                if (currentGroup != null)
                {
                    var currentStates = currentGroup.EditorStates;
                    var stateNames = new string[currentStates.Count];
                    for (int n = 0; n < currentStates.Count; n++)
                    {
                        stateNames[n] = currentStates[n].EditorName;
                    }
                    index = Array.IndexOf(stateNames, currentLink.EditorTargetStateName);
                    index = EditorGUILayout.Popup(index, stateNames);
                    if (index != -1)
                    {
                        if (currentLink.EditorTargetStateName != stateNames[index])
                        {
                            Undo.RegisterCompleteObjectUndo(this, "State Link");
                        }
                        currentLink.EditorTargetStateName = stateNames[index];
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
        }

        private void EditorDrawStateOnSelectedEvent(StateGroup selectedGroup, int selectionIndex)
        {
            var state = selectedGroup.EditorStates[selectionIndex];
            // 未添加事件（m_OnSelectedEvent 为 null）则不显示
            if (state.EditorOnSelectedEvent == null)
                return;
            int groupIndex = EditorGroups.IndexOf(selectedGroup);
            if (groupIndex < 0)
                return;
            var defColor = GUI.color;
            // 标题行：Event 标签 + 删除按钮
            GUILayout.BeginHorizontal();
            GUILayout.Space(45);
            GUI.color = new Color(0.95f, 0.7f, 0.65f);
            GUILayout.Label("Event", GUILayout.Width(45));
            GUILayout.FlexibleSpace();
            GUI.color = new Color(1, 0, 0);
            bool remove = GUILayout.Button("X", GUILayout.ExpandWidth(false));
            GUI.color = defColor;
            GUILayout.EndHorizontal();
            if (remove)
            {
                Undo.RegisterCompleteObjectUndo(this, "Remove Event");
                state.EditorOnSelectedEvent = null;
                return;
            }
            // 事件本体：用 SerializedObject 走 Unity 原生 UnityEventDrawer 绘制可视化（对象 + 方法 + 参数）事件
            // 按字段名逐层导航属性树，避免硬编码 "m_Groups.Array.data[i]..." 字符串路径
            var serializedObject = new SerializedObject(this);
            serializedObject.Update();
            var eventProp = serializedObject
                .FindProperty(nameof(m_Groups))
                .GetArrayElementAtIndex(groupIndex)
                .FindPropertyRelative(StateGroup.EditorStatesFieldName)
                .GetArrayElementAtIndex(selectionIndex)
                .FindPropertyRelative(State.EditorOnSelectedEventFieldName);
            if (eventProp != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(45);
                GUILayout.BeginVertical();
                EditorGUILayout.PropertyField(eventProp, GUIContent.none, true);
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                serializedObject.ApplyModifiedProperties();
            }
        }

        private string[] EditorGetLinkableGroupNames(string groupName, State state, StateLink exceptLink)
        {
            var linkableGroupNames = EditorGetAllGroupNames();
            foreach (var group in EditorGroups)
            {
                if (group.EditorName == groupName)
                {
                    linkableGroupNames.Remove(groupName);
                }
                else
                {
                    foreach (var groupState in group.EditorStates)
                    {
                        foreach (var link in groupState.EditorLinks)
                        {
                            if (!string.IsNullOrEmpty(link.EditorTargetGroupName))
                            {
                                linkableGroupNames.Remove(group.EditorName);
                            }
                        }
                    }
                }
            }
            // 同一状态下其它 link 已占用的目标 Group 不可重复选
            if (state != null)
            {
                foreach (var link in state.EditorLinks)
                {
                    if (link == exceptLink)
                        continue;
                    if (!string.IsNullOrEmpty(link.EditorTargetGroupName))
                    {
                        linkableGroupNames.Remove(link.EditorTargetGroupName);
                    }
                }
            }
            return linkableGroupNames.ToArray();
        }

        // 切换选中的 Group 时清空各状态的改名编辑态
        private void EditorOnSelectedGroupChanged()
        {
            foreach (var group in EditorGroups)
            {
                foreach (var state in group.EditorStates)
                {
                    state.EditorIsRenaming = false;
                }
            }
        }
    }
}
#endif
