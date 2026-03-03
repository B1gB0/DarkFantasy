using System.Collections.Generic;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.DataBase.InitDataSO;
using _Project.Scripts.Enemy;
using _Project.Scripts.Projectile;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using UnityEngine;

namespace _Project.Scripts.Services
{
    public class EnemyService : IEnemyService
    {
        private const string SkeletonPool = nameof(SkeletonPool);
        private const string SkeletonHeavyArmorPool = nameof(SkeletonHeavyArmorPool);
        private const string SkeletonRangerPool = nameof(SkeletonRangerPool);
        private const string ArrowProjectilePool = nameof(ArrowProjectilePool);

        private const bool IsAutoExpand = true;
        private const int MinValue = 0;
        private const int DefaultCountObjectsInPool = 3;

        private readonly Dictionary<EnemyType, EnemyData> _enemiesData = new();

        private IDataBaseService _dataBaseService;
        private IPlayerService _playerService;
        
        private SkeletonInitData _skeletonInitData;

        private ObjectPool<Skeleton> _skeletonPool;
        private ObjectPool<SkeletonHeavyArmor> _skeletonHeavyArmorPool;
        private ObjectPool<SkeletonRanger> _skeletonRangerPool;
        private ObjectPool<Arrow> _arrowProjectilePool;

        public bool IsInitiated { get; private set; }

        [Inject]
        public void Construct(IDataBaseService dataBaseService, IPlayerService playerService)
        {
            _dataBaseService = dataBaseService;
            _playerService = playerService;
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

            skeleton.GetData(_playerService.Player, _enemiesData[EnemyType.Skeleton]);

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

            skeletonHeavyArmor.GetData(_playerService.Player, _enemiesData[EnemyType.SkeletonHeavyArmor]);

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

            skeletonRanger.GetData(_playerService.Player, _enemiesData[EnemyType.SkeletonRanger]);
            skeletonRanger.Longbow.SetData(_playerService.Player.transform, _arrowProjectilePool, data.Damage);

            if (skeletonRanger.Health.TargetHealth <= MinValue)
            {
                skeletonRanger.Health.SetHealthValue(data.Health);
            }

            return skeletonRanger;
        }

        public void GetData(SkeletonInitData skeletonInitData)
        {
            _skeletonInitData = skeletonInitData;
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
                _skeletonInitData.SkeletonPrefab,
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
                _skeletonInitData.SkeletonHeavyArmorPrefab,
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
                _skeletonInitData.SkeletonRangerPrefab,
                DefaultCountObjectsInPool,
                new GameObject(SkeletonRangerPool).transform)
            {
                AutoExpand = IsAutoExpand,
            };

            _arrowProjectilePool = new ObjectPool<Arrow>(
                _skeletonInitData.ArrowProjectilePrefab,
                DefaultCountObjectsInPool,
                new GameObject(ArrowProjectilePool).transform)
            {
                AutoExpand = IsAutoExpand,
            };
        }
    }
}