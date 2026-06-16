#if UNITY_EDITOR
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace StateController
{
    public partial class BaseBooleanLogicState
    {
        [ShowInInspector]
        [BoxGroup("Data1")]
        [LabelText("Data Name")]
        [GUIColor(0.6f, 0.8f, 1f)]
        [PropertyOrder(10)]
        [ValueDropdown(nameof(EditorGetDataNames1))]
        [OnValueChanged(nameof(EditorOnSelectedData1))]
        private string EditorDataName1
        {
            get => m_DataName1;
            set => m_DataName1 = value;
        }

        [ShowInInspector]
        [BoxGroup("Data1")]
        [LabelText("State Values")]
        [PropertyOrder(11)]
        [ShowIf(nameof(EditorIsSelectedData1))]
        [HideReferenceObjectPicker]
        [ListDrawerSettings(DefaultExpandedState = true,
            HideAddButton = true,
            HideRemoveButton = true,
            DraggableItems = false,
            OnBeginListElementGUI = nameof(EditorOnStateDataBeginGUI1),
            OnEndListElementGUI = nameof(EditorOnStateDataEndGUI1))]
        [OnValueChanged(nameof(EditorRefreshSelectedName), true)]
        private List<StateValue<bool>> EditorStateValues1
        {
            set => m_StateValues1 = value;
            get => m_StateValues1;
        }

        [ShowInInspector]
        [PropertyOrder(20)]
        [OnValueChanged(nameof(EditorRefreshSelectedName))]
        public BooleanLogicType EditorBooleanLogicType
        {
            get => m_BooleanLogicType;
            set => m_BooleanLogicType = value;
        }

        [ShowInInspector]
        [BoxGroup("Data2")]
        [LabelText("Data Name")]
        [GUIColor(0.6f, 0.8f, 1f)]
        [PropertyOrder(30)]
        [EnableIf(nameof(EditorCanShowData2))]
        [ValueDropdown(nameof(EditorGetDataNames2))]
        [OnValueChanged(nameof(EditorOnSelectedData2))]
        private string EditorDataName2
        {
            get => m_DataName2;
            set => m_DataName2 = value;
        }

        [ShowInInspector]
        [BoxGroup("Data2")]
        [LabelText("State Values")]
        [PropertyOrder(31)]
        [EnableIf(nameof(EditorCanShowData2))]
        [ShowIf(nameof(EditorIsSelectedData2))]
        [HideReferenceObjectPicker]
        [ListDrawerSettings(DefaultExpandedState = true,
            HideAddButton = true,
            HideRemoveButton = true,
            DraggableItems = false,
            OnBeginListElementGUI = nameof(EditorOnStateDataBeginGUI2),
            OnEndListElementGUI = nameof(EditorOnStateDataEndGUI2))]
        [OnValueChanged(nameof(EditorRefreshSelectedName), true)]
        private List<StateValue<bool>> EditorStateValues2
        {
            set => m_StateValues2 = value;
            get => m_StateValues2;
        }

        private StateControllerData EditorData1
        {
            get
            {
                var controller = EditorControllerMono;
                return controller == null ? null : controller.GetData(EditorDataName1);
            }
        }

        private StateControllerData EditorData2
        {
            get
            {
                var controller = EditorControllerMono;
                return controller == null ? null : controller.GetData(EditorDataName2);
            }
        }

        internal override void EditorRefresh()
        {
            EditorRefreshData1();
            EditorRefreshData2();
        }

        internal override void EditorOnRefresh()
        {
            OnStateInit();
            if (EditorBooleanLogicType == BooleanLogicType.None)
            {
                var data1 = EditorData1;
                if (data1 == null || string.IsNullOrEmpty(data1.EditorSelectedName))
                    return;
                foreach (var stateValue in EditorStateValues1)
                {
                    if (stateValue.EditorStateName == data1.EditorSelectedName)
                    {
                        OnStateChanged(stateValue.EditorValue);
                        break;
                    }
                }
            }
            else
            {
                var data1 = EditorData1;
                if (data1 == null || string.IsNullOrEmpty(data1.EditorSelectedName))
                    return;
                var data2 = EditorData2;
                if (data2 == null || string.IsNullOrEmpty(data2.EditorSelectedName))
                    return;
                bool value1 = false;
                foreach (var stateValue in EditorStateValues1)
                {
                    if (stateValue.EditorStateName == data1.EditorSelectedName)
                    {
                        value1 = stateValue.EditorValue;
                        break;
                    }
                }
                bool value2 = false;
                foreach (var stateValue in EditorStateValues2)
                {
                    if (stateValue.EditorStateName == data2.EditorSelectedName)
                    {
                        value2 = stateValue.EditorValue;
                        break;
                    }
                }
                bool logicResult = false;
                switch (EditorBooleanLogicType)
                {
                    case BooleanLogicType.And:
                        logicResult = value1 && value2;
                        break;
                    case BooleanLogicType.Or:
                        logicResult = value1 || value2;
                        break;
                }
                OnStateChanged(logicResult);
            }
        }

        internal override void EditorOnDataRename(string oldDataName, string newDataName)
        {
            if (EditorDataName1 == oldDataName)
            {
                EditorDataName1 = newDataName;
            }
            else if (EditorDataName2 == oldDataName)
            {
                EditorDataName2 = newDataName;
            }
        }

        internal override void EditorOnDataStateRename(string dataName, string oldStateName, string newStateName)
        {
            if (EditorDataName1 == dataName)
            {
                EditorRenameStateData(EditorStateValues1, oldStateName, newStateName);
            }
            if (EditorDataName2 == dataName)
            {
                EditorRenameStateData(EditorStateValues2, oldStateName, newStateName);
            }
        }

        private static void EditorRenameStateData(List<StateValue<bool>> datas, string oldStateName, string newStateName)
        {
            foreach (var data in datas)
            {
                if (data.EditorStateName == oldStateName)
                {
                    data.EditorStateName = newStateName;
                    break;
                }
            }
        }

        internal override bool EditorCheckIsConnection(StateControllerData data)
        {
            if (EditorDataName1 == data.EditorName || (EditorBooleanLogicType != BooleanLogicType.None && EditorDataName2 == data.EditorName))
            {
                return true;
            }
            return false;
        }

        private bool EditorIsSelectedData1()
        {
            if (EditorControllerMono == null)
                return false;
            return EditorData1 != null;
        }

        private bool EditorIsSelectedData2()
        {
            if (EditorControllerMono == null)
                return false;
            return EditorData2 != null && EditorBooleanLogicType != BooleanLogicType.None;
        }

        private bool EditorCanShowData2()
        {
            return EditorBooleanLogicType != BooleanLogicType.None;
        }

        private readonly List<string> m_EditorEmptyListString = new List<string>();
        private List<string> EditorGetDataNames1()
        {
            var controller = EditorControllerMono;
            if (controller == null)
                return m_EditorEmptyListString;
            return controller.EditorGetAllDataNames();
        }
        
        private List<string> EditorGetDataNames2()
        {
            var controller = EditorControllerMono;
            if (controller == null)
                return m_EditorEmptyListString;
            var names = controller.EditorGetAllDataNames();
            names.Remove(EditorDataName1);
            return names;
        }

        private void EditorRefreshData1()
        {
            var data1 = EditorData1;
            if (data1 == null)
            {
                EditorDataName1 = string.Empty;
                EditorStateValues1.Clear();
                return;
            }
            EditorAlignOne(data1, EditorStateValues1);
        }

        private void EditorRefreshData2()
        {
            var data2 = EditorData2;
            if (data2 == null)
            {
                EditorDataName2 = string.Empty;
                EditorStateValues2.Clear();
                return;
            }
            EditorAlignOne(data2, EditorStateValues2);
        }

        // 把每条（名字,值）数据按 data 的当前状态顺序重建，按名字保留已有值、缺补默认、多删
        private readonly List<StateValue<bool>> m_EditorTempDatas = new List<StateValue<bool>>();
        private void EditorAlignOne(StateControllerData data, List<StateValue<bool>> datas)
        {
            var states = data.EditorStates;
            if (datas.Count == states.Count)
            {
                bool match = true;
                for (int i = 0; i < states.Count; i++)
                {
                    if (datas[i].EditorStateName != states[i].EditorName)
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                    return;
            }
            m_EditorTempDatas.Clear();
            for (int i = 0; i < states.Count; i++)
            {
                var stateName = states[i].EditorName;
                bool value = false;
                foreach (var stateValue in datas)
                {
                    if (stateValue.EditorStateName == stateName)
                    {
                        value = stateValue.EditorValue;
                        break;
                    }
                }
                m_EditorTempDatas.Add(new StateValue<bool> { EditorStateName = stateName, EditorValue = value });
            }
            datas.Clear();
            datas.AddRange(m_EditorTempDatas);
            m_EditorTempDatas.Clear();
        }

        private void EditorOnSelectedData1()
        {
            if (EditorDataName1 == EditorDataName2)
            {
                EditorDataName2 = string.Empty;
                EditorRefreshData2();
            }
            EditorRefreshData1();
            EditorRefreshSelectedName();
        }

        private void EditorOnSelectedData2()
        {
            var controller = EditorControllerMono;
            if (controller == null)
                return;
            foreach (var dataName in controller.EditorGetAllDataNames())
            {
                if (dataName != EditorDataName1 && dataName == EditorDataName2)
                {
                    EditorRefreshData2();
                    break;
                }
            }
            EditorRefreshSelectedName();
        }

        private void EditorRefreshSelectedName()
        {
            var data1 = EditorData1;
            if (data1 != null)
            {
                data1.EditorRefreshSelectedName();
            }
            var data2 = EditorData2;
            if (data2 != null)
            {
                data2.EditorRefreshSelectedName();
            }
        }
        
        private void EditorOnStateDataBeginGUI1(int selectionIndex)
        {
            var controller = EditorControllerMono;
            var stateValues = EditorStateValues1;
            if (controller == null || stateValues == null || selectionIndex >= stateValues.Count)
                return;
            GUILayout.BeginHorizontal();
            GUI.enabled = false;
            var curStateName = stateValues[selectionIndex].EditorStateName;
            var nameColor = GUI.color;
            GUI.color = new Color(1f, 0.82f, 0.45f);
            EditorGUILayout.TextField(curStateName);
            GUI.color = nameColor;
            GUI.enabled = true;
        }

        private void EditorOnStateDataEndGUI1(int selectionIndex)
        {
            var color = GUI.color;
            var controller = EditorControllerMono;
            var stateValues = EditorStateValues1;
            if (controller == null || stateValues == null || selectionIndex >= stateValues.Count)
                return;
            GUI.enabled = true;
            var data = EditorData1;
            var curStateName = stateValues[selectionIndex].EditorStateName;
            if (data.EditorSelectedName == curStateName)
            {
                GUI.color = new Color(0, 1, 0);
            }
            if (GUILayout.Button("Apply", GUILayout.ExpandWidth(false)))
            {
                controller.EditorRefresh();
                data.EditorSelectedName = curStateName;
            }
            GUI.color = color;
            GUILayout.EndHorizontal();
        }

        private void EditorOnStateDataBeginGUI2(int selectionIndex)
        {
            var controller = EditorControllerMono;
            var stateValues = EditorStateValues2;
            if (controller == null || stateValues == null || selectionIndex >= stateValues.Count)
                return;
            GUILayout.BeginHorizontal();
            GUI.enabled = false;
            var curStateName = stateValues[selectionIndex].EditorStateName;
            var nameColor = GUI.color;
            GUI.color = new Color(1f, 0.82f, 0.45f);
            EditorGUILayout.TextField(curStateName);
            GUI.color = nameColor;
            GUI.enabled = true;
        }

        private void EditorOnStateDataEndGUI2(int selectionIndex)
        {
            var color = GUI.color;
            var controller = EditorControllerMono;
            var stateValues = EditorStateValues2;
            if (controller == null || stateValues == null || selectionIndex >= stateValues.Count)
                return;
            GUI.enabled = true;
            var data = EditorData2;
            var curStateName = stateValues[selectionIndex].EditorStateName;
            if (data.EditorSelectedName == curStateName)
            {
                GUI.color = new Color(0, 1, 0);
            }
            if (GUILayout.Button("Apply", GUILayout.ExpandWidth(false)))
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