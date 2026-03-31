using _Project.Scripts.Player.Combat;
using _Project.Scripts.Player.Core;

namespace _Project.Scripts.Player
{
    public class PlayerIdleState : IPlayerState
    {
        private readonly Core.Player _player;
        private readonly PlayerStateMachine _stateMachine;
        private readonly Movement.Movement _movement;
        private readonly Attack _attack;

        public PlayerIdleState(Core.Player player, PlayerStateMachine stateMachine, Movement.Movement movement, Attack attack)
        {
            _player = player;
            _stateMachine = stateMachine;
            _movement = movement;
            _attack = attack;
        }

        public void Enter() { }

        public void Update()
        {
            if (_attack.IsAttacking)
            {
                _stateMachine.SetState(new PlayerAttackState(_player, _stateMachine, _movement, _attack));
                return;
            }

            if (_movement.CurrentSpeed > 0.1f)
            {
                _stateMachine.SetState(new PlayerMoveState(_player, _stateMachine, _movement, _attack));
            }
        }

        public void Exit() { }
    }
}


