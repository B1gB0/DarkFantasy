using System;
using _Project.Scripts.DataBase.InitDataSO;
using _Project.Scripts.Level.Spawners;
using _Project.Scripts.Services;
using UnityEngine;

namespace _Project.Scripts.Level
{
    public abstract class Level : MonoBehaviour
    {
        [SerializeField] protected int CountSkeletonEnemy;
        
        private EnemySpawner _enemySpawner;
        private LevelInitData _levelInitData;
        
        public event Action IsInitiatedSpawners;

        public void GetServices(IEnemyService enemyService, LevelInitData levelInitData)
        {
            _levelInitData = levelInitData;
            InitSpawners(enemyService);
            CreateSkeletonWaveEnemy();
        }

        protected void CreateSkeletonWaveEnemy()
        {
            _enemySpawner.SpawnSkeletonEnemy(_levelInitData.SkeletonEnemySpawnPositions, CountSkeletonEnemy);
        }
        
        private void InitSpawners(IEnemyService enemyService)
        {
            _enemySpawner = new EnemySpawner(enemyService);

            IsInitiatedSpawners?.Invoke();
        }
    }
}