using UnityEngine;

namespace _Project.Scripts.Enemy.StateMachine.Animation
{
    public class AnimationNamesBase
    {
        public readonly int Idle = Animator.StringToHash(nameof(Idle));
        public readonly int Move = Animator.StringToHash(nameof(Move));
        public readonly int Attack = Animator.StringToHash(nameof(Attack));
        public readonly int Aim = Animator.StringToHash(nameof(Aim));
    }
}