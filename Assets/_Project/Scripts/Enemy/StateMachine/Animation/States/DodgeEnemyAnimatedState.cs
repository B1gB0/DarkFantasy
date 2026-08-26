using UnityEngine;

namespace _Project.Scripts.Enemy.StateMachine.Animation.States
{
    public class DodgeEnemyAnimatedState : EnemyAnimatedState
    {
        public DodgeEnemyAnimatedState(Animator animator, EnemyAnimationNamesBase enemyAnimationNamesBase) 
            : base(animator, enemyAnimationNamesBase) { }
    }
}