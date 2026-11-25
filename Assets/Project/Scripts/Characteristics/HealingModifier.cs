using System;
using Newtonsoft.Json;

namespace Project.Scripts.Characteristics
{
    [Serializable]
    public class HealingModifier
    {
        [JsonProperty] private ModifierTimer _timer;
        [JsonProperty] private float _healPerSecond;

        public HealingModifier()
        {
        }

        public HealingModifier(float healPerSecond, float duration)
        {
            _healPerSecond = healPerSecond;
            _timer = new ModifierTimer(duration);
        }

        [JsonIgnore] public ModifierTimer Timer => _timer;
        [JsonIgnore] public float HealPerSecond => _healPerSecond;

        public bool Tick(float deltaTime) => _timer.Tick(deltaTime);
    }
}