using UnityEngine;

namespace StateController
{
    [DisallowMultipleComponent]
    public class GameObjectActiveBinding : BooleanStateBinding
    {
        private GameObject m_TargetGameObject;

        protected override void InitializeTarget()
        {
            m_TargetGameObject = gameObject;
        }

        protected override void ApplyValue(bool value)
        {
            m_TargetGameObject.SetActive(value);
        }
    }
}
