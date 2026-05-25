using _Project.Scripts.Enemy.StateMachine.Behaviour.States;
using UnityEngine;
using YG;

namespace _Project.Scripts.Player.Combat
{
    public class Sword : MonoBehaviour
    {
        [SerializeField] private int _swordDamage = 10;
        [SerializeField] private Collider _collider;

        private void Start()
        {
            DeactivateCollider();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Enemy.Enemy enemy))
            {
                enemy.EnemyStateMachine.SwitchState<HitState>();
                enemy.Health.TakeDamage(YG2.saves.PlayerCharacteristics.Damage, enemy.Armor);
            }
        }

        private void Update()
        {
            Debug.Log(_collider.enabled);
        }

        public void ActivateCollider()
        {
            _collider.enabled = true;
        }

        public void DeactivateCollider()
        {
            _collider.enabled = false;
        }
    }
}