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

        private Rigidbody _rb;
        private Vector2 _moveInput;

        public event Action<float> IsMovePerformed;

        // Порог по квадрату скорости для начала поворота (чтобы игнорировать микродвижения)
        private const float RotationSqrThreshold = 0.04f; // ~0.2^2

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();

            if (_attack == null)
                _attack = GetComponent<Attack>();

            // Разрешаем вращение только вокруг Y (замораживаем X и Z)
            _rb.constraints = RigidbodyConstraints.FreezeRotationX |
                              RigidbodyConstraints.FreezeRotationZ;

            // Плавность и более стабильная физика
            _rb.interpolation = RigidbodyInterpolation.Interpolate;
            _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            // Рекомендуемые начальные значения
            _rb.angularDrag = Mathf.Max(_rb.angularDrag, 2f);
        }

        private void FixedUpdate()
        {
            if (Camera.main == null)
                return;

            // Обнуляем остаточную угловую скорость, чтобы убрать рывки
            if (_rb.angularVelocity.sqrMagnitude > 0f)
                _rb.angularVelocity = Vector3.zero;

            Vector3 camForward = Camera.main.transform.forward;
            Vector3 camRight = Camera.main.transform.right;
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            Vector3 moveDir = camForward * _moveInput.y + camRight * _moveInput.x;

            Move(moveDir);
            RotateByVelocityOrInput(moveDir);
        }

        public void OnMovePerformed(InputAction.CallbackContext ctx)
        {
            _moveInput = ctx.ReadValue<Vector2>();
        }

        public void OnMoveCanceled(InputAction.CallbackContext ctx)
        {
            _moveInput = Vector2.zero;
        }

        private void Move(Vector3 moveDir)
        {
            // Рассчитываем желаемую горизонтальную скорость (стабильная для анимации)
            Vector3 desiredHorizontal = moveDir * _speed;
            Vector3 desiredVelocity = new Vector3(desiredHorizontal.x, _rb.velocity.y, desiredHorizontal.z);

            // Перемещение через MovePosition — корректно с физикой
            Vector3 nextPos = _rb.position + desiredVelocity * Time.fixedDeltaTime;
            _rb.MovePosition(nextPos);

            // Для анимации используем горизонтальную часть desiredVelocity (не rb.velocity)
            float currentSpeedForAnim = new Vector3(desiredVelocity.x, 0f, desiredVelocity.z).magnitude;
            IsMovePerformed?.Invoke(currentSpeedForAnim);
        }

        private void RotateByVelocityOrInput(Vector3 inputMoveDir)
        {
            if (_attack != null && _attack.IsAttacking)
                return;

            // Предпочитаем направление реальной горизонтальной скорости, но используем desired input, если скорость мала
            Vector3 flatVel = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

            Vector3 dirForRotation = Vector3.zero;

            if (flatVel.sqrMagnitude > RotationSqrThreshold)
            {
                dirForRotation = flatVel.normalized;
            }
            else if (inputMoveDir.sqrMagnitude > 0.01f)
            {
                dirForRotation = inputMoveDir.normalized;
            }

            if (dirForRotation.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(dirForRotation, Vector3.up);
                Quaternion newRot = Quaternion.Slerp(_rb.rotation, targetRotation, Time.fixedDeltaTime * _rotationSpeed);
                _rb.MoveRotation(newRot);
            }
        }
    }
}


