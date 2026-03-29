using UnityEngine;

namespace _Project.Scripts.Enemy.StateMachine.Animation.States
{
    public class HitAnimatedState : AnimatedState
    {
        public HitAnimatedState(Animator animator, AnimationNamesBase animationNamesBase) 
            : base(animator, animationNamesBase) { }

        public override void Enter()
        {
            base.Enter();
            Animator.StopPlayback();
            Animator.CrossFade(AnimationBase.Hit, Duration);
        }

        public override void Exit()
        {
            base.Exit();
            Animator.StopPlayback();
        }
    }
}