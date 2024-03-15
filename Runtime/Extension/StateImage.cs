using UnityEngine;
using UnityEngine.UI;

namespace StateController
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image))]
    public class StateImage : BaseSelectableState<Sprite>
    {
        private Image m_Image;

        private void Awake()
        {
            m_Image = GetComponent<Image>();
        }

        protected override void OnStateChanged(Sprite sprite)
        {
            m_Image.sprite = sprite;
        }
    }
}