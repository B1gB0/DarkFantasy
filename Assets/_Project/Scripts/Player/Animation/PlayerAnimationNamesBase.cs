using UnityEngine;

namespace _Project.Scripts.Player.Animation
{
    public class PlayerAnimationNamesBase
    {
        public readonly int Run = Animator.StringToHash(nameof(Run));
        public readonly int Attack = Animator.StringToHash(nameof(Attack));
        public readonly int ComboStep = Animator.StringToHash(nameof(ComboStep));
    }
}