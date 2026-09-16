using System;

namespace _Project.Scripts.Characteristics
{
    [Serializable]
    public class HealingModifier
    {
        public ModifierTimer Timer;
        public float HealPerSecond;

        public HealingModifier(float totalAmount, float duration)
        {
            HealPerSecond = duration > 0f ? totalAmount / duration : 0f;
            Timer = new ModifierTimer(duration);
        }

        public bool Tick(float deltaTime) => Timer.Tick(deltaTime);
    }
}