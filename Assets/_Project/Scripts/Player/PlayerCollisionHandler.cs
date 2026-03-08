using System.Collections.Generic;
using _Project.Scripts.Level.Spawners;
using _Project.Scripts.Level.Triggers;
using UnityEngine;

namespace _Project.Scripts.Player
{
    [RequireComponent(typeof(Player))]
    public class PlayerCollisionHandler : MonoBehaviour
    {
        private Player _player;

        public List<EnemyWave> EnemyWaves { get; private set; }

        private void Awake()
        {
            _player = GetComponent<Player>();
        }

        private void OnTriggerEnter(Collider trigger)
        {
            if (trigger.TryGetComponent(out EnemyWaveFollowTrigger skeletonRangerTrigger))
            {
                foreach (var enemy in EnemyWaves[skeletonRangerTrigger.NumberWaveOfEnemies].Enemies)
                {
                    enemy.ChangeFollowEnemyState(true);
                }
            }
            
            // if (trigger.TryGetComponent(out EntranceTrigger entranceTrigger))
            // {
            //     entranceTrigger.Entrance.OpenGate();
            // }
        }
        
        private void OnTriggerExit(Collider trigger)
        {
            // if (trigger.TryGetComponent(out EntranceTrigger entranceTrigger))
            // {
            //     entranceTrigger.Entrance.CloseGate();
            // }
        }
        //
        // private void OnCollisionEnter(Collision collision)
        // {
        //     if (collision.gameObject.TryGetComponent(out RedCrystal healingCrystal))
        //     {
        //         if (_playerService.PlayerActor.Health.TargetHealth == _playerService.PlayerActor.Health.MaxHealth)
        //             return;
        //
        //         _playerService.PlayerActor.Health.AddHealth(healingCrystal.HealthValue);
        //         healingCrystal.Destroy();
        //     }
        //     else if (collision.gameObject.TryGetComponent(out GoldCrystal goldCrystal))
        //     {
        //         goldCrystal.Destroy();
        //     }
        // }

        public void GetEnemyWaves(List<EnemyWave> enemyWaves)
        {
            EnemyWaves = enemyWaves;
        }
    }
}