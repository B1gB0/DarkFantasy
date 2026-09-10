using _Project.Scripts.Player.Animation;
using _Project.Scripts.Player.Core;
using UnityEngine;

namespace _Project.Scripts.Player
{
    public class PlayerMoveState : IPlayerState
    {
        private const float MinValue = 0f;
        private const float MinMagnitude = 0.01f;
        private const float HeightOffset = 0.1f;
        private const float Gravity = 2f;
        private const float RayLength = 0.5f;
        
        private readonly Core.Player _player;
        private readonly PlayerStateMachine _stateMachine;
        private readonly PlayerAnimatedState _playerAnimatedState;

        private float _currentSpeed => new Vector3(
                _player.Rigidbody.velocity.x,
                MinValue,
                _player.Rigidbody.velocity.z)
            .magnitude;

        public PlayerMoveState(Core.Player player)
        {
            _player = player;
            _stateMachine = _player.StateMachine;
            _playerAnimatedState = _player.PlayerAnimatedState;
        }

        public StateId IdState => StateId.Move;

        public void Enter() { }

        public void Update()
        {
            if (_player.InputController.IsAttackButtonPressed)
                _stateMachine.SwitchState(StateId.Attack);
            if (_player.InputController.IsRollInputPerformed)
                _stateMachine.SwitchState(StateId.Roll);
            if (_player.InputController.IsMoveInputPerformed == false)
                _stateMachine.SwitchState(StateId.Idle);
        }

        public void FixedUpdate()
        {
            Vector3 camForward = UnityEngine.Camera.main.transform.forward;
            Vector3 camRight = UnityEngine.Camera.main.transform.right;

            camForward.y = MinValue;
            camRight.y = MinValue;

            camForward.Normalize();
            camRight.Normalize();

            Vector3 moveDirection =
                camForward * _player.InputController.MoveDirection.y
                + camRight * _player.InputController.MoveDirection.x;

            Move(moveDirection);
            Rotate(moveDirection);
        }

        public void Exit()
        {
            _player.Rigidbody.velocity = Vector3.zero;
            
            _playerAnimatedState.OnMove(MinValue); 
        }

        private void Move(Vector3 moveDirection)
        {
            Vector3 velocity = moveDirection * _player.PlayerCharacteristics.GetCurrentMoveSpeed();
            
            float rayLength = RayLength; 
            Vector3 rayStart = _player.transform.position + Vector3.up * HeightOffset;

            if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, rayLength))
            {
                velocity = Vector3.ProjectOnPlane(velocity, hit.normal);
                
                if (velocity.y < MinValue)
                {
                    velocity.y -= Gravity;
                }
            }
            else
            {
                velocity.y = _player.Rigidbody.velocity.y;
            }

            _player.Rigidbody.velocity = velocity;
            _playerAnimatedState.OnMove(_currentSpeed);
        }

        private void Rotate(Vector3 moveDirection)
        {
            if (moveDirection.sqrMagnitude > MinMagnitude)
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