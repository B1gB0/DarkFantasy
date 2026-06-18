using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace _Project.Scripts.Player.Input
{
    public class InputController : MonoBehaviour
    {
        private const float MinMagnitude = 0.01f;
        private const float MinValue = 0f;

        [Header("Action Locks")]
        [SerializeField] private bool _isMovementLocked;
        [SerializeField] private bool _isAttackLocked;
        [SerializeField] private bool _isRollLocked;
        
        [SerializeField] private float _rollCooldownDuration = 0.7f;

        private InputSystem _inputSystem;
        private Joystick _joystick;
        private Button _attackButton;
        private Button _rollButton;

        private bool _uiAttackPressed;

        private float _rollCooldownTimer;
        private bool _rollRequested;

        public event Action OnAttackButtonPressed;
        public event Action OnMoveButtonsPressed;
        public event Action OnUnlockController;

        public Vector2 MoveDirection { get; private set; }
        public bool IsMoveInputPerformed { get; private set; }

        public bool IsRollInputPerformed => _rollRequested;

        public bool IsAttackButtonPressed => !_isAttackLocked &&
            (_inputSystem.PLayer.Attack.WasPressedThisFrame() || _uiAttackPressed);

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
            _inputSystem.PLayer.Roll.performed += OnRollPerformed;
        }

        private void Update()
        {
            if (_rollCooldownTimer > MinValue)
                _rollCooldownTimer -= Time.deltaTime;
        }

        private void LateUpdate()
        {
            _uiAttackPressed = false;
            _rollRequested = false;
        }

        private void OnDisable()
        {
            _inputSystem.PLayer.Move.performed -= OnMove;
            _inputSystem.PLayer.Move.canceled -= OnMove;

            _inputSystem.PLayer.Attack.performed -= OnAttack;
            _inputSystem.PLayer.Roll.performed -= OnRollPerformed;

            _inputSystem.PLayer.Disable();
        }

        private void OnDestroy()
        {
            if (_joystick != null) _joystick.OnInputHandled -= OnMoveWithJoystick;
            if (_attackButton != null) _attackButton.onClick.RemoveListener(OnAttackByButton);
            if (_rollButton != null) _rollButton.onClick.RemoveListener(OnRollByButton);
        }

        public void GetJoystickWithAttackButton(Joystick joystick, Button attackButton, Button rollButton)
        {
            _joystick = joystick;
            _joystick.OnInputHandled += OnMoveWithJoystick;
            _attackButton = attackButton;
            _attackButton.onClick.AddListener(OnAttackByButton);
            _rollButton = rollButton;
            _rollButton.onClick.AddListener(OnRollByButton);
        }

        public void LockPlayerMovement()
        {
            Debug.Log("LockPlayerMovement");
            _isMovementLocked = true;
            _isAttackLocked = true;
            _isRollLocked = true;
        }
        
        public void UnlockPlayerMovement()
        {
            Debug.Log("UnlockPlayerMovement");
            _isMovementLocked = false;
            _isAttackLocked = false;
            _isRollLocked = false;
            
            OnUnlockController?.Invoke();
        }

        private void OnMoveWithJoystick()
        {
            if (_isMovementLocked)
            {
                MoveDirection = Vector2.zero;
                IsMoveInputPerformed = false;
                return;
            }
            
            MoveDirection = _joystick.Direction;
            IsMoveInputPerformed = MoveDirection.sqrMagnitude > MinMagnitude;
            if (IsMoveInputPerformed) OnMoveButtonsPressed?.Invoke();
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                MoveDirection = context.ReadValue<Vector2>();
                IsMoveInputPerformed = MoveDirection.sqrMagnitude > MinMagnitude;
            }
            else if (context.canceled || _isMovementLocked)
            {
                MoveDirection = Vector2.zero;
                IsMoveInputPerformed = false;
            }

            if (IsMoveInputPerformed) OnMoveButtonsPressed?.Invoke();
        }

        private void OnRollPerformed(InputAction.CallbackContext context)
        {
            if (_rollCooldownTimer > MinValue || _isRollLocked)
                return;

            _rollRequested = true;
            _rollCooldownTimer = _rollCooldownDuration;
        }

        private void OnRollByButton()
        {
            if (_rollCooldownTimer > MinValue || _isRollLocked)
                return;

            _rollRequested = true;
            _rollCooldownTimer = _rollCooldownDuration;
        }

        private void OnAttackByButton()
        {
            if(_isAttackLocked)
                return;
            
            _uiAttackPressed = true;
            OnAttackButtonPressed?.Invoke();
        }

        private void OnAttack(InputAction.CallbackContext context)
        {
            if(_isAttackLocked)
                return;
            
            if (context.performed) OnAttackButtonPressed?.Invoke();
        }
    }
}