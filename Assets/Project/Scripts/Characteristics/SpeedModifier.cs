using System;
using Newtonsoft.Json;

namespace Project.Scripts.Characteristics
{
    [Serializable]
    public class SpeedModifier
    {
        [JsonProperty] private ModifierTimer _timer;
        [JsonProperty] private float _value;
        [JsonProperty] private bool _isMultiplier;

        public SpeedModifier(float value, bool isMultiplier, float duration)
        {
            _value = value;
            _isMultiplier = isMultiplier;
            _timer = new ModifierTimer(duration);
        }

        [JsonIgnore] public ModifierTimer Timer => _timer;
        [JsonIgnore] public float Value => _value;
        [JsonIgnore] public bool IsMultiplier => _isMultiplier;

        public bool Tick(float deltaTime) => _timer.Tick(deltaTime);
    }
}