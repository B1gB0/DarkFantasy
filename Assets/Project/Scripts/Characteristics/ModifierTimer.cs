using System;
using Newtonsoft.Json;

namespace Project.Scripts.Characteristics
{
    [Serializable]
    public struct ModifierTimer
    {
        private const float Epsilon = 0.05f;
        private const float MinValue = 0f;

        [JsonProperty] private float _totalDuration;
        [JsonProperty] private float _remainingTime;

        public ModifierTimer(float duration)
        {
            _totalDuration = duration;
            _remainingTime = duration;
        }

        [JsonIgnore] public bool IsActive => _remainingTime > Epsilon;

        public float GetTotalDuration() => _totalDuration;

        public float GetRemainingTime() => _remainingTime;

        public bool Tick(float deltaTime)
        {
            _remainingTime -= deltaTime;
            return _remainingTime <= MinValue;
        }
    }
}