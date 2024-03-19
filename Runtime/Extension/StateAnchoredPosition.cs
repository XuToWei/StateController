using UnityEngine;

namespace StateController
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public class StateAnchoredPosition : BaseSelectableState<Vector2>
    {
        private RectTransform m_RectTransform;

        protected override void OnStateInit()
        {
            m_RectTransform = GetComponent<RectTransform>();
        }

        protected override void OnStateChanged(Vector2 pos)
        {
            m_RectTransform.anchoredPosition = pos;
        }
    }
}