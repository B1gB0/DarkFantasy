using UnityEngine;

namespace _Project.Scripts.Enemy.StateMachine.Animation.States
{
    public class IdleAnimatedState : AnimatedState
    {
        public IdleAnimatedState(Animator animator, AnimationNamesBase animationNamesBase)
            : base(animator, animationNamesBase) { }

        public override void Enter()
        {
            base.Enter();
            Animator.StopPlayback();
            Animator.CrossFade(AnimationBase.Idle, Duration);
        }

        public override void Exit()
        {
            base.Exit();
            Animator.StopPlayback();
        }
    }
}