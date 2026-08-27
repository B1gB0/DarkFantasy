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
            Animator.CrossFade(EnemyAnimationBase.Hit, Duration);
        }
    }
}