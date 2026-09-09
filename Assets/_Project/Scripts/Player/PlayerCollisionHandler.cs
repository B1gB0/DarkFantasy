using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Level.Spawners;
using _Project.Scripts.Level.Triggers;
using UnityEngine;

namespace _Project.Scripts.Player
{
    [RequireComponent(typeof(Core.Player))]
    public class PlayerCollisionHandler : MonoBehaviour
    {
        private List<EnemyWave> _enemyWaves;
        
        private void OnTriggerEnter(Collider trigger)
        {
            if (!trigger.TryGetComponent(out EnemyWaveFollowTrigger followTrigger)) return;
            foreach (var enemy in followTrigger.NumberWaveOfEnemies.SelectMany(number => _enemyWaves[number].Enemies))
            {
                enemy.ChangeFollowEnemyState(true);
            }
        }
        
        public void GetEnemyWaves(List<EnemyWave> enemyWaves)
        {
            _enemyWaves = enemyWaves;
        }
    }
}