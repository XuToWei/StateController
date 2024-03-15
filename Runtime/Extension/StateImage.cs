using UnityEngine;
using UnityEngine.UI;

namespace StateController
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image))]
    public class StateImage : BaseSelectableState<Sprite>
    {
        [SerializeField]
        private bool m_SetNativeSize;
        
        private Image m_Image;

        protected internal override void OnInit()
        {
            base.OnInit();
            m_Image = GetComponent<Image>();
        }

        protected override void OnStateChanged(Sprite sprite)
        {
            m_Image.sprite = sprite;
            if (m_SetNativeSize)
            {
                m_Image.SetNativeSize();
            }
        }
    }
}