using UnityEngine;

namespace StateController
{
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public class StateAnchoredPosition : BaseSelectableState<Vector2>
    {
        private RectTransform m_RectTransform;

        private void Awake()
        {
            m_RectTransform = GetComponent<RectTransform>();
        }

        protected override void OnStateChanged(Vector2 pos)
        {
            m_RectTransform.anchoredPosition = pos;
        }
    }
}