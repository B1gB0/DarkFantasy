using System;
using _Project.Scripts.Characteristics;
using _Project.Scripts.Effects;
using _Project.Scripts.Player.Animation;
using _Project.Scripts.Player.Combat;
using _Project.Scripts.Player.Input;
using _Project.Scripts.Services;
using UnityEngine;

namespace _Project.Scripts.Player.Core
{
    public class Player : MonoBehaviour 
    {
        private ParticleEffectsService _particleEffectsService;
        private PlayerStateMachine _stateMachine;

        [field: SerializeField] public Health Health { get; private set; }
        [field: SerializeField] public PlayerCollisionHandler PlayerCollisionHandler { get; private set; }
        [field: SerializeField] public SwordHitbox SwordHitbox { get; private set; }

        public Animator Animator { get; private set; }
        public PlayerAnimatedState PlayerAnimatedState { get; private set; }
        public PlayerCharacteristics PlayerCharacteristics { get; private set; }
        public bool CanFollow { get; private set; } = true;
        public InputController InputController { get; private set; }
        public Rigidbody Rigidbody { get; private set; }
        public PlayerAttackState PlayerAttackState { get; private set; }

        private void Awake()
        {
            Animator = GetComponent<Animator>();
            InputController = GetComponent<InputController>();
            Rigidbody = GetComponent<Rigidbody>();
            
            PlayerAnimatedState = new PlayerAnimatedState(Animator);
            _stateMachine = GetComponent<PlayerStateMachine>();

            PlayerAttackState = new PlayerAttackState(this, _stateMachine, PlayerAnimatedState);
        }

        private void Start()
        {
            _stateMachine.AddState(PlayerAttackState);
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

        public void AllowCombo()
        {
            PlayerAttackState.AllowCombo();
        }

        public void StartDamageWindow()
        {
            SwordHitbox.ActivateHitBox();
        }
        
        public void EndDamageWindow()
        {
            SwordHitbox.DeactivateHitBox();
        }

        public void EndAttack()
        {
            PlayerAttackState.EndAttack();
        }

        private void OnPlayHitEffect()
        {
            _particleEffectsService.PlayEffect(ParticleType.RedBloodHit, Health.HitPoint.position);
        }
    }
}