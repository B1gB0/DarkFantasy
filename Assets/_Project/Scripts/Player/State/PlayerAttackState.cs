using _Project.Scripts.Player.Animation;
using _Project.Scripts.Player.Core;
using UnityEngine.InputSystem;

namespace _Project.Scripts.Player
{
    public class PlayerAttackState : IPlayerState
    {
        private const int MinCombo = 0;
        private const int MaxCombo = 3;

        private readonly Core.Player _player;
        private readonly PlayerAnimatedState _anim;
        private readonly PlayerStateMachine _stateMachine;

        private bool _canQueueNextAttack;
        private int _comboCounter;

        public PlayerAttackState(Core.Player player)
        {
            _player = player;
            _anim = player.PlayerAnimatedState;
            _stateMachine = player.StateMachine;
        }

        public StateId IdState => StateId.Attack;

        public void Enter()
        {
            StartCombo();
        }

        public void Update()
        {
            if (Mouse.current.leftButton.isPressed)
            {
                QueueNextCombo();
            }
        }

        public void FixedUpdate()
        {
        }

        public void Exit()
        {
            ResetCombo();
        }

        private void StartCombo()
        {
            _comboCounter = 1;
            _canQueueNextAttack = false;

            _anim.OnAttack(true);
            _anim.OnComboChanged(_comboCounter);
        }

        public void AllowCombo()
        {
            _canQueueNextAttack = true;
        }

        public void QueueNextCombo()
        {
            if (!_canQueueNextAttack)
                return;

            if (_comboCounter > MaxCombo)
            {
                _canQueueNextAttack = false;
                return;
            }

            _comboCounter++;
            _canQueueNextAttack = false;

            _anim.OnComboChanged(_comboCounter);
        }

        public void StartDamageWindow()
        {
            _player.Sword.ActivateCollider();
        }

        public void EndDamageWindow()
        {
            _player.Sword.DeactivateCollider();
        }

        public void EndAttack()
        {
            _anim.OnAttack(true);
            _anim.OnComboChanged(_comboCounter);
            
            ResetCombo();

            if (_player.InputController.IsRollInputPerformed)
            {
                _stateMachine.SwitchState(StateId.Roll);
                return;
            }

            if (_player.InputController.IsMoveInputPerformed)
            {
                _stateMachine.SwitchState(StateId.Move);
                return;
            }

            _stateMachine.SwitchState(StateId.Idle);
        }

        private void ResetCombo()
        {
            _comboCounter = MinCombo;
            _canQueueNextAttack = false;

            _anim.OnAttack(false);
            _anim.OnComboChanged(_comboCounter);
        }
    }
}