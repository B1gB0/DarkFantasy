using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Player.Core
{
    public class PlayerStateMachine : MonoBehaviour
    {
        private IPlayerState _currentState;
        private Player _player;
        private Dictionary<Type, IPlayerState> _states;

        private void Awake()
        {
            _player = GetComponent<Player>();

            _states = new Dictionary<Type, IPlayerState>
            {
                { typeof(PlayerIdleState), new PlayerIdleState(_player, this) },
                { typeof(PlayerMoveState), new PlayerMoveState(_player, this, _player.PlayerAnimatedState) },
            };
        }

        private void Start()
        {
            SwitchState<PlayerIdleState>();
        }

        private void Update()
        {
            _currentState?.Update();
            Debug.Log(_currentState);
        }

        private void FixedUpdate()
        {
            _currentState?.FixedUpdate();
            Debug.Log(_currentState);
        }
        
        public void SwitchState<T>() where T : IPlayerState
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
        
        public void AddState(IPlayerState state)
        {
            var type = state.GetType();

            if (_states.ContainsKey(type) == false)
            {
                _states.TryAdd(type, state);
            }
        }
    }
}


