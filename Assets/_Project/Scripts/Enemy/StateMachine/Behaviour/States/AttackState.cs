using _Project.Scripts.Enemy.StateMachine.Animation;
using _Project.Scripts.Enemy.StateMachine.Animation.States;
using UnityEngine;

namespace _Project.Scripts.Enemy.StateMachine.Behaviour.States
{
    public class AttackState : EnemyState
    {
        private float _lastShotTime;
        private float _attackRange;

        public override void Enter()
        {
            _lastShotTime = 1f;
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


            AnimStateMachine.EnterIn<IdleAnimatedState>();


            Vector3 direction = (Player.transform.position - Enemy.transform.position).normalized;
            float rotationSpeed = Data.RotationSpeed;

            Enemy.transform.forward = Vector3.RotateTowards(
                Enemy.transform.forward,
                direction,
                rotationSpeed * Time.fixedDeltaTime,
                0f);

            if (_lastShotTime <= 0f)
            {
                AnimStateMachine.EnterIn<AttackAnimatedState>();
                // Здесь можно вызвать метод нанесения урона, например Enemy.Attack(Player);
                _lastShotTime = Data.FireRate;
            }
            else if (_lastShotTime <= Data.FireRate)
            {
                AnimStateMachine.EnterIn<AimAnimatedState>();
            }

            _lastShotTime -= Time.fixedDeltaTime;
        }
    }
}