#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace StateController
{
    public partial class StateValueBinding<T>
    {
        private float? m_NameWidth;
        private float NameWidth
        {
            get
            {
                if (m_NameWidth.HasValue)
                {
                    return m_NameWidth.Value;
                }
                m_NameWidth = -1;
                var objects = GetType().GetCustomAttributes(typeof(StateNameWidthAttribute), true);
                if (objects.Length > 0)
                {
                    var nameWidth = (StateNameWidthAttribute)objects[0];
                    m_NameWidth = nameWidth.Width;
                }
                return m_NameWidth.Value;
            }
        }

        [ShowInInspector]
        [BoxGroup("Group")]
        [LabelText("Group Name")]
        [GUIColor(0.6f, 0.8f, 1f)]
        [PropertyOrder(10)]
        [ValueDropdown(nameof(EditorGetGroupNames))]
        [OnValueChanged(nameof(EditorOnSelectedGroup))]
        private string EditorGroupName
        {
            get => m_GroupName;
            set => m_GroupName = value;
        }

        [HideReferenceObjectPicker]
        [ShowInInspector]
        [BoxGroup("Group")]
        [LabelText("State Values")]
        [PropertyOrder(11)]
        [ShowIf(nameof(EditorHasSelectedGroup))]
        [ListDrawerSettings(DefaultExpandedState = true,
            HideAddButton = true,
            HideRemoveButton = true,
            DraggableItems = false,
            OnBeginListElementGUI = nameof(EditorOnStateValueBeginGUI),
            OnEndListElementGUI = nameof(EditorOnStateValueEndGUI))]
        [OnValueChanged(nameof(EditorApplyGroupCurrentState), true)]
        private List<StateValueMapping<T>> EditorValueMappings
        {
            set => m_ValueMappings = value;
            get => m_ValueMappings;
        }

        private StateGroup EditorGroup
        {
            get
            {
                var controller = EditorController;
                return controller == null ? null : controller.EditorGetGroup(EditorGroupName);
            }
        }

        internal override void EditorApplyCurrentState()
        {
            InitializeTarget();
            var group = EditorGroup;
            if (group == null || string.IsNullOrEmpty(group.EditorCurrentStateName))
                return;
            foreach (var valueMapping in EditorValueMappings)
            {
                if (valueMapping.EditorStateName == group.EditorCurrentStateName)
                {
                    ApplyValue(valueMapping.EditorValue);
                    break;
                }
            }
        }

        internal override void EditorOnGroupRename(string oldGroupName, string newGroupName)
        {
            if (EditorGroupName == oldGroupName)
            {
                EditorGroupName = newGroupName;
            }
        }

        internal override void EditorOnGroupStateRename(string groupName, string oldStateName, string newStateName)
        {
            if (EditorGroupName != groupName)
                return;
            foreach (var valueMapping in EditorValueMappings)
            {
                if (valueMapping.EditorStateName == oldStateName)
                {
                    valueMapping.EditorStateName = newStateName;
                    break;
                }
            }
        }

        internal override bool EditorIsBoundTo(StateGroup group)
        {
            return EditorGroupName == group.EditorName;
        }

        // 按名字对齐重建本组件存的（状态名,值），处理控制器状态的新增/删除/重排
        private readonly List<StateValueMapping<T>> m_EditorAlignedValueMappings = new List<StateValueMapping<T>>();
        internal override void EditorRefresh()
        {
            var group = EditorGroup;
            if (group == null)
            {
                EditorValueMappings.Clear();
                return;
            }
            var states = group.EditorStates;
            if (EditorValueMappings.Count == states.Count)
            {
                bool match = true;
                for (int i = 0; i < states.Count; i++)
                {
                    if (EditorValueMappings[i].EditorStateName != states[i].EditorName)
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                    return;
            }
            bool canCreate = typeof(T).GetConstructor(Type.EmptyTypes) != null;
            m_EditorAlignedValueMappings.Clear();
            for (int i = 0; i < states.Count; i++)
            {
                var stateName = states[i].EditorName;
                T value = canCreate ? Activator.CreateInstance<T>() : default;
                foreach (var valueMapping in EditorValueMappings)
                {
                    if (valueMapping.EditorStateName == stateName)
                    {
                        value = valueMapping.EditorValue;
                        break;
                    }
                }
                m_EditorAlignedValueMappings.Add(new StateValueMapping<T> { EditorStateName = stateName, EditorValue = value });
            }
            EditorValueMappings.Clear();
            EditorValueMappings.AddRange(m_EditorAlignedValueMappings);
            m_EditorAlignedValueMappings.Clear();
        }

        private readonly List<string> m_EmptyStringList = new List<string>();
        private List<string> EditorGetGroupNames()
        {
            var controller = EditorController;
            if (controller != null)
            {
                return controller.EditorGetAllGroupNames();
            }
            return m_EmptyStringList;
        }

        private bool EditorHasSelectedGroup()
        {
            return EditorGroup != null;
        }

        private void EditorOnSelectedGroup()
        {
            EditorRefresh();
        }

        private void EditorApplyGroupCurrentState()
        {
            var group = EditorGroup;
            if (group != null)
            {
                group.EditorApplyCurrentState();
            }
        }

        private void EditorOnStateValueBeginGUI(int selectionIndex)
        {
            var controller = EditorController;
            var group = EditorGroup;
            var valueMappings = EditorValueMappings;
            if (controller == null || group == null || selectionIndex >= valueMappings.Count)
            {
                return;
            }
            GUILayout.BeginHorizontal();
            GUI.enabled = false;
            var currentStateName = valueMappings[selectionIndex].EditorStateName;
            var nameColor = GUI.color;
            GUI.color = new Color(1f, 0.82f, 0.45f);
            if (NameWidth > 0)
            {
                EditorGUILayout.TextField(currentStateName, GUILayout.Width(NameWidth));
            }
            else
            {
                EditorGUILayout.TextField(currentStateName);
            }
            GUI.color = nameColor;
            GUI.enabled = true;
        }

        private void EditorOnStateValueEndGUI(int selectionIndex)
        {
            var controller = EditorController;
            var group = EditorGroup;
            var valueMappings = EditorValueMappings;
            if (controller == null || group == null || selectionIndex >= valueMappings.Count)
            {
                return;
            }
            GUI.enabled = true;
            var color = GUI.color;
            var currentStateName = valueMappings[selectionIndex].EditorStateName;
            if (group.EditorCurrentStateName == currentStateName)
            {
                GUI.color = new Color(0,1,0);
            }
            if (GUILayout.Button("Apply", GUILayout.ExpandWidth(false)))
            {
                controller.EditorRefresh();
                group.EditorCurrentStateName = currentStateName;
            }
            GUI.color = color;
            GUILayout.EndHorizontal();
        }
    }
}
#endif
