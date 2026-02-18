using System;
using _Project.Scripts.Level.Triggers;
using Reflex.Attributes;
using UnityEngine;

namespace _Project.Scripts.Player
{
    [RequireComponent(typeof(Player))]
    public class PlayerCollisionHandler : MonoBehaviour
    {
        private Player _player;
        
        // private IPlayerService _playerService;
        //
        // [Inject]
        // private void Construct(IPlayerService playerService)
        // {
        //     _playerService = playerService;
        // }

        private void Awake()
        {
            _player = GetComponent<Player>();
        }

        private void OnTriggerEnter(Collider trigger)
        {
            if (trigger.TryGetComponent(out SkeletonRangerTrigger skeletonRangerTrigger))
            {
                _player.ChangeFollowEnemyState(true);
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
    }
}