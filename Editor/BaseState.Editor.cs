#if UNITY_EDITOR
namespace StateController
{
    public partial class BaseState
    {
        protected virtual void OnValidate()
        {
            var controller = EditorController;
            if (controller != null)
            {
                controller.EditorRefresh();
            }
        }

        internal StateController EditorController => GetComponentInParent<StateController>(true);
        internal abstract void EditorRefresh();
        internal abstract void EditorOnRefresh();
        internal abstract void EditorOnDataRename(string oldDataName, string newDataName);
        internal abstract void EditorOnDataRemoveState(string dataName, int index);
        internal abstract bool EditorCheckIsConnection(StateControllerData data);
    }
}
#endif