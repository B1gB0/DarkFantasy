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
        private Core.Player _player;

        public List<EnemyWave> EnemyWaves { get; private set; }

        private void Awake()
        {
            _player = GetComponent<Core.Player>();
        }

        private void OnTriggerEnter(Collider trigger)
        {
            if (trigger.TryGetComponent(out EnemyWaveFollowTrigger followTrigger))
            {
                foreach (var enemy in followTrigger.NumberWaveOfEnemies.SelectMany(number => EnemyWaves[number].Enemies))
                {
                    enemy.ChangeFollowEnemyState(true);
                }
            }
        }

        public void GetEnemyWaves(List<EnemyWave> enemyWaves)
        {
            EnemyWaves = enemyWaves;
        }
    }
}