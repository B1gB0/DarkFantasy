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

        private AttackSubState _currentSubState;
        private PriestAttackState _lastPriestRangedAttack = PriestAttackState.None;
        private PriestAttackState _activePriestAttack = PriestAttackState.None;
        private float _subStateTimer;

        public override void Enter()
        {
            _currentSubState = Enemy.Type is EnemyType.BanditRanger
                or EnemyType.SkeletonRanger ? AttackSubState.Aiming : AttackSubState.Idle;

            _attackRange = Data.RangeAttack;
            _activePriestAttack = PriestAttackState.None;
        }

        public override void Update()
        {
        }

        public override void FixedUpdate()
        {
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

            if (_subStateTimer <= MinValue)
            {
                if (Enemy.Type == EnemyType.SkeletonRanger || Enemy.Type == EnemyType.BanditRanger)
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
                    // Если активная атака не выбрана — выбираем новую
                    if (_activePriestAttack == PriestAttackState.None)
                    {
                        float distance = Vector3.Distance(Enemy.transform.position, Player.transform.position);
                        _activePriestAttack = ChoosePriestAttack(distance);
                    }

                    // Выполняем соответствующую атаку
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
                            break;
                    }
                }
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
                    _activePriestAttack = PriestAttackState.None; // атака завершена
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
                    _activePriestAttack = PriestAttackState.None; // атака завершена
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
                    _activePriestAttack = PriestAttackState.None; // атака завершена
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
            }
        }
    }
}