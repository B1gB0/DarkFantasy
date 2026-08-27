using UnityEngine;

namespace _Project.Scripts.Enemy.StateMachine.Animation.States
{
    public class OmniEnemyAnimatedState : EnemyAnimatedState
    {
        public OmniEnemyAnimatedState(Animator animator, EnemyAnimationNamesBase enemyAnimationBase)
            : base(animator, enemyAnimationBase) { }
        
        public override void Enter()
        {
            base.Enter();
            Animator.CrossFade(EnemyAnimationBase.Omni, Duration);
        }
    }
}