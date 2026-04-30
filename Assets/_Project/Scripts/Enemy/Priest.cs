using _Project.Scripts.Audio.Sounds;
using _Project.Scripts.Effects;
using _Project.Scripts.Weapon.Enemy;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Enemy
{
    public class Priest : Enemy
    {
        [field: SerializeField] public UnityEngine.AI.NavMeshAgent NavMeshAgent { get; private set; }
        [field: SerializeField] public FireballSpell FireballSpell { get; private set; }
        [field: SerializeField] public Coil Coil { get; private set; }
        [field: SerializeField] public Omni Omni { get; private set; }
        
        protected override void OnPlayHitEffect()
        {
            AudioSoundsService.PlaySound(SoundsType.SkeletonHit).Forget();
            ParticleEffectsService.PlayEffect(ParticleType.RedBloodHit, Player.transform.position);
        }
    }
}