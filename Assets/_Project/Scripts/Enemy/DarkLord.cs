using _Project.Scripts.Audio.Sounds;
using _Project.Scripts.Effects;
using _Project.Scripts.Weapon.Enemy;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Enemy
{
    public class DarkLord : EnemyMelee
    {
        [field: SerializeField] public FireballSpell FireballSpell { get; private set; }
        [field: SerializeField] public Coil Coil { get; private set; }
        
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