using UnityEngine;

namespace StateController
{
    public abstract class BaseState : MonoBehaviour
    {
        internal abstract void OnInit(StateController controller);
        internal abstract void OnRefresh();

#if UNITY_EDITOR
        protected void OnValidate()
        {
            EditorController.EditorRefresh();
        }

        internal StateController EditorController => GetComponentInParent<StateController>(true);
        internal abstract void EditorOnRefresh();
        internal abstract void Editor_OnDataRename(string oldDataName, string newDataName);
        internal abstract void Editor_OnDataRemoveState(string dataName, int index);
        internal abstract bool Editor_CheckIsConnection(StateControllerData data);
#endif
    }
}