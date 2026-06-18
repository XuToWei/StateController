using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace StateController
{
    [Serializable]
    internal partial class StateControllerStateLink
    {
        [SerializeField]
        private string m_TargetDataName = string.Empty;
        [SerializeField]
        private string m_TargetSelectedName = string.Empty;
        public string TargetDataName => m_TargetDataName;
        public string TargetSelectedName => m_TargetSelectedName;
    }

    [Serializable]
    internal partial class StateControllerState
    {
        // 编辑器里这些字段由 StateControllerMono 的列表自行手动绘制，隐藏 Odin 自动绘制
        [HideInInspector]
        [SerializeField]
        private string m_Name = string.Empty;
        [HideInInspector]
        [SerializeField]
        private List<StateControllerStateLink> m_Links = new List<StateControllerStateLink>();
        // 状态被选中时触发的可视化事件（类似 Button 的 onClick），由 StateControllerMono 编辑器手动绘制
        // 用 [SerializeReference] 让 m_OnSelectedEvent 可为 null（普通序列化的 UnityEvent 反序列化后必为非空），null 即表示“未添加事件”
        [HideInInspector]
        [SerializeReference]
        private UnityEvent m_OnSelectedEvent;
        public string Name => m_Name;
        public List<StateControllerStateLink> Links => m_Links;
        public UnityEvent OnSelected => m_OnSelectedEvent;
    }

    [Serializable]
    public sealed partial class StateControllerData
    {
        [SerializeField]
        private string m_Name;
        // 每个状态（名字 + 其 link 列表，一对多）
        [SerializeField]
        private List<StateControllerState> m_States = new List<StateControllerState>();

        private string m_SelectedName;
        private int m_SelectedIndex = -1;
        private StateControllerMono m_StateControllerMono;

        public string Name => m_Name;
        public Action<string> OnSelectedNameChanged;
        public Action<int> OnSelectedIndexChanged;

        public string SelectedName
        {
            get => m_SelectedName;
            set
            {
                if (m_SelectedName == value)
                    return;
                int index = -1;
                for (int i = 0; i < m_States.Count; i++)
                {
                    if (m_States[i].Name == value)
                    {
                        index = i;
                        break;
                    }
                }
                if (index < 0)
                    throw new Exception($"State name '{value}' is not in data '{m_Name}'.");
                SetSelectedInternal(value, index);
            }
        }

        public int SelectedIndex
        {
            get => m_SelectedIndex;
            set
            {
                if (m_SelectedIndex == value)
                    return;
                if (value < 0 || value >= m_States.Count)
                    throw new Exception($"State index '{value}' is not in data '{m_Name}'.");
                SetSelectedInternal(m_States[value].Name, value);
            }
        }

        private void SetSelectedInternal(string name, int index)
        {
            m_SelectedName = name;
            m_SelectedIndex = index;
            foreach (var state in m_StateControllerMono.States)
            {
                state.OnRefresh();
            }
            var curState = m_States[index];
            foreach (var link in curState.Links)
            {
                var targetData = m_StateControllerMono.GetData(link.TargetDataName);
                if (targetData != null && !string.IsNullOrEmpty(link.TargetSelectedName))
                {
                    targetData.SelectedName = link.TargetSelectedName;
                }
            }
            if (curState.OnSelected != null)
            {
                curState.OnSelected.Invoke();
            }
            OnSelectedNameChanged?.Invoke(m_SelectedName);
            OnSelectedIndexChanged?.Invoke(m_SelectedIndex);
        }

        internal List<StateControllerState> States => m_States;

        internal void OnInit(StateControllerMono controllerMono)
        {
            m_StateControllerMono = controllerMono;
        }
    }
}
