using _Project.Scripts.Enemy.StateMachine;
using _Project.Scripts.Enemy.StateMachine.Animation;
using _Project.Scripts.Enemy.StateMachine.Animation.States;
using UnityEngine;
using UnityEngine.AI;

namespace _Project.Scripts.Enemy
{
    public class FollowComponent : MonoBehaviour
    {
        private Player.Core.Player _player;
        private NavMeshAgent _agent;
        private EnemyAnimatedStateMachine _animatedStateMachine;
        private Enemy _enemy;

        private void Start()
        {
            _enemy = GetComponent<Enemy>();
            _agent = GetComponent<NavMeshAgent>();
        }

        private void FixedUpdate()
        {
            if (!_agent.gameObject.activeSelf) 
                return;

            if (_player == null || !_player.CanFollow || _animatedStateMachine == null)
                return;
            
            _agent.destination = _player.transform.position;

            var isMoving = _agent.remainingDistance > _agent.stoppingDistance;
            
            var direction = (_player.transform.position - _enemy.transform.position)
                .normalized;

            _enemy.transform.forward = isMoving ? _agent.transform.forward : direction;
            
            if(isMoving)
                _animatedStateMachine.EnterIn<MoveEnemyAnimatedState>();
        }

        public void InitFollower(
            EnemyAnimatedStateMachine animatedStateMachine,
            Player.Core.Player player)
        {
            _animatedStateMachine = animatedStateMachine;
            _player = player;
        }
    }
}