using UnityEngine;
using UnityEngine.UI;

namespace StateController
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image))]
    public class ImageSpriteBinding : StateValueBinding<Sprite>
    {
        [SerializeField]
        private bool m_UseNativeSize;

        private Image m_TargetImage;

        protected override void InitializeTarget()
        {
            m_TargetImage = GetComponent<Image>();
        }

        protected override void ApplyValue(Sprite value)
        {
            m_TargetImage.sprite = value;
            if (m_UseNativeSize)
            {
                m_TargetImage.SetNativeSize();
            }
        }
    }
}
