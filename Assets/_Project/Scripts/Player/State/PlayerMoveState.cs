using _Project.Scripts.Player.Animation;
using _Project.Scripts.Player.Core;
using UnityEngine;

namespace _Project.Scripts.Player
{
    public class PlayerMoveState : IPlayerState
    {
        private readonly Core.Player _player;
        private readonly PlayerStateMachine _stateMachine;
        private readonly PlayerAnimatedState _playerAnimatedState;
        
        private float _currentSpeed => new Vector3(
            _player.Rigidbody.velocity.x,
            0,
            _player.Rigidbody.velocity.z)
            .magnitude;

        public PlayerMoveState(
            Core.Player player,
            PlayerStateMachine stateMachine,
            PlayerAnimatedState playerAnimatedState)
        {
            _player = player;
            _stateMachine = stateMachine;
            _playerAnimatedState = playerAnimatedState;
        }

        public void Enter() { }

        public void Update()
        {
            
        }

        public void FixedUpdate()
        {
            if (_player.InputController.IsAttackButtonPressed)
            {
                _stateMachine.SwitchState<PlayerAttackState>();
                return;
            }

            if (!_player.InputController.IsMoveInputPerformed)
            {
                _stateMachine.SwitchState<PlayerIdleState>();
                return;
            }
            
            Vector3 camForward = UnityEngine.Camera.main.transform.forward;
            Vector3 camRight = UnityEngine.Camera.main.transform.right;

            camForward.y = 0;
            camRight.y = 0;

            camForward.Normalize();
            camRight.Normalize();

            Vector3 moveDirection = camForward * _player.InputController.MoveDirection.y
                                    + camRight * _player.InputController.MoveDirection.x;

            Move(moveDirection);
            Rotate(moveDirection);
        }

        public void Exit()
        {
            _player.Rigidbody.velocity = Vector3.zero;
        }

        private void Move(Vector3 moveDirection)
        {
            Vector3 velocity = moveDirection * _player.PlayerCharacteristics.MoveSpeed;
            velocity.y = _player.Rigidbody.velocity.y;

            _player.Rigidbody.velocity = velocity;
            
            _playerAnimatedState.OnMove(_currentSpeed);
        }

        private void Rotate(Vector3 moveDirection)
        {
            if (_player.InputController.IsAttackButtonPressed)
                return;
            
            if (!_player.InputController.IsMoveInputPerformed)
                return;

            if (moveDirection.sqrMagnitude > 0.01f)
            {
                Quaternion target = Quaternion.LookRotation(moveDirection);
                _player.transform.rotation = Quaternion.Slerp(
                    _player.transform.rotation,
                    target,
                    Time.fixedDeltaTime * _player.PlayerCharacteristics.RotationSpeed);
            }
        }
    }
}