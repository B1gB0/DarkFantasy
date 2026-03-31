using _Project.Scripts.Enemy.StateMachine.Animation.States;
using UnityEngine;

namespace _Project.Scripts.Enemy.StateMachine.Behaviour.States
{
    public class FollowState : EnemyState
    {
        private float _attackRange;

        public override void Enter()
        {
            Agent.updateRotation = false;
            Agent.stoppingDistance = Data.StopDistance;
            _attackRange = Data.RangeAttack;
        }

        public override void Exit()
        {
            Agent.updateRotation = true;
        }

        public override void Update()
        {
            if (Player == null || !Player.CanFollow || !Enemy.CanFollow)
            {
                EnemyStateMachine.SwitchState<PatrolState>();
                return;
            }

            float distanceToPlayer = Vector3.Distance(Enemy.transform.position, Player.transform.position);
            
            if (distanceToPlayer <= _attackRange)
            {
                EnemyStateMachine.SwitchState<AttackState>();
                return;
            }
            
            Agent.destination = Player.transform.position;
            
            Vector3 direction = (Player.transform.position - Enemy.transform.position).normalized;
            
            float rotationSpeed = Data.RotationSpeed;
            
            Enemy.transform.forward = Vector3.RotateTowards(
                Enemy.transform.forward,
                direction, 
                rotationSpeed * Time.fixedDeltaTime,
                MinValue);
            
            bool isMoving = Agent.remainingDistance > Agent.stoppingDistance;
            
            if (isMoving)
                AnimStateMachine.EnterIn<MoveEnemyAnimatedState>();
            else
                AnimStateMachine.EnterIn<IdleEnemyAnimatedState>();
        }
    }
}