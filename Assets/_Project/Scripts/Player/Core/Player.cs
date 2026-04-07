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
        [field: SerializeField] public Health Health { get; private set; }
        [field: SerializeField] public PlayerCollisionHandler PlayerCollisionHandler { get; private set; }
        [field: SerializeField] public SwordHitbox SwordHitbox { get; private set; }

        private ParticleEffectsService _particleEffectsService;

        private Animator _animator;
        private Rigidbody _rigidbody;
        private InputController _inputController;

        private PlayerStateMachine _stateMachine;
        private PlayerAnimatedState _playerAnimatedState;
        
        public Animator Animator => _animator;
        public Rigidbody Rigidbody => _rigidbody;

        public PlayerStateMachine StateMachine => _stateMachine;
        public InputController InputController => _inputController;
        public PlayerAnimatedState PlayerAnimatedState => _playerAnimatedState;

        public PlayerCharacteristics PlayerCharacteristics { get; private set; }

        public bool CanFollow { get; private set; }

        private void Awake()
        {
            Initialize();
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

        //Не совсем понятно для чего 
        public void ChangeFollowEnemyState(bool canFollow)
        {
            CanFollow = canFollow;
        }

        private void OnPlayHitEffect()
        {
            _particleEffectsService.PlayEffect(ParticleType.RedBloodHit, Health.HitPoint.position);
        }

        private void Initialize()
        {
            _animator = GetComponent<Animator>();
            _rigidbody = GetComponent<Rigidbody>();
            _inputController = GetComponent<InputController>();
            _stateMachine = GetComponent<PlayerStateMachine>();
            
            _stateMachine.Initialize(this);

            _playerAnimatedState = new PlayerAnimatedState(Animator);

            PlayerIdleState playerIdleState = new PlayerIdleState(this);
            PlayerAttackState playerAttackState = new PlayerAttackState(this);
            PlayerMoveState playerMoveState = new PlayerMoveState(this);
            PlayerRollState playerRollState = new PlayerRollState(this);

            _stateMachine.AddState(playerIdleState);
            _stateMachine.AddState(playerMoveState);
            _stateMachine.AddState(playerRollState);
            _stateMachine.AddState(playerAttackState);

            _stateMachine.SwitchState(StateId.Idle);
        }
    }
}