using _Project.Scripts.Player.Core;

namespace _Project.Scripts.Player
{
    public class PlayerIdleState : IPlayerState
    {
        private readonly Core.Player _player;
        private readonly PlayerStateMachine _stateMachine;

        public PlayerIdleState(Core.Player player)
        {
            _player = player;
            _stateMachine = _player.StateMachine;
        }

        public StateId IdState => StateId.Idle;

        public void Enter()
        {
        }

        public void Update()
        {
            if (_player.InputController.IsAttackButtonPressed)
            {
                _stateMachine.SwitchState(StateId.Attack);
                return;
            }

            if (_player.InputController.IsRollInputPerformed)
            {
                _stateMachine.SwitchState(StateId.Roll);
                return;
            }

            if (_player.InputController.IsMoveInputPerformed)
            {
                _stateMachine.SwitchState(StateId.Move);
            }
        }

        public void FixedUpdate()
        {
        }

        public void Exit()
        {
            
        }
    }
}