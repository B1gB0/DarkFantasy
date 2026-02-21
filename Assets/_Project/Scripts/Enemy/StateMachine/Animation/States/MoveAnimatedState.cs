using UnityEngine;

namespace _Project.Scripts.Enemy.StateMachine.Animation.States
{
    public class MoveAnimatedState : AnimatedState
    {
        private const float Duration = 0.1f;

        public MoveAnimatedState(Animator animator, AnimationNamesBase animationNamesBase)
            : base(animator, animationNamesBase) { }

        public override void Enter()
        {
            base.Enter();
            Animator.StopPlayback();
            Animator.CrossFade(AnimationBase.Move, Duration);
        }

        public override void Exit()
        {
            base.Exit();
            Animator.StopPlayback();
        }
    }
}