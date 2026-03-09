using System.Collections.Generic;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.DataBase.InitDataSO;
using _Project.Scripts.Enemy;
using _Project.Scripts.Projectile;
using Cysharp.Threading.Tasks;
using Project.Scripts.Services;
using Reflex.Attributes;
using UnityEngine;

namespace _Project.Scripts.Services
{
    public class EnemyService : IEnemyService
    {
        private const string SkeletonPool = nameof(SkeletonPool);
        private const string SkeletonHeavyArmorPool = nameof(SkeletonHeavyArmorPool);
        private const string SkeletonRangerPool = nameof(SkeletonRangerPool);
        private const string PriestPool = nameof(PriestPool);
        private const string ArrowProjectilePool = nameof(ArrowProjectilePool);
        private const string MagicBallProjectilePool = nameof(MagicBallProjectilePool);

        private const bool IsAutoExpand = true;
        private const int MinValue = 0;
        private const int DefaultCountObjectsInPool = 3;

        private readonly Dictionary<EnemyType, EnemyData> _enemiesData = new();

        private IDataBaseService _dataBaseService;
        private IPlayerService _playerService;
        private IFloatingTextService _floatingTextService;
        private AudioSoundsService _audioSoundsService;
        private ParticleEffectsService _particleEffectsService;

        private EnemyInitData _enemyInitData;

        private ObjectPool<Skeleton> _skeletonPool;
        private ObjectPool<SkeletonHeavyArmor> _skeletonHeavyArmorPool;
        private ObjectPool<SkeletonRanger> _skeletonRangerPool;
        private ObjectPool<Priest> _priestPool;
        private ObjectPool<Arrow> _arrowProjectilePool;
        private ObjectPool<MagicBall> _magicBallProjectilePool;

        public bool IsInitiated { get; private set; }

        [Inject]
        public void Construct(
            IDataBaseService dataBaseService,
            IPlayerService playerService,
            AudioSoundsService audioSoundsService,
            ParticleEffectsService particleEffectsService,
            IFloatingTextService floatingTextService)
        {
            _dataBaseService = dataBaseService;
            _playerService = playerService;
            _audioSoundsService = audioSoundsService;
            _particleEffectsService = particleEffectsService;
            _floatingTextService = floatingTextService;
        }

        public UniTask Init()
        {
            if (IsInitiated)
                return UniTask.CompletedTask;

            foreach (var enemy in _dataBaseService.Content.Enemies)
            {
                _enemiesData.TryAdd(enemy.Type, enemy);
            }

            IsInitiated = true;

            return UniTask.CompletedTask;
        }

        public Skeleton CreateSkeleton()
        {
            CreateEnemySkeletonPool();

            var data = _enemiesData[EnemyType.Skeleton];
            var skeleton = _skeletonPool.GetFreeElement();

            skeleton.Construct(
                _playerService.Player,
                data,
                _floatingTextService,
                _particleEffectsService,
                _audioSoundsService);

            if (skeleton.Health.TargetHealth <= MinValue)
            {
                skeleton.Health.SetHealthValue(data.Health);
            }

            return skeleton;
        }

        public SkeletonHeavyArmor CreateSkeletonHeavyArmor()
        {
            CreateHeavyArmorSkeletonPool();

            var data = _enemiesData[EnemyType.SkeletonHeavyArmor];
            var skeletonHeavyArmor = _skeletonHeavyArmorPool.GetFreeElement();

            skeletonHeavyArmor.Construct(
                _playerService.Player,
                data,
                _floatingTextService,
                _particleEffectsService,
                _audioSoundsService);

            if (skeletonHeavyArmor.Health.TargetHealth <= MinValue)
            {
                skeletonHeavyArmor.Health.SetHealthValue(data.Health);
            }

            return skeletonHeavyArmor;
        }

        public SkeletonRanger CreateSkeletonRanger()
        {
            CreateRangerSkeletonPool();

            var data = _enemiesData[EnemyType.SkeletonRanger];
            var skeletonRanger = _skeletonRangerPool.GetFreeElement();

            skeletonRanger.Construct(
                _playerService.Player,
                data,
                _floatingTextService,
                _particleEffectsService,
                _audioSoundsService);
            
            skeletonRanger.Longbow.SetData(_playerService.Player.transform, _arrowProjectilePool, data.Damage);

            if (skeletonRanger.Health.TargetHealth <= MinValue)
            {
                skeletonRanger.Health.SetHealthValue(data.Health);
            }

            return skeletonRanger;
        }

        public Priest CreatePriest()
        {
            CreatePriestPool();

            var data = _enemiesData[EnemyType.Priest];
            var priest = _priestPool.GetFreeElement();

            priest.Construct(
                _playerService.Player,
                data,
                _floatingTextService,
                _particleEffectsService,
                _audioSoundsService);

            priest.MagicSpell.GetServices(_audioSoundsService, _particleEffectsService);
            priest.MagicSpell.SetData(_playerService.Player.transform, _magicBallProjectilePool, data.Damage);

            if (priest.Health.TargetHealth <= MinValue)
            {
                priest.Health.SetHealthValue(data.Health);
            }

            return priest;
        }

        public void GetData(EnemyInitData enemyInitData)
        {
            _enemyInitData = enemyInitData;
        }

        public EnemyData GetEnemyDataByType(EnemyType type)
        {
            return _enemiesData[type];
        }

        private void CreateEnemySkeletonPool()
        {
            if (_skeletonPool != null)
                return;

            _skeletonPool = new ObjectPool<Skeleton>(
                _enemyInitData.SkeletonPrefab,
                DefaultCountObjectsInPool,
                new GameObject(SkeletonPool).transform)
            {
                AutoExpand = IsAutoExpand,
            };
        }

        private void CreateHeavyArmorSkeletonPool()
        {
            if (_skeletonHeavyArmorPool != null)
                return;

            _skeletonHeavyArmorPool = new ObjectPool<SkeletonHeavyArmor>(
                _enemyInitData.SkeletonHeavyArmorPrefab,
                DefaultCountObjectsInPool,
                new GameObject(SkeletonHeavyArmorPool).transform)
            {
                AutoExpand = IsAutoExpand,
            };
        }

        private void CreateRangerSkeletonPool()
        {
            if (_skeletonRangerPool != null)
                return;

            _skeletonRangerPool = new ObjectPool<SkeletonRanger>(
                _enemyInitData.SkeletonRangerPrefab,
                DefaultCountObjectsInPool,
                new GameObject(SkeletonRangerPool).transform)
            {
                AutoExpand = IsAutoExpand,
            };

            _arrowProjectilePool = new ObjectPool<Arrow>(
                _enemyInitData.ArrowProjectilePrefab,
                DefaultCountObjectsInPool,
                new GameObject(ArrowProjectilePool).transform)
            {
                AutoExpand = IsAutoExpand,
            };
        }

        private void CreatePriestPool()
        {
            if (_magicBallProjectilePool != null)
                return;

            _priestPool = new ObjectPool<Priest>(
                _enemyInitData.PriestPrefab,
                DefaultCountObjectsInPool,
                new GameObject(PriestPool).transform)
            {
                AutoExpand = IsAutoExpand,
            };

            _magicBallProjectilePool = new ObjectPool<MagicBall>(
                _enemyInitData.MagicBallProjectilePrefab,
                DefaultCountObjectsInPool,
                new GameObject(MagicBallProjectilePool).transform)
            {
                AutoExpand = IsAutoExpand,
            };
        }
    }
}