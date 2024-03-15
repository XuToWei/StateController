﻿#if UNITY_EDITOR
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
        private string EditorDataName
        {
            get => m_DataName;
            set => m_DataName = value;
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
        private List<T> EditorStateDatas
        {
            set => m_StateDatas = value;
            get => m_StateDatas;
        }

        private StateControllerData EditorData => EditorController.EditorGetData(EditorDataName);

        internal override void EditorRefresh()
        {
            EditorRefreshData();
        }

        internal override void EditorOnRefresh()
        {
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
                for (int i = EditorStateDatas.Count; i < data.EditorStateNames.Count; i++)
                {
                    EditorStateDatas.Add(default);
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

        private List<string> EditorGetDataNames()
        {
            return EditorController.EditorGetAllDataNames();
        }

        private bool EditorIsSelectedData()
        {
            return EditorData != null;
        }

        private void EditorOnSelectedData()
        {
            EditorRefreshData();
        }
        
        internal void EditorRefreshSelectedName()
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
            GUI.enabled = false;
            var data = EditorData;
            GUILayout.TextField(data.EditorStateNames[selectionIndex], GUILayout.Width(160));
            GUI.enabled = true;
            var color = GUI.color;
            if (data.EditorSelectedName == data.EditorStateNames[selectionIndex])
            {
                GUI.color = new Color(0,1,0);
            }
            if (GUILayout.Button("Apply", GUILayout.Width(80)))
            {
                EditorController.EditorRefresh();
                data.EditorSelectedName = data.EditorStateNames[selectionIndex];
            }
            GUI.color = color;
            GUILayout.EndHorizontal();
        }
    }
}
#endif