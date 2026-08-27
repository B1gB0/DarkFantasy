using UnityEngine;

namespace _Project.Scripts.Enemy.StateMachine.Animation.States
{
    public class Attack2EnemyAnimatedState : EnemyAnimatedState
    {
        public Attack2EnemyAnimatedState(Animator animator, EnemyAnimationNamesBase enemyAnimationNamesBase) 
            : base(animator, enemyAnimationNamesBase) { }
        
        public override void Enter()
        {
            base.Enter();
            Animator.CrossFade(EnemyAnimationBase.Attack2, Duration);
        }
    }
}