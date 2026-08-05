using System;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Effects;
using _Project.Scripts.Enemy.StateMachine.Animation;
using _Project.Scripts.Enemy.StateMachine.Behaviour;
using _Project.Scripts.Experience;
using _Project.Scripts.Services;
using UnityEngine;

namespace _Project.Scripts.Enemy
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public abstract class Enemy : MonoBehaviour, IAcceptable, IExperienceScoreActor
    {
        [SerializeField] private Animator _animator;

        protected IFloatingTextService FloatingTextService;
        protected AudioSoundsService AudioSoundsService;
        protected ParticleEffectsService ParticleEffectsService;
        protected IExperiencePoints ExperiencePoints;
        protected ICurrencyService CurrencyService;

        public event Action<Enemy> Die;

        [field: SerializeField] public Health Health { get; private set; }
        [field: SerializeField] public EnemyStateMachine EnemyStateMachine { get; private set; }
        [field: SerializeField] public Rigidbody Rigidbody { get; private set; }
        [field: SerializeField] public Collider Collider { get; private set; }

        public int Experience { get; private set; }
        public int Score { get; private set; }
        public bool IsEnemy { get; private set; }

        public EnemyData Data { get; private set; }
        public Player.Core.Player Player { get; private set; }
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
            Player.Core.Player player,
            EnemyData enemyData,
            IFloatingTextService floatingTextService,
            ParticleEffectsService particleEffectsService,
            AudioSoundsService audioSoundsService,
            IExperiencePoints experiencePoints,
            ICurrencyService currencyService)
        {
            Player = player;
            Data = enemyData;
            Type = Data.Type;

            Score = Data.Score;
            Experience = Data.Experience;
            IsEnemy = true;

            FloatingTextService = floatingTextService;
            ParticleEffectsService = particleEffectsService;
            AudioSoundsService = audioSoundsService;
            ExperiencePoints = experiencePoints;
            CurrencyService = currencyService;

            Armor = Data.Armor;

            Health.IsSpawnedDamageText += FloatingTextService.OnSpawnFloatingText;
        }

        public void ChangeFollowEnemyState(bool canFollow)
        {
            CanFollow = canFollow;
        }
        
        public void AcceptScore(IScoreActorVisitor visitor)
        {
            visitor.Visit(this);
            CurrencyService.AddGold(Data.Gold);
        }

        public virtual void OnReactState(bool isEnteredToState)
        {
        }

        protected virtual void OnDie()
        {
            // ResetModifiers();
            Health.IsSpawnedDamageText -= FloatingTextService.OnSpawnFloatingText;
            // OnChangeSpeed -= UpdateCurrentSpeed;

            Die?.Invoke(this);

            // gameObject.SetActive(false);
        }

        protected virtual void OnPlayHitEffect()
        {
            ParticleEffectsService.PlayEffect(ParticleType.BasicHit, Health.HitPoint.position);
        }
    }
}