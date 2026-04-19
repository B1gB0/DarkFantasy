using System;
using _Project.Scripts.Player.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Scripts.Player.Movement
{
    [RequireComponent(typeof(Rigidbody))]
    public class Movement : MonoBehaviour
    {
        [SerializeField] private float _speed = 5f;
        [SerializeField] private float _rotationSpeed = 10f;

        private Rigidbody _rb;
        private Vector2 _moveInput;
        private Transform _cameraTransform;
        private Core.Player _player;

        private const float MoveThreshold = 0.01f;

        public float CurrentSpeed => new Vector3(_rb.velocity.x, 0, _rb.velocity.z).magnitude;
        public event Action<float> IsMovePerformed;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _player = GetComponent<Core.Player>();

            if (UnityEngine.Camera.main != null)
                _cameraTransform = UnityEngine.Camera.main.transform;
        }

        public void OnMovePerformed(InputAction.CallbackContext ctx)
        {
            _moveInput = ctx.ReadValue<Vector2>();
        }

        public void OnMoveCanceled(InputAction.CallbackContext ctx)
        {
            _moveInput = Vector2.zero;
        }

        private void FixedUpdate()
        {
            if (_cameraTransform == null)
                return;

            Vector3 camForward = _cameraTransform.forward;
            Vector3 camRight = _cameraTransform.right;

            camForward.y = 0;
            camRight.y = 0;

            camForward.Normalize();
            camRight.Normalize();

            Vector3 moveDir = camForward * _moveInput.y + camRight * _moveInput.x;

            if (moveDir.sqrMagnitude > MoveThreshold * MoveThreshold)
            {
                moveDir.Normalize();
                Move(moveDir);
                Rotate(moveDir);
            }
            else
            {
                Vector3 vel = _rb.velocity;
                vel.x = 0;
                vel.z = 0;
                _rb.velocity = vel;

                IsMovePerformed?.Invoke(0f);
            }
        }

        private void Move(Vector3 moveDir)
        {
            Vector3 velocity = moveDir * _speed;
            velocity.y = _rb.velocity.y;

            _rb.velocity = velocity;

            IsMovePerformed?.Invoke(CurrentSpeed);
        }

        private void Rotate(Vector3 moveDir)
        {
            // Блокируем поворот, если игрок в состоянии атаки
            if (_player.StateMachine.CurentState.IdState == StateId.Attack)
                return;

            Quaternion target = Quaternion.LookRotation(moveDir);
            Quaternion newRot = Quaternion.Slerp(_rb.rotation, target, Time.fixedDeltaTime * _rotationSpeed);

            _rb.MoveRotation(newRot);
        }
    }
}
