using _Project.Scripts.Audio.Sounds;
using _Project.Scripts.Effects;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Enemy
{
    public class BanditLeader : EnemyMelee
    {
        [field: SerializeField] public UnityEngine.AI.NavMeshAgent NavMeshAgent { get; private set; }
        
        protected override void OnPlayHitEffect()
        {
            AudioSoundsService.PlaySound(SoundsType.LightArmorHit).Forget();
            ParticleEffectsService.PlayEffect(ParticleType.RedBloodHit, Health.HitPoint.position);
        }
        
        protected override void OnDie()
        {
            ExperiencePoints.OnKill(this);
            base.OnDie();
        }
    }
}