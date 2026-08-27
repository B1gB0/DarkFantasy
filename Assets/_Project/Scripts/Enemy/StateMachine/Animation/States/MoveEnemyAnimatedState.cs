using UnityEngine;

namespace _Project.Scripts.Enemy.StateMachine.Animation.States
{
    public class MoveEnemyAnimatedState : EnemyAnimatedState
    {
        public MoveEnemyAnimatedState(Animator animator, EnemyAnimationNamesBase enemyAnimationNamesBase)
            : base(animator, enemyAnimationNamesBase) { }
        
        public override void Enter()
        {
            base.Enter();
            Animator.CrossFade(EnemyAnimationBase.Move, Duration);
        }
    }
}