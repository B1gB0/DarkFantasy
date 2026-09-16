using System;
using System.Threading;
using _Project.Scripts.Characteristics;
using _Project.Scripts.Game.Constant;
using _Project.Scripts.UI.View;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts
{
    public class Health : MonoBehaviour
    {
        private const float MinValue = 0f;
        private const float CancellationBorder = 0.01f;
        private const float MinAliveHealth = 1f;
        private const float DamageFirstFactor = 100f;
        private const float DamageSecondFactor = 1f;
        private const float RecoveryRate = 10f;
        
        [SerializeField] private Transform _hitPoint;

        private CancellationTokenSource _healthCts;
        private HealingModifier _healingModifier;

        public event Action Die;
        public event Action<Health> DieHealth;

        public event Action<string, Transform, FloatingTextViewType, Color> IsSpawnedDamageText;
        public event Action<string, Transform, FloatingTextViewType, Color> IsSpawnedHealingText;

        public event Action IsDamaged;

        public event Action<float, float, float> HealthChanged;
        public event Action<float> TargetHealthChanged;

        public float MaxHealth { get; private set; }
        public float TargetHealth { get; private set; }
        public float CurrentHealth { get; private set; }

        public bool IsHitting { get; private set; }

        public Transform HitPoint => _hitPoint;
        public HealingModifier HealingModifier => _healingModifier;
        

        private void Start()
        {
            HealthChanged?.Invoke(CurrentHealth, MaxHealth, TargetHealth);
            TargetHealthChanged?.Invoke(TargetHealth);
        }
        
        private void Update()
        {
            if (_healingModifier == null || !_healingModifier.Timer.IsActive)
                return;

            float dt = Time.deltaTime;
            float heal = _healingModifier.HealPerSecond * dt;
            AddHealthSilent(heal);

            if (_healingModifier.Tick(dt))
                _healingModifier = null;
        }

        private void OnDestroy()
        {
            _healthCts?.Cancel();
        }

        public void TakeDamage(float damage, bool isShowTextDamage = false, float armor = MinValue)
        {
            if (TargetHealth <= MinValue)
                return;

            IsDamaged?.Invoke();

            float finalDamage = CalculateFinalDamage(damage, armor);

            IsSpawnedDamageText?.Invoke(
                damage.ToString(),
                transform,
                FloatingTextViewType.Damage,
                Colors.GetColor(ColorName.DefaultWhiteTextColor));

            TargetHealth -= finalDamage;

            OnChangeHealth();

            if (TargetHealth == MinValue)
            {
                _healingModifier = null;
                _healthCts?.Cancel();
                Die?.Invoke();
                DieHealth?.Invoke(this);
            }
        }

        public void ImproveHealth(float newHealthValue)
        {
            var currentHealthPercentage = TargetHealth / MaxHealth;
            var maxHealth = MaxHealth + newHealthValue;

            MaxHealth = maxHealth;
            var currentHealth = MaxHealth * currentHealthPercentage;

            SetHealthValue(currentHealth);
        }

        public void LoadHealth(float maxHealth, float targetHealth)
        {
            MaxHealth = maxHealth;

            SetHealthValue(targetHealth);
        }

        public void AddHealth(float healthValue)
        {
            IsSpawnedHealingText?.Invoke(
                healthValue.ToString(),
                transform,
                FloatingTextViewType.Healing,
                Colors.GetColor(ColorName.HealingColor));

            TargetHealth += healthValue;

            OnChangeHealth();
        }
        
        public void RestoreHealingState(HealingModifier state)
        {
            _healingModifier = state;
        }
        
        public bool TryStartHealingOverTime(float totalAmount, float duration)
        {
            if (duration <= MinValue)
                return false;

            if (_healingModifier != null && _healingModifier.Timer.IsActive)
                return false;

            _healingModifier = new HealingModifier(totalAmount, duration);
            return true;
        }

        public void SetHealthValue(float healthValue)
        {
            TargetHealth = healthValue;

            OnChangeHealth();
        }
        
        private float CalculateFinalDamage(float damage, float armor)
        {
            if (armor <= MinValue)
                return Mathf.Max(MinValue, damage - armor);

            float reduction = armor / (armor + DamageFirstFactor);
            return Mathf.Max(MinValue, damage * (DamageSecondFactor - reduction));
        }
        
        private void AddHealthSilent(float value)
        {
            TargetHealth += value;
            OnChangeHealth();
        }

        private void OnChangeHealth()
        {
            TargetHealth = Mathf.Clamp(TargetHealth, MinValue, MaxHealth);
            
            if (TargetHealth is > MinValue and < MinAliveHealth)
                TargetHealth = MinAliveHealth;

            if (_healthCts != null && !_healthCts.IsCancellationRequested) return;
            _healthCts = new CancellationTokenSource();
            ChangeHealthAsync(_healthCts.Token, _healthCts).Forget();
        }

        private async UniTaskVoid ChangeHealthAsync(
            CancellationToken cancellationToken,
            CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested &&
                       Mathf.Abs(CurrentHealth - TargetHealth) > CancellationBorder)
                {
                    CurrentHealth = Mathf.MoveTowards(
                        CurrentHealth,
                        TargetHealth,
                        RecoveryRate * Time.unscaledDeltaTime);

                    HealthChanged?.Invoke(CurrentHealth, MaxHealth, TargetHealth);
                    TargetHealthChanged?.Invoke(TargetHealth);

                    await UniTask.NextFrame(PlayerLoopTiming.Update, cancellationToken);
                }
            }
            finally
            {
                if (!cancellationTokenSource.IsCancellationRequested)
                    cancellationTokenSource.Cancel();
            }
        }
    }
}