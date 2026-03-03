using System;
using System.Collections.Generic;
using _Project.Scripts.DataBase.InitDataSO;
using _Project.Scripts.Level.Spawners;
using _Project.Scripts.Player;
using _Project.Scripts.Services;
using Cinemachine;
using UnityEngine;

namespace _Project.Scripts.Level
{
    public abstract class Level : MonoBehaviour
    {
        protected const float MinValue = 0f;
        protected const int FirstWaveEnemy = 0;
        protected const int SecondWaveEnemy = 1;
        protected const int ThirdWaveEnemy = 2;
        
        private readonly List<EnemyWave> _enemyWaves = new();
        
        [Header("EnemyWaves")]
        [SerializeField] protected float SpawnWaveOfEnemyDelay = 10f;
        [SerializeField] private int _countEnemyWaves;

        private IPlayerService _playerService;
        private ParticleEffectsService _particleEffectsService;

        private EnemySpawner _enemySpawner;
        private LevelInitData _levelInitData;
        private PlayerInitData _playerInitData;
        private CinemachineFreeLook _cinemachineFreeLook;
        
        protected float LastSpawnTime;

        public event Action IsInitiatedSpawners;
        public event Action PlayerIsSpawned;
        
        [field: SerializeField] public int CountSkeletonEnemy { get; private set; }
        [field: SerializeField] public int CountSkeletonFleshEnemy { get; private set; }
        [field: SerializeField] public int CountSkeletonHeavyArmorEnemy { get; private set; }
        [field: SerializeField] public int CountSkeletonLightArmorEnemy { get; private set; }
        [field: SerializeField] public int CountSkeletonRangerEnemy { get; private set; }

        public void GetServices(
            IEnemyService enemyService,
            LevelInitData levelInitData,
            PlayerInitData playerInitData,
            IPlayerService playerService,
            CinemachineFreeLook cinemachineFreeLook,
            ParticleEffectsService particleEffectsService
        )
        {
            _levelInitData = levelInitData;
            _playerInitData = playerInitData;
            _playerService = playerService;
            _cinemachineFreeLook = cinemachineFreeLook;
            _particleEffectsService = particleEffectsService;
            
            CreatePlayer();
            
            InitSpawners(enemyService);
        }

        protected void CreatePlayer()
        {
            var data = _playerService.GetPlayerDataByType(PlayerType.CommonHero);
            
            Player.Player player = _playerService.CreatePlayerByPrefab(
                _playerInitData.CommonHero,
                _levelInitData.PlayerSpawnPosition);
            
            var playerCharacteristics = _playerService.InitPlayerCharacteristics();
            
            player.Construct(playerCharacteristics, _particleEffectsService);
            player.Health.SetHealthValue(data.Health);

            var playerTransform = player.transform;
            
            _cinemachineFreeLook.LookAt = playerTransform;
            _cinemachineFreeLook.Follow = playerTransform;
            
            PlayerIsSpawned?.Invoke();
        }
        
        protected virtual void CreateWaveOfEnemy(int numberWaveEnemy)
        {
            if (LastSpawnTime <= MinValue)
            {
                CreateWaveOfSkeleton(numberWaveEnemy);
                CreateWaveOfSkeletonHeavyArmor(numberWaveEnemy);
                CreateWaveOfSkeletonRanger(numberWaveEnemy);

                LastSpawnTime = SpawnWaveOfEnemyDelay;
            }

            LastSpawnTime -= Time.fixedDeltaTime;
        }

        protected void CreateWaveOfSkeleton(int numberWaveEnemy)
        {
            _enemySpawner.SpawnSkeletonEnemy(
                _enemyWaves[numberWaveEnemy].WaveSpawnPoints,
                CountSkeletonEnemy,
                _enemyWaves[numberWaveEnemy].PatrolPoints);
        }

        protected void CreateWaveOfSkeletonHeavyArmor(int numberWaveEnemy)
        {
            _enemySpawner.SpawnSkeletonHeavyArmorEnemy(
                _enemyWaves[numberWaveEnemy].WaveSpawnPoints,
                CountSkeletonHeavyArmorEnemy,
                _enemyWaves[numberWaveEnemy].PatrolPoints);
        }

        protected void CreateWaveOfSkeletonRanger(int numberWaveEnemy)
        {
            _enemySpawner.SpawnSkeletonRangerEnemy(
                _enemyWaves[numberWaveEnemy].WaveSpawnPoints,
                CountSkeletonRangerEnemy,
                _enemyWaves[numberWaveEnemy].PatrolPoints);
        }

        private void InitSpawners(IEnemyService enemyService)
        {
            InitEnemyWaves();
            
            _enemySpawner = new EnemySpawner(enemyService);

            IsInitiatedSpawners?.Invoke();
        }
        
        private void InitEnemyWaves()
        {
            for (int i = 0; i < _countEnemyWaves; i++)
            {
                EnemyWave enemyWave = new EnemyWave();

                switch (i)
                {
                    case FirstWaveEnemy:
                        enemyWave.GetEnemyPositions(
                            _levelInitData.FirstWaveSpawnPoints,
                            _levelInitData.EnemyFirstPatrolPositions);
                        break;
                    case SecondWaveEnemy:
                        enemyWave.GetEnemyPositions(
                            _levelInitData.SecondWaveSpawnPoints,
                            _levelInitData.EnemySecondPatrolPositions);
                        break;
                    case ThirdWaveEnemy:
                        enemyWave.GetEnemyPositions(
                            _levelInitData.ThirdWaveSpawnPoints,
                            _levelInitData.EnemyThirdPatrolPositions);
                        break;
                    default:
                        throw new Exception("There is not enough data for new waves");
                }

                _enemyWaves.Add(enemyWave);
            }
        }
    }
}