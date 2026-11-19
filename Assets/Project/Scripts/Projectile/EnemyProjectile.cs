using Project.Scripts.Player;
using UnityEngine;

namespace Project.Scripts.Projectile
{
    public abstract class EnemyProjectile : Projectile
    {
        protected override void OnTriggerEnter(Collider collision)
        {
            if (collision.gameObject.TryGetComponent(out PlayerHitBox hitBox))
            {
                hitBox.HandleEnemyAttack(Damage);
                gameObject.SetActive(false);
            }

            CheckBordersLayers(collision);
        }
    }
}