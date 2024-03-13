using System;
using UnityEngine;

namespace StateController
{
    [ExecuteAlways]
    public abstract class BaseState : MonoBehaviour
    {
        internal abstract void OnInit(StateController controller);
        internal abstract void OnRefresh();

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
                return;
            var controller = Editor_Controller;
            if (controller == null)
            {
                throw new Exception($"State '{gameObject.name}' must require a parent component 'StateController'!");
            }
            Editor_OnRefresh();
#endif
        }

        internal StateController Editor_Controller => GetComponentInParent<StateController>(true);
        internal abstract void Editor_OnRefresh();
        internal abstract void Editor_OnDataRename(string oldDataName, string newDataName);
        internal abstract void Editor_OnDataRemoveState(string dataName, int index);
        internal abstract bool Editor_CheckIsConnection(StateControllerData data);
#endif
    }
}