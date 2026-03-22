using System;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Effects;
using _Project.Scripts.Enemy.StateMachine.Animation;
using _Project.Scripts.Enemy.StateMachine.Behaviour;
using _Project.Scripts.Services;
using Project.Scripts.Services;
using UnityEngine;

namespace _Project.Scripts.Enemy
{
    [RequireComponent(typeof(Animator))]
    public abstract class Enemy : MonoBehaviour
    {
        [SerializeField] private Animator _animator;

        protected IFloatingTextService FloatingTextService;
        protected AudioSoundsService AudioSoundsService;
        protected ParticleEffectsService ParticleEffectsService;
        
        public event Action<Enemy> Die;

        [field: SerializeField] public Health Health { get; private set; }
        [field: SerializeField] public EnemyStateMachine EnemyStateMachine { get; private set; }
        
        public EnemyData Data { get; private set; }
        public Player.Player Player { get; private set; }
        public EnemyAnimatedStateMachine AnimatedStateMachine { get; private set; }
        public EnemyType Type { get; private set; }
        public bool CanFollow { get; private set; }
        public float Armor { get; private set; }

        private void Start()
        {
            AnimatedStateMachine = new EnemyAnimatedStateMachine(_animator);
        }
        
        private void OnEnable()
        {
            Health.Die += OnDie;
            Health.IsDamaged += OnPlayHitEffect;
        }

        private void OnDisable()
        {
            Health.Die -= OnDie;
            Health.IsDamaged -= OnPlayHitEffect;
        }

        public void Construct(
            Player.Player player,
            EnemyData enemyData,
            IFloatingTextService floatingTextService,
            ParticleEffectsService particleEffectsService,
            AudioSoundsService audioSoundsService)
        {
            Player = player;
            Data = enemyData;
            Type = Data.Type;

            FloatingTextService = floatingTextService;
            ParticleEffectsService = particleEffectsService;
            AudioSoundsService = audioSoundsService;

            Armor = Data.Armor;
            
            Health.IsSpawnedDamageText += FloatingTextService.OnSpawnFloatingText;
        }

        public void ChangeFollowEnemyState(bool canFollow)
        {
            CanFollow = canFollow;
        }

        protected virtual void OnDie()
        {
            // ResetModifiers();
            Health.IsSpawnedDamageText -= FloatingTextService.OnSpawnFloatingText;
            // OnChangeSpeed -= UpdateCurrentSpeed;
            
            Die?.Invoke(this);
            
            gameObject.SetActive(false);
        }
        
        protected virtual void OnPlayHitEffect()
        {
            ParticleEffectsService.PlayEffect(ParticleType.RedBloodHit, Health.HitPoint.position);
        }
    }
}