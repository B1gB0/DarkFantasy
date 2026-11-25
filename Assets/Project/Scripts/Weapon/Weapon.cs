using Project.Scripts.Services;
using UnityEngine;

namespace Project.Scripts.Weapon
{
    public abstract class Weapon : MonoBehaviour
    {
        private AudioSoundsService _audioSoundsService;
        private ParticleEffectsService _particleEffectsService;
        private float _damage;
        private Transform _target;

        protected AudioSoundsService AudioSoundsService => _audioSoundsService;
        protected ParticleEffectsService ParticleEffectsService => _particleEffectsService;
        protected float Damage => _damage;
        protected Transform Target => _target;

        public abstract void Attack();

        public void GetServices(AudioSoundsService audioSoundsService, ParticleEffectsService particleEffectsService)
        {
            _audioSoundsService = audioSoundsService;
            _particleEffectsService = particleEffectsService;
        }

        public virtual void SetData(Transform target, float damage)
        {
            _target = target;
            _damage = damage;
        }
    }
}