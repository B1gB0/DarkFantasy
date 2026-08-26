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
            Animator.applyRootMotion = true;
            base.Enter();
        }

        public override void Exit()
        {
            base.Exit();
            Animator.applyRootMotion = false;
        }
    }
}