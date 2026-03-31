using _Project.Scripts.Characteristics;
using _Project.Scripts.Effects;
using _Project.Scripts.Player.Combat;
using _Project.Scripts.Services;
using UnityEngine;

namespace _Project.Scripts.Player.Core
{
    public class Player : MonoBehaviour 
    {
        private ParticleEffectsService _particleEffectsService;
        private PlayerStateMachine _stateMachine;
        private Movement.Movement _movement;
        private Attack _attack;

        [field: SerializeField] public Health Health { get; private set; }
        [field: SerializeField] public PlayerCollisionHandler PlayerCollisionHandler { get; private set; }

        public PlayerCharacteristics PlayerCharacteristics { get; private set; }
        
        public bool CanFollow { get; private set; }

        private void Awake()
        {
            _stateMachine = GetComponent<PlayerStateMachine>();
            _movement = GetComponent<Movement.Movement>();
            _attack = GetComponent<Attack>();
        }

        private void Start()
        {
            _stateMachine.SetState(new PlayerIdleState(this, _stateMachine, _movement, _attack));
        }

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