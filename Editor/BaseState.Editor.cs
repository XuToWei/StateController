#if UNITY_EDITOR
using Sirenix.OdinInspector;

namespace StateController
{
    internal partial class StateValue<T>
    {
        public string EditorStateName
        {
            get => m_StateName;
            set => m_StateName = value;
        }

        public T EditorValue
        {
            get => m_Value;
            set => m_Value = value;
        }
    }

    public partial class BaseState
    {
        protected virtual void OnValidate()
        {
            var controller = EditorControllerMono;
            if (controller != null)
            {
                controller.EditorRefresh();
            }
        }

        [ShowInInspector]
        internal StateControllerMono EditorControllerMono => GetComponentInParent<StateControllerMono>(true);
        // 刷新本组件：按名字对齐重建存的（状态名,值），处理控制器状态的新增/删除/重排
        internal abstract void EditorRefresh();
        internal abstract void EditorOnRefresh();
        internal abstract void EditorOnDataRename(string oldDataName, string newDataName);
        // 状态改名时迁移对应的值（键变了无法靠 EditorRefresh 自动对齐，需单独入口）
        internal abstract void EditorOnDataStateRename(string dataName, string oldStateName, string newStateName);
        internal abstract bool EditorCheckIsConnection(StateControllerData data);
    }
}
#endif
