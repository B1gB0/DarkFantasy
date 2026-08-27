using UnityEngine;

namespace _Project.Scripts.Enemy.StateMachine.Animation.States
{
    public class IdleEnemyAnimatedState : EnemyAnimatedState
    {
        public IdleEnemyAnimatedState(Animator animator, EnemyAnimationNamesBase enemyAnimationNamesBase)
            : base(animator, enemyAnimationNamesBase) { }
        
        public override void Enter()
        {
            base.Enter();
            Animator.CrossFade(EnemyAnimationBase.Idle, Duration);
        }
    }
}