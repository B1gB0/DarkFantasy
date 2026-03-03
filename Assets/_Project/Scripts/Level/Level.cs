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
        
        [Header("EnemyWaves")]
        [SerializeField] private List<EnemyWave> _enemyWaves;
        [SerializeField] protected float SpawnWaveOfEnemyDelay = 10f;

        private IPlayerService _playerService;
        private ParticleEffectsService _particleEffectsService;

        private EnemySpawner _enemySpawner;
        private LevelInitData _levelInitData;
        private PlayerInitData _playerInitData;
        private CinemachineFreeLook _cinemachineFreeLook;
        
        protected float LastSpawnTime;

        public event Action IsInitiatedSpawners;
        public event Action PlayerIsSpawned;

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
            
            CreateWaveOfDifferentSkeletons(FirstWaveEnemy);
            CreateWaveOfDifferentSkeletons(SecondWaveEnemy);
            CreateWaveOfDifferentSkeletons(ThirdWaveEnemy);
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
        
        // protected virtual void CreateWaveOfEnemy(int numberWaveEnemy)
        // {
        //     if (LastSpawnTime <= MinValue)
        //     {
        //         CreateWaveOfSkeleton(numberWaveEnemy);
        //         CreateWaveOfSkeletonHeavyArmor(numberWaveEnemy);
        //         CreateWaveOfSkeletonRanger(numberWaveEnemy);
        //
        //         LastSpawnTime = SpawnWaveOfEnemyDelay;
        //     }
        //
        //     LastSpawnTime -= Time.fixedDeltaTime;
        // }

        protected void CreateWaveOfDifferentSkeletons(int numberWave)
        {
            _enemySpawner.SpawnWave(_enemyWaves[numberWave]);
        }

        // protected void CreateWaveOfSkeleton(int numberWaveEnemy)
        // {
        //     _enemySpawner.SpawnSkeletonEnemy(
        //         _enemyWaves[numberWaveEnemy].WaveSpawnPoints,
        //         _enemyWaves[numberWaveEnemy].SkeletonEnemyCount,
        //         _enemyWaves[numberWaveEnemy].PatrolPoints);
        // }
        //
        // protected void CreateWaveOfSkeletonHeavyArmor(int numberWaveEnemy)
        // {
        //     _enemySpawner.SpawnSkeletonHeavyArmorEnemy(
        //         _enemyWaves[numberWaveEnemy].WaveSpawnPoints,
        //         _enemyWaves[numberWaveEnemy].SkeletonHeavyArmorCount,
        //         _enemyWaves[numberWaveEnemy].PatrolPoints);
        // }
        //
        // protected void CreateWaveOfSkeletonRanger(int numberWaveEnemy)
        // {
        //     _enemySpawner.SpawnSkeletonRangerEnemy(
        //         _enemyWaves[numberWaveEnemy].WaveSpawnPoints,
        //         _enemyWaves[numberWaveEnemy].SkeletonRangerCount,
        //         _enemyWaves[numberWaveEnemy].PatrolPoints);
        // }

        private void InitSpawners(IEnemyService enemyService)
        {
            InitEnemyWaves();
            
            _enemySpawner = new EnemySpawner(enemyService);

            IsInitiatedSpawners?.Invoke();
        }
        
        private void InitEnemyWaves()
        {
            for (int i = 0; i < _enemyWaves.Count; i++)
            {
                switch (i)
                {
                    case FirstWaveEnemy:
                        _enemyWaves[i].GetEnemyPositions(
                            _levelInitData.FirstWaveSpawnPoints,
                            _levelInitData.EnemyFirstPatrolPositions);
                        break;
                    case SecondWaveEnemy:
                        _enemyWaves[i].GetEnemyPositions(
                            _levelInitData.SecondWaveSpawnPoints,
                            _levelInitData.EnemySecondPatrolPositions);
                        break;
                    case ThirdWaveEnemy:
                        _enemyWaves[i].GetEnemyPositions(
                            _levelInitData.ThirdWaveSpawnPoints,
                            _levelInitData.EnemyThirdPatrolPositions);
                        break;
                    default:
                        throw new Exception("There is not enough data for new waves");
                }
            }
        }
    }
}