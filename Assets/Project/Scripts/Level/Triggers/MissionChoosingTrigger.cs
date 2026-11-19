using System;
using UnityEngine;

namespace Project.Scripts.Level.Triggers
{
    public class MissionChoosingTrigger : Trigger
    {
        public event Action OnOpenMissionPanel;

        private bool _isMissionPanelOpen;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out Player.Core.Player _))
                return;

            if (_isMissionPanelOpen)
                return;

            _isMissionPanelOpen = true;
            OnOpenMissionPanel?.Invoke();
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Player.Core.Player _))
                _isMissionPanelOpen = false;
        }
    }
}
