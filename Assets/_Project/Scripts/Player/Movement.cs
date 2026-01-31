using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Scripts.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class Movement : MonoBehaviour
    {
        [SerializeField] private float _speed = 5f;
        [SerializeField] private float _rotationSpeed = 10f;

        private InputSystem _inputSystem;
        private Rigidbody _rigidbody;
        private Vector2 _moveInput;

        private void Awake()
        {
            _inputSystem = new InputSystem();
            _rigidbody = GetComponent<Rigidbody>();

            // Блокируем вращение физики
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
            // Если нет камеры — не двигаем
            if (Camera.main == null)
                return;

            // Берём forward камеры, но убираем наклон
            Vector3 camForward = Camera.main.transform.forward;
            camForward.y = 0;
            camForward.Normalize();

            // Берём right камеры
            Vector3 camRight = Camera.main.transform.right;
            camRight.y = 0;
            camRight.Normalize();

            // Преобразуем ввод в направление движения относительно камеры
            Vector3 moveDir = camForward * _moveInput.y + camRight * _moveInput.x;

            // Двигаем Rigidbody
            Vector3 velocity = moveDir * _speed;
            velocity.y = _rigidbody.velocity.y; // сохраняем гравитацию
            _rigidbody.velocity = velocity;

            // Поворот игрока в сторону движения
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

        private void OnMovePerformed(InputAction.CallbackContext ctx)
        {
            _moveInput = ctx.ReadValue<Vector2>();
        }

        private void OnMoveCanceled(InputAction.CallbackContext ctx)
        {
            _moveInput = Vector2.zero;
        }
    }
}
