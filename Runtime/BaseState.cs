using UnityEngine;

namespace StateController
{
    public abstract partial class BaseState : MonoBehaviour
    {
        internal virtual void OnInit(StateController controller)
        {
            OnInit();
        }

        internal abstract void OnRefresh();

        protected virtual void OnInit()
        {
            
        }
    }
}