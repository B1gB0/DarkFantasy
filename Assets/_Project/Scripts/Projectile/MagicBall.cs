using _Project.Scripts.Audio.Sounds;
using _Project.Scripts.Effects;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Projectile
{
    public class MagicBall : EnemyProjectile
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