using System;
using _Project.Scripts.Player.Animation;
using _Project.Scripts.Player.Core;

namespace _Project.Scripts.Player
{
    public class PlayerAttackState : IPlayerState
    {
        private readonly Core.Player _player;
        private readonly PlayerStateMachine _stateMachine;
        private readonly PlayerAnimatedState _playerAnimatedState;

        private bool _canQueueNextAttack;
        private int _comboStep;

        public PlayerAttackState(
            Core.Player player,
            PlayerStateMachine stateMachine,
            PlayerAnimatedState playerAnimatedState)
        {
            _player = player;
            _stateMachine = stateMachine;
            _playerAnimatedState = playerAnimatedState;
        }

        public void Enter() { }

        public void Update()
        {
            if (!_player.InputController.IsAttackButtonPressed && _comboStep == 0)
            {
                if (_player.InputController.IsMoveInputPerformed)
                    _stateMachine.SwitchState<PlayerMoveState>();
                else
                    _stateMachine.SwitchState<PlayerIdleState>();
                
                return;
            }
            
            OnAttackPerformed();
        }

        public void FixedUpdate() { }

        public void Exit() { }

        public void OnAttackPerformed()
        {
            if (!_player.IsAttacking)
            {
                StartCombo();
                return;
            }

            if (_canQueueNextAttack)
                QueueNextCombo();
        }

        private void StartCombo()
        {
            _comboStep = 1;
            _canQueueNextAttack = false;
            
            _playerAnimatedState.OnAttack(true);
            _playerAnimatedState.OnComboChanged(_comboStep);
        }

        private void QueueNextCombo()
        {
            _comboStep++;
            _canQueueNextAttack = false;
            
            _playerAnimatedState.OnComboChanged(_comboStep);
        }

        public void AllowCombo()
        {
            _canQueueNextAttack = true;
        }

        public void StartDamageWindow()
        {
            _player.SwordHitbox.ActivateHitBox();
        }
        
        public void EndDamageWindow()
        {
            _player.SwordHitbox.DeactivateHitBox();
        }

        public void EndAttack()
        {
            ResetCombo();
        }

        private void ResetCombo()
        {
            _comboStep = 0;
            _canQueueNextAttack = false;
            
            _playerAnimatedState.OnAttack(false);
            _playerAnimatedState.OnComboChanged(_comboStep);
        }
    }
}