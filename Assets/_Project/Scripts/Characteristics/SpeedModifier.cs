using System;

namespace _Project.Scripts.Characteristics
{
    [Serializable]
    public class SpeedModifier
    {
        public ModifierTimer Timer;
        public float Value;
        public bool IsMultiplier;

        public SpeedModifier(float value, bool isMultiplier, float duration)
        {
            Value = value;
            IsMultiplier = isMultiplier;
            Timer = new ModifierTimer(duration);
        }

        public bool Tick(float deltaTime) => Timer.Tick(deltaTime);
    }
}