#if UNITY_EDITOR
using Sirenix.OdinInspector;

namespace StateController
{
    internal partial class StateValueMapping<T>
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

    public partial class StateBinding
    {
        protected virtual void OnValidate()
        {
            var controller = EditorController;
            if (controller != null)
            {
                controller.EditorRefresh();
            }
        }

        [ShowInInspector]
        internal StateController EditorController => GetComponentInParent<StateController>(true);
        // 刷新本组件：按名字对齐重建存的（状态名,值），处理控制器状态的新增/删除/重排
        internal abstract void EditorRefresh();
        internal abstract void EditorApplyCurrentState();
        internal abstract void EditorOnGroupRename(string oldGroupName, string newGroupName);
        // 状态改名时迁移对应的值（键变了无法靠 EditorRefresh 自动对齐，需单独入口）
        internal abstract void EditorOnGroupStateRename(string groupName, string oldStateName, string newStateName);
        internal abstract bool EditorIsBoundTo(StateGroup group);
    }
}
#endif
