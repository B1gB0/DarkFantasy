using System;
using _Project.Scripts.Player.State;
using UnityEngine;
using YG;

namespace _Project.Scripts.Level.Triggers
{
    public class CampfireTrigger : Trigger
    {
        [SerializeField] private float _healAmountPerTick = 2f;
        [SerializeField] private float _healInterval = 1f;
        [Range(0f, 1f)] [SerializeField] private float _maxHealPercent = 0.5f;

        private Player.Core.Player _playerInside;

        private float _healTimer;
        private float _remainingHealAmount;

        private StateId _lastState;

        public event Action OnPlayerInside;
        public event Action OnPlayerOutside;
        public event Action OnPlayerSatDown;
        public event Action OnPlayerStoodUp;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Player.Core.Player player))
            {
                OnPlayerInside?.Invoke();
                _playerInside = player;
                _healTimer = _healInterval;
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
            if (_playerInside == null) return;

            var currentState = _playerInside.StateMachine.CurrentState.IdState;

            if (_playerInside.InputController.IsPickUpPressed && currentState == StateId.Idle)
            {
                _playerInside.StateMachine.SwitchState(StateId.Sit);
                _playerInside.transform.LookAt(new Vector3(
                    transform.position.x,
                    _playerInside.transform.position.y,
                    transform.position.z));

                _healTimer = _healInterval;
                OnPlayerSatDown?.Invoke();

                if (!YG2.saves.IsCampfireHealed && _remainingHealAmount <= 0f)
                {
                    var health = _playerInside.Health;
                    if (health != null)
                    {
                        _remainingHealAmount = health.TargetHealth * _maxHealPercent;
                    }
                }
            }

            if (_lastState == StateId.Sit && currentState != StateId.Sit)
            {
                OnPlayerStoodUp?.Invoke();
            }

            _lastState = currentState;

            if (currentState == StateId.Sit)
            {
                HealPlayer();
            }
        }

        private void HealPlayer()
        {
            if (_remainingHealAmount <= 0f) return;

            var health = _playerInside.Health;
            if (health == null) return;

            _healTimer += Time.unscaledDeltaTime;

            if (_healTimer >= _healInterval)
            {
                _healTimer -= _healInterval;

                float amountToHeal = Mathf.Min(_healAmountPerTick, _remainingHealAmount);

                health.AddHealthSilent(amountToHeal);
                _remainingHealAmount -= amountToHeal;

                if (_remainingHealAmount <= 0f)
                {
                    YG2.saves.IsCampfireHealed = true;
                    YG2.SaveProgress();
                }
            }
        }
    }
}