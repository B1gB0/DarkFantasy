using System;
using _Project.Scripts.Player.Combat;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace _Project.Scripts.Player.Input
{
    public class InputController : MonoBehaviour
    {
        private const float MinMagnitude = 0f;
        
        [SerializeField] private Movement.Movement _movement;
        [SerializeField] private Attack _attack;

        private InputSystem _inputSystem;
        private Joystick _joystick;
        
        public event Action OnMoveButtonsPressed;

        public Vector2 MoveDirection { get; private set; }
        public bool IsMoveInputPerformed { get; private set; }

        private void Awake()
        {
            _inputSystem = new InputSystem();
        }

        private void OnEnable()
        {
            _inputSystem.PLayer.Enable();

          
            _inputSystem.PLayer.Move.performed += _movement.OnMovePerformed;
            _inputSystem.PLayer.Move.canceled += _movement.OnMoveCanceled;

          
            _inputSystem.PLayer.Attack.performed += _attack.OnAttackPerformed;
        }

        private void OnDisable()
        {
            _inputSystem.PLayer.Move.performed -= _movement.OnMovePerformed;
            _inputSystem.PLayer.Move.canceled -= _movement.OnMoveCanceled;

            _inputSystem.PLayer.Attack.performed -= _attack.OnAttackPerformed;

            _inputSystem.PLayer.Disable();
        }
        
        private void OnMove(InputAction.CallbackContext context)
        {
            MoveDirection = context.action.ReadValue<Vector2>();

            IsMoveInputPerformed = MoveDirection.sqrMagnitude > MinMagnitude;

            if (IsMoveInputPerformed)
                OnMoveButtonsPressed?.Invoke();
        }
    }
}