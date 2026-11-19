using Cysharp.Threading.Tasks;
using Project.Scripts.Audio.Sounds;
using Project.Scripts.Effects;
using UnityEngine;

namespace Project.Scripts.Enemy
{
    public class Bandit : EnemyMelee
    {
        [SerializeField]
        [Range(0f, 1f)] private float _hatChance = 0.5f;

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