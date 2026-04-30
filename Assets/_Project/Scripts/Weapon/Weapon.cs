using _Project.Scripts.Services;
using UnityEngine;

namespace _Project.Scripts.Weapon
{
    public abstract class Weapon : MonoBehaviour
    {
        protected AudioSoundsService AudioSoundsService;
        protected ParticleEffectsService ParticleEffectsService;
        protected float Damage;
        protected Transform Target;
        
        public abstract void Attack();
        
        public void GetServices(AudioSoundsService audioSoundsService, ParticleEffectsService particleEffectsService)
        {
            AudioSoundsService = audioSoundsService;
            ParticleEffectsService = particleEffectsService;
        }

        public virtual void SetData(Transform target, float damage)
        {
            Target = target;
            Damage = damage;
        }
    }
}