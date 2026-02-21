using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Scripts.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class Movement : MonoBehaviour
    {
        [SerializeField] private float _speed = 5f;
        [SerializeField] private float _rotationSpeed = 10f;
        [SerializeField] private Attack _attack;   

        private InputSystem _inputSystem;
        private Rigidbody _rigidbody;
        private Vector2 _moveInput;

        public event Action<float> IsMovePerformed;

        private void Awake()
        {
            _inputSystem = new InputSystem();
            _rigidbody = GetComponent<Rigidbody>();

            if (_attack == null)
                _attack = GetComponent<Attack>();

            _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX |
                                     RigidbodyConstraints.FreezeRotationZ;
        }

        private void OnEnable()
        {
            _inputSystem.PLayer.Enable();
            _inputSystem.PLayer.Move.performed += OnMovePerformed;
            _inputSystem.PLayer.Move.canceled += OnMoveCanceled;
        }

        private void OnDisable()
        {
            _inputSystem.PLayer.Disable();
            _inputSystem.PLayer.Move.performed -= OnMovePerformed;
            _inputSystem.PLayer.Move.canceled -= OnMoveCanceled;
        }

        private void FixedUpdate()
        {
            if (Camera.main == null)
                return;

            Vector3 camForward = SetDirection(Camera.main.transform.forward);
            Vector3 camRight = SetDirection(Camera.main.transform.right);

            Vector3 moveDir = camForward * _moveInput.y + camRight * _moveInput.x;

            Move(moveDir);
            Rotate(moveDir);
        }

        private void OnMovePerformed(InputAction.CallbackContext ctx)
        {
            _moveInput = ctx.ReadValue<Vector2>();
        }

        private void OnMoveCanceled(InputAction.CallbackContext ctx)
        {
            _moveInput = Vector2.zero;
        }

        private Vector3 SetDirection(Vector3 direction)
        {
            direction.y = 0;
            direction.Normalize();
            return direction;
        }

        private void Move(Vector3 moveDir)
        {
            Vector3 velocity = moveDir * _speed;
            velocity.y = _rigidbody.velocity.y;
            _rigidbody.velocity = velocity;

            float currentSpeed = _rigidbody.velocity.magnitude;
            IsMovePerformed?.Invoke(currentSpeed);
        }

        private void Rotate(Vector3 moveDir)
        {
            // КЛЮЧЕВОЕ: не вращаемся, если идёт атака
            if (_attack != null && _attack.IsAttacking)
                return;

            if (moveDir.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDir);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    Time.fixedDeltaTime * _rotationSpeed
                );
            }
        }
    }
}
