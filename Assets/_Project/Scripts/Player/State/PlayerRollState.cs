using _Project.Scripts.Player.Animation;
using _Project.Scripts.Player.Core;
using UnityEngine;

namespace _Project.Scripts.Player
{
    public class PlayerRollState : IPlayerState
    {
        private readonly Core.Player _player;
        private readonly PlayerAnimatedState _animationSystem;

        private Vector3 _rollDirection;
        private float _rollSpeed;
        private float _rollDistance;
        private bool _isRolling;

        public PlayerRollState(Core.Player player)
        {
            _player = player;
            _animationSystem = player.PlayerAnimatedState;
        }

        public StateId IdState => StateId.Roll;

        public void Enter()
        {
            _player.HitBox.enabled = false;
            Vector2 moveInput = _player.InputController.MoveDirection;

            Vector3 camForward = UnityEngine.Camera.main.transform.forward;
            Vector3 camRight = UnityEngine.Camera.main.transform.right;
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            Vector3 moveDirection = camForward * moveInput.y + camRight * moveInput.x;

            if (moveDirection.sqrMagnitude < 0.01f)
            {
                moveDirection = _player.transform.forward;
                moveDirection.y = 0f;
                moveDirection.Normalize();
            }
            else
            {
                moveDirection.Normalize();
            }

            _rollDirection = moveDirection;

            if (_rollDirection.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(_rollDirection);
                _player.Rigidbody.MoveRotation(targetRotation);
            }

            _rollSpeed = _player.RollSpeed;
            _isRolling = true;
            
            _animationSystem.OnRoll();
        }

        public void Update() { }

        public void FixedUpdate()
        {
            if (!_isRolling)
                return;
            
            Vector3 targetVelocity = _rollDirection * _rollSpeed;
            _player.Rigidbody.velocity = new Vector3(targetVelocity.x, _player.Rigidbody.velocity.y, targetVelocity.z);
        }

        public void Exit()
        {
            _player.HitBox.enabled = true;
            _isRolling = false;
            
            _player.Rigidbody.velocity = new Vector3(0f, _player.Rigidbody.velocity.y, 0f);
        }
    }
}