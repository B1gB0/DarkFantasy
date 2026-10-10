using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace _Project.Scripts.Player.Input
{
    public class InputController : MonoBehaviour
    {
        private const float MinMagnitude = 0.01f;
        private const float MinValue = 0f;
        private const float CameraDragMultiplier = 100f;

        [Header("Action Locks")] 
        [SerializeField] private bool _isMovementLocked;

        [SerializeField] private bool _isAttackLocked;
        [SerializeField] private bool _isRollLocked;

        [SerializeField] private float _rollCooldownDuration = 0.7f;

        [Header("Camera")] 
        [SerializeField] private float _cameraSensitivityX = 3f;
        [SerializeField] private float _cameraSensitivityY = 2f;

        private InputSystem _inputSystem;
        private Joystick _moveJoystick;
        private Joystick _cameraJoystick;
        private Button _attackButton;
        private Button _rollButton;
        private Button _inventoryButton;
        private Button _equippedItemButton;
        private Button _pickUpButton;

        private bool _uiAttackPressed;
        private bool _isPickUpLocked;

        private float _rollCooldownTimer;
        private bool _rollRequested;

        public event Action OnEquippedItemButtonPressed;
        public event Action OnInventoryButtonPressed;
        public event Action OnAttackButtonPressed;
        public event Action OnMoveButtonsPressed;
        public event Action OnUnlockController;

        public Vector2 MoveDirection { get; private set; }
        public Vector2 CameraLookDirection { get; private set; }
        public bool IsMoveInputPerformed { get; private set; }
        public bool IsCameraRotating { get; private set; }
        public bool IsPickUpPressed { get; private set; }

        public bool IsRollInputPerformed => _rollRequested;

        private void Awake()
        {
            _inputSystem = new InputSystem();
        }

        private void OnEnable()
        {
            _inputSystem.Player.Enable();

            _inputSystem.Player.Move.performed += OnMove;
            _inputSystem.Player.Move.canceled += OnMove;

            _inputSystem.Player.Roll.performed += OnRollPerformed;
        }

        private void Update()
        {
            if (_rollCooldownTimer > MinValue)
                _rollCooldownTimer -= Time.deltaTime;

            UpdateCameraInput();
        }

        private void LateUpdate()
        {
            UpdateAttackInput();
            UpdateInteractInput();

            _uiAttackPressed = false;
            _rollRequested = false;
        }

        private void OnDisable()
        {
            _inputSystem.Player.Move.performed -= OnMove;
            _inputSystem.Player.Move.canceled -= OnMove;

            _inputSystem.Player.Roll.performed -= OnRollPerformed;

            _inputSystem.Player.Disable();
        }

        private void OnDestroy()
        {
            if (_moveJoystick != null) _moveJoystick.OnInputHandled -= OnWithMoveJoystick;
            if (_attackButton != null) _attackButton.onClick.RemoveListener(OnAttackByButton);
            if (_rollButton != null) _rollButton.onClick.RemoveListener(OnRollByButton);
            if (_inventoryButton != null) _inventoryButton.onClick.RemoveListener(OnInventoryButtonClicked);
            if (_equippedItemButton != null) _equippedItemButton.onClick.RemoveListener(OnEquippedItemButtonClicked);
            if (_pickUpButton != null) _pickUpButton.onClick.RemoveListener(OnPickUpByButton);
        }

        public void GetButtons(
            Joystick moveJoystick,
            Joystick cameraJoystick,
            Button attackButton,
            Button rollButton,
            Button inventoryButton,
            Button equippedItemButton,
            Button pickUpButton)
        {
            _moveJoystick = moveJoystick;
            _cameraJoystick = cameraJoystick;
            _moveJoystick.OnInputHandled += OnWithMoveJoystick;
            _attackButton = attackButton;
            _attackButton.onClick.AddListener(OnAttackByButton);
            _rollButton = rollButton;
            _rollButton.onClick.AddListener(OnRollByButton);
            _inventoryButton = inventoryButton;
            _inventoryButton.onClick.AddListener(OnInventoryButtonClicked);
            _equippedItemButton = equippedItemButton;
            _equippedItemButton.onClick.AddListener(OnEquippedItemButtonClicked);
            _pickUpButton = pickUpButton;
            _pickUpButton.onClick.AddListener(OnPickUpByButton);
        }

        public void LockPlayerMovement()
        {
            _isMovementLocked = true;
            _isAttackLocked = true;
            _isRollLocked = true;
        }

        public void UnlockPlayerMovement()
        {
            _isMovementLocked = false;
            _isAttackLocked = false;
            _isRollLocked = false;

            OnUnlockController?.Invoke();
        }
        
        public void LockPickUp()
        {
            _isPickUpLocked = true;
        }
        
        public void UnLockPickUp()
        {
            _isPickUpLocked = false;
        }

        private void UpdateAttackInput()
        {
            if (_isAttackLocked) return;

            bool inputAttack = _inputSystem.Player.Attack.WasPressedThisFrame()
                               && !IsPointerOverUI();

            if (inputAttack || _uiAttackPressed)
                OnAttackButtonPressed?.Invoke();
        }

        private void UpdateInteractInput()
        {
            IsPickUpPressed = _inputSystem.Player.PickUp.WasPressedThisFrame()
                              && !IsPointerOverUI() && !_isPickUpLocked;
        }

        private bool IsPointerOverUI()
        {
            var eventSystem = EventSystem.current;
            if (eventSystem == null)
                return false;

            if (Mouse.current != null && eventSystem.IsPointerOverGameObject())
                return true;

            if (Touchscreen.current != null)
            {
                foreach (var touch in Touchscreen.current.touches)
                {
                    if (!touch.press.isPressed) continue;

                    int fingerId = touch.touchId.ReadValue();
                    if (eventSystem.IsPointerOverGameObject(fingerId))
                        return true;
                }
            }

            return false;
        }

        private void UpdateCameraInput()
        {
            if (_cameraJoystick != null)
            {
                var dir = _cameraJoystick.Direction;
                if (dir.sqrMagnitude > MinMagnitude)
                {
                    CameraLookDirection = dir;
                    IsCameraRotating = true;
                    return;
                }
            }

            if (IsPointerOverUI())
            {
                CameraLookDirection = Vector2.zero;
                IsCameraRotating = false;
                return;
            }

            if (_inputSystem.Player.CameraDragButton.IsPressed())
            {
                var delta = _inputSystem.Player.Look.ReadValue<Vector2>();

                float normalizedX = delta.x / Screen.width;
                float normalizedY = delta.y / Screen.height;

                CameraLookDirection = new Vector2(
                    normalizedX * _cameraSensitivityX * CameraDragMultiplier,
                    normalizedY * _cameraSensitivityY * CameraDragMultiplier);

                IsCameraRotating = true;
                return;
            }

            CameraLookDirection = Vector2.zero;
            IsCameraRotating = false;
        }

        private void OnWithMoveJoystick()
        {
            if (_isMovementLocked)
            {
                MoveDirection = Vector2.zero;
                IsMoveInputPerformed = false;
                return;
            }

            MoveDirection = _moveJoystick.Direction;
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
            if (_isAttackLocked)
                return;

            _uiAttackPressed = true;
        }

        private void OnPickUpByButton()
        {
            if (_isPickUpLocked) return;
            IsPickUpPressed = true;
        }

        private void OnInventoryButtonClicked()
        {
            OnInventoryButtonPressed?.Invoke();
        }

        private void OnEquippedItemButtonClicked()
        {
            OnEquippedItemButtonPressed?.Invoke();
        }
    }
}