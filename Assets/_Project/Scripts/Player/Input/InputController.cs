using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Scripts.Player.Input
{
    public class InputController : MonoBehaviour
    {
        private const float MinMagnitude = 0.01f;

        private InputSystem _inputSystem;
        private Joystick _joystick;
        
        public event Action OnAttackButtonPressed;

        public Vector2 MoveDirection { get; private set; }
        public bool IsMoveInputPerformed { get; private set; }
        
        public bool IsRollInputPerformed => _inputSystem.PLayer.Roll.WasPressedThisFrame();
        public bool IsAttackButtonPressed => _inputSystem.PLayer.Attack.WasPressedThisFrame();

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
        }

        private void OnDisable()
        {
            _inputSystem.PLayer.Move.performed -= OnMove;
            _inputSystem.PLayer.Move.canceled -= OnMove;
            
            _inputSystem.PLayer.Attack.performed -= OnAttack;

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
        }

        private void OnMove(InputAction.CallbackContext context) 
        {
            if (context.performed)
            {
                MoveDirection = context.ReadValue<Vector2>();
                IsMoveInputPerformed = MoveDirection.sqrMagnitude > MinMagnitude;
            }
            else if (context.canceled)
            {
                MoveDirection = Vector2.zero;
                IsMoveInputPerformed = false;
            }
        }
        
        private void OnAttack(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnAttackButtonPressed?.Invoke();
        }
    }
}