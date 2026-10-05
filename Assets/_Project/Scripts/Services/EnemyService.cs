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
        private IProgressionService _progressionService;

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
            ICurrencyService currencyService,
            IProgressionService progressionService)
        {
            _dataBaseService = dataBaseService;
            _playerService = playerService;
            _audioSoundsService = audioSoundsService;
            _particleEffectsService = particleEffectsService;
            _floatingTextService = floatingTextService;
            _experiencePoints = experiencePoints;
            _currencyService = currencyService;
            _progressionService = progressionService;
        }

        public UniTask Init()
        {
            if (IsInitiated) return UniTask.CompletedTask;

            foreach (var enemy in _dataBaseService.Content.Enemies)
                _enemiesData.TryAdd(enemy.Type, enemy);

            IsInitiated = true;
            return UniTask.CompletedTask;
        }

        public Skeleton CreateSkeleton()
        {
            CreateEnemySkeletonPool();

            var data = _enemiesData[EnemyType.Skeleton];
            var (hpMul, dmgMul) = GetMultipliers();
            var enemy = _skeletonPool.GetFreeElement();

            ConstructBase(enemy, data);
            ApplyHealth(enemy, data, hpMul);
            enemy.MeleeWeapon.SetData(_playerService.Player.transform, data.Damage * dmgMul);

            return enemy;
        }

        public SkeletonHeavyArmor CreateSkeletonHeavyArmor()
        {
            CreateHeavyArmorSkeletonPool();

            var data = _enemiesData[EnemyType.SkeletonHeavyArmor];
            var (hpMul, dmgMul) = GetMultipliers();
            var enemy = _skeletonHeavyArmorPool.GetFreeElement();

            ConstructBase(enemy, data);
            ApplyHealth(enemy, data, hpMul);
            enemy.MeleeWeapon.SetData(_playerService.Player.transform, data.Damage * dmgMul);

            return enemy;
        }

        public SkeletonRanger CreateSkeletonRanger()
        {
            CreateRangerSkeletonPool();

            var data = _enemiesData[EnemyType.SkeletonRanger];
            var (hpMul, dmgMul) = GetMultipliers();
            var enemy = _skeletonRangerPool.GetFreeElement();

            ConstructBase(enemy, data);
            ApplyHealth(enemy, data, hpMul);

            enemy.Longbow.SetProjectile(_arrowProjectilePool, data.SpeedProjectile);
            enemy.Longbow.SetData(_playerService.Player.transform, data.Damage * dmgMul);

            return enemy;
        }

        public Priest CreatePriest()
        {
            CreatePriestPool();

            var data = _enemiesData[EnemyType.Priest];
            var (hpMul, dmgMul) = GetMultipliers();
            var enemy = _priestPool.GetFreeElement();

            ConstructBase(enemy, data);
            ApplyHealth(enemy, data, hpMul);

            var target = _playerService.Player.transform;
            float damage = data.Damage * dmgMul;

            enemy.FireballSpell.GetServices(_audioSoundsService, _particleEffectsService);
            enemy.FireballSpell.SetProjectile(_magicBallProjectilePool, data.SpeedProjectile);
            enemy.FireballSpell.SetData(target, damage);

            enemy.Coil.SetData(target, damage);
            enemy.Coil.GetServices(_audioSoundsService, _particleEffectsService);

            enemy.Omni.SetData(target, damage);
            enemy.Omni.GetServices(_audioSoundsService, _particleEffectsService);

            return enemy;
        }

        public Bandit CreateBandit()
        {
            CreateEnemyBanditPool();

            var data = _enemiesData[EnemyType.BanditMelee];
            var (hpMul, dmgMul) = GetMultipliers();
            var enemy = _banditPool.GetFreeElement();

            ConstructBase(enemy, data);
            ApplyHealth(enemy, data, hpMul);
            enemy.MeleeWeapon.SetData(_playerService.Player.transform, data.Damage * dmgMul);

            return enemy;
        }

        public BanditRanger CreateBanditRanger()
        {
            CreateEnemyBanditRangerPool();

            var data = _enemiesData[EnemyType.BanditRanger];
            var (hpMul, dmgMul) = GetMultipliers();
            var enemy = _banditRangerPool.GetFreeElement();

            ConstructBase(enemy, data);
            ApplyHealth(enemy, data, hpMul);

            enemy.Longbow.SetProjectile(_arrowProjectilePool, data.SpeedProjectile);
            enemy.Longbow.SetData(_playerService.Player.transform, data.Damage * dmgMul);

            return enemy;
        }

        public BanditLeader CreateBanditLeader()
        {
            CreateEnemyBanditLeaderPool();

            var data = _enemiesData[EnemyType.BanditLeader];
            var (hpMul, dmgMul) = GetMultipliers();
            var enemy = _banditLeaderPool.GetFreeElement();

            ConstructBase(enemy, data);
            ApplyHealth(enemy, data, hpMul);
            enemy.MeleeWeapon.SetData(_playerService.Player.transform, data.Damage * dmgMul);

            return enemy;
        }

        public DarkLord CreateDarkLord()
        {
            CreateEnemyDarkLordPool();

            var data = _enemiesData[EnemyType.DarkLord];
            var (hpMul, dmgMul) = GetMultipliers();
            var enemy = _darkLordPool.GetFreeElement();

            ConstructBase(enemy, data);
            ApplyHealth(enemy, data, hpMul);

            var target = _playerService.Player.transform;
            float damage = data.Damage * dmgMul;

            enemy.FireballSpell.GetServices(_audioSoundsService, _particleEffectsService);
            enemy.FireballSpell.SetProjectile(_magicBallProjectilePool, data.SpeedProjectile);
            enemy.FireballSpell.SetData(target, damage);

            enemy.Coil.SetData(target, damage);
            enemy.Coil.GetServices(_audioSoundsService, _particleEffectsService);

            enemy.MeleeWeapon.SetData(target, damage);

            return enemy;
        }

        private (float hp, float dmg) GetMultipliers()
        {
            return (_progressionService.GetEnemyHealthMultiplier(),
                    _progressionService.GetEnemyDamageMultiplier());
        }
        
        private void ConstructBase(Enemy.Enemy enemy, EnemyData data)
        {
            enemy.Construct(
                _playerService.Player,
                data,
                _floatingTextService,
                _particleEffectsService,
                _audioSoundsService,
                _experiencePoints,
                _currencyService);
        }
        
        private void ApplyHealth(Enemy.Enemy enemy, EnemyData data, float hpMultiplier)
        {
            if (enemy.Health.TargetHealth > MinValue) return;

            float hp = data.Health * hpMultiplier;
            enemy.Health.LoadHealth(hp, hp);
        }

        public void GetData(EnemyInitData enemyInitData) => _enemyInitData = enemyInitData;

        public EnemyData GetEnemyDataByType(EnemyType type) => _enemiesData[type];

        private void CreateEnemySkeletonPool()
        {
            if (_skeletonPool != null) return;

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
            if (_skeletonHeavyArmorPool != null) return;

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
            if (_skeletonRangerPool != null) return;

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
            if (_priestPool != null) return;

            _priestPool = new ObjectPool<Priest>(
                _enemyInitData.PriestPrefab,
                DefaultCountObjectsInPool,
                new GameObject(PriestPool).transform)
            {
                AutoExpand = IsAutoExpand,
            };

            CreateMagicBallPool();
        }

        private void CreateEnemyBanditPool()
        {
            if (_banditPool != null) return;

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
            if (_banditRangerPool != null) return;

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
            if (_banditLeaderPool != null) return;

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
            if (_darkLordPool != null) return;

            _darkLordPool = new ObjectPool<DarkLord>(
                _enemyInitData.DarkLordPrefab,
                DefaultCountObjectsInPool,
                new GameObject(DarkLordPool).transform)
            {
                AutoExpand = IsAutoExpand,
            };

            CreateMagicBallPool();
        }

        private void CreateArrowPool()
        {
            if (_arrowProjectilePool != null) return;

            _arrowProjectilePool = new ObjectPool<Arrow>(
                _enemyInitData.ArrowProjectilePrefab,
                DefaultCountObjectsInPool,
                new GameObject(ArrowProjectilePool).transform)
            {
                AutoExpand = IsAutoExpand,
            };
        }

        private void CreateMagicBallPool()
        {
            if (_magicBallProjectilePool != null) return;

            _magicBallProjectilePool = new ObjectPool<Fireball>(
                _enemyInitData.FireballProjectilePrefab,
                DefaultCountObjectsInPool,
                new GameObject(MagicBallProjectilePool).transform)
            {
                AutoExpand = IsAutoExpand,
            };
        }
    }
}