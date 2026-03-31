using UnityEngine;

namespace _Project.Scripts.Enemy.StateMachine.Animation.States
{
    public class HitEnemyAnimatedState : EnemyAnimatedState
    {
        public HitEnemyAnimatedState(Animator animator, EnemyAnimationNamesBase enemyAnimationNamesBase) 
            : base(animator, enemyAnimationNamesBase) { }

        public override void Enter()
        {
            base.Enter();
            Animator.StopPlayback();
            Animator.CrossFade(EnemyAnimationBase.Hit, Duration);
        }

        public override void Exit()
        {
            base.Exit();
            Animator.StopPlayback();
        }
    }
}