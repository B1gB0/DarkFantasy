using UnityEngine;

namespace _Project.Scripts.Player.Core
{
    public class PlayerStateMachine : MonoBehaviour
    {
        private IPlayerState _currentState;

        public void SetState(IPlayerState newState)
        {
            _currentState?.Exit();
            _currentState = newState;
            _currentState.Enter();
        }

        private void Update()
        {
            _currentState?.Update();
        }
    }
}


