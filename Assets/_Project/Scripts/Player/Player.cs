using _Project.Scripts.Characteristics;
using _Project.Scripts.Effects;
using _Project.Scripts.Services;
using UnityEngine;

namespace _Project.Scripts.Player
{
    public class Player : MonoBehaviour
    {
        private ParticleEffectsService _particleEffectsService;

        [field: SerializeField] public Health Health { get; private set; }

        public PlayerCharacteristics PlayerCharacteristics { get; private set; }

        public bool CanFollow { get; private set; }

        private void OnEnable()
        {
            Health.IsDamaged += OnPlayHitEffect;
        }

        private void OnDisable()
        {
            Health.IsDamaged -= OnPlayHitEffect;
        }

        private void OnDestroy()
        {
            Health.TargetHealthChanged -= PlayerCharacteristics.SaveTargetHealth;
        }

        public void Construct(
            PlayerCharacteristics playerCharacteristics,
            ParticleEffectsService particleEffectsService)
        {
            PlayerCharacteristics = playerCharacteristics;
            _particleEffectsService = particleEffectsService;
            Health.TargetHealthChanged += PlayerCharacteristics.SaveTargetHealth;
        }

        public void ChangeFollowEnemyState(bool canFollow)
        {
            CanFollow = canFollow;
        }

        private void OnPlayHitEffect()
        {
            _particleEffectsService.PlayEffect(ParticleType.RedBloodHit, Health.HitPoint.position);
        }
    }
}