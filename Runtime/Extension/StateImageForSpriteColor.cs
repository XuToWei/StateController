using UnityEngine;
using UnityEngine.UI;

namespace StateController
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image))]
    // [SelectableStateDrawSetting(true)]
    public class StateImageForSpriteColor : BaseSelectableState<SpriteColorData>
    {
        [SerializeField]
        private bool m_SetNativeSize;

        private Image m_Image;

        protected override void OnStateInit()
        {
            m_Image = GetComponent<Image>();
        }

        protected override void OnStateChanged(SpriteColorData stateData)
        {
            m_Image.sprite = stateData.Sprite;
            m_Image.color = stateData.Color;
            if (m_SetNativeSize)
            {
                m_Image.SetNativeSize();
            }
        }
    }
}