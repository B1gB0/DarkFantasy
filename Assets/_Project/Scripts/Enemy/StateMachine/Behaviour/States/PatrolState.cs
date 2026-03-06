using System.Collections.Generic;
using _Project.Scripts.Enemy.StateMachine.Animation.States;
using UnityEngine;

namespace _Project.Scripts.Enemy.StateMachine.Behaviour.States
{
    public class PatrolState : EnemyState
    {
        private const int StepIndex = 1;
        private const float StopDistance = 0.2f;
        private const float WaitTimeAtWaypoint = 2f;
        
        private readonly List<Vector3> _waypoints;

        private int _currentWaypointIndex;
        private bool _isPatrolStarted;
        
        private float _waitTimer;
        private bool _isWaiting;

        public PatrolState(List<Vector3> waypoints)
        {
            _waypoints = waypoints;
        }

        public override void Enter()
        {
            if (_waypoints == null || _waypoints.Count == MinValue)
            {
                Debug.LogWarning("No waypoints for patrol, staying idle.");
                return;
            }

            Agent.stoppingDistance = StopDistance;

            _isPatrolStarted = false;
            _isWaiting = false;
        }

        public override void Exit()
        {
            Agent.ResetPath();
        }
        
        public override void Update()
        {
            if (Player != null && Player.CanFollow && Enemy.CanFollow)
            {
                EnemyStateMachine.SwitchState<FollowState>();
                return;
            }

            if (_waypoints == null || _waypoints.Count == 0)
                return;
            
            if (_isWaiting)
            {
                _waitTimer -= Time.deltaTime;
                if (_waitTimer <= 0f)
                {
                    _isWaiting = false;
                    
                    SetNextWaypoint();
                    GoToCurrentWaypoint();
                    
                    AnimStateMachine.EnterIn<MoveAnimatedState>();
                }
                return;
            }
            
            if (!_isPatrolStarted)
            {
                AnimStateMachine.EnterIn<MoveAnimatedState>();
                GoToCurrentWaypoint();
                _isPatrolStarted = true;
            }
            
            if (!Agent.pathPending && Agent.remainingDistance <= Agent.stoppingDistance)
            {
                _isWaiting = true;
                _waitTimer = WaitTimeAtWaypoint;

                Agent.ResetPath();
                
                AnimStateMachine.EnterIn<IdleAnimatedState>();
            }
        }

        private void GoToCurrentWaypoint()
        {
            Agent.destination = _waypoints[_currentWaypointIndex];
        }

        private void SetNextWaypoint()
        {
            _currentWaypointIndex = (_currentWaypointIndex + StepIndex) % _waypoints.Count;
        }
    }
}