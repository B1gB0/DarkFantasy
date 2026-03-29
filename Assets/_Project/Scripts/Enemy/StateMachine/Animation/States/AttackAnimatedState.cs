using UnityEngine;

namespace _Project.Scripts.Enemy.StateMachine.Animation.States
{
    public class AttackAnimatedState : AnimatedState
    {
        public AttackAnimatedState(Animator animator, AnimationNamesBase animationNamesBase) 
            : base(animator, animationNamesBase) { }

        public override void Enter()
        {
            base.Enter();
            Animator.StopPlayback();
            Animator.CrossFade(AnimationBase.Attack, Duration);
        }

        public override void Exit()
        {
            base.Exit();
            Animator.StopPlayback();
        }
    }
}