using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace StateController
{
    [Serializable]
    public class SpriteColorData
    {
        [HorizontalGroup]
        [SerializeField]
        private Sprite m_Sprite;
        [HorizontalGroup]
        [SerializeField]
        private Color m_Color = Color.white;
        public Sprite Sprite => m_Sprite;
        public Color Color => m_Color;
    }

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image))]
    // [SelectableStateDrawSetting(true)]
    public class StateImageColor : BaseSelectableState<SpriteColorData>
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