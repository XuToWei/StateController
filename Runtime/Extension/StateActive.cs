using UnityEngine;

namespace StateController
{
    [DisallowMultipleComponent]
    public class StateActive : BaseBooleanLogicState
    {
        protected override void OnStateChanged(bool logicResult)
        {
            gameObject.SetActive(logicResult);
        }
    }
}