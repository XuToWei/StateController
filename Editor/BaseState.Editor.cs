#if UNITY_EDITOR
namespace StateController
{
    public partial class BaseState
    {
        protected void OnValidate()
        {
            EditorController.EditorRefresh();
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