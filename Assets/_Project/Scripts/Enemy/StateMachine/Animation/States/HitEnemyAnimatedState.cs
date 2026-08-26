using UnityEngine;

namespace _Project.Scripts.Enemy.StateMachine.Animation.States
{
    public class HitEnemyAnimatedState : EnemyAnimatedState
    {
        public HitEnemyAnimatedState(Animator animator, EnemyAnimationNamesBase enemyAnimationNamesBase) 
            : base(animator, enemyAnimationNamesBase) { }
    }
}