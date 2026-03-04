using UnityEngine;

namespace _Project.Scripts.Level.Triggers
{
    public class EnemyWaveFollowTrigger : Trigger
    {
        [field: SerializeField] public int NumberWaveOfEnemies { get; private set; }
    }
}