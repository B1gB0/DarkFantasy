using _Project.Scripts.Audio.Sounds;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace _Project.Scripts.Enemy
{
    public class SkeletonLightArmor : Enemy
    {
        [field: SerializeField] public NavMeshAgent NavMeshAgent { get; private set; }
        
        protected override void OnPlayHitEffect()
        {
            AudioSoundsService.PlaySound(SoundsType.LightArmorHit).Forget();
            base.OnPlayHitEffect();
        }
    }
}