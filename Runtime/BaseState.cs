using UnityEngine;

namespace StateController
{
    public abstract partial class BaseState : MonoBehaviour
    {
        internal abstract void OnInit(StateControllerMono controllerMono);

        internal abstract void OnRefresh();
    }
}