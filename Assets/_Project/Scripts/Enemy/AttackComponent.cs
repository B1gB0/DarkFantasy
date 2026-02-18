using _Project.Scripts.Enemy.StateMachine;
using _Project.Scripts.Enemy.StateMachine.States;
using UnityEngine;
using UnityEngine.AI;

namespace _Project.Scripts.Enemy
{
    public class AttackComponent : MonoBehaviour
    {
        private const int MinValue = 0;
        private const float MinRemainingDistance = 1f;
        
        private NavMeshAgent _agent;
        private EnemyAnimatedStateMachine _animatedStateMachine;
        private Player.Player _player;
        private Enemy _enemy;
        
        private float _lastShotTime = 1f;

        private void Start()
        {
            _enemy = GetComponent<Enemy>();
        }

        private void FixedUpdate()
        {
            if (_player == null || !_agent.isActiveAndEnabled
                                               || !_player.CanFollow
                                               || _agent.remainingDistance <= MinRemainingDistance)
            {
                return;
            }
            
            if (_player.Health.TargetHealth > MinValue &&
                _enemy.Health.TargetHealth > MinValue)
            {
                if (_lastShotTime <= MinValue)
                {
                    _animatedStateMachine.EnterIn<AttackState>();
                    _lastShotTime = _enemy.Data.FireRate;
                }
                else if (_lastShotTime <= _enemy.Data.FireRate)
                {
                    _animatedStateMachine.EnterIn<AimState>();
                }

                _lastShotTime -= Time.fixedDeltaTime;
            }
            else
            {
                _animatedStateMachine.EnterIn<MoveState>();
            }
        }

        public void InitAttacker(
            NavMeshAgent agent,
            EnemyAnimatedStateMachine animatedStateMachine,
            Player.Player player)
        {
            _agent = agent;
            _animatedStateMachine = animatedStateMachine;
            _player = player;
        }
    }
}