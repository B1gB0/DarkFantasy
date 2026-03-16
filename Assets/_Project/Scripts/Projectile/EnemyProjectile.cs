using UnityEngine;

namespace _Project.Scripts.Projectile
{
    public abstract class EnemyProjectile : Projectile
    {
        protected override void OnTriggerEnter(Collider collision)
        {
            if (collision.gameObject.TryGetComponent(out Player.Player player))
            {
                player.Health.TakeDamage(Damage, player.PlayerCharacteristics.Armor);
                gameObject.SetActive(false);
            }

            CheckDefaultAndResourceLayer(collision);
        }
    }
}