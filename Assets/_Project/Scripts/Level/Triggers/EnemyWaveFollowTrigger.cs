using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Level.Triggers
{
    public class EnemyWaveFollowTrigger : Trigger
    {
        [field: SerializeField] public List<int> NumberWaveOfEnemies { get; private set; }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Player.Core.Player _))
                Deactivate();
        }
    }
}