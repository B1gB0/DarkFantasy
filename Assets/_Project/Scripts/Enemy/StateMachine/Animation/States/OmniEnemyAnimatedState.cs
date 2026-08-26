using UnityEngine;

namespace _Project.Scripts.Enemy.StateMachine.Animation.States
{
    public class OmniEnemyAnimatedState : EnemyAnimatedState
    {
        public OmniEnemyAnimatedState(Animator animator, EnemyAnimationNamesBase enemyAnimationBase)
            : base(animator, enemyAnimationBase) { }
    }
}