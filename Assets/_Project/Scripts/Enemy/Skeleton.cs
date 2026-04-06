using _Project.Scripts.Audio.Sounds;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Enemy
{
    public class Skeleton : Enemy
    {
        [field: SerializeField] public UnityEngine.AI.NavMeshAgent NavMeshAgent { get; private set; }
        
        protected override void OnPlayHitEffect()
        {
            AudioSoundsService.PlaySound(SoundsType.SkeletonHit).Forget();
            base.OnPlayHitEffect();
        }
    }
}