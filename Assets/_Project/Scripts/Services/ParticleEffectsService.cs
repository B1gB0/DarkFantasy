using System;
using System.Collections.Generic;
using _Project.Scripts.Effects;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using UnityEngine;

namespace _Project.Scripts.Services
{
    public class ParticleEffectsService : MonoBehaviour, IService
    {
        private const string ParticlesConfigPath = "ParticlesConfig";
        private const string ParticleEffects = nameof(ParticleEffects);
        
        private readonly Dictionary<ParticleType, ParticleSystem> _particles = new();
        
        private Dictionary<ParticleType, Queue<ParticleSystem>> _particlePool;
        
        private IResourceService _resourceService;
        private Transform _particleParent;
        
        public bool IsInitiated { get; private set; }
        
        [Inject]
        private void Construct(IResourceService resourceService)
        {
            _resourceService = resourceService;
        }
        
        public async UniTask Init()
        {
            if (IsInitiated)
                return;
            
            _particleParent = new GameObject(ParticleEffects).transform;
            _particleParent.SetParent(transform);

            _particlePool = new Dictionary<ParticleType, Queue<ParticleSystem>>();
            
            await LoadParticles();
            
            IsInitiated = true;
        }
        
        private async UniTask LoadParticles()
        {
            ParticlesConfig config = await _resourceService.Load<ParticlesConfig>(ParticlesConfigPath);
            if (config == null)
            {
                Debug.LogError($"Failed to load ParticlesConfig at {ParticlesConfigPath}");
                return;
            }

            foreach (var effect in config.Particles)
            {
                if (string.IsNullOrEmpty(effect.ParticleName))
                {
                    Debug.LogWarning($"Particle effect {effect.ParticleName} has no key, skipping.");
                    continue;
                }
                
                GameObject prefab = await _resourceService.Load<GameObject>(effect.ParticleName);
                if (prefab == null)
                {
                    Debug.LogError($"Failed to load particle prefab with key: {effect.ParticleName}");
                    continue;
                }

                ParticleSystem particle = prefab.GetComponent<ParticleSystem>();
                if (particle == null)
                {
                    Debug.LogError($"Loaded GameObject for key {effect.ParticleName} does not have ParticleSystem component.");
                    continue;
                }

                if (!Enum.TryParse(effect.ParticleName, out ParticleType type))
                {
                    Debug.LogWarning($"Could not parse ParticleType from {effect.ParticleName}, skipping.");
                    continue;
                }
                
                _particles[type] = particle;
            }
        }
        
        public void PlayEffect(ParticleType effectType, Vector3 position)
        {
            if (!IsInitiated)
                return;

            if (!_particles.ContainsKey(effectType))
                return;

            PlayEffectAsync(effectType, position).Forget();
        }

        private async UniTaskVoid PlayEffectAsync(ParticleType effectType, Vector3 position)
        {
            var particleEffect = GetOrCreateParticleSystem(effectType);

            var transformOfEffect = particleEffect.transform;
            transformOfEffect.position = position;

            particleEffect.Play(true);

            await WaitForParticleSystem(particleEffect);

            ReturnParticleSystemToPool(effectType, particleEffect);
        }

        private ParticleSystem GetOrCreateParticleSystem(ParticleType effectType)
        {
            if (!_particlePool.ContainsKey(effectType))
            {
                _particlePool[effectType] = new Queue<ParticleSystem>();
            }

            var pool = _particlePool[effectType];

            while (pool.Count > 0)
            {
                var particleEffect = pool.Dequeue();

                if (particleEffect == null || particleEffect.isPlaying)
                    continue;

                particleEffect.gameObject.SetActive(true);
                return particleEffect;
            }

            return CreateNewParticleSystem(effectType);
        }

        private ParticleSystem CreateNewParticleSystem(ParticleType effectType)
        {
            if (!_particles.ContainsKey(effectType))
                return null;

            var prefab = _particles[effectType];
            var particleEffect = Instantiate(prefab, _particleParent);

            return particleEffect;
        }

        private void ReturnParticleSystemToPool(ParticleType effectType, ParticleSystem particleSystem)
        {
            if (particleSystem == null)
                return;

            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            particleSystem.gameObject.SetActive(false);
            particleSystem.Clear(true);

            if (_particlePool.TryGetValue(effectType, out var queueEffects))
            {
                queueEffects.Enqueue(particleSystem);
            }
        }

        private async UniTask WaitForParticleSystem(ParticleSystem particleSystem)
        {
            if (particleSystem == null)
                return;

            await UniTask.WaitUntil(() => particleSystem == null || !particleSystem.IsAlive(true));
        }

        private void OnDestroy()
        {
            if (_particlePool == null)
                return;

            foreach (var pool in _particlePool.Values)
            {
                foreach (var particleSystem in pool)
                {
                    if (particleSystem != null)
                        Destroy(particleSystem.gameObject);
                }
            }

            _particlePool.Clear();
        }
    }
}