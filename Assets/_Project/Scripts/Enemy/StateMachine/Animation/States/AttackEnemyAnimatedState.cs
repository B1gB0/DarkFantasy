using UnityEngine;

namespace _Project.Scripts.Enemy.StateMachine.Animation.States
{
    public class AttackEnemyAnimatedState : EnemyAnimatedState
    {
        public AttackEnemyAnimatedState(Animator animator, EnemyAnimationNamesBase enemyAnimationNamesBase) 
            : base(animator, enemyAnimationNamesBase) { }
        
        public override void Enter()
        {
            base.Enter();
            Animator.CrossFade(EnemyAnimationBase.Attack, Duration);
        }
    }
}