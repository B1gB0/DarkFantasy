using System;
using UnityEngine;

namespace _Project.Scripts.Level.Triggers
{
    public class NextLevelTrigger : Trigger
    {
        public event Action OnGoToNextLevel;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out Player.Core.Player _))
                return;
            
            OnGoToNextLevel?.Invoke();
        }
    }
}