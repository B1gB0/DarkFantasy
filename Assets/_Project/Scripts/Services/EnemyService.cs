using System.Collections.Generic;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.DataBase.InitDataSO;
using _Project.Scripts.Enemy;
using _Project.Scripts.Enemy.StateMachine.Behaviour.States;
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
        
        private readonly Dictionary<EnemyType, EnemyData> _enemiesData = new ();

        private IDataBaseService _dataBaseService;
        private IPlayerService _playerService;
        
        private LevelInitData _levelInitData;
        private Level.Level _level;
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
            var data = _enemiesData[EnemyType.Skeleton];
            var skeleton = _skeletonPool.GetFreeElement();

            skeleton.GetData(_playerService.Player, _enemiesData[EnemyType.Skeleton]);
            
            if (skeleton.Health.TargetHealth <= MinValue)
            {
                skeleton.Health.SetHealthValue(data.Health);
            }
            
            skeleton.EnemyStateMachine.AddState(new PatrolState(_levelInitData.EnemyFirstPatrolPositions));
            skeleton.EnemyStateMachine.InitializeAllStates();
            skeleton.EnemyStateMachine.SwitchState<PatrolState>();

            return skeleton;
        }

        public SkeletonHeavyArmor CreateSkeletonHeavyArmor()
        {
            var data = _enemiesData[EnemyType.SkeletonHeavyArmor];
            var skeletonHeavyArmor = _skeletonHeavyArmorPool.GetFreeElement();

            skeletonHeavyArmor.GetData(_playerService.Player, _enemiesData[EnemyType.SkeletonHeavyArmor]);
            
            if (skeletonHeavyArmor.Health.TargetHealth <= MinValue)
            {
                skeletonHeavyArmor.Health.SetHealthValue(data.Health);
            }
            
            skeletonHeavyArmor.EnemyStateMachine.AddState(new PatrolState(_levelInitData.EnemyFirstPatrolPositions));
            skeletonHeavyArmor.EnemyStateMachine.InitializeAllStates();
            skeletonHeavyArmor.EnemyStateMachine.SwitchState<PatrolState>();

            return skeletonHeavyArmor;
        }

        public SkeletonRanger CreateSkeletonRanger()
        {
            var data = _enemiesData[EnemyType.SkeletonRanger];
            var skeletonRanger = _skeletonRangerPool.GetFreeElement();

            skeletonRanger.GetData(_playerService.Player, _enemiesData[EnemyType.SkeletonRanger]);
            skeletonRanger.Longbow.SetData(_playerService.Player.transform, _arrowProjectilePool, data.Damage);
            
            if (skeletonRanger.Health.TargetHealth <= MinValue)
            {
                skeletonRanger.Health.SetHealthValue(data.Health);
            }
            
            skeletonRanger.EnemyStateMachine.InitializeAllStates();
            skeletonRanger.EnemyStateMachine.SwitchState<IdleState>();

            return skeletonRanger;
        }

        public void GetData(LevelInitData levelInitData, SkeletonInitData skeletonInitData, Level.Level level)
        {
            _levelInitData = levelInitData;
            _skeletonInitData = skeletonInitData;
            _level = level;
            CreateEnemyObjectPools();
        }

        public EnemyData GetEnemyDataByType(EnemyType type)
        {
            return _enemiesData[type];
        }

        private void CreateEnemyObjectPools()
        {
            if (_level.CountSkeletonEnemy > MinValue)
            {
                _skeletonPool = new ObjectPool<Skeleton>(
                    _skeletonInitData.SkeletonPrefab,
                    DefaultCountObjectsInPool,
                    new GameObject(SkeletonPool).transform)
                {
                    AutoExpand = IsAutoExpand,
                };
            }
            
            if (_level.CountSkeletonHeavyArmorEnemy > MinValue)
            {
                _skeletonHeavyArmorPool = new ObjectPool<SkeletonHeavyArmor>(
                    _skeletonInitData.SkeletonHeavyArmorPrefab,
                    DefaultCountObjectsInPool,
                    new GameObject(SkeletonHeavyArmorPool).transform)
                {
                    AutoExpand = IsAutoExpand,
                };
            }
            
            if (_level.CountSkeletonRangerEnemy > MinValue)
            {
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
}