using _Project.Scripts.Effects;
using _Project.Scripts.Enemy.StateMachine.Animation.States;
using UnityEngine;

namespace _Project.Scripts.Enemy.StateMachine.Behaviour.States
{
    public class AttackState : EnemyState
    {
        private const float OmniRange = 3f;
        private const float OffsetHeight = 0.7f;
        private const float AttackChance = 0.5f;

        private float _attackRange;

        private float _reloadDuration = 2f;
        private float _aimDuration = 2f;
        private float _attackDuration = 2f;

        private AttackSubState _currentSubState;
        private PriestAttackState _lastPriestRangedAttack = PriestAttackState.None;
        private float _subStateTimer;

        public override void Enter()
        {
            _currentSubState = Enemy.Type == EnemyType.SkeletonRanger ? AttackSubState.Aiming : AttackSubState.Idle;
            _attackRange = Data.RangeAttack;
        }

        public override void Update()
        {
        }

        public override void FixedUpdate()
        {
            if (Player == null
                || !Player.CanFollow
                || Player.Health.TargetHealth <= MinValue
                || Enemy.Health.TargetHealth <= MinValue)
            {
                EnemyStateMachine.SwitchState<PatrolState>();
                return;
            }

            float distanceToPlayer = Vector3.Distance(Enemy.transform.position, Player.transform.position);

            if (distanceToPlayer > _attackRange)
            {
                EnemyStateMachine.SwitchState<FollowState>();
                return;
            }

            Vector3 direction = (Player.transform.position - Enemy.transform.position).normalized;
            float rotationSpeed = Data.RotationSpeed;

            Enemy.transform.forward = Vector3.RotateTowards(
                Enemy.transform.forward,
                direction,
                rotationSpeed * Time.fixedDeltaTime,
                MinValue);

            _subStateTimer -= Time.fixedDeltaTime;

            if (_subStateTimer <= MinValue)
            {
                if (Enemy.Type == EnemyType.SkeletonRanger)
                {
                    switch (_currentSubState)
                    {
                        case AttackSubState.Reloading:
                            EnterAttackSubState(AttackSubState.Aiming);
                            break;
                        case AttackSubState.Aiming:
                            Enemy.OnReactState(false);
                            EnterAttackSubState(AttackSubState.Attack);
                            break;
                        case AttackSubState.Attack:
                            Enemy.OnReactState(true);
                            EnterAttackSubState(AttackSubState.Reloading);
                            break;
                    }
                }
                else if (Enemy.Type == EnemyType.Priest)
                {
                    float distance = Vector3.Distance(Enemy.transform.position, Player.transform.position);

                    if (distance <= OmniRange)
                    {
                        EnterInOmniAttack();
                    }
                    else
                    {
                        PriestAttackState nextAttack = GetNextRangedAttack();
                        if (nextAttack == PriestAttackState.Fireball)
                            EnterInFireballAttack();
                        else
                            EnterInCoilAttack();
                    }
                }
                else
                {
                    switch (_currentSubState)
                    {
                        case AttackSubState.Attack:
                            EnterAttackSubState(AttackSubState.Idle);
                            break;
                        case AttackSubState.Idle:
                            EnterAttackSubState(AttackSubState.Attack);
                            Player.Health.TakeDamage(Enemy.Data.Damage, Player.PlayerCharacteristics.Armor);
                            break;
                    }
                }

                Debug.Log(_currentSubState);
            }
        }
        
        private PriestAttackState GetNextRangedAttack()
        {
            _lastPriestRangedAttack = Random.value > AttackChance ? PriestAttackState.Fireball : PriestAttackState.Coil;
            return _lastPriestRangedAttack;
        }

        private void EnterAttackSubState(AttackSubState newSubState)
        {
            _currentSubState = newSubState;
            switch (_currentSubState)
            {
                case AttackSubState.Reloading:
                    AnimStateMachine.EnterIn<ReloadingEnemyAnimatedState>();
                    _subStateTimer = _reloadDuration;
                    break;
                case AttackSubState.Aiming:
                    AnimStateMachine.EnterIn<AimEnemyAnimatedState>();
                    _subStateTimer = _aimDuration;
                    break;
                case AttackSubState.Attack:
                    AnimStateMachine.EnterIn<AttackEnemyAnimatedState>();
                    _subStateTimer = _attackDuration;
                    break;
                case AttackSubState.Idle:
                    AnimStateMachine.EnterIn<IdleEnemyAnimatedState>();
                    _subStateTimer = _aimDuration;
                    break;
                case AttackSubState.Coil:
                    AnimStateMachine.EnterIn<CoilEnemyAnimatedState>();
                    _subStateTimer = _attackDuration;
                    break;
                case AttackSubState.Omni:
                    AnimStateMachine.EnterIn<OmniEnemyAnimatedState>();
                    _subStateTimer = _attackDuration;
                    break;
            }
        }

        private void EnterInFireballAttack()
        {
            switch (_currentSubState)
            {
                case AttackSubState.Idle:
                    EnterAttackSubState(AttackSubState.Attack);
                    break;
                case AttackSubState.Attack:
                    EnterAttackSubState(AttackSubState.Idle);
                    break;
                default:
                    EnterAttackSubState(AttackSubState.Idle);
                    ParticleEffectsService.StopEffect(ParticleType.ShieldEffect);
                    ParticleEffectsService.StopEffect(ParticleType.MagicChargeBlue);
                    break;
            }
        }

        private void EnterInCoilAttack()
        {
            switch (_currentSubState)
            {
                case AttackSubState.Idle:
                    EnterAttackSubState(AttackSubState.Coil);
                    break;
                case AttackSubState.Coil:
                    EnterAttackSubState(AttackSubState.Idle);
                    break;
                default:
                    EnterAttackSubState(AttackSubState.Idle);
                    ParticleEffectsService.StopEffect(ParticleType.ShieldEffect);
                    ParticleEffectsService.StopEffect(ParticleType.MagicChargeBlue);
                    break;
            }
        }

        private void EnterInOmniAttack()
        {
            switch (_currentSubState)
            {
                case AttackSubState.Idle:
                    var position = Enemy.transform.position;
                    position.y += OffsetHeight;
                    ParticleEffectsService.PlayEffect(ParticleType.ShieldEffect, position);
                    ParticleEffectsService.PlayEffect(ParticleType.MagicChargeBlue, position);
                    EnterAttackSubState(AttackSubState.Aiming);
                    break;
                case AttackSubState.Aiming:
                    ParticleEffectsService.StopEffect(ParticleType.ShieldEffect);
                    ParticleEffectsService.StopEffect(ParticleType.MagicChargeBlue);
                    EnterAttackSubState(AttackSubState.Omni);
                    break;
                case AttackSubState.Omni:
                    EnterAttackSubState(AttackSubState.Idle);
                    break;
                default:
                    EnterAttackSubState(AttackSubState.Idle);
                    ParticleEffectsService.StopEffect(ParticleType.ShieldEffect);
                    ParticleEffectsService.StopEffect(ParticleType.MagicChargeBlue);
                    break;
            }
        }
    }
}