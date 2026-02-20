using System;
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
        [SerializeField] protected int CountSkeletonEnemy;
        [SerializeField] protected int CountSkeletonFleshEnemy;
        [SerializeField] protected int CountSkeletonHeavyArmorEnemy;
        [SerializeField] protected int CountSkeletonLightArmorEnemy;
        [SerializeField] protected int CountSkeletonRangerEnemy;

        private IPlayerService _playerService;

        private EnemySpawner _enemySpawner;
        private LevelInitData _levelInitData;
        private PlayerInitData _playerInitData;
        private CinemachineFreeLook _cinemachineFreeLook;

        public event Action IsInitiatedSpawners;
        public event Action PlayerIsSpawned;

        public void GetServices(
            IEnemyService enemyService,
            LevelInitData levelInitData,
            PlayerInitData playerInitData,
            IPlayerService playerService,
            CinemachineFreeLook cinemachineFreeLook
        )
        {
            _levelInitData = levelInitData;
            _playerInitData = playerInitData;
            _playerService = playerService;
            _cinemachineFreeLook = cinemachineFreeLook;
            
            CreatePlayer();
            
            InitSpawners(enemyService);
            CreateSkeletonWaveEnemy();
        }

        protected void CreatePlayer()
        {
            var data = _playerService.GetPlayerDataByType(PlayerType.CommonHero);
            
            Player.Player player = _playerService.CreatePlayerByPrefab(
                _playerInitData.CommonHero,
                _levelInitData.PlayerSpawnPosition);
            
            player.Health.SetHealthValue(data.Health);

            var playerTransform = player.transform;
            
            _cinemachineFreeLook.LookAt = playerTransform;
            _cinemachineFreeLook.Follow = playerTransform;
            
            PlayerIsSpawned?.Invoke();
        }

        protected void CreateSkeletonWaveEnemy()
        {
            _enemySpawner.SpawnSkeletonEnemy(_levelInitData.SkeletonEnemySpawnPositions, CountSkeletonEnemy);

            _enemySpawner.SpawnSkeletonHeavyArmorEnemy(
                _levelInitData.SkeletonHeavyArmorEnemySpawnPositions,
                CountSkeletonHeavyArmorEnemy);

            _enemySpawner.SpawnSkeletonRangerEnemy(
                _levelInitData.SkeletonRangerEnemySpawnPositions,
                CountSkeletonRangerEnemy);
        }

        private void InitSpawners(IEnemyService enemyService)
        {
            _enemySpawner = new EnemySpawner(enemyService);

            IsInitiatedSpawners?.Invoke();
        }
    }
}