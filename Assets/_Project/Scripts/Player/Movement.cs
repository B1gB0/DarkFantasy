using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Scripts.Player
{
    public class Movement : MonoBehaviour
    {
        [SerializeField] private float _speed = 5;
        [SerializeField] private float _gravity = -9.81f;

        private CharacterController _characterController;
        private InputSystem _inputSystem;

        private Vector3 _moveInput;
        private Vector3 _velocity;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _inputSystem = new InputSystem();
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


        private void OnMovePerformed(InputAction.CallbackContext ctx)
        {
            _moveInput = ctx.ReadValue<Vector2>();
        }

        private void OnMoveCanceled(InputAction.CallbackContext ctx)
        {
            _moveInput = Vector3.zero;
        }

        private void Update()
        {
            Vector3 move = new Vector3(_moveInput.x, 0, _moveInput.y);
            move = transform.TransformDirection(move);

            _characterController.Move(move * _speed * Time.deltaTime);

            if (_characterController.isGrounded && _velocity.y < 0)
            {
                _velocity.y = -2f;
            }

            _velocity.y += _gravity * Time.deltaTime;
            _characterController.Move(_velocity * Time.deltaTime);
        }
    }
}