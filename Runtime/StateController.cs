using System.Collections.Generic;
using UnityEngine;

namespace StateController
{
    [DisallowMultipleComponent]
    public sealed partial class StateController : MonoBehaviour
    {
        [HideInInspector]
        [SerializeField]
        private List<StateGroup> m_Groups = new List<StateGroup>();

        private readonly List<StateBinding> m_Bindings = new List<StateBinding>();
        internal List<StateBinding> Bindings => m_Bindings;

        private bool m_IsInitialized;

        private void Awake()
        {
            EnsureInitialized();
        }

        private void EnsureInitialized()
        {
            if(m_IsInitialized)
                return;
            m_IsInitialized = true;
            m_Bindings.Clear();
            GetComponentsInChildren<StateBinding>(true, m_Bindings);
            //不是该控制器的子节点就移除
            GameObject controllerGameObject = gameObject;
            for(int i = m_Bindings.Count - 1; i >= 0; i--)
            {
                if (m_Bindings[i].gameObject != controllerGameObject && m_Bindings[i].GetComponentInParent<StateController>(true) != this)
                {
                    m_Bindings.RemoveAt(i);
                }
            }
            foreach (var group in m_Groups)
            {
                group.Initialize(this);
            }
            foreach (var binding in m_Bindings)
            {
                binding.Initialize(this);
            }
        }

        public void SetCurrentStateName(string groupName, string stateName)
        {
            var group = GetGroup(groupName);
            if (group != null)
            {
                group.CurrentStateName = stateName;
            }
        }

        public void SetCurrentStateIndex(string groupName, int stateIndex)
        {
            var group = GetGroup(groupName);
            if (group != null)
            {
                group.CurrentStateIndex = stateIndex;
            }
        }

        public string GetCurrentStateName(string groupName)
        {
            var group = GetGroup(groupName);
            if (group != null)
            {
                return group.CurrentStateName;
            }
            return null;
        }

        public int GetCurrentStateIndex(string groupName)
        {
            var group = GetGroup(groupName);
            if (group != null)
            {
                return group.CurrentStateIndex;
            }
            return -1;
        }

        public string[] GetStateNames(string groupName)
        {
            var group = GetGroup(groupName);
            if (group != null)
            {
                var states = group.States;
                var names = new string[states.Count];
                for (int i = 0; i < names.Length; i++)
                {
                    names[i] = states[i].Name;
                }
                return names;
            }
            return null;
        }

        public void GetStateNames(string groupName, List<string> results)
        {
            if (results == null)
            {
                throw new System.ArgumentNullException(nameof(results));
            }
            results.Clear();
            var group = GetGroup(groupName);
            if (group != null)
            {
                foreach (var state in group.States)
                {
                    results.Add(state.Name);
                }
            }
        }

        public StateGroup GetGroup(string groupName)
        {
            EnsureInitialized();
            foreach (var group in m_Groups)
            {
                if (group.Name == groupName)
                    return group;
            }
            return null;
        }
    }
}
