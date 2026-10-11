using UnityEngine;

namespace _Project.Scripts.Player
{
    public class PlayerHitBox : MonoBehaviour
    {
        private Core.Player _player;

        private void Awake()
        {
            _player = GetComponentInParent<Core.Player>();
        }

        public void HandleEnemyAttack(float damage, Health healthOwner = null)
        {
            _player.Health.TakeDamage(
                damage,
                _player.PlayerCharacteristics.GetTotalArmor(),
                healthOwner);
        }
    }
}