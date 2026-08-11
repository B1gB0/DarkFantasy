using UnityEngine;

namespace _Project.Scripts.Projectile
{
    public abstract class EnemyProjectile : Projectile
    {
        protected override void OnTriggerEnter(Collider collision)
        {
            if (collision.gameObject.TryGetComponent(out Player.Core.Player player))
            {
                player.Health.TakeDamage(Damage, false, player.PlayerCharacteristics.Armor);
                gameObject.SetActive(false);
            }

            CheckBordersLayers(collision);
        }
    }
}