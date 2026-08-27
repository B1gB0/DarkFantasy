using System;
using System.Threading;
using _Project.Scripts.Game.Constant;
using _Project.Scripts.UI.View;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts
{
    public class Health : MonoBehaviour
    {
        private const int MinValue = 0;
        private const float DamageFirstFactor = 100f;
        private const float DamageSecondFactor = 1f;
        private const float RecoveryRate = 10f;

        [SerializeField] private float _value;
        [SerializeField] private Transform _hitPoint;

        private CancellationTokenSource _healthCts;

        public event Action Die;
        public event Action<Health> DieHealth;

        public event Action<string, Transform, FloatingTextViewType, Color> IsSpawnedDamageText;
        // public event Action<string, Transform, FloatingTextViewType, Color> IsSpawnedHealingText;

        public event Action IsDamaged;

        public event Action<float, float, float> HealthChanged;
        public event Action<float> TargetHealthChanged;

        public float MaxHealth { get; private set; }
        public float TargetHealth { get; private set; }
        public float CurrentHealth { get; private set; }
        public bool IsHealingModifierActivated { get; private set; }

        public bool IsHitting { get; private set; }

        public Transform HitPoint => _hitPoint;

        private void Start()
        {
            HealthChanged?.Invoke(CurrentHealth, MaxHealth, TargetHealth);
            TargetHealthChanged?.Invoke(TargetHealth);
        }

        private void OnDestroy()
        {
            _healthCts?.Cancel();
        }

        public void TakeDamage(float damage, bool isShowTextDamage = false, float armor = MinValue)
        {
            if (TargetHealth == MinValue)
                return;

            IsDamaged?.Invoke();

            float finalDamage;

            if (armor > 0)
            {
                float damageReduction = armor / (armor + DamageFirstFactor);
                finalDamage = damage * (DamageSecondFactor - damageReduction);
                finalDamage = Mathf.Max(MinValue, finalDamage);
            }
            else
            {
                finalDamage = damage - armor;
            }

            IsSpawnedDamageText?.Invoke(damage.ToString(),
                transform,
                FloatingTextViewType.Damage,
                Colors.GetColor(ColorName.DefaultWhiteTextColor));

            TargetHealth -= finalDamage;

            OnChangeHealth();

            if (TargetHealth < MinValue)
                TargetHealth = MinValue;

            if (TargetHealth == MinValue)
            {
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
            // IsSpawnedHealingText?.Invoke(
            //     healthValue.ToString(),
            //     transform,
            //     FloatingTextViewType.Healing,
            //     Colors.GetColor(ColorName.HealingColor));

            TargetHealth += healthValue;

            OnChangeHealth();

            if (TargetHealth > MaxHealth)
                TargetHealth = MaxHealth;
        }
        
        public bool TryStartHealingOverTime(float totalAmount, float duration)
        {
            if (IsHealingModifierActivated)
                return false;

            IsHealingModifierActivated = true;
            AddHealthOverTime(totalAmount, duration).Forget();
            return true;
        }

        public void SetHealthValue(float healthValue)
        {
            _value = healthValue;
            TargetHealth = _value;

            OnChangeHealth();
        }

        public void SetHit(bool isHitting)
        {
            IsHitting = isHitting;
        }
        
        private async UniTaskVoid AddHealthOverTime(float totalAmount, float duration)
        {
            try
            {
                float healPerSecond = totalAmount / duration;
                float elapsed = 0f;

                while (elapsed < duration)
                {
                    float deltaTime = Time.deltaTime;
                    float healThisFrame = healPerSecond * deltaTime;
                    AddHealth(healThisFrame);

                    elapsed += deltaTime;
                    await UniTask.NextFrame();
                }
            }
            finally
            {
                IsHealingModifierActivated = false;
            }
        }

        private void OnChangeHealth()
        {
            _healthCts?.Cancel();
            _healthCts = new CancellationTokenSource();

            ChangeHealthAsync(_healthCts.Token).Forget();
        }

        private async UniTaskVoid ChangeHealthAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested &&
                   Math.Abs(CurrentHealth - TargetHealth) > Mathf.Epsilon)
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
    }
}