using System;
using UnityEngine;

namespace _Project.Scripts.Player
{
    public class Attack : MonoBehaviour
    {
        [SerializeField] private float power = 10;
        [SerializeField] private float _velocity;

        public event Action<bool> OnAttaked;

        public bool IsAttacking { get; private set; }
        private Transform _currentTarget;

        private void OnTriggerStay(Collider other)
        {
            if (other.TryGetComponent(out Enemy.Enemy enemy))
            {
                _currentTarget = other.transform;
                IsAttacking = true;
                
                Vector3 lookPos = _currentTarget.position - transform.position;
                lookPos.y = 0f;
                if (lookPos.sqrMagnitude > 0.001f)
                {
                    transform.rotation = Quaternion.LookRotation(lookPos);
                }

                OnAttaked?.Invoke(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_currentTarget != null && other.transform == _currentTarget)
            {
                _currentTarget = null;
                IsAttacking = false;
                OnAttaked?.Invoke(false);
            }
        }
    }
}