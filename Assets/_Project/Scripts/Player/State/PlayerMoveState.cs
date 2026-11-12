using _Project.Scripts.Player.Animation;
using _Project.Scripts.Player.Core;
using UnityEngine;

namespace _Project.Scripts.Player.State
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
        private readonly Transform _cameraTransform;
        
        private Vector3 _cachedMoveDirection;

        private float CurrentSpeed => new Vector3(
                _player.Rigidbody.velocity.x,
                MinValue,
                _player.Rigidbody.velocity.z)
            .magnitude;

        public PlayerMoveState(Core.Player player)
        {
            _player = player;
            _stateMachine = _player.StateMachine;
            _playerAnimatedState = _player.PlayerAnimatedState;
            _cameraTransform = UnityEngine.Camera.main.transform;
        }

        public StateId IdState => StateId.Move;

        public void Enter()
        {
            _player.InputController.OnAttackButtonPressed += EnterAttack;
        }

        public void Update()
        {
            if (_player.InputController.IsRollInputPerformed)
            {
                _stateMachine.SwitchState(StateId.Roll);
                return;
            }

            if (_player.InputController.IsMoveInputPerformed == false)
            {
                _stateMachine.SwitchState(StateId.Idle);
                return;
            }
            
            Vector3 camForward = _cameraTransform.forward;
            Vector3 camRight = _cameraTransform.right;

            camForward.y = MinValue;
            camRight.y = MinValue;
            
            camForward.Normalize();
            camRight.Normalize();

            _cachedMoveDirection = 
                camForward * _player.InputController.MoveDirection.y 
                + camRight * _player.InputController.MoveDirection.x;
            
        }

        public void FixedUpdate()
        {
            Move(_cachedMoveDirection);
            Rotate(_cachedMoveDirection);
        }

        public void Exit()
        {
            _player.InputController.OnAttackButtonPressed -= EnterAttack;
            
            _player.Rigidbody.velocity = Vector3.zero;
            
            _playerAnimatedState.OnMove(MinValue); 
        }

        private void Move(Vector3 moveDirection)
        {
            float currentSpeed = _player.PlayerCharacteristics.GetCurrentMoveSpeed();
            Vector3 velocity = moveDirection * currentSpeed;
            
            float rayLength = RayLength; 
            Vector3 rayStart = _player.transform.position + Vector3.up * HeightOffset;

            if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, rayLength))
            {
                Vector3 projectedVelocity = Vector3.ProjectOnPlane(velocity, hit.normal);
                
                if (projectedVelocity.sqrMagnitude > MinMagnitude)
                {
                    velocity = projectedVelocity.normalized * currentSpeed;
                }
                else
                {
                    velocity = projectedVelocity;
                }
                
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
            
            float maxSpeed = _player.PlayerCharacteristics.GetCurrentMoveSpeed();
            float safeMaxSpeed = Mathf.Max(MinMagnitude, maxSpeed);
            
            float normalizedSpeed = CurrentSpeed / safeMaxSpeed;
            
            _playerAnimatedState.OnMove(normalizedSpeed); 
        }

        private void Rotate(Vector3 moveDirection)
        {
            if (moveDirection.sqrMagnitude > MinMagnitude)
            {
                Quaternion target = Quaternion.LookRotation(moveDirection);
                
                Quaternion newRotation = Quaternion.Slerp(
                    _player.Rigidbody.rotation,
                    target,
                    Time.fixedDeltaTime * _player.PlayerCharacteristics.RotationSpeed);
            
                _player.Rigidbody.MoveRotation(newRotation);
            }
        }

        private void EnterAttack()
        {
            _stateMachine.SwitchState(StateId.Attack);
        }
    }
}