using _Project.Scripts.Audio.Sounds;
using _Project.Scripts.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Enemy
{
    public class SkeletonHeavyArmor : Enemy
    {
        [field: SerializeField] public UnityEngine.AI.NavMeshAgent NavMeshAgent { get; private set; }

        protected override void OnPlayHitEffect()
        {
            AudioSoundsService.PlaySound(SoundsType.HeavyArmorHit).Forget();
            base.OnPlayHitEffect();
        }
    }
}