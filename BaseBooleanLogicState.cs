using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace StateController
{
    public abstract class BaseBooleanLogicState : BaseState
    {
        [SerializeField]
        [BoxGroup("Data1")]
        [LabelText("Data Name")]
        [PropertyOrder(10)]
        [ValueDropdown("GetDataNames1")]
        [OnValueChanged("OnSelectedData1")]
        private string m_DataName1 = string.Empty;

        [SerializeField]
        [BoxGroup("Data1")]
        [LabelText("State Datas")]
        [PropertyOrder(11)]
        [ShowIf("IsSelectedData1")]
        [ListDrawerSettings(DefaultExpandedState = true,
            HideAddButton = true, HideRemoveButton = true,
            DraggableItems = false,
            OnBeginListElementGUI = "OnStateDataBeginGUI1",
            OnEndListElementGUI = "OnStateDataEndGUI1")]
        private List<bool> m_StateDatas1 = new List<bool>();

        [SerializeField, PropertyOrder(20)]
        private BooleanLogicType m_BooleanLogicType;

        [SerializeField]
        [BoxGroup("Data2")]
        [LabelText("Data Name")]
        [PropertyOrder(30)]
        [EnableIf("CanShowData2")]
        [ValueDropdown("GetDataNames2")]
        [OnValueChanged("OnSelectedData2")]
        private string m_DataName2 = string.Empty;

        [SerializeField]
        [BoxGroup("Data2")]
        [LabelText("State Datas")]
        [PropertyOrder(31)]
        [EnableIf("CanShowData2")]
        [ShowIf("IsSelectedData2")]
        [ListDrawerSettings(DefaultExpandedState = true,
            HideAddButton = true, HideRemoveButton = true,
            DraggableItems = false,
            OnBeginListElementGUI = "OnStateDataBeginGUI2",
            OnEndListElementGUI = "OnStateDataEndGUI2")]
        private List<bool> m_StateDatas2 = new List<bool>();

        private string m_CurStateName1;
        private string m_CurStateName2;
        private readonly Dictionary<string, bool> m_StateDataDict1 = new Dictionary<string, bool>();
        private readonly Dictionary<string, bool> m_StateDataDict2 = new Dictionary<string, bool>();
        private StateControllerData m_Data1;
        private StateControllerData m_Data2;

        internal override void OnInit(StateController controller)
        {
            m_Data1 = controller.GetData(m_DataName1);
            if (m_Data1 != null)
            {
                m_StateDataDict1.Clear();
                for (int i = 0; i < m_Data1.Editor_StateNames.Count; i++)
                {
                    m_StateDataDict1.Add(m_Data1.Editor_StateNames[i], m_StateDatas1[i]);
                }
            }
            if (m_BooleanLogicType != BooleanLogicType.None)
            {
                m_Data2 = controller.GetData(m_DataName2);
                if (m_Data2 != null)
                {
                    m_StateDataDict2.Clear();
                    for (int i = 0; i < m_Data2.Editor_StateNames.Count; i++)
                    {
                        m_StateDataDict2.Add(m_Data2.Editor_StateNames[i], m_StateDatas2[i]);
                    }
                }
            }
        }

        internal override void OnRefresh()
        {
            if (m_BooleanLogicType == BooleanLogicType.None)
            {
                if (m_Data1 == null || string.IsNullOrEmpty(m_Data1.SelectedName))
                    return;
                if (m_Data1.SelectedName == m_CurStateName1)
                    return;
                m_CurStateName1 = m_Data1.SelectedName;
                OnStateChanged(m_StateDataDict1[m_CurStateName1]);
            }
            else
            {
                if (m_Data1 == null || string.IsNullOrEmpty(m_Data1.SelectedName))
                    return;
                if (m_Data2 == null || string.IsNullOrEmpty(m_Data2.SelectedName))
                    return;
                if (m_Data1.SelectedName == m_CurStateName1 && m_Data2.SelectedName == m_CurStateName2)
                    return;
                m_CurStateName1 = m_Data1.SelectedName;
                m_CurStateName2 = m_Data2.SelectedName;
                bool logicResult = false;
                switch (m_BooleanLogicType)
                {
                    case BooleanLogicType.And:
                        logicResult = m_StateDataDict1[m_CurStateName1] && m_StateDataDict2[m_CurStateName2];
                        break;
                    case BooleanLogicType.Or:
                        logicResult = m_StateDataDict1[m_CurStateName1] || m_StateDataDict2[m_CurStateName2];
                        break;
                }
                OnStateChanged(logicResult);
            }
        }

        protected abstract void OnStateChanged(bool logicResult);

#if UNITY_EDITOR
        internal override void Editor_OnRefresh()
        {
            RefreshData1();
            RefreshData2();
        }

        internal override void Editor_OnDataRename(string oldDataName, string newDataName)
        {
            if (m_DataName1 == oldDataName)
            {
                m_DataName1 = newDataName;
            }
            else if (m_DataName1 == oldDataName)
            {
                m_DataName2 = newDataName;
            }
        }

        internal override void Editor_OnDataRemoveState(string dataName, int index)
        {
            if (m_DataName1 == dataName)
            {
                m_StateDatas1.RemoveAt(index);
                if (m_StateDataDict1 != null)
                {
                    m_StateDataDict1.Remove(m_Data1.Editor_StateNames[index]);
                }
            }
            else if (m_DataName2 == dataName)
            {
                m_StateDatas2.RemoveAt(index);
                if (m_StateDataDict2 != null)
                {
                    m_StateDataDict2.Remove(m_Data2.Editor_StateNames[index]);
                }
            }
        }

        internal override bool Editor_CheckIsConnection(StateControllerData data)
        {
            if (m_DataName1 == data.Name || (m_BooleanLogicType != BooleanLogicType.None && m_DataName2 == data.Name))
            {
                return true;
            }
            return false;
        }

        private bool IsSelectedData1()
        {
            return m_Data1 != null;
        }

        private bool IsSelectedData2()
        {
            return m_Data2 != null && m_BooleanLogicType != BooleanLogicType.None;
        }

        private bool CanShowData2()
        {
            return m_BooleanLogicType != BooleanLogicType.None;
        }

        private List<string> GetDataNames1()
        {
            return Editor_Controller.GetAllDataNames();
        }
        
        private List<string> GetDataNames2()
        {
            var names = Editor_Controller.GetAllDataNames();
            names.Remove(m_DataName1);
            return names;
        }

        private void RefreshData1()
        {
            m_Data1 = Editor_Controller.Editor_GetData(m_DataName1);
            if (m_Data1 != null)
            {
                for (int i = m_StateDatas1.Count; i < m_Data1.Editor_StateNames.Count; i++)
                {
                    m_StateDatas1.Add(default);
                }
                for (int i = m_StateDatas1.Count - 1; i >= m_Data1.Editor_StateNames.Count; i--)
                {
                    m_StateDatas1.RemoveAt(i);
                }
                m_StateDataDict1.Clear();
                for (int i = 0; i < m_Data1.Editor_StateNames.Count; i++)
                {
                    m_StateDataDict1.Add(m_Data1.Editor_StateNames[i], m_StateDatas1[i]);
                }
            }
            else
            {
                m_StateDatas1.Clear();
                m_StateDataDict1.Clear();
            }
        }

        private void RefreshData2()
        {
            m_Data2 = Editor_Controller.Editor_GetData(m_DataName2);
            if (m_Data2 != null)
            {
                for (int i = m_StateDatas2.Count; i < m_Data2.Editor_StateNames.Count; i++)
                {
                    m_StateDatas2.Add(default);
                }
                for (int i = m_StateDatas2.Count - 1; i >= m_Data2.Editor_StateNames.Count; i--)
                {
                    m_StateDatas2.RemoveAt(i);
                }
                m_StateDataDict2.Clear();
                for (int i = 0; i < m_Data1.Editor_StateNames.Count; i++)
                {
                    m_StateDataDict2.Add(m_Data1.Editor_StateNames[i], m_StateDatas1[i]);
                }
            }
            else
            {
                m_StateDatas2.Clear();
                m_StateDataDict2.Clear();
            }
        }

        private void OnSelectedData1()
        {
            m_Data1 = Editor_Controller.Editor_GetData(m_DataName1);
            if (m_DataName1 == m_DataName2)
            {
                m_DataName2 = string.Empty;
                m_Data2 = null;
                RefreshData2();
            }
            RefreshData1();
        }

        private void OnSelectedData2()
        {
            foreach (var dataName in Editor_Controller.GetAllDataNames())
            {
                if (dataName != m_DataName1 && dataName == m_DataName2)
                {
                    m_Data2 = Editor_Controller.Editor_GetData(dataName);
                    RefreshData2();
                    return;
                }
            }
        }
        
        private void OnStateDataBeginGUI1(int selectionIndex)
        {
            GUILayout.BeginHorizontal();
        }

        private void OnStateDataEndGUI1(int selectionIndex)
        {
            bool enable = GUI.enabled;
            GUI.enabled = false;
            GUILayout.TextField(m_Data1.Editor_StateNames[selectionIndex]);
            GUI.enabled = enable;
            GUILayout.EndHorizontal();
        }

        private void OnStateDataBeginGUI2(int selectionIndex)
        {
            GUILayout.BeginHorizontal();
        }

        private void OnStateDataEndGUI2(int selectionIndex)
        {
            bool enable = GUI.enabled;
            GUI.enabled = false;
            GUILayout.TextField(m_Data2.Editor_StateNames[selectionIndex]);
            GUI.enabled = enable;
            GUILayout.EndHorizontal();
        }
#endif
    }
}