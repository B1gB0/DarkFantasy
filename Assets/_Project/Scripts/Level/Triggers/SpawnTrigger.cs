using System;
using UnityEngine;

namespace _Project.Scripts.Level.Triggers
{
    public class SpawnTrigger : Trigger
    {
        public event Action OnSpawnEnemies;
        
        public bool IsEnemySpawned { get; private set; }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Player.Core.Player _))
            {
                OnSpawnEnemies?.Invoke();
                IsEnemySpawned = true;
                Deactivate();
            }
        }
    }
}