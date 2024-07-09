using System;
using Sirenix.OdinInspector;
using UnityEngine;

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
}
