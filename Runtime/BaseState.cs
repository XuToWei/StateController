using UnityEngine;

namespace StateController
{
    public abstract partial class BaseState : MonoBehaviour
    {
        internal abstract void OnInit(StateController controller);

        internal abstract void OnRefresh();
    }
}