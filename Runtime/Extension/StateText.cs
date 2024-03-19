using UnityEngine;
using UnityEngine.UI;

namespace StateController
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Text))]
    public class StateText : BaseSelectableState<string>
    {
        private Text m_Text;
        
        protected override void OnStateInit()
        {
            m_Text = GetComponent<Text>();
        }

        protected override void OnStateChanged(string text)
        {
            m_Text.text = text;
        }
    }
}