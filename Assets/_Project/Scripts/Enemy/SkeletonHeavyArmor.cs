using _Project.Scripts.Audio.Sounds;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Enemy
{
    public class SkeletonHeavyArmor : EnemyMelee
    {
        protected override void OnPlayHitEffect()
        {
            AudioSoundsService.PlaySound(SoundsType.HeavyArmorHit).Forget();
            base.OnPlayHitEffect();
        }
        
        protected override void OnDie()
        {
            ExperiencePoints.OnKill(this);
            base.OnDie();
        }
    }
}