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
        private Dictionary<ParticleType, HashSet<ParticleSystem>> _activeParticles;

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
            _activeParticles = new Dictionary<ParticleType, HashSet<ParticleSystem>>();

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
                    Debug.LogError(
                        $"Loaded GameObject for key {effect.ParticleName} does not have ParticleSystem component.");
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

            if (particleEffect != null && particleEffect.gameObject.activeSelf)
                ReturnParticleSystemToPool(effectType, particleEffect);
        }
        
        public void StopEffect(ParticleType effectType)
        {
            if (!_activeParticles.TryGetValue(effectType, out var activeSet))
                return;
            
            var toStop = new List<ParticleSystem>(activeSet);
            foreach (var particleEffect in toStop)
            {
                if (particleEffect != null)
                    ReturnParticleSystemToPool(effectType, particleEffect);
            }
        }

        private ParticleSystem GetOrCreateParticleSystem(ParticleType effectType)
        {
            if (!_particlePool.ContainsKey(effectType))
                _particlePool[effectType] = new Queue<ParticleSystem>();

            var pool = _particlePool[effectType];

            while (pool.Count > 0)
            {
                var particleEffect = pool.Dequeue();

                if (particleEffect == null || particleEffect.isPlaying)
                    continue;

                particleEffect.gameObject.SetActive(true);

                RegisterActive(effectType, particleEffect);

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

            RegisterActive(effectType, particleEffect);

            return particleEffect;
        }

        private void RegisterActive(ParticleType type, ParticleSystem ps)
        {
            if (!_activeParticles.ContainsKey(type))
                _activeParticles[type] = new HashSet<ParticleSystem>();

            _activeParticles[type].Add(ps);
        }

        private void ReturnParticleSystemToPool(ParticleType effectType, ParticleSystem particleSystem)
        {
            if (particleSystem == null)
                return;

            if (_activeParticles.TryGetValue(effectType, out var activeSet))
                activeSet.Remove(particleSystem);

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
            if (_activeParticles != null)
            {
                foreach (var set in _activeParticles.Values)
                {
                    foreach (var particleEffect in set)
                        if (particleEffect != null) Destroy(particleEffect.gameObject);
                }
                _activeParticles.Clear();
            }

            if (_particlePool != null)
            {
                foreach (var pool in _particlePool.Values)
                {
                    foreach (var particleEffect in pool)
                        if (particleEffect != null) Destroy(particleEffect.gameObject);
                }
                _particlePool.Clear();
            }
        }
    }
}