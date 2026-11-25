using UnityEngine;

namespace Project.Scripts.Player.Animation
{
    public class PlayerAnimationId
    {
        public int Run { get; } = Animator.StringToHash(nameof(Run));
        public int Attack { get; } = Animator.StringToHash(nameof(Attack));
        public int ComboStep { get; } = Animator.StringToHash(nameof(ComboStep));
        public int Roll { get; } = Animator.StringToHash(nameof(Roll));
        public int SitDown { get; } = Animator.StringToHash(nameof(SitDown));
        public int StandUp { get; } = Animator.StringToHash(nameof(StandUp));
    }
}