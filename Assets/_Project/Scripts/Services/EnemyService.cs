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
        private const string SkeletonEnemyPool = nameof(SkeletonEnemyPool);
        
        private const bool IsAutoExpand = true;
        private const int MinValue = 0;
        private const int DefaultCountObjectsInPool = 3;
        
        private readonly Dictionary<EnemyType, EnemyData> _enemiesData = new ();

        private IDataBaseService _dataBaseService;
        private LevelInitData _levelInitData;
        private SkeletonInitData _skeletonInitData;
        
        private ObjectPool<Skeleton> _skeletonEnemyPool;

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
            var skeleton = _skeletonEnemyPool.GetFreeElement();

            return skeleton;
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
                _skeletonEnemyPool = new ObjectPool<Skeleton>(
                    _skeletonInitData.SkeletonPrefab,
                    DefaultCountObjectsInPool,
                    new GameObject(SkeletonEnemyPool).transform)
                {
                    AutoExpand = IsAutoExpand,
                };
            }
        }
    }
}