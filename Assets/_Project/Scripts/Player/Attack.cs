using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Scripts.Player
{
    public class Attack : MonoBehaviour
    {
        [SerializeField] private float power = 10;
        [SerializeField] private float _velocity;

        private Transform _currentTarget;
        private InputSystem _inputSystem;
        private bool _isPressedMouse;
        
        public event Action<bool> OnAttaсked;

        [SerializeField] private SwordHitbox _swordHitbox;
        
        public bool IsAttacking { get; private set; }

        public void OnAttackPerformed(InputAction.CallbackContext context)
        {
            if (IsAttacking) return;
            CommonAttack();
        }
        
        public void OnAttackCanceled(InputAction.CallbackContext context)
        {
            if (context.control!= null && context.control.device is Mouse)
            {
                _isPressedMouse = false;
            }
        }

        private void CommonAttack()
        {
            IsAttacking = true;
            OnAttaсked?.Invoke(true);
        }
    
        public void StartDamageWindow()
        {
            _swordHitbox.CanDamage = true;
        }

        public void EndDamageWindow()
        {
            _swordHitbox.CanDamage = false;
            IsAttacking = false;
            OnAttaсked?.Invoke(false);
        }

    }
}