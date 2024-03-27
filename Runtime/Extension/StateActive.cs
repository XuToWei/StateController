using UnityEngine;

namespace StateController
{
    [DisallowMultipleComponent]
    public class StateActive : BaseBooleanLogicState
    {
        private GameObject m_GameObject;
        
        protected override void OnStateInit()
        {
            m_GameObject = gameObject;
        }

        protected override void OnStateChanged(bool logicResult)
        {
            m_GameObject.SetActive(logicResult);
        }
    }
}