using System;
using UnityEngine;

namespace _Project.Scripts.Level.Triggers
{
    public class LootChestTrigger : Trigger
    {
        public event Action OnGotLoot;

        private bool _isLootGot;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out Player.Core.Player _)) return;
            if (_isLootGot) return;

            OnGotLoot?.Invoke();
        }
        
        public void MarkAsUsed()
        {
            _isLootGot = true;
            Deactivate();
        }
    }
}