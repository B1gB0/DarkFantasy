using System;
using _Project.Scripts.Player;
using UnityEngine;
using YG;

namespace _Project.Scripts.Level.Triggers
{
    public class CampfireTrigger : Trigger
    {
        [Header("Healing Settings")] 
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

            var currentState = _playerInside.StateMachine.CurentState.IdState;

            // Игрок садится
            if (_playerInside.InputController.IsPickUpPressed && currentState == StateId.Idle)
            {
                _playerInside.StateMachine.SwitchState(StateId.Sit);
                _playerInside.transform.LookAt(new Vector3(
                    transform.position.x,
                    _playerInside.transform.position.y,
                    transform.position.z));

                _healTimer = _healInterval;
                OnPlayerSatDown?.Invoke();
                
                // ВОТ ТУТ МЫ КЭШИРУЕМ ЗНАЧЕНИЕ ОДИН РАЗ ПРИ ПОСАДКЕ
                // Проверяем, что флаг еще не стоит и мы не возобновляем прерванное лечение
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

            // Используем unscaledDeltaTime, чтобы таймер работал даже при открытом инвентаре
            _healTimer += Time.unscaledDeltaTime;

            if (_healTimer >= _healInterval)
            {
                _healTimer -= _healInterval;
                
                // Вычисляем, сколько вылечить за этот тик (не больше, чем осталось)
                float amountToHeal = Mathf.Min(_healAmountPerTick, _remainingHealAmount);

                health.AddHealthSilent(amountToHeal); // ИСПРАВЛЕНО: лечим ровно amountToHeal, а не _healAmountPerTick
                _remainingHealAmount -= amountToHeal;
                
                // Если вылечили весь объем — ставим флаг
                if (_remainingHealAmount <= 0f)
                {
                    YG2.saves.IsCampfireHealed = true;
                    YG2.SaveProgress();
                }
            }
        }
    }
}