using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Level.Spawners
{
    public class EnemyWave
    {
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