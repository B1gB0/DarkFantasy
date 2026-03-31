using UnityEngine;

namespace _Project.Scripts.Player.Animation
{
    public class PlayerAnimatedState
    {
        protected readonly Animator Animator;
        protected readonly PlayerAnimationNamesBase PlayerAnimationNamesBase;
        
        public PlayerAnimatedState(Animator animator)
        {
            Animator = animator;
            PlayerAnimationNamesBase = new PlayerAnimationNamesBase();
        }

        public void OnMove(float speed)
        {
            Animator.SetFloat(PlayerAnimationNamesBase.Run, speed);
        }

        public void OnAttack(bool isAttacking)
        {
            Debug.Log(isAttacking);
            Animator.SetBool(PlayerAnimationNamesBase.Attack, isAttacking);
        }

        public void OnComboChanged(int step)
        {
            Animator.SetInteger(PlayerAnimationNamesBase.ComboStep, step);
        }
    }
}