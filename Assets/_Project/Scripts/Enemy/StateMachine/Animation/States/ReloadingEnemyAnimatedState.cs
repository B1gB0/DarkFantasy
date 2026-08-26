using UnityEngine;

namespace _Project.Scripts.Enemy.StateMachine.Animation.States
{
    public class ReloadingEnemyAnimatedState : EnemyAnimatedState
    {
        public ReloadingEnemyAnimatedState(Animator animator, EnemyAnimationNamesBase enemyAnimationBase)
            : base(animator, enemyAnimationBase) { }
    }
}