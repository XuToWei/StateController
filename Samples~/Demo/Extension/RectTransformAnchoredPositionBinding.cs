using UnityEngine;

namespace StateController.Demo
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public class RectTransformAnchoredPositionBinding : StateValueBinding<Vector2>
    {
        private RectTransform m_TargetRectTransform;

        protected override void InitializeTarget()
        {
            m_TargetRectTransform = GetComponent<RectTransform>();
        }

        protected override void ApplyValue(Vector2 value)
        {
            m_TargetRectTransform.anchoredPosition = value;
        }
    }
}
