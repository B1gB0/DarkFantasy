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
        private readonly PlayerAnimatedState _playerAnimatedState;
        private readonly PlayerStateMachine _stateMachine;

        private bool _canQueueNextAttack;
        private bool _comboExtended;
        private int _comboCounter;

        public PlayerAttackState(Core.Player player)
        {
            _player = player;
            _playerAnimatedState = _player.PlayerAnimatedState;
            _stateMachine = _player.StateMachine;
        }

        public StateId IdState => StateId.Attack;

        public void Enter()
        {
            if (_player.InputController.IsAttackButtonPressed)
                OnAttackPressedHandler();
        }

        public void Update()
        {
        }

        public void FixedUpdate()
        {
        }

        public void Exit()
        {
        }


        private void OnAttackPressedHandler()
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