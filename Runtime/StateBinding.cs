using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace StateController
{
    // 单个状态对应的值（状态名 + 值合为一条数据，与控制器状态顺序无关）
    [Serializable]
    [InlineProperty]
    internal partial class StateValueMapping<T>
    {
        [HideInInspector]
        [SerializeField]
        private string m_StateName = string.Empty;
        [HideLabel]
        [SerializeField]
        private T m_Value;
        public string StateName => m_StateName;
        public T Value => m_Value;
    }

    public abstract partial class StateBinding : MonoBehaviour
    {
        internal abstract void Initialize(StateController controller);

        internal abstract void RefreshBinding();
    }
}
