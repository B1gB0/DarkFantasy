using System.Collections.Generic;
using _Project.Scripts.Player.State;
using UnityEngine;

namespace _Project.Scripts.Player.Core
{
    public class PlayerStateMachine : MonoBehaviour
    {
        private readonly Dictionary<StateId, IPlayerState> _states = new ();

        private IPlayerState _currentState;

        public IPlayerState CurrentState => _currentState;

        private void Update()
        {
            _currentState?.Update();
        }

        private void FixedUpdate()
        {
            _currentState?.FixedUpdate();
        }

        public void SwitchState(StateId stateID)
        {
            if (stateID == _currentState?.IdState)
                return;

            _currentState?.Exit();
            _currentState = _states[stateID];
            _currentState.Enter();
        }

        public void AddState(IPlayerState state)
        {
            if (_states.ContainsKey(state.IdState) == false)
            {
                _states.TryAdd(state.IdState, state);
            }
        }
    }
}