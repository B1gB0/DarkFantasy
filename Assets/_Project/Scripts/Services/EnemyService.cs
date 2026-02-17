using System.Collections.Generic;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.DataBase.InitDataSO;
using _Project.Scripts.Enemy;
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
        
        private const bool IsAutoExpand = true;
        private const int MinValue = 0;
        private const int DefaultCountObjectsInPool = 3;
        
        private readonly Dictionary<EnemyType, EnemyData> _enemiesData = new ();

        private IDataBaseService _dataBaseService;
        private LevelInitData _levelInitData;
        private SkeletonInitData _skeletonInitData;
        
        private ObjectPool<Skeleton> _skeletonPool;
        private ObjectPool<SkeletonHeavyArmor> _skeletonHeavyArmorPool;
        private ObjectPool<SkeletonRanger> _skeletonRangerPool;

        public bool IsInitiated { get; private set; }

        [Inject]
        public void Construct(IDataBaseService dataBaseService)
        {
            _dataBaseService = dataBaseService;
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
            var data = _enemiesData[EnemyType.Skeleton];
            var skeleton = _skeletonPool.GetFreeElement();
            
            skeleton.EnemyPatrolComponent.InitPatrol(
                _levelInitData.EnemyPatrolPositions,
                skeleton.AnimatedStateMachine);

            return skeleton;
        }

        public SkeletonHeavyArmor CreateSkeletonHeavyArmor()
        {
            var data = _enemiesData[EnemyType.SkeletonHeavyArmor];
            var skeletonHeavyArmor = _skeletonHeavyArmorPool.GetFreeElement();
            
            skeletonHeavyArmor.EnemyPatrolComponent.InitPatrol(
                _levelInitData.EnemyPatrolPositions,
                skeletonHeavyArmor.AnimatedStateMachine);

            return skeletonHeavyArmor;
        }

        public SkeletonRanger CreateSkeletonRanger()
        {
            var data = _enemiesData[EnemyType.SkeletonRanger];
            var skeletonRanger = _skeletonRangerPool.GetFreeElement();
            
            skeletonRanger.EnemyPatrolComponent.InitPatrol(
                _levelInitData.EnemyPatrolPositions,
                skeletonRanger.AnimatedStateMachine);

            return skeletonRanger;
        }

        public void GetData(LevelInitData levelInitData, SkeletonInitData skeletonInitData)
        {
            _levelInitData = levelInitData;
            _skeletonInitData = skeletonInitData;
            CreateEnemyObjectPools();
        }

        public EnemyData GetEnemyDataByType(EnemyType type)
        {
            return _enemiesData[type];
        }

        private void CreateEnemyObjectPools()
        {
            if (_levelInitData.SkeletonEnemySpawnPositions.Count > MinValue)
            {
                _skeletonPool = new ObjectPool<Skeleton>(
                    _skeletonInitData.SkeletonPrefab,
                    DefaultCountObjectsInPool,
                    new GameObject(SkeletonPool).transform)
                {
                    AutoExpand = IsAutoExpand,
                };
            }
            
            if (_levelInitData.SkeletonHeavyArmorEnemySpawnPositions.Count > MinValue)
            {
                _skeletonHeavyArmorPool = new ObjectPool<SkeletonHeavyArmor>(
                    _skeletonInitData.SkeletonHeavyArmorPrefab,
                    DefaultCountObjectsInPool,
                    new GameObject(SkeletonHeavyArmorPool).transform)
                {
                    AutoExpand = IsAutoExpand,
                };
            }
            
            if (_levelInitData.SkeletonRangerEnemySpawnPositions.Count > MinValue)
            {
                _skeletonRangerPool = new ObjectPool<SkeletonRanger>(
                    _skeletonInitData.SkeletonRangerPrefab,
                    DefaultCountObjectsInPool,
                    new GameObject(SkeletonRangerPool).transform)
                {
                    AutoExpand = IsAutoExpand,
                };
            }
        }
    }
}