using UnityEngine;

namespace _Project.Scripts.Enemy.StateMachine.Animation.States
{
    public class ReloadingEnemyAnimatedState : EnemyAnimatedState
    {
        public ReloadingEnemyAnimatedState(Animator animator, EnemyAnimationNamesBase enemyAnimationBase)
            : base(animator, enemyAnimationBase) { }

        public override void Enter()
        {
            base.Enter();
            Animator.StopPlayback();
            Animator.CrossFade(EnemyAnimationBase.Load, Duration);
        }

        public override void Exit()
        {
            base.Exit();
            Animator.StopPlayback();
        }
    }
}