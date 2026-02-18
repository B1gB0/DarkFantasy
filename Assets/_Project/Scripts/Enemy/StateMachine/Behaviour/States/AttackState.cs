using _Project.Scripts.Enemy.StateMachine.Animation;
using _Project.Scripts.Enemy.StateMachine.Animation.States;
using UnityEngine;

namespace _Project.Scripts.Enemy.StateMachine.Behaviour.States
{
    public class AttackState : EnemyState
    {
        private float _attackRange;
        
        private float _reloadDuration = 3f;
        private float _aimDuration = 2f;
        private float _shootDuration = 3f;
        
        private AttackSubState _currentSubState;
        private float _subStateTimer;

        public override void Enter()
        {
            _attackRange = Data.RangeAttack;
        }
        
        private void EnterSubState(AttackSubState newSubState)
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
                case AttackSubState.Shooting:
                    AnimStateMachine.EnterIn<AttackAnimatedState>(); // или ShootAnimatedState
                    _subStateTimer = _shootDuration;
                    // Наносим урон в момент выстрела (можно также через Animation Event)
                    // Enemy.Attack(Player);
                    break;
            }
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
                // Переход к следующему подсостоянию
                switch (_currentSubState)
                {
                    case AttackSubState.Reloading:
                        EnterSubState(AttackSubState.Aiming);
                        break;
                    case AttackSubState.Aiming:
                        EnterSubState(AttackSubState.Shooting);
                        break;
                    case AttackSubState.Shooting:
                        EnterSubState(AttackSubState.Reloading);
                        break;
                }
            }
        }
    }
}