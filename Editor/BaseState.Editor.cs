#if UNITY_EDITOR
namespace StateController
{
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

        internal StateControllerMono EditorControllerMono => GetComponentInParent<StateControllerMono>(true);
        internal abstract void EditorRefresh();
        internal abstract void EditorOnRefresh();
        internal abstract void EditorOnDataRename(string oldDataName, string newDataName);
        internal abstract void EditorOnDataRemoveState(string dataName, int index);
        internal abstract void EditorOnDataSwitchState(string dataName, int index1, int index2);
        internal abstract bool EditorCheckIsConnection(StateControllerData data);
    }
}
#endif