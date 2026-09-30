using _Project.Scripts.Enemy.StateMachine.Behaviour.States;
using UnityEngine;
using YG;

namespace _Project.Scripts.Player.Combat
{
    public class Sword : MonoBehaviour
    {
        [SerializeField] private Collider _collider;
        [SerializeField] [Range(0f, 1f)] private float _hitChance = 0.3f;
        
        private Core.Player _player;

        private void Awake()
        {
            _player = GetComponentInParent<Core.Player>();
        }

        private void Start()
        {
            DeactivateCollider();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out Enemy.Enemy enemy))
                return;
            
            enemy.Health.TakeDamage(_player.PlayerCharacteristics.GetTotalDamage(), true, enemy.Armor);

            if (Random.value < _hitChance)
            {
                enemy.EnemyStateMachine.SwitchState<HitState>();
            }
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