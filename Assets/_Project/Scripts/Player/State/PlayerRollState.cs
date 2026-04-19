using _Project.Scripts.Player.Animation;
using _Project.Scripts.Player.Core;
using UnityEngine;

namespace _Project.Scripts.Player
{
    public class PlayerRollState : IPlayerState
    {
        private readonly Core.Player _player;
        private readonly PlayerAnimatedState _animationSystem;
        private readonly PlayerStateMachine _stateMachine;
        private readonly Animator _animator;

        private bool _finished;

        public PlayerRollState(Core.Player player)
        {
            _player = player;
            _animator = player.Animator;
            _stateMachine = player.StateMachine;
            _animationSystem = player.PlayerAnimatedState;
        }

        public StateId IdState => StateId.Roll;

        public void Enter()
        {
            _animationSystem.OnRoll();
        }

        public void Update()
        {
            _stateMachine.SwitchState(StateId.Idle);
        }


        public void FixedUpdate()
        {
        }

        public void Exit()
        {
        }
    }
}