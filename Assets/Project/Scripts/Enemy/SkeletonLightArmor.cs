using Cysharp.Threading.Tasks;
using Project.Scripts.Audio.Sounds;

namespace Project.Scripts.Enemy
{
    public class SkeletonLightArmor : EnemyMelee
    {
        protected override void OnPlayHitEffect()
        {
            AudioSoundsService.PlaySound(SoundsType.LightArmorHit).Forget();
            base.OnPlayHitEffect();
        }

        protected override void OnDie()
        {
            ExperiencePoints.OnKill(this);
            base.OnDie();
        }
    }
}