using System.Collections.Generic;
using _Project.Scripts.Services;
using UnityEngine;

namespace _Project.Scripts.Weapon
{
    public abstract class ExplodingObject : Projectile.Projectile
    {
        private readonly Collider[] _colliderBuffer = new Collider[32];
        private readonly List<Scripts.Enemy.Enemy> _enemyBuffer = new(32);

        [SerializeField] private LayerMask _layerEnemy;

        protected float ExplosionRadius;

        protected AudioSoundsService AudioSoundsService;
        protected ParticleEffectsService ParticleEffectsService;

        public void GetExplosionEffects(
            ParticleEffectsService particleEffectsService,
            AudioSoundsService audioSoundsService)
        {
            ParticleEffectsService = particleEffectsService;
            AudioSoundsService = audioSoundsService;
        }

        protected virtual void Explode()
        {
            // ParticleEffectsService.PlayEffect(ParticleEffectType.MineExplosion, Transform.position);
            // AudioSoundsService.PlaySound(SoundsType.Mines).Forget();

            foreach (Scripts.Enemy.Enemy explosiveObject in GetEnemies())
            {
                explosiveObject.Health.TakeDamage(Damage, true, explosiveObject.Armor);
            }

            gameObject.SetActive(false);
        }

        protected List<Scripts.Enemy.Enemy> GetEnemies()
        {
            _enemyBuffer.Clear();

            int hitCount = Physics.OverlapSphereNonAlloc(
                Transform.position,
                ExplosionRadius,
                _colliderBuffer,
                _layerEnemy);

            for (int i = 0; i < hitCount; i++)
            {
                Collider hit = _colliderBuffer[i];

                if (hit.attachedRigidbody != null &&
                    hit.gameObject.TryGetComponent(out Scripts.Enemy.Enemy enemyActor))
                {
                    _enemyBuffer.Add(enemyActor);
                }
            }

            return _enemyBuffer;
        }
    }
}