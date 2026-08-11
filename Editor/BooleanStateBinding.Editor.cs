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
        [BoxGroup("Primary Group")]
        [LabelText("Group Name")]
        [GUIColor(0.6f, 0.8f, 1f)]
        [PropertyOrder(10)]
        [ValueDropdown(nameof(EditorGetPrimaryGroupNames))]
        [OnValueChanged(nameof(EditorOnSelectedPrimaryGroup))]
        private string EditorPrimaryGroupName
        {
            get => m_PrimaryGroupName;
            set => m_PrimaryGroupName = value;
        }

        [ShowInInspector]
        [BoxGroup("Primary Group")]
        [LabelText("State Values")]
        [PropertyOrder(11)]
        [ShowIf(nameof(EditorHasSelectedPrimaryGroup))]
        [HideReferenceObjectPicker]
        [ListDrawerSettings(DefaultExpandedState = true,
            HideAddButton = true,
            HideRemoveButton = true,
            DraggableItems = false,
            OnBeginListElementGUI = nameof(EditorOnPrimaryStateValueBeginGUI),
            OnEndListElementGUI = nameof(EditorOnPrimaryStateValueEndGUI))]
        [OnValueChanged(nameof(EditorApplyGroupCurrentState), true)]
        private List<StateValueMapping<bool>> EditorPrimaryValueMappings
        {
            set => m_PrimaryValueMappings = value;
            get => m_PrimaryValueMappings;
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
        [BoxGroup("Secondary Group")]
        [LabelText("Group Name")]
        [GUIColor(0.6f, 0.8f, 1f)]
        [PropertyOrder(30)]
        [EnableIf(nameof(EditorCanShowSecondaryGroup))]
        [ValueDropdown(nameof(EditorGetSecondaryGroupNames))]
        [OnValueChanged(nameof(EditorOnSelectedSecondaryGroup))]
        private string EditorSecondaryGroupName
        {
            get => m_SecondaryGroupName;
            set => m_SecondaryGroupName = value;
        }

        [ShowInInspector]
        [BoxGroup("Secondary Group")]
        [LabelText("State Values")]
        [PropertyOrder(31)]
        [EnableIf(nameof(EditorCanShowSecondaryGroup))]
        [ShowIf(nameof(EditorHasSelectedSecondaryGroup))]
        [HideReferenceObjectPicker]
        [ListDrawerSettings(DefaultExpandedState = true,
            HideAddButton = true,
            HideRemoveButton = true,
            DraggableItems = false,
            OnBeginListElementGUI = nameof(EditorOnSecondaryStateValueBeginGUI),
            OnEndListElementGUI = nameof(EditorOnSecondaryStateValueEndGUI))]
        [OnValueChanged(nameof(EditorApplyGroupCurrentState), true)]
        private List<StateValueMapping<bool>> EditorSecondaryValueMappings
        {
            set => m_SecondaryValueMappings = value;
            get => m_SecondaryValueMappings;
        }

        private StateGroup EditorPrimaryGroup
        {
            get
            {
                var controller = EditorController;
                return controller == null ? null : controller.GetGroup(EditorPrimaryGroupName);
            }
        }

        private StateGroup EditorSecondaryGroup
        {
            get
            {
                var controller = EditorController;
                return controller == null ? null : controller.GetGroup(EditorSecondaryGroupName);
            }
        }

        internal override void EditorRefresh()
        {
            EditorRefreshPrimaryGroup();
            EditorRefreshSecondaryGroup();
        }

        internal override void EditorApplyCurrentState()
        {
            InitializeTarget();
            if (EditorBooleanMode == BooleanMode.Single)
            {
                var primaryGroup = EditorPrimaryGroup;
                if (primaryGroup == null || string.IsNullOrEmpty(primaryGroup.EditorCurrentStateName))
                    return;
                foreach (var valueMapping in EditorPrimaryValueMappings)
                {
                    if (valueMapping.EditorStateName == primaryGroup.EditorCurrentStateName)
                    {
                        ApplyValue(valueMapping.EditorValue);
                        break;
                    }
                }
            }
            else
            {
                var primaryGroup = EditorPrimaryGroup;
                if (primaryGroup == null || string.IsNullOrEmpty(primaryGroup.EditorCurrentStateName))
                    return;
                var secondaryGroup = EditorSecondaryGroup;
                if (secondaryGroup == null || string.IsNullOrEmpty(secondaryGroup.EditorCurrentStateName))
                    return;
                bool primaryValue = false;
                foreach (var valueMapping in EditorPrimaryValueMappings)
                {
                    if (valueMapping.EditorStateName == primaryGroup.EditorCurrentStateName)
                    {
                        primaryValue = valueMapping.EditorValue;
                        break;
                    }
                }
                bool secondaryValue = false;
                foreach (var valueMapping in EditorSecondaryValueMappings)
                {
                    if (valueMapping.EditorStateName == secondaryGroup.EditorCurrentStateName)
                    {
                        secondaryValue = valueMapping.EditorValue;
                        break;
                    }
                }
                bool result = false;
                switch (EditorBooleanMode)
                {
                    case BooleanMode.And:
                        result = primaryValue && secondaryValue;
                        break;
                    case BooleanMode.Or:
                        result = primaryValue || secondaryValue;
                        break;
                }
                ApplyValue(result);
            }
        }

        internal override void EditorOnGroupRename(string oldGroupName, string newGroupName)
        {
            if (EditorPrimaryGroupName == oldGroupName)
            {
                EditorPrimaryGroupName = newGroupName;
            }
            else if (EditorSecondaryGroupName == oldGroupName)
            {
                EditorSecondaryGroupName = newGroupName;
            }
        }

        internal override void EditorOnGroupStateRename(string groupName, string oldStateName, string newStateName)
        {
            if (EditorPrimaryGroupName == groupName)
            {
                EditorRenameStateMapping(EditorPrimaryValueMappings, oldStateName, newStateName);
            }
            if (EditorSecondaryGroupName == groupName)
            {
                EditorRenameStateMapping(EditorSecondaryValueMappings, oldStateName, newStateName);
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
            if (EditorPrimaryGroupName == group.EditorName || (EditorBooleanMode != BooleanMode.Single && EditorSecondaryGroupName == group.EditorName))
            {
                return true;
            }
            return false;
        }

        private bool EditorHasSelectedPrimaryGroup()
        {
            if (EditorController == null)
                return false;
            return EditorPrimaryGroup != null;
        }

        private bool EditorHasSelectedSecondaryGroup()
        {
            if (EditorController == null)
                return false;
            return EditorSecondaryGroup != null && EditorBooleanMode != BooleanMode.Single;
        }

        private bool EditorCanShowSecondaryGroup()
        {
            return EditorBooleanMode != BooleanMode.Single;
        }

        private readonly List<string> m_EditorEmptyStringList = new List<string>();
        private List<string> EditorGetPrimaryGroupNames()
        {
            var controller = EditorController;
            if (controller == null)
                return m_EditorEmptyStringList;
            return controller.EditorGetAllGroupNames();
        }
        
        private List<string> EditorGetSecondaryGroupNames()
        {
            var controller = EditorController;
            if (controller == null)
                return m_EditorEmptyStringList;
            var names = controller.EditorGetAllGroupNames();
            names.Remove(EditorPrimaryGroupName);
            return names;
        }

        private void EditorRefreshPrimaryGroup()
        {
            var primaryGroup = EditorPrimaryGroup;
            if (primaryGroup == null)
            {
                EditorPrimaryGroupName = string.Empty;
                EditorPrimaryValueMappings.Clear();
                return;
            }
            EditorAlignOne(primaryGroup, EditorPrimaryValueMappings);
        }

        private void EditorRefreshSecondaryGroup()
        {
            var secondaryGroup = EditorSecondaryGroup;
            if (secondaryGroup == null)
            {
                EditorSecondaryGroupName = string.Empty;
                EditorSecondaryValueMappings.Clear();
                return;
            }
            EditorAlignOne(secondaryGroup, EditorSecondaryValueMappings);
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

        private void EditorOnSelectedPrimaryGroup()
        {
            if (EditorPrimaryGroupName == EditorSecondaryGroupName)
            {
                EditorSecondaryGroupName = string.Empty;
                EditorRefreshSecondaryGroup();
            }
            EditorRefreshPrimaryGroup();
            EditorApplyGroupCurrentState();
        }

        private void EditorOnSelectedSecondaryGroup()
        {
            var controller = EditorController;
            if (controller == null)
                return;
            foreach (var groupName in controller.EditorGetAllGroupNames())
            {
                if (groupName != EditorPrimaryGroupName && groupName == EditorSecondaryGroupName)
                {
                    EditorRefreshSecondaryGroup();
                    break;
                }
            }
            EditorApplyGroupCurrentState();
        }

        private void EditorApplyGroupCurrentState()
        {
            var primaryGroup = EditorPrimaryGroup;
            if (primaryGroup != null)
            {
                primaryGroup.EditorApplyCurrentState();
            }
            var secondaryGroup = EditorSecondaryGroup;
            if (secondaryGroup != null)
            {
                secondaryGroup.EditorApplyCurrentState();
            }
        }
        
        private void EditorOnPrimaryStateValueBeginGUI(int selectionIndex)
        {
            var controller = EditorController;
            var stateValues = EditorPrimaryValueMappings;
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

        private void EditorOnPrimaryStateValueEndGUI(int selectionIndex)
        {
            var color = GUI.color;
            var controller = EditorController;
            var stateValues = EditorPrimaryValueMappings;
            if (controller == null || stateValues == null || selectionIndex >= stateValues.Count)
                return;
            GUI.enabled = true;
            var group = EditorPrimaryGroup;
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

        private void EditorOnSecondaryStateValueBeginGUI(int selectionIndex)
        {
            var controller = EditorController;
            var stateValues = EditorSecondaryValueMappings;
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

        private void EditorOnSecondaryStateValueEndGUI(int selectionIndex)
        {
            var color = GUI.color;
            var controller = EditorController;
            var stateValues = EditorSecondaryValueMappings;
            if (controller == null || stateValues == null || selectionIndex >= stateValues.Count)
                return;
            GUI.enabled = true;
            var group = EditorSecondaryGroup;
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