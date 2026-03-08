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
        protected const int FourthWaveEnemy = 3;
        protected const int FifthWaveNumber = 4;
        
        [Header("EnemyWaves")]
        [SerializeField] protected float SpawnWaveOfEnemyDelay = 10f;
        
        [SerializeField] private List<EnemyWave> _enemyWaves;
        [SerializeField] private int _limitEnemies;

        private IPlayerService _playerService;
        private ParticleEffectsService _particleEffectsService;

        private EnemySpawner _enemySpawner;
        private LevelInitData _levelInitData;
        private PlayerInitData _playerInitData;
        private CinemachineFreeLook _cinemachineFreeLook;
        private CinemachineVirtualCamera _testCamera;
        
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

            _testCamera.LookAt = playerTransform;
            _testCamera.Follow = playerTransform;
            
            PlayerIsSpawned?.Invoke();
            
            _playerService.Player.PlayerCollisionHandler.GetEnemyWaves(_enemyWaves);
        }
        
        protected void CreateWaveOfEnemyByTimer(int numberWaveEnemy)
        {
            if (LastSpawnTime <= MinValue)
            {
                CreateWaveOfEnemies(numberWaveEnemy);
                
                foreach (var enemy in _enemyWaves[numberWaveEnemy].Enemies)
                {
                    enemy.ChangeFollowEnemyState(true);
                }

                LastSpawnTime = SpawnWaveOfEnemyDelay;
            }
        
            LastSpawnTime -= Time.fixedDeltaTime;
        }

        protected void CreateWaveOfEnemies(int numberWave)
        {
            if(_enemyWaves.Count == 0)
                return;
            
            _enemySpawner.SpawnWave(_enemyWaves[numberWave]);
        }

        private void InitSpawners(IEnemyService enemyService)
        {
            InitEnemyWaves();
            
            _enemySpawner = new EnemySpawner(enemyService, _limitEnemies);

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
                    case FourthWaveEnemy:
                        _enemyWaves[i].GetEnemyPositions(
                            _levelInitData.FourthWaveSpawnPoints,
                            _levelInitData.EnemyFourthPatrolPositions);
                        break;
                    case FifthWaveNumber:
                        _enemyWaves[i].GetEnemyPositions(
                            _levelInitData.FifthWaveSpawnPoints,
                            _levelInitData.EnemyFifthPatrolPositions);
                        break;
                    default:
                        throw new Exception("There is not enough data for new waves");
                }
            }
        }
    }
}