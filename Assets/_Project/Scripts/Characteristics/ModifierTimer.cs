using System;

namespace _Project.Scripts.Characteristics
{
    [Serializable]
    public struct ModifierTimer
    {
        private const float Epsilon = 0.05f;
        private const float MinValue = 0f;
        
        public float TotalDuration;
        public float RemainingTime;

        public ModifierTimer(float duration)
        {
            TotalDuration = duration;
            RemainingTime = duration;
        }

        public bool IsActive => RemainingTime > Epsilon;

        public float Progress01 =>
            TotalDuration <= MinValue ? MinValue : Math.Max(MinValue, RemainingTime / TotalDuration);
        
        public bool Tick(float deltaTime)
        {
            RemainingTime -= deltaTime;
            return RemainingTime <= MinValue;
        }
    }
}