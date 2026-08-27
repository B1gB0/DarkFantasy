using UnityEngine;

namespace _Project.Scripts.Enemy.StateMachine.Animation.States
{
    public class AimEnemyAnimatedState : EnemyAnimatedState
    {
        public AimEnemyAnimatedState(Animator animator, EnemyAnimationNamesBase enemyAnimationNamesBase)
            : base(animator, enemyAnimationNamesBase) { }

        public override void Enter()
        {
            base.Enter();
            Animator.CrossFade(EnemyAnimationBase.Aim, Duration);
        }
    }
}