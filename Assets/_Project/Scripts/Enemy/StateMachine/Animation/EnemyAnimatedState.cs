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

        public virtual void Enter() { }

        public virtual void Exit() { }
    }
}