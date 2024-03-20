#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace StateController
{
    public partial class BaseSelectableState<T>
    {
        [ShowInInspector]
        [BoxGroup("Data")]
        [LabelText("Data Name")]
        [PropertyOrder(10)]
        [ValueDropdown("EditorGetDataNames")]
        [OnValueChanged("EditorOnSelectedData")]
        internal string EditorDataName
        {
            get => m_DataName;
            private set => m_DataName = value;
        }

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
        internal List<T> EditorStateDatas
        {
            private set => m_StateDatas = value;
            get => m_StateDatas;
        }

        private StateControllerData EditorData
        {
            get
            {
                var controller = EditorController;
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
            var controller = EditorController;
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
        }

        private void EditorOnStateDataEndGUI(int selectionIndex)
        {
            var controller = EditorController;
            if (controller == null)
            {
                GUILayout.EndHorizontal();
                return;
            }
            GUI.enabled = false;
            var data = EditorData;
            var curStateName = data.EditorStateNames[selectionIndex];
            GUILayout.TextField(curStateName);
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