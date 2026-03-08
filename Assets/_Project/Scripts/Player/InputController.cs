using UnityEngine;

namespace _Project.Scripts.Player
{
    public class InputController : MonoBehaviour
    {
        [SerializeField] private Movement _movement;
        [SerializeField] private Attack _attack;

        private InputSystem _inputSystem;

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
            _inputSystem.PLayer.Attack.canceled += _attack.OnAttackCanceled;
        }

        private void OnDisable()
        {
            _inputSystem.PLayer.Disable();
            _inputSystem.PLayer.Move.performed -= _movement.OnMovePerformed;
            _inputSystem.PLayer.Move.canceled -= _movement.OnMoveCanceled;

            _inputSystem.PLayer.Attack.performed -= _attack.OnAttackPerformed;
            _inputSystem.PLayer.Attack.canceled -= _attack.OnAttackCanceled;
        }
    }
}