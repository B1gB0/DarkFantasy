using _Project.Scripts.Audio.Sounds;
using _Project.Scripts.Effects;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Projectile
{
    public class MagicBall : EnemyProjectile
    {
        protected override void OnTriggerEnter(Collider collision)
        {
            base.OnTriggerEnter(collision);
            ParticleEffectsService.PlayEffect(ParticleType.ExplosionFireball, transform.position);
            AudioSoundsService.PlaySound(SoundsType.ExplosionFireballSound).Forget();
        }
    }
}