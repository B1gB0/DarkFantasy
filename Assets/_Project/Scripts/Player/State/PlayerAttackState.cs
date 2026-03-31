using _Project.Scripts.Player.Animation;
using _Project.Scripts.Player.Core;

namespace _Project.Scripts.Player
{
    public class PlayerAttackState : IPlayerState
    {
        private const int MinCombo = 0;
        private const int MaxCombo = 3;
        private const int ComboStep = 1;

        private readonly Core.Player _player;
        private readonly PlayerStateMachine _stateMachine;
        private readonly PlayerAnimatedState _playerAnimatedState;

        private bool _canQueueNextAttack;
        private int _comboCounter;
        private bool _comboExtended;

        public PlayerAttackState(
            Core.Player player,
            PlayerStateMachine stateMachine,
            PlayerAnimatedState playerAnimatedState)
        {
            _player = player;
            _stateMachine = stateMachine;
            _playerAnimatedState = playerAnimatedState;
        }

        public void Enter()
        {
            _player.InputController.OnAttackButtonPressed += OnAttackButtonPressedHandler;

            if (_player.InputController.IsAttackButtonPressed)
                OnAttackButtonPressedHandler();
        }

        public void Update()
        {
            if (!_player.InputController.IsAttackButtonPressed && _comboCounter == MinCombo)
            {
                if (_player.InputController.IsMoveInputPerformed)
                    _stateMachine.SwitchState<PlayerMoveState>();
                else
                    _stateMachine.SwitchState<PlayerIdleState>();
            }
        }

        public void FixedUpdate()
        {
        }

        public void Exit()
        {
            _player.InputController.OnAttackButtonPressed -= OnAttackButtonPressedHandler;
        }

        private void OnAttackButtonPressedHandler()
        {
            if (_comboCounter == MinCombo)
            {
                StartCombo();
            }
            else if (_canQueueNextAttack)
            {
                QueueNextCombo();
            }
        }

        private void StartCombo()
        {
            _comboCounter = 1;
            _canQueueNextAttack = false;
            _comboExtended = false;

            _playerAnimatedState.OnAttack(true);
            _playerAnimatedState.OnComboChanged(_comboCounter);
        }

        private void QueueNextCombo()
        {
            if (_comboCounter + ComboStep > MaxCombo)
            {
                _canQueueNextAttack = false;
                return;
            }

            _comboCounter++;
            _canQueueNextAttack = false;
            _comboExtended = true;
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
            if (_comboExtended)
            {
                _comboExtended = false;
                _playerAnimatedState.OnAttack(true);
                _playerAnimatedState.OnComboChanged(_comboCounter);
            }
            else
            {
                ResetCombo();
            }
        }

        private void ResetCombo()
        {
            _comboCounter = MinCombo;
            _canQueueNextAttack = false;
            _comboExtended = false;

            _playerAnimatedState.OnAttack(false);
            _playerAnimatedState.OnComboChanged(_comboCounter);
        }
    }
}