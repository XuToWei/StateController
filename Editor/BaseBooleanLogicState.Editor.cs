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
        [PropertyOrder(10)]
        [ValueDropdown("EditorGetDataNames1")]
        [OnValueChanged("EditorOnSelectedData1")]
        private string EditorDataName1
        {
            get => m_DataName1;
            set => m_DataName1 = value;
        }

        [ShowInInspector]
        [BoxGroup("Data1")]
        [LabelText("State Datas")]
        [PropertyOrder(11)]
        [ShowIf("EditorIsSelectedData1")]
        [ListDrawerSettings(DefaultExpandedState = true,
            HideAddButton = true, HideRemoveButton = true,
            DraggableItems = false,
            OnBeginListElementGUI = "EditorOnStateDataBeginGUI1",
            OnEndListElementGUI = "EditorOnStateDataEndGUI1")]
        [OnValueChanged("EditorRefreshSelectedName", true)]
        private List<bool> EditorStateDatas1
        {
            set => m_StateDatas1 = value;
            get => m_StateDatas1;
        }

        [ShowInInspector]
        [PropertyOrder(20)]
        [OnValueChanged("EditorRefreshSelectedName")]
        public BooleanLogicType EditorBooleanLogicType
        {
            get => m_BooleanLogicType;
            set => m_BooleanLogicType = value;
        }

        [ShowInInspector]
        [BoxGroup("Data2")]
        [LabelText("Data Name")]
        [PropertyOrder(30)]
        [EnableIf("EditorCanShowData2")]
        [ValueDropdown("EditorGetDataNames2")]
        [OnValueChanged("EditorOnSelectedData2")]
        private string EditorDataName2
        {
            get => m_DataName2;
            set => m_DataName2 = value;
        }

        [ShowInInspector]
        [BoxGroup("Data2")]
        [LabelText("State Datas")]
        [PropertyOrder(31)]
        [EnableIf("EditorCanShowData2")]
        [ShowIf("EditorIsSelectedData2")]
        [ListDrawerSettings(DefaultExpandedState = true,
            HideAddButton = true, HideRemoveButton = true,
            DraggableItems = false,
            OnBeginListElementGUI = "EditorOnStateDataBeginGUI2",
            OnEndListElementGUI = "EditorOnStateDataEndGUI2")]
        [OnValueChanged("EditorRefreshSelectedName", true)]
        private List<bool> EditorStateDatas2
        {
            set => m_StateDatas2 = value;
            get => m_StateDatas2;
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
                OnStateChanged(EditorStateDatas1[data1.EditorSelectedIndex]);
            }
            else
            {
                var data1 = EditorData1;
                if (data1 == null || string.IsNullOrEmpty(data1.EditorSelectedName))
                    return;
                var data2 = EditorData2;
                if (data2 == null || string.IsNullOrEmpty(data2.EditorSelectedName))
                    return;
                bool logicResult = false;
                switch (EditorBooleanLogicType)
                {
                    case BooleanLogicType.And:
                        logicResult = EditorStateDatas1[data1.EditorSelectedIndex] && EditorStateDatas2[data2.EditorSelectedIndex];
                        break;
                    case BooleanLogicType.Or:
                        logicResult = EditorStateDatas1[data1.EditorSelectedIndex] || EditorStateDatas2[data2.EditorSelectedIndex];
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
            else if (EditorDataName1 == oldDataName)
            {
                EditorDataName2 = newDataName;
            }
        }

        internal override void EditorOnDataRemoveState(string dataName, int index)
        {
            if (EditorDataName1 == dataName)
            {
                EditorStateDatas1.RemoveAt(index);
            }
            else if (EditorDataName2 == dataName)
            {
                EditorStateDatas2.RemoveAt(index);
            }
        }

        internal override void EditorOnDataSwitchState(string dataName, int index1, int index2)
        {
            if (EditorDataName1 == dataName)
            {
                (EditorStateDatas1[index1], EditorStateDatas1[index2]) = (EditorStateDatas1[index2], EditorStateDatas1[index1]);
            }
            else if (EditorDataName2 == dataName)
            {
                (EditorStateDatas2[index1], EditorStateDatas2[index2]) = (EditorStateDatas2[index2], EditorStateDatas2[index1]);
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
            if (data1 != null)
            {
                for (int i = EditorStateDatas1.Count; i < data1.EditorStateNames.Count; i++)
                {
                    EditorStateDatas1.Add(default);
                }
                for (int i = EditorStateDatas1.Count - 1; i >= data1.EditorStateNames.Count; i--)
                {
                    EditorStateDatas1.RemoveAt(i);
                }
            }
            else
            {
                EditorDataName1 = string.Empty;
                EditorStateDatas1.Clear();
            }
        }

        private void EditorRefreshData2()
        {
            var data2 = EditorData2;
            if (data2 != null)
            {
                for (int i = EditorStateDatas2.Count; i < data2.EditorStateNames.Count; i++)
                {
                    EditorStateDatas2.Add(default);
                }
                for (int i = EditorStateDatas2.Count - 1; i >= data2.EditorStateNames.Count; i--)
                {
                    EditorStateDatas2.RemoveAt(i);
                }
            }
            else
            {
                EditorDataName2 = string.Empty;
                EditorStateDatas2.Clear();
            }
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
            GUILayout.BeginHorizontal();
        }

        private void EditorOnStateDataEndGUI1(int selectionIndex)
        {
            var controller = EditorControllerMono;
            if (controller == null)
            {
                GUILayout.EndHorizontal();
                return;
            }
            GUI.enabled = false;
            var data1 = EditorData1;
            var curStateName = data1.EditorStateNames[selectionIndex];
            EditorGUILayout.TextField(curStateName);
            GUI.enabled = true;
            var color = GUI.color;
            if (data1.EditorSelectedName == curStateName)
            {
                GUI.color = new Color(0,1,0);
            }
            if (GUILayout.Button("Apply"))
            {
                controller.EditorRefresh();
                data1.EditorSelectedName = curStateName;
            }
            GUI.color = color;
            GUILayout.EndHorizontal();
        }

        private void EditorOnStateDataBeginGUI2(int selectionIndex)
        {
            GUILayout.BeginHorizontal();
        }

        private void EditorOnStateDataEndGUI2(int selectionIndex)
        {
            var controller = EditorControllerMono;
            if (controller == null)
            {
                GUILayout.EndHorizontal();
                return;
            }
            GUI.enabled = false;
            var data2 = EditorData2;
            var curStateName = data2.EditorStateNames[selectionIndex];
            EditorGUILayout.TextField(curStateName);
            GUI.enabled = true;
            var color = GUI.color;
            if (data2.EditorSelectedName == curStateName)
            {
                GUI.color = new Color(0,1,0);
            }
            if (GUILayout.Button("Apply"))
            {
                controller.EditorRefresh();
                data2.EditorSelectedName = curStateName;
            }
            GUI.color = color;
            GUILayout.EndHorizontal();
        }
    }
}
#endif