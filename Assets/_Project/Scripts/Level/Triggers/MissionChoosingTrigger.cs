using System;
using UnityEngine;

namespace _Project.Scripts.Level.Triggers
{
    public class MissionChoosingTrigger : Trigger
    {
        public event Action OnOpenMissionPanel;
        
        public bool IsMissionPanelOpen { get; private set; }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out Player.Core.Player _))
                return;

            if (IsMissionPanelOpen)
                return;

            IsMissionPanelOpen = true;
            OnOpenMissionPanel?.Invoke();
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Player.Core.Player _))
                IsMissionPanelOpen = false;
        }
    }
}