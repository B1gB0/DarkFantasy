using Cysharp.Threading.Tasks;
using Project.Scripts.Audio.Sounds;
using Project.Scripts.Effects;

namespace Project.Scripts.Projectile
{
    public class Fireball : EnemyProjectile
    {
        protected override void OnDisable()
        {
            base.OnDisable();

            if (ParticleEffectsService != null)
                ParticleEffectsService.PlayEffect(ParticleType.ExplosionFireball, transform.position);

            if (AudioSoundsService != null)
                AudioSoundsService.PlaySound(SoundsType.ExplosionFireballSound).Forget();
        }
    }
}