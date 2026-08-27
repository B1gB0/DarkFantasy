using _Project.Scripts.Player.Animation;
using _Project.Scripts.Player.Core;
using UnityEngine;

namespace _Project.Scripts.Player
{
    public class PlayerAttackState : IPlayerState
    {
        private const int MaxCombo = 3;
        private const float FallbackEndThreshold = 0.95f;

        private readonly Core.Player _player;
        private readonly PlayerAnimatedState _anim;
        private readonly PlayerStateMachine _stateMachine;

        private bool _isComboWindowOpen;
        private bool _isAttackBuffered;
        private int _comboCounter;
        private bool _fallbackEndInvoked;

        public PlayerAttackState(Core.Player player)
        {
            _player = player;
            _anim = player.PlayerAnimatedState;
            _stateMachine = player.StateMachine;
        }

        public StateId IdState => StateId.Attack;

        public void Enter()
        {
            _player.InputController.OnAttackButtonPressed += OnAttackButtonPressed;
            StartCombo();
            _fallbackEndInvoked = false;
        }

        public void Update()
        {
            AnimatorStateInfo stateInfo = _player.Animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsTag("Attack") && stateInfo.normalizedTime >= FallbackEndThreshold && !_fallbackEndInvoked)
            {
                _fallbackEndInvoked = true;
                EndAttack();
            }
        }

        public void FixedUpdate() { }

        public void Exit()
        {
            _player.InputController.OnAttackButtonPressed -= OnAttackButtonPressed;
            ResetCombo();
        }

        public void StartDamageWindow() => _player.Sword.ActivateCollider();
        public void EndDamageWindow()   => _player.Sword.DeactivateCollider();

        public void EndAttack()
        {
            ResetCombo();

            if (_player.InputController.IsRollInputPerformed)
                _stateMachine.SwitchState(StateId.Roll);
            else if (_player.InputController.IsMoveInputPerformed)
                _stateMachine.SwitchState(StateId.Move);
            else
                _stateMachine.SwitchState(StateId.Idle);
        }

        public void AllowCombo()
        {
            _isComboWindowOpen = true;

            if (_isAttackBuffered)
            {
                ContinueCombo();
            }
        }

        private void StartCombo()
        {
            _comboCounter = 1;
            _isComboWindowOpen = false;
            _isAttackBuffered = false;
            _fallbackEndInvoked = false;

            _anim.OnAttack(true);
            _anim.OnComboChanged(_comboCounter);
        }

        private void OnAttackButtonPressed()
        {
            if (_comboCounter >= MaxCombo)
                return;

            if (_isComboWindowOpen)
            {
                ContinueCombo();
            }
            else
            {
                _isAttackBuffered = true;
            }
        }

        private void ContinueCombo()
        {
            _isAttackBuffered = false;
            _isComboWindowOpen = false;
            _fallbackEndInvoked = false;

            _comboCounter++;
            _anim.OnComboChanged(_comboCounter);
        }

        private void ResetCombo()
        {
            _comboCounter = 0;
            _isComboWindowOpen = false;
            _isAttackBuffered = false;
            _anim.OnAttack(false);
            _anim.OnComboChanged(_comboCounter);
        }
    }
}