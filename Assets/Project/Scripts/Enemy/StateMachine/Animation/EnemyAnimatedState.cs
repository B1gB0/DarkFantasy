using UnityEngine;

namespace Project.Scripts.Enemy.StateMachine.Animation
{
    public abstract class EnemyAnimatedState
    {
        protected const float Duration = 0.1f;

        private readonly Animator _animator;
        private readonly EnemyAnimationNamesBase _enemyAnimationBase;

        protected EnemyAnimatedState(Animator animator, EnemyAnimationNamesBase enemyAnimationBase)
        {
            _animator = animator;
            _enemyAnimationBase = enemyAnimationBase;
        }

        protected Animator Animator => _animator;
        protected EnemyAnimationNamesBase EnemyAnimationBase => _enemyAnimationBase;

        public virtual void Enter()
        {
            _animator.StopPlayback();
        }

        public virtual void Exit()
        {
            _animator.StopPlayback();
        }
    }
}