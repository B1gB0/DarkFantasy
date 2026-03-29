using UnityEngine;

namespace _Project.Scripts.Enemy.StateMachine.Animation
{
    public abstract class AnimatedState
    {
        protected const float Duration = 0.1f;
        
        protected readonly Animator Animator;
        protected readonly AnimationNamesBase AnimationBase;

        protected AnimatedState(Animator animator, AnimationNamesBase animationBase)
        {
            Animator = animator;
            AnimationBase = animationBase;
        }

        public virtual void Enter() { }

        public virtual void Exit() { }
    }
}