using _Project.Scripts.Effects;
using _Project.Scripts.Enemy.StateMachine.Animation.States;
using UnityEngine;

namespace _Project.Scripts.Enemy.StateMachine.Behaviour.States
{
    public class AttackState : EnemyState
    {
        private const float OmniRange = 3f;
        private const float OffsetHeight = 1f;
        private const float AttackChance = 0.5f;
        private const float FireballChance = 0.33f;
        private const float CoilChance = 0.66f;

        private float _attackRange;
        private float _reloadDuration = 0.4f;
        private float _aimDuration = 0.85f;
        private float _idleDuration = 1f;
        private float _attackDuration = 2f;
        private float _priestAimDuration = 1f;

        private int _comboLength;
        private int _currentComboIndex;
        private bool _comboInProgress;
        private bool _canDodge;

        private float _comboChance1 = 0.3f;
        private float _comboChance2 = 0.4f;
        private float _comboChance3 = 0.3f;
        private float _dodgeChance = 0.2f;
        private float _attack1Duration = 0.8f;
        private float _attack2Duration = 1.0f;
        private float _attack3Duration = 1.2f;
        private float _dodgeDuration = 0.6f;
        private float _dodgeDistance = 3f;
        private float _dodgeSpeed = 6f;

        private AttackSubState _currentSubState;
        private PriestAttackState _lastPriestRangedAttack = PriestAttackState.None;
        private PriestAttackState _activePriestAttack = PriestAttackState.None;
        private float _subStateTimer;

        public override void Enter()
        {
            _currentSubState = Enemy.Type is EnemyType.BanditRanger
                or EnemyType.SkeletonRanger
                ? AttackSubState.Aiming
                : AttackSubState.Idle;

            _attackRange = Data.RangeAttack;
            _activePriestAttack = PriestAttackState.None;
        }

        public override void Update()
        {
        }

        public override void FixedUpdate()
        {
            if (Enemy.Health.CurrentHealth <= MinValue)
            {
                EnemyStateMachine.SwitchState<DeathState>();
                return;
            }

            if (Player == null
                || !Player.CanFollow
                || Player.Health.TargetHealth <= MinValue)
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

            if (!(_subStateTimer <= MinValue)) return;

            switch (Enemy.Type)
            {
                case EnemyType.SkeletonRanger:
                case EnemyType.BanditRanger:
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

                    break;
                case EnemyType.Priest:
                {
                    if (_activePriestAttack == PriestAttackState.None)
                    {
                        float distance = Vector3.Distance(Enemy.transform.position, Player.transform.position);
                        _activePriestAttack = ChoosePriestAttack(distance);
                    }

                    switch (_activePriestAttack)
                    {
                        case PriestAttackState.Fireball:
                            EnterInFireballAttack();
                            break;
                        case PriestAttackState.Coil:
                            EnterInCoilAttack();
                            break;
                        case PriestAttackState.Omni:
                            EnterInOmniAttack();
                            break;
                    }

                    break;
                }
                case EnemyType.BanditLeader:

                    break;
                default:
                    switch (_currentSubState)
                    {
                        case AttackSubState.Attack:
                            EnterAttackSubState(AttackSubState.Idle);
                            break;
                        case AttackSubState.Idle:
                            EnterAttackSubState(AttackSubState.Attack);
                            break;
                    }

                    break;
            }
        }

        public override void Exit()
        {
            ParticleEffectsService.StopEffect(ParticleType.ShieldEffect);
            ParticleEffectsService.StopEffect(ParticleType.MagicChargeBlue);
            _activePriestAttack = PriestAttackState.None;
        }

        private PriestAttackState ChoosePriestAttack(float distance)
        {
            if (!(distance <= OmniRange))
                return Random.value > AttackChance ? PriestAttackState.Fireball : PriestAttackState.Coil;

            float rand = Random.value;

            switch (rand)
            {
                case < FireballChance:
                    return PriestAttackState.Fireball;
                case < CoilChance:
                    return PriestAttackState.Coil;
                default:
                    return PriestAttackState.Omni;
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
                    _activePriestAttack = PriestAttackState.None;
                    break;
                default:
                    ParticleEffectsService.StopEffect(ParticleType.ShieldEffect);
                    ParticleEffectsService.StopEffect(ParticleType.MagicChargeBlue);
                    EnterAttackSubState(AttackSubState.Idle);
                    _activePriestAttack = PriestAttackState.None;
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
                    _activePriestAttack = PriestAttackState.None;
                    break;
                default:
                    ParticleEffectsService.StopEffect(ParticleType.ShieldEffect);
                    ParticleEffectsService.StopEffect(ParticleType.MagicChargeBlue);
                    EnterAttackSubState(AttackSubState.Idle);
                    _activePriestAttack = PriestAttackState.None;
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
                    _subStateTimer = _priestAimDuration;
                    EnterAttackSubState(AttackSubState.Omni);
                    break;
                case AttackSubState.Omni:
                    ParticleEffectsService.StopEffect(ParticleType.ShieldEffect);
                    ParticleEffectsService.StopEffect(ParticleType.MagicChargeBlue);
                    EnterAttackSubState(AttackSubState.Idle);
                    _activePriestAttack = PriestAttackState.None;
                    break;
                default:
                    ParticleEffectsService.StopEffect(ParticleType.ShieldEffect);
                    ParticleEffectsService.StopEffect(ParticleType.MagicChargeBlue);
                    EnterAttackSubState(AttackSubState.Idle);
                    _activePriestAttack = PriestAttackState.None;
                    break;
            }
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
                    _subStateTimer = _idleDuration;
                    break;
                case AttackSubState.Coil:
                    AnimStateMachine.EnterIn<CoilEnemyAnimatedState>();
                    _subStateTimer = _attackDuration;
                    break;
                case AttackSubState.Omni:
                    AnimStateMachine.EnterIn<OmniEnemyAnimatedState>();
                    _subStateTimer = _attackDuration;
                    break;
                case AttackSubState.Attack1:
                    AnimStateMachine.EnterIn<Attack1EnemyAnimatedState>();
                    _subStateTimer = _attack1Duration;
                    break;
                case AttackSubState.Attack2:
                    AnimStateMachine.EnterIn<Attack2EnemyAnimatedState>();
                    _subStateTimer = _attack2Duration;
                    break;
                case AttackSubState.Attack3:
                    AnimStateMachine.EnterIn<Attack3EnemyAnimatedState>();
                    _subStateTimer = _attack3Duration;
                    break;
                case AttackSubState.Dodge:
                    AnimStateMachine.EnterIn<DodgeEnemyAnimatedState>();
                    _subStateTimer = _dodgeDuration;
                    break;
            }
        }
    }
}