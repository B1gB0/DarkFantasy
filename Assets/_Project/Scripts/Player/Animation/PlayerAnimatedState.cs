using UnityEngine;

namespace _Project.Scripts.Player.Animation
{
    public class PlayerAnimatedState
    {
        protected readonly Animator Animator;
        protected readonly PlayerAnimationId PlayerAnimationId;

        public PlayerAnimatedState(Animator animator)
        {
            Animator = animator;
            PlayerAnimationId = new PlayerAnimationId();
        }

        public void OnMove(float speed)
        {
            Animator.SetFloat(PlayerAnimationId.Run, speed);
        }

        public void OnAttack(bool isAttacking)
        {
            Animator.SetBool(PlayerAnimationId.Attack, isAttacking);
        }

        public void OnComboChanged(int step)
        {
            Animator.SetInteger(PlayerAnimationId.ComboStep, step);
        }

        public void OnRoll()
        {
            Animator.SetTrigger(PlayerAnimationId.Roll);
        }
    }
}