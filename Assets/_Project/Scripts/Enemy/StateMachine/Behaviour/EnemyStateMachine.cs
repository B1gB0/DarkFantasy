using System;
using System.Collections.Generic;
using _Project.Scripts.Enemy.StateMachine.Animation;
using _Project.Scripts.Enemy.StateMachine.Behaviour.States;
using UnityEngine;
using UnityEngine.AI;

namespace _Project.Scripts.Enemy.StateMachine.Behaviour
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyStateMachine : MonoBehaviour
    {
        private Enemy _enemy;
        private NavMeshAgent _agent;

        private EnemyState _currentState;
        private Dictionary<Type, EnemyState> _states;

        private void Awake()
        {
            _enemy = GetComponent<Enemy>();
            _agent = GetComponent<NavMeshAgent>();
            
            _states = new Dictionary<Type, EnemyState>
            {
                { typeof(IdleState), new IdleState() },
                { typeof(FollowState), new FollowState() },
                { typeof(AttackState), new AttackState() },
            };
        }

        private void FixedUpdate()
        {
            _currentState?.Update();
            Debug.Log(_currentState);
        }

        public void InitializeAllStates()
        {
            foreach (var state in _states.Values)
            {
                state.Initialize(_enemy, _agent);
            }
        }

        public void AddState(EnemyState state)
        {
            var type = state.GetType();

            if (_states.ContainsKey(type) == false)
            {
                _states.TryAdd(type, state);
            }
        }

        public void SwitchState<T>() where T : EnemyState
        {
            Type newStateType = typeof(T);
            if (!_states.ContainsKey(newStateType))
            {
                Debug.LogError($"State {newStateType} not found!");
                return;
            }

            _currentState?.Exit();
            _currentState = _states[newStateType];
            _currentState.Enter();
        }
    }
}