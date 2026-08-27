using UnityEngine;

namespace _Project.Scripts.Enemy.StateMachine.Animation.States
{
    public class Attack1EnemyAnimatedState : EnemyAnimatedState
    {
        public Attack1EnemyAnimatedState(Animator animator, EnemyAnimationNamesBase enemyAnimationNamesBase) 
            : base(animator, enemyAnimationNamesBase) { }
        
        public override void Enter()
        {
            base.Enter();
            Animator.CrossFade(EnemyAnimationBase.Attack1, Duration);
        }
    }
}