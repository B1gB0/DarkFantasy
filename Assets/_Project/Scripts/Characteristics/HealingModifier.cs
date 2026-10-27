using System;

namespace _Project.Scripts.Characteristics
{
    [Serializable]
    public class HealingModifier
    {
        public ModifierTimer Timer;
        public float HealPerSecond;

        public HealingModifier(float healPerSecond, float duration)
        {
            HealPerSecond = healPerSecond;
            Timer = new ModifierTimer(duration);
        }

        public bool Tick(float deltaTime) => Timer.Tick(deltaTime);
    }
}