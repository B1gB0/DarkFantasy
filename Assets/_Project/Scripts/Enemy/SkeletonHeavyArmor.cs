using _Project.Scripts.Audio.Sounds;
using _Project.Scripts.Weapon.Enemy;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace _Project.Scripts.Enemy
{
    public class SkeletonHeavyArmor : Enemy
    {
        [field: SerializeField] public NavMeshAgent NavMeshAgent { get; private set; }
        [field: SerializeField] public MeleeWeapon MeleeWeapon { get; private set; }

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