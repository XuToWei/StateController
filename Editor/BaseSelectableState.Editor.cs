#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace StateController
{
    public partial class BaseSelectableState<T>
    {
        private bool? m_DrawSettingLineBreak;
        private bool DrawSettingLineBreak
        {
            get
            {
                if (m_DrawSettingLineBreak.HasValue)
                {
                    return m_DrawSettingLineBreak.Value;
                }
                m_DrawSettingLineBreak = false;
                var objects = GetType().GetCustomAttributes(typeof(SelectableStateDrawSettingAttribute), true);
                if (objects.Length > 0)
                {
                    var drawSetting = (SelectableStateDrawSettingAttribute)objects[0];
                    m_DrawSettingLineBreak = drawSetting.LineBreak;
                }
                return m_DrawSettingLineBreak.Value;
            }
        }

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
                var objects = GetType().GetCustomAttributes(typeof(SelectableStateNameWidthAttribute), true);
                if (objects.Length > 0)
                {
                    var nameWidth = (SelectableStateNameWidthAttribute)objects[0];
                    m_NameWidth = nameWidth.Width;
                }
                return m_NameWidth.Value;
            }
        }
        
        [ShowInInspector]
        [BoxGroup("Data")]
        [LabelText("Data Name")]
        [GUIColor(0.6f, 0.8f, 1f)]
        [PropertyOrder(10)]
        [ValueDropdown(nameof(EditorGetDataNames))]
        [OnValueChanged(nameof(EditorOnSelectedData))]
        private string EditorDataName
        {
            get => m_DataName;
            set => m_DataName = value;
        }

        [HideReferenceObjectPicker]
        [ShowInInspector]
        [BoxGroup("Data")]
        [LabelText("State Values")]
        [GUIColor(1f, 0.82f, 0.45f)]
        [PropertyOrder(11)]
        [ShowIf(nameof(EditorIsSelectedData))]
        [ListDrawerSettings(DefaultExpandedState = true,
            HideAddButton = true, HideRemoveButton = true,
            DraggableItems = false,
            OnBeginListElementGUI = nameof(EditorOnStateDataBeginGUI),
            OnEndListElementGUI = nameof(EditorOnStateDataEndGUI))]
        [OnValueChanged(nameof(EditorRefreshSelectedName), true)]
        private List<StateValue<T>> EditorStateValues
        {
            set => m_StateValues = value;
            get => m_StateValues;
        }

        private StateControllerData EditorData
        {
            get
            {
                var controller = EditorControllerMono;
                return controller == null ? null : controller.EditorGetData(EditorDataName);
            }
        }

        internal override void EditorOnRefresh()
        {
            OnStateInit();
            var data = EditorData;
            if (data == null || string.IsNullOrEmpty(data.EditorSelectedName))
                return;
            foreach (var stateValue in EditorStateValues)
            {
                if (stateValue.EditorStateName == data.EditorSelectedName)
                {
                    OnStateChanged(stateValue.EditorValue);
                    break;
                }
            }
        }

        internal override void EditorOnDataRename(string oldDataName, string newDataName)
        {
            if (EditorDataName == oldDataName)
            {
                EditorDataName = newDataName;
            }
        }

        internal override void EditorOnDataStateRename(string dataName, string oldStateName, string newStateName)
        {
            if (EditorDataName != dataName)
                return;
            foreach (var stateData in EditorStateValues)
            {
                if (stateData.EditorStateName == oldStateName)
                {
                    stateData.EditorStateName = newStateName;
                    break;
                }
            }
        }

        internal override bool EditorCheckIsConnection(StateControllerData data)
        {
            return EditorDataName == data.EditorName;
        }

        // 按名字对齐重建本组件存的（状态名,值），处理控制器状态的新增/删除/重排
        private readonly List<StateValue<T>> m_EditorTempDatas = new List<StateValue<T>>();
        internal override void EditorRefresh()
        {
            var data = EditorData;
            if (data == null)
            {
                EditorStateValues.Clear();
                return;
            }
            var states = data.EditorStates;
            if (EditorStateValues.Count == states.Count)
            {
                bool match = true;
                for (int i = 0; i < states.Count; i++)
                {
                    if (EditorStateValues[i].EditorStateName != states[i].EditorName)
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                    return;
            }
            bool canCreate = typeof(T).GetConstructor(Type.EmptyTypes) != null;
            m_EditorTempDatas.Clear();
            for (int i = 0; i < states.Count; i++)
            {
                var stateName = states[i].EditorName;
                T value = canCreate ? Activator.CreateInstance<T>() : default;
                foreach (var stateValue in EditorStateValues)
                {
                    if (stateValue.EditorStateName == stateName)
                    {
                        value = stateValue.EditorValue;
                        break;
                    }
                }
                m_EditorTempDatas.Add(new StateValue<T> { EditorStateName = stateName, EditorValue = value });
            }
            EditorStateValues.Clear();
            EditorStateValues.AddRange(m_EditorTempDatas);
            m_EditorTempDatas.Clear();
        }

        private readonly List<string> m_EmptyListString = new List<string>();
        private List<string> EditorGetDataNames()
        {
            var controller = EditorControllerMono;
            if (controller != null)
            {
                return controller.EditorGetAllDataNames();
            }
            return m_EmptyListString;
        }

        private bool EditorIsSelectedData()
        {
            return EditorData != null;
        }

        private void EditorOnSelectedData()
        {
            EditorRefresh();
        }
        
        private void EditorRefreshSelectedName()
        {
            var data = EditorData;
            if (data != null)
            {
                data.EditorRefreshSelectedName();
            }
        }

        private void EditorOnStateDataBeginGUI(int selectionIndex)
        {
            GUILayout.BeginHorizontal();
            if (DrawSettingLineBreak)
            {
                EditorOnStateDataTitleGUI(selectionIndex);
            }
        }

        private void EditorOnStateDataEndGUI(int selectionIndex)
        {
            if (!DrawSettingLineBreak)
            {
                EditorOnStateDataTitleGUI(selectionIndex);
            }
        }

        private void EditorOnStateDataTitleGUI(int selectionIndex)
        {
            var controller = EditorControllerMono;
            var data = EditorData;
            if (controller == null || data == null || selectionIndex >= EditorStateValues.Count)
            {
                GUILayout.EndHorizontal();
                return;
            }
            GUI.enabled = false;
            var curStateName = EditorStateValues[selectionIndex].EditorStateName;
            var nameColor = GUI.color;
            GUI.color = new Color(1f, 0.82f, 0.45f);
            if (NameWidth > 0)
            {
                EditorGUILayout.TextField(curStateName, GUILayout.Width(NameWidth));
            }
            else
            {
                EditorGUILayout.TextField(curStateName);
            }
            GUI.color = nameColor;
            GUI.enabled = true;
            var color = GUI.color;
            if (data.EditorSelectedName == curStateName)
            {
                GUI.color = new Color(0,1,0);
            }
            if (GUILayout.Button("Apply"))
            {
                controller.EditorRefresh();
                data.EditorSelectedName = curStateName;
            }
            GUI.color = color;
            GUILayout.EndHorizontal();
        }
    }
}
#endif