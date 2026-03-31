using System;
using _Project.Scripts.Player.Combat;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Scripts.Player.Movement
{
    [RequireComponent(typeof(Rigidbody))]
    public class Movement : MonoBehaviour
    {
        [SerializeField] private float _speed = 5f;
        [SerializeField] private float _rotationSpeed = 10f;
        [SerializeField] private Attack _attack;

        private Rigidbody _rb;
        private Vector2 _moveInput;

        public float CurrentSpeed => new Vector3(_rb.velocity.x, 0, _rb.velocity.z).magnitude;
        public event Action<float> IsMovePerformed;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            if (_attack == null)
                _attack = GetComponent<Attack>();
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
            Vector3 camForward = UnityEngine.Camera.main.transform.forward;
            Vector3 camRight = UnityEngine.Camera.main.transform.right;

            camForward.y = 0;
            camRight.y = 0;

            camForward.Normalize();
            camRight.Normalize();

            Vector3 moveDir = camForward * _moveInput.y + camRight * _moveInput.x;

            Move(moveDir);
            Rotate(moveDir);
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
            if (_attack.IsAttacking)
                return;

            if (moveDir.sqrMagnitude > 0.01f)
            {
                Quaternion target = Quaternion.LookRotation(moveDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.fixedDeltaTime * _rotationSpeed);
            }
        }
    }
}
