using System.Collections.Generic;
using _Project.Scripts.Enemy.StateMachine.Animation.States;
using UnityEngine;

namespace _Project.Scripts.Enemy.StateMachine.Behaviour.States
{
    public class PatrolState : EnemyState
    {
        private readonly List<Vector3> _waypoints;

        private int _currentWaypointIndex;
        private bool _isPatrolStarted;

        public PatrolState(List<Vector3> waypoints)
        {
            _waypoints = waypoints;
        }

        public override void Enter()
        {
            if (_waypoints == null || _waypoints.Count == 0)
            {
                Debug.LogWarning("No waypoints for patrol, staying idle.");
                return;
            }

            _isPatrolStarted = false;
        }

        public override void Exit()
        {
            Agent.ResetPath(); // останавливаем движение при выходе из состояния
        }

        public override void Update()
        {
            // Проверяем, нужно ли переключиться на преследование && IsPlayerInSight()
            if (Player != null && Player.CanFollow)
            {
                EnemyStateMachine.SwitchState<FollowState>();
                return;
            }

            if (_waypoints == null || _waypoints.Count == 0)
                return;

            if (!_isPatrolStarted)
            {
                AnimStateMachine.EnterIn<MoveAnimatedState>();
                GoToCurrentWaypoint();
                _isPatrolStarted = true;
            }

            // Если достигли текущей точки – переходим к следующей
            if (!Agent.pathPending && Agent.remainingDistance <= Agent.stoppingDistance)
            {
                SetNextWaypoint();
                GoToCurrentWaypoint();
            }
        }

        private void GoToCurrentWaypoint()
        {
            Agent.destination = _waypoints[_currentWaypointIndex];
        }

        private void SetNextWaypoint()
        {
            _currentWaypointIndex = (_currentWaypointIndex + 1) % _waypoints.Count;
        }

        // private bool IsPlayerInSight()
        // {
        //     float dist = Vector3.Distance(Enemy.transform.position, Player.transform.position);
        //     return dist < Data.ViewDistance
        //            && !Physics.Linecast(Enemy.transform.position, Player.transform.position, Data.ObstacleMask);
        // }
    }
}