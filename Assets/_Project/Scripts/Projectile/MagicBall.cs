using _Project.Scripts.Effects;
using UnityEngine;

namespace _Project.Scripts.Projectile
{
    public class MagicBall : EnemyProjectile
    {
        protected override void OnTriggerEnter(Collider collision)
        {
            base.OnTriggerEnter(collision);
            ParticleEffectsService.PlayEffect(ParticleType.ExplosionFireball, transform.position);
        }
    }
}