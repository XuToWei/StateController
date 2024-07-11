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
        private SelectableStateDrawSettingAttribute DrawSettingAttribute
        {
            get
            {
                var objects = GetType().GetCustomAttributes(typeof(SelectableStateDrawSettingAttribute), true);
                if (objects.Length > 0)
                {
                    return (SelectableStateDrawSettingAttribute)objects[0];
                }
                return null;
            }
        }

        private bool DrawSettingLineBreak
        {
            get
            {
                var drawSetting = DrawSettingAttribute;
                return drawSetting != null && drawSetting.LineBreak;
            }
        }
        
        [ShowInInspector]
        [BoxGroup("Data")]
        [LabelText("Data Name")]
        [PropertyOrder(10)]
        [ValueDropdown("EditorGetDataNames")]
        [OnValueChanged("EditorOnSelectedData")]
        private string EditorDataName
        {
            get => m_DataName;
            set => m_DataName = value;
        }

        [HideReferenceObjectPicker]
        [ShowInInspector]
        [BoxGroup("Data")]
        [LabelText("State Datas")]
        [PropertyOrder(11)]
        [ShowIf("EditorIsSelectedData")]
        [ListDrawerSettings(DefaultExpandedState = true,
            HideAddButton = true, HideRemoveButton = true,
            DraggableItems = false,
            OnBeginListElementGUI = "EditorOnStateDataBeginGUI",
            OnEndListElementGUI = "EditorOnStateDataEndGUI")]
        [OnValueChanged("EditorRefreshSelectedName", true)]
        private List<T> EditorStateDatas
        {
            set => m_StateDatas = value;
            get => m_StateDatas;
        }

        private StateControllerData EditorData
        {
            get
            {
                var controller = EditorControllerMono;
                return controller == null ? null : controller.EditorGetData(EditorDataName);
            }
        }

        internal override void EditorRefresh()
        {
            EditorRefreshData();
        }

        internal override void EditorOnRefresh()
        {
            OnStateInit();
            var data = EditorData;
            if (data == null || string.IsNullOrEmpty(data.EditorSelectedName))
                return;
            OnStateChanged(EditorStateDatas[data.EditorSelectedIndex]);
        }

        internal override void EditorOnDataRename(string oldDataName, string newDataName)
        {
            if (EditorDataName == oldDataName)
            {
                EditorDataName = newDataName;
            }
        }

        internal override void EditorOnDataRemoveState(string dataName, int index)
        {
            if (EditorDataName == dataName)
            {
                EditorStateDatas.RemoveAt(index);
            }
        }

        internal override void EditorOnDataSwitchState(string dataName, int index1, int index2)
        {
            if (EditorDataName == dataName)
            {
                (EditorStateDatas[index1], EditorStateDatas[index2]) = (EditorStateDatas[index2], EditorStateDatas[index1]);
            }
        }

        internal override bool EditorCheckIsConnection(StateControllerData data)
        {
            return EditorDataName == data.EditorName;
        }

        private void EditorRefreshData()
        {
            var data = EditorData;
            if (data != null)
            {
                bool canCreate = typeof(T).GetConstructor(Type.EmptyTypes) != null;
                for (int i = EditorStateDatas.Count; i < data.EditorStateNames.Count; i++)
                {
                    if (canCreate)
                    {
                        EditorStateDatas.Add(Activator.CreateInstance<T>());
                    }
                    else
                    {
                        EditorStateDatas.Add(default);
                    }
                }
                for (int i = EditorStateDatas.Count - 1; i >= data.EditorStateNames.Count; i--)
                {
                    EditorStateDatas.RemoveAt(i);
                }
            }
            else
            {
                EditorStateDatas.Clear();
            }
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
            EditorRefreshData();
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
            if (controller == null)
            {
                GUILayout.EndHorizontal();
                return;
            }
            GUI.enabled = false;
            var data = EditorData;
            var curStateName = data.EditorStateNames[selectionIndex];
            EditorGUILayout.TextField(curStateName);
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