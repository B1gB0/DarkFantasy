using UnityEngine;

namespace _Project.Scripts.Enemy.StateMachine.Animation.States
{
    public class DeathAnimatedState : EnemyAnimatedState
    {
        public DeathAnimatedState(Animator animator, EnemyAnimationNamesBase enemyAnimationBase)
            : base(animator, enemyAnimationBase)
        {
        }
        
        public override void Enter()
        {
            base.Enter();
            Animator.StopPlayback();
            Animator.CrossFade(EnemyAnimationBase.Coil, Duration);
        }

        public override void Exit()
        {
            base.Exit();
            Animator.StopPlayback();
        }
    }
}