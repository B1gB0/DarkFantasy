using _Project.Scripts.Characteristics;
using _Project.Scripts.Effects;
using _Project.Scripts.Player.Animation;
using _Project.Scripts.Player.Input;
using _Project.Scripts.Services;
using UnityEngine;
using Sword = _Project.Scripts.Player.Combat.Sword;

namespace _Project.Scripts.Player.Core
{
    public class Player : MonoBehaviour
    {
        [field: SerializeField] public Health Health { get; private set; }
        [field: SerializeField] public PlayerCollisionHandler PlayerCollisionHandler { get; private set; }
        [field: SerializeField] public Sword Sword { get; private set; }

        private ParticleEffectsService _particleEffectsService;

        private Animator _animator;
        private Rigidbody _rigidbody;
        private InputController _inputController;

        private PlayerStateMachine _stateMachine;
        private PlayerAnimatedState _playerAnimatedState;

        private PlayerIdleState _playerIdleState;
        private PlayerAttackState _playerAttackState;
        private PlayerMoveState _playerMoveState;
        private PlayerRollState _playerRollState;

        public Animator Animator => _animator;
        public Rigidbody Rigidbody => _rigidbody;

        public PlayerStateMachine StateMachine => _stateMachine;
        public InputController InputController => _inputController;
        public PlayerAnimatedState PlayerAnimatedState => _playerAnimatedState;

        public PlayerCharacteristics PlayerCharacteristics { get; private set; }
        public PlayerAttackState PlayerAttackState => _playerAttackState; 

        public bool CanFollow { get; private set; } = true;

        private void Awake()
        {
            Initialize();
        }

        private void OnEnable()
        {
            if (Health != null)
                Health.IsDamaged += OnPlayHitEffect;
        }

        private void OnDisable()
        {
            if (Health != null)
                Health.IsDamaged -= OnPlayHitEffect;
        }

        private void OnDestroy()
        {
            if (PlayerCharacteristics != null && Health != null)
                Health.TargetHealthChanged -= PlayerCharacteristics.SaveTargetHealth;
        }

        public void Construct(
            PlayerCharacteristics playerCharacteristics,
            ParticleEffectsService particleEffectsService)
        {
            PlayerCharacteristics = playerCharacteristics;
            _particleEffectsService = particleEffectsService;

            if (Health != null && PlayerCharacteristics != null)
                Health.TargetHealthChanged += PlayerCharacteristics.SaveTargetHealth;
        }

        public void ChangeFollowEnemyState(bool canFollow)
        {
            CanFollow = canFollow;
        }

        private void OnPlayHitEffect()
        {
            if (_particleEffectsService != null && Health != null && Health.HitPoint != null)
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

            _playerIdleState = new PlayerIdleState(this);
            _playerAttackState = new PlayerAttackState(this);
            _playerMoveState = new PlayerMoveState(this);
            _playerRollState = new PlayerRollState(this);

            _stateMachine.AddState(_playerIdleState);
            _stateMachine.AddState(_playerMoveState);
            _stateMachine.AddState(_playerRollState);
            _stateMachine.AddState(_playerAttackState);

            _stateMachine.SwitchState(StateId.Idle);
        }

        public void AnimationEvent_AllowCombo()
        {
            _playerAttackState.AllowCombo();
        }

        public void AnimationEvent_StartDamage()
        {
            _playerAttackState.StartDamageWindow();
        }

        public void AnimationEvent_EndDamage()
        {
            _playerAttackState.EndDamageWindow();
        }

        public void AnimationEvent_EndAttack()
        {
            _playerAttackState.EndAttack();
        }
    }
}