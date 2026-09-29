using UnityEngine;
using UnityEngine.UI;

namespace StateController.Demo
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Text))]
    public class TextContentBinding : StateValueBinding<string>
    {
        private Text m_TargetText;

        protected override void InitializeTarget()
        {
            m_TargetText = GetComponent<Text>();
        }

        protected override void ApplyValue(string value)
        {
            m_TargetText.text = value;
        }
    }
}
