using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Scripts.Player.Combat
{
    public class Attack : MonoBehaviour
    {
        [SerializeField] private SwordHitbox _swordHitbox;

        public event Action<bool> OnAttacked;
        public event Action<int> OnComboChanged;

        public bool IsAttacking { get; private set; }

        private int _comboStep = 0;
        private bool _canQueueNextAttack = false;

        public void OnAttackPerformed(InputAction.CallbackContext context)
        {
            if (!IsAttacking)
            {
                StartCombo();
                return;
            }

            if (_canQueueNextAttack)
                QueueNextCombo();
        }

        private void StartCombo()
        {
            _comboStep = 1;
            IsAttacking = true;
            _canQueueNextAttack = false;

            OnAttacked?.Invoke(true);
            OnComboChanged?.Invoke(_comboStep);
        }

        private void QueueNextCombo()
        {
            _comboStep++;
            _canQueueNextAttack = false;

            OnComboChanged?.Invoke(_comboStep);
        }

        public void AllowCombo()
        {
            _canQueueNextAttack = true;
        }

        public void StartDamageWindow()
        {
            _swordHitbox.CanDamage = true;
        }

        public void EndDamageWindow()
        {
            _swordHitbox.CanDamage = false;
        }

        public void EndAttack()
        {
            ResetCombo();
        }

        private void ResetCombo()
        {
            _comboStep = 0;
            IsAttacking = false;
            _canQueueNextAttack = false;

            OnAttacked?.Invoke(false);
            OnComboChanged?.Invoke(0);
        }
    }
}