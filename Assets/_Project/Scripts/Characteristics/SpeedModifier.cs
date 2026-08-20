using System;

namespace _Project.Scripts.Characteristics
{
    public class SpeedModifier
    {
        public float Value;      // абсолютное значение или множитель
        public bool IsMultiplier; // true – множитель, false – абсолютное прибавление
        public DateTime EndTime; // момент окончания действия (или использовать UniTask)

        public SpeedModifier(float value, bool isMultiplier, float duration)
        {
            Value = value;
            IsMultiplier = isMultiplier;
            EndTime = DateTime.UtcNow.AddSeconds(duration);
        }
    }
}