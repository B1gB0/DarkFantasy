using System.Collections.Generic;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.DataBase.InitDataSO;
using _Project.Scripts.Enemy;
using _Project.Scripts.Experience;
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
        private const string PriestPool = nameof(PriestPool);
        private const string BanditPool = nameof(BanditPool);
        private const string BanditRangerPool = nameof(BanditRangerPool);
        private const string BanditLeaderPool = nameof(BanditLeaderPool);
        private const string DarkLordPool = nameof(DarkLordPool);
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
        private IExperiencePoints _experiencePoints;
        private ICurrencyService _currencyService;

        private EnemyInitData _enemyInitData;

        private ObjectPool<Skeleton> _skeletonPool;
        private ObjectPool<SkeletonHeavyArmor> _skeletonHeavyArmorPool;
        private ObjectPool<SkeletonRanger> _skeletonRangerPool;
        private ObjectPool<Priest> _priestPool;
        private ObjectPool<Bandit> _banditPool;
        private ObjectPool<BanditRanger> _banditRangerPool;
        private ObjectPool<BanditLeader> _banditLeaderPool;
        private ObjectPool<DarkLord> _darkLordPool;
        private ObjectPool<Arrow> _arrowProjectilePool;
        private ObjectPool<Fireball> _magicBallProjectilePool;

        public bool IsInitiated { get; private set; }

        [Inject]
        public void Construct(
            IDataBaseService dataBaseService,
            IPlayerService playerService,
            AudioSoundsService audioSoundsService,
            ParticleEffectsService particleEffectsService,
            IFloatingTextService floatingTextService,
            IExperiencePoints experiencePoints,
            ICurrencyService currencyService)
        {
            _dataBaseService = dataBaseService;
            _playerService = playerService;
            _audioSoundsService = audioSoundsService;
            _particleEffectsService = particleEffectsService;
            _floatingTextService = floatingTextService;
            _experiencePoints = experiencePoints;
            _currencyService = currencyService;
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
                _audioSoundsService,
                _experiencePoints,
                _currencyService);
            
            skeleton.MeleeWeapon.SetData(_playerService.Player.transform, data.Damage);

            if (skeleton.Health.TargetHealth <= MinValue)
            {
                skeleton.Health.LoadHealth(data.Health, data.Health);
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
                _audioSoundsService,
                _experiencePoints,
                _currencyService);
            
            skeletonHeavyArmor.MeleeWeapon.SetData(_playerService.Player.transform, data.Damage);

            if (skeletonHeavyArmor.Health.TargetHealth <= MinValue)
            {
                skeletonHeavyArmor.Health.LoadHealth(data.Health, data.Health);
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
                _audioSoundsService,
                _experiencePoints,
                _currencyService);

            skeletonRanger.Longbow.SetProjectile(_arrowProjectilePool, data.SpeedProjectile);
            skeletonRanger.Longbow.SetData(_playerService.Player.transform, data.Damage);

            if (skeletonRanger.Health.TargetHealth <= MinValue)
            {
                skeletonRanger.Health.LoadHealth(data.Health, data.Health);
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
                _audioSoundsService,
                _experiencePoints,
                _currencyService);

            priest.FireballSpell.GetServices(_audioSoundsService, _particleEffectsService);
            priest.FireballSpell.SetProjectile(_magicBallProjectilePool, data.SpeedProjectile);
            priest.FireballSpell.SetData(_playerService.Player.transform, data.Damage);
            
            priest.Coil.SetData(_playerService.Player.transform, data.Damage);
            priest.Coil.GetServices(_audioSoundsService, _particleEffectsService);
            
            priest.Omni.SetData(_playerService.Player.transform, data.Damage);
            priest.Omni.GetServices(_audioSoundsService, _particleEffectsService);

            if (priest.Health.TargetHealth <= MinValue)
            {
                priest.Health.LoadHealth(data.Health, data.Health);
            }

            return priest;
        }
        
        public Bandit CreateBandit()
        {
            CreateEnemyBanditPool();

            var data = _enemiesData[EnemyType.BanditMelee];
            var bandit = _banditPool.GetFreeElement();

            bandit.Construct(
                _playerService.Player,
                data,
                _floatingTextService,
                _particleEffectsService,
                _audioSoundsService,
                _experiencePoints,
                _currencyService);
            
            bandit.MeleeWeapon.SetData(_playerService.Player.transform, data.Damage);

            if (bandit.Health.TargetHealth <= MinValue)
            {
                bandit.Health.LoadHealth(data.Health, data.Health);
            }

            return bandit;
        }
        
        public BanditRanger CreateBanditRanger()
        {
            CreateEnemyBanditRangerPool();

            var data = _enemiesData[EnemyType.BanditRanger];
            var bandit = _banditRangerPool.GetFreeElement();

            bandit.Construct(
                _playerService.Player,
                data,
                _floatingTextService,
                _particleEffectsService,
                _audioSoundsService,
                _experiencePoints,
                _currencyService);
            
            bandit.Longbow.SetProjectile(_arrowProjectilePool, data.SpeedProjectile);
            bandit.Longbow.SetData(_playerService.Player.transform, data.Damage);

            if (bandit.Health.TargetHealth <= MinValue)
            {
                bandit.Health.LoadHealth(data.Health, data.Health);
            }

            return bandit;
        }
        
        public BanditLeader CreateBanditLeader()
        {
            CreateEnemyBanditLeaderPool();

            var data = _enemiesData[EnemyType.BanditLeader];
            var banditLeader = _banditLeaderPool.GetFreeElement();

            banditLeader.Construct(
                _playerService.Player,
                data,
                _floatingTextService,
                _particleEffectsService,
                _audioSoundsService,
                _experiencePoints,
                _currencyService);
            
            banditLeader.MeleeWeapon.SetData(_playerService.Player.transform, data.Damage);

            if (banditLeader.Health.TargetHealth <= MinValue)
            {
                banditLeader.Health.LoadHealth(data.Health, data.Health);
            }

            return banditLeader;
        }
        
        public DarkLord CreateDarkLord()
        {
            CreateEnemyDarkLordPool();

            var data = _enemiesData[EnemyType.DarkLord];
            var darkLord = _darkLordPool.GetFreeElement();

            darkLord.Construct(
                _playerService.Player,
                data,
                _floatingTextService,
                _particleEffectsService,
                _audioSoundsService,
                _experiencePoints,
                _currencyService);
            
            darkLord.MeleeWeapon.SetData(_playerService.Player.transform, data.Damage);

            if (darkLord.Health.TargetHealth <= MinValue)
            {
                darkLord.Health.LoadHealth(data.Health, data.Health);
            }

            return darkLord;
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
            
            CreateArrowPool();
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

            _magicBallProjectilePool = new ObjectPool<Fireball>(
                _enemyInitData.FireballProjectilePrefab,
                DefaultCountObjectsInPool,
                new GameObject(MagicBallProjectilePool).transform)
            {
                AutoExpand = IsAutoExpand,
            };
        }
        
        private void CreateEnemyBanditPool()
        {
            if (_banditPool != null)
                return;

            _banditPool = new ObjectPool<Bandit>(
                _enemyInitData.BanditPrefab,
                DefaultCountObjectsInPool,
                new GameObject(BanditPool).transform)
            {
                AutoExpand = IsAutoExpand,
            };
        }
        
        private void CreateEnemyBanditRangerPool()
        {
            if (_banditRangerPool != null)
                return;

            _banditRangerPool = new ObjectPool<BanditRanger>(
                _enemyInitData.BanditRangerPrefab,
                DefaultCountObjectsInPool,
                new GameObject(BanditRangerPool).transform)
            {
                AutoExpand = IsAutoExpand,
            };
            
            CreateArrowPool();
        }
        
        private void CreateEnemyBanditLeaderPool()
        {
            if (_banditLeaderPool != null)
                return;

            _banditLeaderPool = new ObjectPool<BanditLeader>(
                _enemyInitData.BanditLeaderPrefab,
                DefaultCountObjectsInPool,
                new GameObject(BanditLeaderPool).transform)
            {
                AutoExpand = IsAutoExpand,
            };
        }
        
        private void CreateEnemyDarkLordPool()
        {
            if (_darkLordPool != null)
                return;

            _darkLordPool = new ObjectPool<DarkLord>(
                _enemyInitData.DarkLordPrefab,
                DefaultCountObjectsInPool,
                new GameObject(BanditLeaderPool).transform)
            {
                AutoExpand = IsAutoExpand,
            };
        }

        private void CreateArrowPool()
        {
            if(_arrowProjectilePool != null)
                return;

            _arrowProjectilePool = new ObjectPool<Arrow>(
                _enemyInitData.ArrowProjectilePrefab,
                DefaultCountObjectsInPool,
                new GameObject(ArrowProjectilePool).transform)
            {
                AutoExpand = IsAutoExpand,
            };
        }
    }
}