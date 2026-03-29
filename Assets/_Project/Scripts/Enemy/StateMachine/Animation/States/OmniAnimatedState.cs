using UnityEngine;

namespace _Project.Scripts.Enemy.StateMachine.Animation.States
{
    public class OmniAnimatedState : AnimatedState
    {
        public OmniAnimatedState(Animator animator, AnimationNamesBase animationBase) : base(animator, animationBase)
        {
        }
        
        public override void Enter()
        {
            base.Enter();
            Animator.StopPlayback();
            Animator.CrossFade(AnimationBase.Coil, Duration);
        }

        public override void Exit()
        {
            base.Exit();
            Animator.StopPlayback();
        }
    }
}