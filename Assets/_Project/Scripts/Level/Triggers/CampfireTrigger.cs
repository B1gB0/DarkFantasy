using System;
using _Project.Scripts.Player;
using UnityEngine;

namespace _Project.Scripts.Level.Triggers
{
    public class CampfireTrigger : Trigger
    {
        private Player.Core.Player _playerInside;
        
        public event Action OnPlayerInside;
        public event Action OnPlayerOutside;
        public event Action OnCampfireVisited;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Player.Core.Player player))
            {
                OnPlayerInside?.Invoke();
                _playerInside = player;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Player.Core.Player player) && player == _playerInside)
            {
                OnPlayerOutside?.Invoke();
                _playerInside = null;
            }
        }

        private void Update()
        {
            if (_playerInside != null && _playerInside.InputController.IsPickUpPressed)
            {
                OnCampfireVisited?.Invoke();
                
                _playerInside.StateMachine.SwitchState(StateId.Sit);
                
                _playerInside.transform.LookAt(new Vector3(transform.position.x, _playerInside.transform.position.y, transform.position.z));
            }
        }
    }
}