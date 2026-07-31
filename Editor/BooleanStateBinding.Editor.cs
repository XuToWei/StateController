#if UNITY_EDITOR
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace StateController
{
    public partial class BooleanStateBinding
    {
        [ShowInInspector]
        [BoxGroup("Group1")]
        [LabelText("Group Name")]
        [GUIColor(0.6f, 0.8f, 1f)]
        [PropertyOrder(10)]
        [ValueDropdown(nameof(EditorGetGroupNames1))]
        [OnValueChanged(nameof(EditorOnSelectedGroup1))]
        private string EditorGroupName1
        {
            get => m_GroupName1;
            set => m_GroupName1 = value;
        }

        [ShowInInspector]
        [BoxGroup("Group1")]
        [LabelText("State Values")]
        [PropertyOrder(11)]
        [ShowIf(nameof(EditorHasSelectedGroup1))]
        [HideReferenceObjectPicker]
        [ListDrawerSettings(DefaultExpandedState = true,
            HideAddButton = true,
            HideRemoveButton = true,
            DraggableItems = false,
            OnBeginListElementGUI = nameof(EditorOnStateValueBeginGUI1),
            OnEndListElementGUI = nameof(EditorOnStateValueEndGUI1))]
        [OnValueChanged(nameof(EditorApplyGroupCurrentState), true)]
        private List<StateValueMapping<bool>> EditorValueMappings1
        {
            set => m_ValueMappings1 = value;
            get => m_ValueMappings1;
        }

        [ShowInInspector]
        [PropertyOrder(20)]
        [OnValueChanged(nameof(EditorApplyGroupCurrentState))]
        public BooleanMode EditorBooleanMode
        {
            get => m_BooleanMode;
            set => m_BooleanMode = value;
        }

        [ShowInInspector]
        [BoxGroup("Group2")]
        [LabelText("Group Name")]
        [GUIColor(0.6f, 0.8f, 1f)]
        [PropertyOrder(30)]
        [EnableIf(nameof(EditorCanShowGroup2))]
        [ValueDropdown(nameof(EditorGetGroupNames2))]
        [OnValueChanged(nameof(EditorOnSelectedGroup2))]
        private string EditorGroupName2
        {
            get => m_GroupName2;
            set => m_GroupName2 = value;
        }

        [ShowInInspector]
        [BoxGroup("Group2")]
        [LabelText("State Values")]
        [PropertyOrder(31)]
        [EnableIf(nameof(EditorCanShowGroup2))]
        [ShowIf(nameof(EditorHasSelectedGroup2))]
        [HideReferenceObjectPicker]
        [ListDrawerSettings(DefaultExpandedState = true,
            HideAddButton = true,
            HideRemoveButton = true,
            DraggableItems = false,
            OnBeginListElementGUI = nameof(EditorOnStateValueBeginGUI2),
            OnEndListElementGUI = nameof(EditorOnStateValueEndGUI2))]
        [OnValueChanged(nameof(EditorApplyGroupCurrentState), true)]
        private List<StateValueMapping<bool>> EditorValueMappings2
        {
            set => m_ValueMappings2 = value;
            get => m_ValueMappings2;
        }

        private StateGroup EditorGroup1
        {
            get
            {
                var controller = EditorController;
                return controller == null ? null : controller.GetGroup(EditorGroupName1);
            }
        }

        private StateGroup EditorGroup2
        {
            get
            {
                var controller = EditorController;
                return controller == null ? null : controller.GetGroup(EditorGroupName2);
            }
        }

        internal override void EditorRefresh()
        {
            EditorRefreshGroup1();
            EditorRefreshGroup2();
        }

        internal override void EditorApplyCurrentState()
        {
            InitializeTarget();
            if (EditorBooleanMode == BooleanMode.Single)
            {
                var group1 = EditorGroup1;
                if (group1 == null || string.IsNullOrEmpty(group1.EditorCurrentStateName))
                    return;
                foreach (var valueMapping in EditorValueMappings1)
                {
                    if (valueMapping.EditorStateName == group1.EditorCurrentStateName)
                    {
                        ApplyValue(valueMapping.EditorValue);
                        break;
                    }
                }
            }
            else
            {
                var group1 = EditorGroup1;
                if (group1 == null || string.IsNullOrEmpty(group1.EditorCurrentStateName))
                    return;
                var group2 = EditorGroup2;
                if (group2 == null || string.IsNullOrEmpty(group2.EditorCurrentStateName))
                    return;
                bool value1 = false;
                foreach (var valueMapping in EditorValueMappings1)
                {
                    if (valueMapping.EditorStateName == group1.EditorCurrentStateName)
                    {
                        value1 = valueMapping.EditorValue;
                        break;
                    }
                }
                bool value2 = false;
                foreach (var valueMapping in EditorValueMappings2)
                {
                    if (valueMapping.EditorStateName == group2.EditorCurrentStateName)
                    {
                        value2 = valueMapping.EditorValue;
                        break;
                    }
                }
                bool result = false;
                switch (EditorBooleanMode)
                {
                    case BooleanMode.And:
                        result = value1 && value2;
                        break;
                    case BooleanMode.Or:
                        result = value1 || value2;
                        break;
                }
                ApplyValue(result);
            }
        }

        internal override void EditorOnGroupRename(string oldGroupName, string newGroupName)
        {
            if (EditorGroupName1 == oldGroupName)
            {
                EditorGroupName1 = newGroupName;
            }
            else if (EditorGroupName2 == oldGroupName)
            {
                EditorGroupName2 = newGroupName;
            }
        }

        internal override void EditorOnGroupStateRename(string groupName, string oldStateName, string newStateName)
        {
            if (EditorGroupName1 == groupName)
            {
                EditorRenameStateMapping(EditorValueMappings1, oldStateName, newStateName);
            }
            if (EditorGroupName2 == groupName)
            {
                EditorRenameStateMapping(EditorValueMappings2, oldStateName, newStateName);
            }
        }

        private static void EditorRenameStateMapping(List<StateValueMapping<bool>> valueMappings, string oldStateName, string newStateName)
        {
            foreach (var valueMapping in valueMappings)
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
            if (EditorGroupName1 == group.EditorName || (EditorBooleanMode != BooleanMode.Single && EditorGroupName2 == group.EditorName))
            {
                return true;
            }
            return false;
        }

        private bool EditorHasSelectedGroup1()
        {
            if (EditorController == null)
                return false;
            return EditorGroup1 != null;
        }

        private bool EditorHasSelectedGroup2()
        {
            if (EditorController == null)
                return false;
            return EditorGroup2 != null && EditorBooleanMode != BooleanMode.Single;
        }

        private bool EditorCanShowGroup2()
        {
            return EditorBooleanMode != BooleanMode.Single;
        }

        private readonly List<string> m_EditorEmptyStringList = new List<string>();
        private List<string> EditorGetGroupNames1()
        {
            var controller = EditorController;
            if (controller == null)
                return m_EditorEmptyStringList;
            return controller.EditorGetAllGroupNames();
        }
        
        private List<string> EditorGetGroupNames2()
        {
            var controller = EditorController;
            if (controller == null)
                return m_EditorEmptyStringList;
            var names = controller.EditorGetAllGroupNames();
            names.Remove(EditorGroupName1);
            return names;
        }

        private void EditorRefreshGroup1()
        {
            var group1 = EditorGroup1;
            if (group1 == null)
            {
                EditorGroupName1 = string.Empty;
                EditorValueMappings1.Clear();
                return;
            }
            EditorAlignOne(group1, EditorValueMappings1);
        }

        private void EditorRefreshGroup2()
        {
            var group2 = EditorGroup2;
            if (group2 == null)
            {
                EditorGroupName2 = string.Empty;
                EditorValueMappings2.Clear();
                return;
            }
            EditorAlignOne(group2, EditorValueMappings2);
        }

        // 按 Group 的当前状态顺序重建每条（状态名, 值）映射，按名字保留已有值、缺补默认、多删
        private readonly List<StateValueMapping<bool>> m_EditorAlignedValueMappings = new List<StateValueMapping<bool>>();
        private void EditorAlignOne(StateGroup group, List<StateValueMapping<bool>> valueMappings)
        {
            var states = group.EditorStates;
            if (valueMappings.Count == states.Count)
            {
                bool match = true;
                for (int i = 0; i < states.Count; i++)
                {
                    if (valueMappings[i].EditorStateName != states[i].EditorName)
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                    return;
            }
            m_EditorAlignedValueMappings.Clear();
            for (int i = 0; i < states.Count; i++)
            {
                var stateName = states[i].EditorName;
                bool value = false;
                foreach (var valueMapping in valueMappings)
                {
                    if (valueMapping.EditorStateName == stateName)
                    {
                        value = valueMapping.EditorValue;
                        break;
                    }
                }
                m_EditorAlignedValueMappings.Add(new StateValueMapping<bool> { EditorStateName = stateName, EditorValue = value });
            }
            valueMappings.Clear();
            valueMappings.AddRange(m_EditorAlignedValueMappings);
            m_EditorAlignedValueMappings.Clear();
        }

        private void EditorOnSelectedGroup1()
        {
            if (EditorGroupName1 == EditorGroupName2)
            {
                EditorGroupName2 = string.Empty;
                EditorRefreshGroup2();
            }
            EditorRefreshGroup1();
            EditorApplyGroupCurrentState();
        }

        private void EditorOnSelectedGroup2()
        {
            var controller = EditorController;
            if (controller == null)
                return;
            foreach (var groupName in controller.EditorGetAllGroupNames())
            {
                if (groupName != EditorGroupName1 && groupName == EditorGroupName2)
                {
                    EditorRefreshGroup2();
                    break;
                }
            }
            EditorApplyGroupCurrentState();
        }

        private void EditorApplyGroupCurrentState()
        {
            var group1 = EditorGroup1;
            if (group1 != null)
            {
                group1.EditorApplyCurrentState();
            }
            var group2 = EditorGroup2;
            if (group2 != null)
            {
                group2.EditorApplyCurrentState();
            }
        }
        
        private void EditorOnStateValueBeginGUI1(int selectionIndex)
        {
            var controller = EditorController;
            var stateValues = EditorValueMappings1;
            if (controller == null || stateValues == null || selectionIndex >= stateValues.Count)
                return;
            GUILayout.BeginHorizontal();
            GUI.enabled = false;
            var currentStateName = stateValues[selectionIndex].EditorStateName;
            var nameColor = GUI.color;
            GUI.color = new Color(1f, 0.82f, 0.45f);
            EditorGUILayout.TextField(currentStateName);
            GUI.color = nameColor;
            GUI.enabled = true;
        }

        private void EditorOnStateValueEndGUI1(int selectionIndex)
        {
            var color = GUI.color;
            var controller = EditorController;
            var stateValues = EditorValueMappings1;
            if (controller == null || stateValues == null || selectionIndex >= stateValues.Count)
                return;
            GUI.enabled = true;
            var group = EditorGroup1;
            var currentStateName = stateValues[selectionIndex].EditorStateName;
            if (group.EditorCurrentStateName == currentStateName)
            {
                GUI.color = new Color(0, 1, 0);
            }
            if (GUILayout.Button("Apply", GUILayout.ExpandWidth(false)))
            {
                controller.EditorRefresh();
                group.EditorCurrentStateName = currentStateName;
            }
            GUI.color = color;
            GUILayout.EndHorizontal();
        }

        private void EditorOnStateValueBeginGUI2(int selectionIndex)
        {
            var controller = EditorController;
            var stateValues = EditorValueMappings2;
            if (controller == null || stateValues == null || selectionIndex >= stateValues.Count)
                return;
            GUILayout.BeginHorizontal();
            GUI.enabled = false;
            var currentStateName = stateValues[selectionIndex].EditorStateName;
            var nameColor = GUI.color;
            GUI.color = new Color(1f, 0.82f, 0.45f);
            EditorGUILayout.TextField(currentStateName);
            GUI.color = nameColor;
            GUI.enabled = true;
        }

        private void EditorOnStateValueEndGUI2(int selectionIndex)
        {
            var color = GUI.color;
            var controller = EditorController;
            var stateValues = EditorValueMappings2;
            if (controller == null || stateValues == null || selectionIndex >= stateValues.Count)
                return;
            GUI.enabled = true;
            var group = EditorGroup2;
            var currentStateName = stateValues[selectionIndex].EditorStateName;
            if (group.EditorCurrentStateName == currentStateName)
            {
                GUI.color = new Color(0, 1, 0);
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