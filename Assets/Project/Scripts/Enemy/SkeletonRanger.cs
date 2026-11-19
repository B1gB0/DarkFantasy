using Cysharp.Threading.Tasks;
using Project.Scripts.Audio.Sounds;

namespace Project.Scripts.Enemy
{
    public class SkeletonRanger : EnemyRanger
    {
        protected override void OnPlayHitEffect()
        {
            AudioSoundsService.PlaySound(SoundsType.SkeletonHit).Forget();
            base.OnPlayHitEffect();
        }
        
        protected override void OnDie()
        {
            ExperiencePoints.OnKill(this);
            base.OnDie();
        }
    }
}