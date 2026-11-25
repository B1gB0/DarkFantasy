using UnityEngine;

namespace Project.Scripts.Enemy.StateMachine.Animation
{
    public class EnemyAnimationNamesBase
    {
        public int Idle { get; } = Animator.StringToHash(nameof(Idle));
        public int Move { get; } = Animator.StringToHash(nameof(Move));
        public int Attack { get; } = Animator.StringToHash(nameof(Attack));
        public int Attack1 { get; } = Animator.StringToHash(nameof(Attack1));
        public int Attack2 { get; } = Animator.StringToHash(nameof(Attack2));
        public int Attack3 { get; } = Animator.StringToHash(nameof(Attack3));
        public int Dodge { get; } = Animator.StringToHash(nameof(Dodge));
        public int Aim { get; } = Animator.StringToHash(nameof(Aim));
        public int Load { get; } = Animator.StringToHash(nameof(Load));
        public int Hit { get; } = Animator.StringToHash(nameof(Hit));
        public int Coil { get; } = Animator.StringToHash(nameof(Coil));
        public int Omni { get; } = Animator.StringToHash(nameof(Omni));
        public int Death { get; } = Animator.StringToHash(nameof(Death));
    }
}