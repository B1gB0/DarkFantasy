using UnityEngine;

namespace _Project.Scripts.Player.Animation
{
    public class PlayerAnimatedState
    {
        private const float MultiplierSpeed = 5f;

        private readonly Animator _animator;
        private readonly PlayerAnimationId _playerAnimationId;

        public PlayerAnimatedState(Animator animator)
        {
            _animator = animator;
            _playerAnimationId = new PlayerAnimationId();
        }

        public void OnMove(float speed)
        {
            float currentSpeed = _animator.GetFloat(_playerAnimationId.Run);
            
            float smoothedSpeed = Mathf.MoveTowards(
                currentSpeed, 
                speed,
                Time.deltaTime * MultiplierSpeed);
    
            _animator.SetFloat(_playerAnimationId.Run, smoothedSpeed);
        }

        public void OnAttack(bool isAttacking)
        {
            _animator.SetBool(_playerAnimationId.Attack, isAttacking);
        }

        public void OnComboChanged(int step)
        {
            _animator.SetInteger(_playerAnimationId.ComboStep, step);
        }

        public void OnRoll()
        {
            _animator.SetTrigger(_playerAnimationId.Roll);
        }

        public void ChangeAnimationSpeed(float speed)
        {
            _animator.speed = speed;
        }
        
        public void OnSitDown()
        {
            _animator.SetTrigger(_playerAnimationId.SitDown);
        }

        public void OnStandUp()
        {
            _animator.SetTrigger(_playerAnimationId.StandUp);
        }
    }
}