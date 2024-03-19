#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace StateController
{
    public partial class StateControllerLinkData
    {
        private StateControllerLink m_EditorLink;
        private bool m_EditorIsInit = false;
        internal void OnValidate(StateControllerLink link)
        {
            m_EditorLink = link;
            EditorEnsureLegal();
        }
        
        [ShowInInspector]
        [LabelText("Data")]
        [PropertyOrder(1)]
        [ValueDropdown("EditorTargetDataNameValueDropdown")]
        [OnValueChanged("EditorEnsureLegal")]
        [InfoBox("$m_EditorErrorMsg",
            InfoMessageType.Error,
            "EditorTargetDataNameIsIllegal")]
        internal string EditorTargetDataName
        {
            get => m_TargetDataName;
            private set => m_TargetDataName = value;
        }

        [ShowInInspector]
        [LabelText("Selected Name")]
        [PropertyOrder(2)]
        [ValueDropdown("EditorTargetSelectedNameValueDropdown")]
        private string EditorTargetSelectedName
        {
            set => m_TargetSelectedName = value;
            get => m_TargetSelectedName;
        }

        private string m_EditorErrorMsg = String.Empty;
        
        private void EditorEnsureLegal()
        {
            m_EditorErrorMsg = string.Empty;
            if (m_EditorLink != null && !string.IsNullOrEmpty(m_EditorLink.EditorDataName) && !string.IsNullOrEmpty(EditorTargetDataName))
            {
                var controller = m_EditorLink.EditorController;
                if (controller != null)
                {
                    foreach (var state in controller.EditorStates)
                    {
                        if (state != m_EditorLink && state is StateControllerLink link2)
                        {
                            if (link2.EditorDataName == EditorTargetDataName)
                            {
                                m_TargetDataName = string.Empty;
                                m_TargetSelectedName = string.Empty;
                                m_EditorErrorMsg = $"Loop reference data name : {EditorTargetDataName}";
                                return;
                            }
                            foreach (var stateData in link2.EditorStateDatas)
                            {
                                if (stateData.EditorTargetDataName == m_EditorLink.EditorDataName)
                                {
                                    m_TargetDataName = string.Empty;
                                    m_TargetSelectedName = string.Empty;
                                    m_EditorErrorMsg = $"Loop reference data name : {EditorTargetDataName}";
                                    return;
                                }
                            }
                        }
                    }
                }
                if (EditorTargetDataName == m_EditorLink.EditorDataName)
                {
                    m_TargetDataName = string.Empty;
                    m_TargetSelectedName = string.Empty;
                    m_EditorErrorMsg = $"Can‘t be same name as the link data name : {EditorTargetDataName}";
                    return;
                }
            }
        }
        
        private readonly List<string> m_EmptyListString = new List<string>();
        private List<string> EditorTargetDataNameValueDropdown()
        {
            if (m_EditorLink == null)
            {
                return m_EmptyListString;
            }
            var controller = m_EditorLink.EditorController;
            if (controller == null)
            {
                return m_EmptyListString;
            }
            var names = controller.EditorGetAllDataNames();
            names.Remove(m_EditorLink.EditorDataName);
            return names;
        }

        private List<string> EditorTargetSelectedNameValueDropdown()
        {
            if (m_EditorLink == null)
            {
                return m_EmptyListString;
            }
            var controller = m_EditorLink.EditorController;
            if (controller == null)
            {
                return m_EmptyListString;
            }
            var data = controller.GetData(EditorTargetDataName);
            if (data == null)
            {
                return m_EmptyListString;
            }
            return data.EditorStateNames;
        }

        private bool EditorTargetDataNameIsIllegal()
        {
            return !string.IsNullOrEmpty(m_EditorErrorMsg);
        }
    }

    public partial class StateControllerLink
    {
        protected override void OnValidate()
        {
            base.OnValidate();
            foreach (var data in EditorStateDatas)
            {
                if (data != null)
                {
                    data.OnValidate(this);
                }
            }
        }
    }
}
#endif