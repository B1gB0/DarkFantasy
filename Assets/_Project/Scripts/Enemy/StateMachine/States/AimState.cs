using UnityEngine;

namespace _Project.Scripts.Enemy.StateMachine.States
{
    public class AimState : AnimatedState
    {
        private const float Duration = 0.1f;

        public AimState(Animator animator, AnimationNamesBase animationNamesBase)
            : base(animator, animationNamesBase) { }

        public override void Enter()
        {
            base.Enter();
            Animator.StopPlayback();
            Animator.CrossFade(AnimationBase.Aim, Duration);
        }

        public override void Exit()
        {
            base.Exit();
            Animator.StopPlayback();
        }
    }
}