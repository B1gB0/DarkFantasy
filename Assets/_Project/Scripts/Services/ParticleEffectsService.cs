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
        
        private readonly Dictionary<ParticleType, ParticleEffect> _particlesDictionary = new();
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

            await InitializeSoundDictionary();
            
            IsInitiated = true;
        }
        
        public void PlayEffect(ParticleType effectType, Vector3 position)
        {
            if (!IsInitiated)
                return;

            if (!_particlesDictionary.ContainsKey(effectType))
                return;

            PlayEffectAsync(effectType, position).Forget();
        }
        
        private async UniTask InitializeSoundDictionary()
        {
            ParticlesConfig particlesConfig = await _resourceService.Load<ParticlesConfig>(ParticlesConfigPath);

            foreach (var particleEffect in particlesConfig.Particles)
            {
                ParticleSystem clip = await _resourceService.Load<ParticleSystem>(particleEffect.ParticleName);
                particleEffect.ParticleSystem = clip;
                Enum.TryParse(particleEffect.ParticleName, out ParticleType particleType);
                _particlesDictionary.TryAdd(particleType, particleEffect);
            }
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
            if (!_particlesDictionary.ContainsKey(effectType))
                return null;

            var config = _particlesDictionary[effectType];
            var particleEffect = Instantiate(config.ParticleSystem, _particleParent);

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