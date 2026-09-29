using UnityEngine;
using UnityEngine.UI;

namespace StateController.Demo
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image))]
    public class ImageSpriteColorBinding : StateValueBinding<SpriteColorValue>
    {
        [SerializeField]
        private bool m_UseNativeSize;

        private Image m_TargetImage;

        protected override void InitializeTarget()
        {
            m_TargetImage = GetComponent<Image>();
        }

        protected override void ApplyValue(SpriteColorValue value)
        {
            m_TargetImage.sprite = value.Sprite;
            m_TargetImage.color = value.Color;
            if (m_UseNativeSize)
            {
                m_TargetImage.SetNativeSize();
            }
        }
    }
}
