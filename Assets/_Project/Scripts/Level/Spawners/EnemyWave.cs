using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Level.Spawners
{
    [CreateAssetMenu(fileName = "EnemyWave")]
    public class EnemyWave : ScriptableObject
    {
        [field: SerializeField] public int SkeletonEnemyCount { get; private set; }
        [field: SerializeField] public int SkeletonHeavyArmorCount { get; private set; }
        [field: SerializeField] public int SkeletonRangerCount { get; private set; }

        public List<Enemy.Enemy> Enemies { get; private set; } = new();
        public List<Vector3> WaveSpawnPoints { get; private set; }
        public List<Vector3> PatrolPoints { get; private set; }

        public void GetEnemyPositions(
            List<Vector3> waveSpawnPoints,
            List<Vector3> patrolPoints)
        {
            WaveSpawnPoints = waveSpawnPoints;
            PatrolPoints = patrolPoints;
        }

        public void AddEnemy(Enemy.Enemy enemy)
        {
            Enemies.Add(enemy);
            enemy.Die += RemoveEnemy;
        }
        
        private void RemoveEnemy(Enemy.Enemy enemy)
        {
            Enemies.Remove(enemy);
            enemy.Die -= RemoveEnemy;
        }
    }
}