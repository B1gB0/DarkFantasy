using System.Collections.Generic;
using _Project.Scripts.Enemy.StateMachine;
using _Project.Scripts.Enemy.StateMachine.Animation;
using _Project.Scripts.Enemy.StateMachine.Animation.States;
using UnityEngine;
using UnityEngine.AI;

namespace _Project.Scripts.Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyPatrolComponent : MonoBehaviour
    {
        private const int MinValue = 0;
        private const int StepPoint = 1;

        private List<Vector3> _waypoints = new();
        private int _currentWaypointIndex;
        private Player.Player _player;
        private NavMeshAgent _agent;
        private EnemyAnimatedStateMachine _animatedStateMachine;

        public bool IsPatrol { get; private set; }

        private void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        private void FixedUpdate()
        {
            if (!_agent.gameObject.activeSelf || _animatedStateMachine == null)
                return;

            if (_player.CanFollow)
            {
                IsPatrol = false;
                return;
            }

            if (!IsPatrol)
            {
                _animatedStateMachine.EnterIn<MoveAnimatedState>();
                GotoCurrentPoint();
                IsPatrol = true;
            }

            if (_agent.remainingDistance < _agent.stoppingDistance)
            {
                SetNextPoint();
                GotoCurrentPoint();
            }
        }

        public void InitPatrol(
            List<Vector3> waypoints,
            EnemyAnimatedStateMachine animatedStateMachine,
            Player.Player player)
        {
            _waypoints = waypoints;
            _animatedStateMachine = animatedStateMachine;
            _player = player;
        }

        private void SetNextPoint()
        {
            _currentWaypointIndex = (_currentWaypointIndex + StepPoint) % _waypoints.Count;
        }

        private void GotoCurrentPoint()
        {
            if (_waypoints.Count == MinValue)
                return;

            _agent.destination = _waypoints[_currentWaypointIndex];
        }
    }
}