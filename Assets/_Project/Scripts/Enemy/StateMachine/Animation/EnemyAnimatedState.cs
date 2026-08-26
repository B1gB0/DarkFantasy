using UnityEngine;

namespace _Project.Scripts.Enemy.StateMachine.Animation
{
    public abstract class EnemyAnimatedState
    {
        protected const float Duration = 0.1f;
        
        protected readonly Animator Animator;
        protected readonly EnemyAnimationNamesBase EnemyAnimationBase;

        protected EnemyAnimatedState(Animator animator, EnemyAnimationNamesBase enemyAnimationBase)
        {
            Animator = animator;
            EnemyAnimationBase = enemyAnimationBase;
        }

        public virtual void Enter()
        {
            Animator.StopPlayback();
            Animator.CrossFade(EnemyAnimationBase.Aim, Duration);
        }

        public virtual void Exit()
        {
            Animator.StopPlayback();
        }
    }
}