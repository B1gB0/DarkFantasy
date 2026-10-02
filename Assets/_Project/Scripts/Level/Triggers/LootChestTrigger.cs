using System;
using UnityEngine;

namespace _Project.Scripts.Level.Triggers
{
    public class LootChestTrigger : Trigger
    {
        public event Action OnGotLoot;
        
        public bool IsLootGot { get; private set; }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Player.Core.Player _))
            {
                OnGotLoot?.Invoke();
                IsLootGot = true;
                Deactivate();
            }
        }
    }
}