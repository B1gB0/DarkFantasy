using System;
using _Project.Scripts.Player.Combat;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Scripts.Player.Input
{
    public class InputController : MonoBehaviour
    {
        private const float MinMagnitude = 0f;

        private InputSystem _inputSystem;
        private Joystick _joystick;
        
        public event Action OnMoveButtonsPressed;
        public event Action OnAttackButtonPressed;

        public Vector2 MoveDirection { get; private set; }
        public bool IsMoveInputPerformed { get; private set; }
        public bool IsAttackButtonPressed { get; private set; }

        private void Awake()
        {
            _inputSystem = new InputSystem();
        }

        private void OnEnable()
        {
            _inputSystem.PLayer.Enable();

            _inputSystem.PLayer.Move.performed += OnMove;
            _inputSystem.PLayer.Move.canceled += OnMove;
            
            _inputSystem.PLayer.Attack.performed += OnAttack;
            _inputSystem.PLayer.Attack.canceled += OnAttack;
        }

        private void OnDisable()
        {
            _inputSystem.PLayer.Move.performed -= OnMove;
            _inputSystem.PLayer.Move.canceled -= OnMove;

            _inputSystem.PLayer.Attack.performed -= OnAttack;
            _inputSystem.PLayer.Attack.canceled -= OnAttack;

            _inputSystem.PLayer.Disable();
        }
        
        private void OnDestroy()
        {
            // _joystick.OnInputHandled -= OnMoveWithJoystick;
        }

        public void GetJoystick(Joystick joystick)
        {
            // _joystick = joystick;
            // _joystick.OnInputHandled += OnMoveWithJoystick;
        }

        private void OnMoveWithJoystick()
        {
            MoveDirection = _joystick.Direction;

            IsMoveInputPerformed = MoveDirection.sqrMagnitude > MinMagnitude;

            if (IsMoveInputPerformed)
                OnMoveButtonsPressed?.Invoke();
        }
        
        private void OnMove(InputAction.CallbackContext context)
        {
            MoveDirection = context.ReadValue<Vector2>();

            IsMoveInputPerformed = MoveDirection.sqrMagnitude > MinMagnitude;

            if (IsMoveInputPerformed)
                OnMoveButtonsPressed?.Invoke();
        }

        private void OnAttack(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                IsAttackButtonPressed = true;
                OnAttackButtonPressed?.Invoke();
            }
            else if (context.canceled)
            {
                IsAttackButtonPressed = false;
            }
        }
    }
}