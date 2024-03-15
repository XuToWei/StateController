using UnityEngine;

namespace StateController
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public class StateAnchoredPosition : BaseSelectableState<Vector2>
    {
        private RectTransform m_RectTransform;

        protected internal override void OnInit()
        {
            base.OnInit();
            m_RectTransform = GetComponent<RectTransform>();
        }

        protected override void OnStateChanged(Vector2 pos)
        {
            m_RectTransform.anchoredPosition = pos;
        }
    }
}