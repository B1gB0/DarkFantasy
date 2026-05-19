using _Project.Scripts.Audio.Sounds;
using _Project.Scripts.Effects;
using _Project.Scripts.Weapon.Enemy;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace _Project.Scripts.Enemy
{
    public class Bandit : Enemy
    {
        [SerializeField, Range(0f, 1f)] private float _hatChance = 0.5f;
        
        [field: SerializeField] public NavMeshAgent NavMeshAgent { get; private set; }
        [field: SerializeField] public MeleeWeapon MeleeWeapon { get; private set; }
        [field: SerializeField] public GameObject Hat { get; private set; }

        private void Awake()
        {
            Hat.gameObject.SetActive(Random.value < _hatChance);
        }
        
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