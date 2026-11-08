using _Project.Scripts.Player.Animation;
using _Project.Scripts.Player.Core;
using UnityEngine;

namespace _Project.Scripts.Player
{
    public class PlayerAttackState : IPlayerState
    {
        private const int MaxCombo = 3;
        private const int MinValue = 0;
        private const int StartCounter = 1;
        
        private const float DefaultAnimationSpeed = 1f;
        private const float FallbackEndThreshold = 0.95f;
        private const float LungeForce = 7f;
        private const float MultiplierLungeForce = 20f;

        private readonly Core.Player _player;
        private readonly PlayerAnimatedState _anim;
        private readonly PlayerStateMachine _stateMachine;
        private readonly Transform _cameraTransform;

        private float _currentLungeForce;
        private bool _isComboWindowOpen;
        private bool _isAttackBuffered;
        private int _comboCounter;
        private bool _fallbackEndInvoked;
        
        private Vector3 _lockedAttackDirection; 

        public PlayerAttackState(Core.Player player)
        {
            _player = player;
            _anim = player.PlayerAnimatedState;
            _stateMachine = player.StateMachine;
            
            _cameraTransform = UnityEngine.Camera.main.transform;
        }

        public StateId IdState => StateId.Attack;

        public void Enter()
        {
            _anim.OnMove(MinValue); 
            _player.InputController.OnAttackButtonPressed += OnAttackButtonPressed;
            
            Vector2 input = _player.InputController.MoveDirection;
            
            if (input.sqrMagnitude > 0.01f || _player.InputController.IsMoveInputPerformed)
            {
                _currentLungeForce = LungeForce;
                
                // Берем направление от камеры
                Vector3 attackDir = _cameraTransform.forward;
                attackDir.y = 0f; 
                attackDir.Normalize();
                
                if (attackDir.sqrMagnitude > 0.01f)
                {
                    _lockedAttackDirection = attackDir;
                    
                    _player.Rigidbody.rotation = Quaternion.LookRotation(_lockedAttackDirection);
                    _player.Rigidbody.angularVelocity = Vector3.zero;
                }
            }
            else
            {
                _currentLungeForce = MinValue; 
                _lockedAttackDirection = _player.transform.forward;
            }
            
            float animationSpeed = _player.PlayerCharacteristics?.GetAttackSpeedMultiplier() ?? DefaultAnimationSpeed;
            _anim.ChangeAnimationSpeed(animationSpeed);
            
            StartCombo();
            
            _fallbackEndInvoked = false;
        }

        public void Update()
        {
            _anim.OnMove(MinValue);
            
            AnimatorStateInfo stateInfo = _player.Animator.GetCurrentAnimatorStateInfo(MinValue);
            if (stateInfo.IsTag("Attack") && stateInfo.normalizedTime >= FallbackEndThreshold && !_fallbackEndInvoked)
            {
                _fallbackEndInvoked = true;
                EndAttack();
            }
            
            if (_player.InputController.IsRollInputPerformed)
                _stateMachine.SwitchState(StateId.Roll);
        }

        public void FixedUpdate()
        {
            if (_currentLungeForce > 0.1f)
            {
                _player.Rigidbody.velocity =
                    _lockedAttackDirection * _currentLungeForce +
                    new Vector3(MinValue, _player.Rigidbody.velocity.y, MinValue);
                
                _currentLungeForce = Mathf.MoveTowards(
                        _currentLungeForce,
                        MinValue,
                        Time.fixedDeltaTime * MultiplierLungeForce);
            }
        }

        public void Exit()
        {
            _player.InputController.OnAttackButtonPressed -= OnAttackButtonPressed;
            _anim.ChangeAnimationSpeed(DefaultAnimationSpeed);
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
            _comboCounter = StartCounter;
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
            _comboCounter = MinValue;
            _isComboWindowOpen = false;
            _isAttackBuffered = false;
            _anim.OnAttack(false);
            _anim.OnComboChanged(_comboCounter);
        }
    }
}