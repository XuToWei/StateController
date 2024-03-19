using System;
using UnityEngine;

namespace StateController
{
    
    [Serializable]
    public partial class StateControllerLinkData
    {
        [HideInInspector]
        [SerializeField]
        private string m_TargetDataName = string.Empty;
        [HideInInspector]
        [SerializeField]
        private string m_TargetSelectedName = string.Empty;
        public string TargetDataName => m_TargetDataName;
        public string TargetSelectedName => m_TargetSelectedName;
    }
    
    public partial class StateControllerLink : BaseSelectableState<StateControllerLinkData>
    {
        private StateController m_StateController;
        internal override void OnInit(StateController controller)
        {
            m_StateController = controller;
            base.OnInit(controller);
        }

        protected override void OnStateInit()
        {
            
        }

        protected override void OnStateChanged(StateControllerLinkData linkData)
        {
            if (m_StateController != null && linkData != null)
            {
                var data = m_StateController.GetData(linkData.TargetDataName);
                if (data != null && !string.IsNullOrEmpty(linkData.TargetSelectedName))
                {
                    data.SelectedName = linkData.TargetSelectedName;
                }
            }
        }
    }
}