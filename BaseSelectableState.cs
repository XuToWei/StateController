using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace StateController
{
    public abstract class BaseSelectableState<T> : BaseState where T : BaseStateData, new()
    {
        [SerializeField]
        [BoxGroup("Data")]
        [LabelText("Data Name")]
        [PropertyOrder(10)]
        [ValueDropdown("GetDataNames")]
        [OnValueChanged("OnSelectedData")]
        private string m_DataName = string.Empty;

        [SerializeField]
        [BoxGroup("Data")]
        [LabelText("State Datas")]
        [PropertyOrder(11)]
        [ShowIf("IsSelectedData")]
        [ListDrawerSettings(DefaultExpandedState = true,
            HideAddButton = true, HideRemoveButton = true,
            DraggableItems = false,
            OnBeginListElementGUI = "OnStateDataBeginGUI",
            OnEndListElementGUI = "OnStateDataEndGUI")]
        private List<T> m_StateDatas = new List<T>();

        private string m_CurStateName;
        private Dictionary<string, T> m_StateDataDict = new Dictionary<string, T>();
        private StateControllerData m_Data;

        internal override void OnInit(StateController controller)
        {
            m_Data = controller.GetData(m_DataName);
            if (m_Data != null)
            {
                m_StateDataDict.Clear();
                for (int i = 0; i < m_Data.Editor_StateNames.Count; i++)
                {
                    m_StateDataDict.Add(m_Data.Editor_StateNames[i], m_StateDatas[i]);
                }
            }
        }

        internal override void OnRefresh()
        {
            if (m_Data == null || string.IsNullOrEmpty(m_Data.SelectedName))
                return;
            if (m_CurStateName == m_Data.SelectedName)
                return;
            m_CurStateName = m_Data.SelectedName;
            OnStateChanged(m_StateDataDict[m_Data.SelectedName]);
        }

        protected abstract void OnStateChanged(T stateData);

#if UNITY_EDITOR
        internal override void Editor_OnRefresh()
        {
            RefreshData();
        }

        internal override void Editor_OnDataRename(string oldDataName, string newDataName)
        {
            if (m_DataName == oldDataName)
            {
                m_DataName = newDataName;
            }
        }

        internal override void Editor_OnDataRemoveState(string dataName, int index)
        {
            if (m_DataName == dataName)
            {
                m_StateDatas.RemoveAt(index);
                if (m_StateDataDict != null)
                {
                    m_StateDataDict.Remove(m_Data.Editor_StateNames[index]);
                }
            }
        }
        
        internal override bool Editor_CheckIsConnection(StateControllerData data)
        {
            return m_DataName == data.Name;
        }

        private void RefreshData()
        {
            m_Data = Editor_Controller.Editor_GetData(m_DataName);
            if (m_Data != null)
            {
                for (int i = m_StateDatas.Count; i < m_Data.Editor_StateNames.Count; i++)
                {
                    m_StateDatas.Add(new T());
                }
                for (int i = m_StateDatas.Count - 1; i >= m_Data.Editor_StateNames.Count; i--)
                {
                    m_StateDatas.RemoveAt(i);
                }
                m_StateDataDict.Clear();
                for (int i = 0; i < m_Data.Editor_StateNames.Count; i++)
                {
                    m_StateDataDict.Add(m_Data.Editor_StateNames[i], m_StateDatas[i]);
                }
            }
            else
            {
                m_StateDatas.Clear();
                m_StateDataDict.Clear();
            }
        }

        private List<string> GetDataNames()
        {
            return Editor_Controller.GetAllDataNames();
        }

        private bool IsSelectedData()
        {
            return m_Data != null;
        }

        private void OnSelectedData()
        {
            m_Data = Editor_Controller.Editor_GetData(m_DataName);
            RefreshData();
        }

        private void OnStateDataBeginGUI(int selectionIndex)
        {
            GUILayout.BeginHorizontal();
        }

        private void OnStateDataEndGUI(int selectionIndex)
        {
            bool enable = GUI.enabled;
            GUI.enabled = false;
            GUILayout.TextField(m_Data.Editor_StateNames[selectionIndex]);
            GUI.enabled = enable;
            GUILayout.EndHorizontal();
        }
#endif
    }
}