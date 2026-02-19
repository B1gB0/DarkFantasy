using _Project.Scripts.Enemy.StateMachine.Animation;
using _Project.Scripts.Enemy.StateMachine.Animation.States;
using UnityEngine;

namespace _Project.Scripts.Enemy.StateMachine.Behaviour.States
{
    public class AttackState : EnemyState
    {
        private float _attackRange;
        
        private float _reloadDuration = 2f;
        private float _aimDuration = 2f;
        private float _attackDuration = 2f;
        
        private AttackSubState _currentSubState;
        private float _subStateTimer;

        public override void Enter()
        {
            _currentSubState = Enemy.Type == EnemyType.SkeletonRanger ? AttackSubState.Aiming : AttackSubState.Idle;
            _attackRange = Data.RangeAttack;
        }

        public override void Update()
        {
            if (Player == null
                || !Player.CanFollow
                || Player.Health.TargetHealth <= 0
                || Enemy.Health.TargetHealth <= 0)
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
                0f);

            _subStateTimer -= Time.fixedDeltaTime;
            if (_subStateTimer <= 0f)
            {
                if (Enemy.Type == EnemyType.SkeletonRanger)
                {
                    switch (_currentSubState)
                    {
                        case AttackSubState.Reloading:
                            EnterAttackRangerSubState(AttackSubState.Aiming);
                            break;
                        case AttackSubState.Aiming:
                            EnterAttackRangerSubState(AttackSubState.Attack);
                            break;
                        case AttackSubState.Attack:
                            EnterAttackRangerSubState(AttackSubState.Reloading);
                            break;
                    }
                }
                else
                {
                    switch (_currentSubState)
                    {
                        case AttackSubState.Attack:
                            EnterAttackRangerSubState(AttackSubState.Idle);
                            break;
                        case AttackSubState.Idle:
                            EnterAttackRangerSubState(AttackSubState.Attack);
                            break;
                    }
                }
            }
        }
        
        private void EnterAttackRangerSubState(AttackSubState newSubState)
        {
            _currentSubState = newSubState;
            switch (_currentSubState)
            {
                case AttackSubState.Reloading:
                    AnimStateMachine.EnterIn<ReloadingAnimatedState>();
                    _subStateTimer = _reloadDuration;
                    break;
                case AttackSubState.Aiming:
                    AnimStateMachine.EnterIn<AimAnimatedState>();
                    _subStateTimer = _aimDuration;
                    break;
                case AttackSubState.Attack:
                    AnimStateMachine.EnterIn<AttackAnimatedState>(); // или ShootAnimatedState
                    _subStateTimer = _attackDuration;
                    // Наносим урон в момент выстрела (можно также через Animation Event)
                    // Enemy.Attack(Player);
                    break;
                case AttackSubState.Idle:
                    AnimStateMachine.EnterIn<IdleAnimatedState>();
                    _subStateTimer = _aimDuration;
                    break;
            }
        }
    }
}