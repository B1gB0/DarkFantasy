using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Level.Spawners
{
    public class EnemyWave : ScriptableObject
    {
        [field: SerializeField] public int SkeletonEnemyCount { get; private set; }
        [field: SerializeField] public int SkeletonHeavyArmorCount { get; private set; }
        [field: SerializeField] public int SkeletonRangerCount { get; private set; }
        
        public List<Vector3> WaveSpawnPoints { get; private set; }
        public List<Vector3> PatrolPoints { get; private set; }

        public void GetEnemyPositions(
            List<Vector3> waveSpawnPoints,
            List<Vector3> patrolPoints)
        {
            WaveSpawnPoints = waveSpawnPoints;
            PatrolPoints = patrolPoints;
        }
    }
}