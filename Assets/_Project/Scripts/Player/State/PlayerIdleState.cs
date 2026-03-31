using _Project.Scripts.Player.Core;

namespace _Project.Scripts.Player
{
    public class PlayerIdleState : IPlayerState
    {
        private readonly Core.Player _player;
        private readonly PlayerStateMachine _stateMachine;

        public PlayerIdleState(Core.Player player, PlayerStateMachine stateMachine)
        {
            _player = player;
            _stateMachine = stateMachine;
        }

        public void Enter() { }

        public void Update()
        {
            if (_player.InputController.IsAttackButtonPressed)
            {
                _stateMachine.SwitchState<PlayerAttackState>();
                return;
            }

            if (_player.InputController.IsMoveInputPerformed)
            {
                _stateMachine.SwitchState<PlayerMoveState>();
            }
        }

        public void FixedUpdate() { }

        public void Exit() { }
    }
}


