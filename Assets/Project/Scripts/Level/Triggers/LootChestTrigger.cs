using System;
using UnityEngine;

namespace Project.Scripts.Level.Triggers
{
    public class LootChestTrigger : Trigger
    {
        private bool _isLootGot;

        public event Action OnGotLoot;

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
