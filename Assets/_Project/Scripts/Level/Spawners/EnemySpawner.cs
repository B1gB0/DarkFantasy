using System.Collections.Generic;
using _Project.Scripts.Enemy;
using _Project.Scripts.Services;
using UnityEngine;

namespace _Project.Scripts.Level.Spawners
{
    public class EnemySpawner
    {
        private const int MinValue = 0;
        private const float RandomPositionFactor = 2f;
        private const int CorrectCountFactor = 1;

        private readonly IEnemyService _enemyService;

        private int _counterSmallEnemies;
        private int _counterBigEnemies;
        private int _counterGunnerEnemies;

        public EnemySpawner(IEnemyService enemyService)
        {
            _enemyService = enemyService;
        }

        public void SpawnSkeletonEnemy(List<Vector3> spawnPointPositions, int countEnemies)
        {
            if (spawnPointPositions.Count == MinValue)
                return;

            foreach (var enemyPosition in spawnPointPositions)
            {
                if (_counterGunnerEnemies > countEnemies - CorrectCountFactor)
                    return;

                Skeleton skeleton = _enemyService.CreateSkeleton();

                skeleton.NavMeshAgent.enabled = false;

                var enemySpawnPosition = enemyPosition +
                                         (Vector3.one * Random.Range(-RandomPositionFactor, RandomPositionFactor));

                enemySpawnPosition.y = enemyPosition.y;

                skeleton.transform.position = enemySpawnPosition;

                skeleton.NavMeshAgent.enabled = true;

                // gunnerEnemy.Die += OnKillGunnerEnemy;
                _counterGunnerEnemies++;
            }
        }

        // public void SpawnSmallAlienEnemy(List<Vector3> spawnPointPositions, int countEnemies)
        // {
        //     if (spawnPointPositions.Count == MinValue)
        //         return;
        //
        //     foreach (var enemyPosition in spawnPointPositions)
        //     {
        //         if (_counterSmallEnemies > countEnemies - CorrectCountFactor)
        //             return;
        //
        //         SmallEnemy smallEnemy = _gameInitSystem.CreateSmallAlienEnemy(_gameInitSystem.Player);
        //
        //         smallEnemy.NavMeshAgent.enabled = false;
        //
        //         var enemySpawnPosition = enemyPosition +
        //                                  (Vector3.one * Random.Range(-RandomPositionFactor, RandomPositionFactor));
        //
        //         enemySpawnPosition.y = enemyPosition.y;
        //
        //         smallEnemy.transform.position = enemySpawnPosition;
        //
        //         smallEnemy.NavMeshAgent.enabled = true;
        //
        //         smallEnemy.Die += OnKillSmallEnemy;
        //         _counterSmallEnemies++;
        //     }
        // }
        //
        // public void SpawnBigEnemyAlien(List<Vector3> spawnPointPositions, int countEnemies)
        // {
        //     if (spawnPointPositions.Count == MinValue)
        //         return;
        //
        //     foreach (var enemyPosition in spawnPointPositions)
        //     {
        //         if (_counterBigEnemies > countEnemies - CorrectCountFactor)
        //             return;
        //
        //         BigEnemy bigEnemy = _gameInitSystem.CreateBigAlienEnemy(_gameInitSystem.Player);
        //
        //         bigEnemy.NavMeshAgent.enabled = false;
        //
        //         var enemySpawnPosition = enemyPosition +
        //                                  (Vector3.one * Random.Range(-RandomPositionFactor, RandomPositionFactor));
        //
        //         enemySpawnPosition.y = enemyPosition.y;
        //
        //         bigEnemy.transform.position = enemySpawnPosition;
        //
        //         bigEnemy.NavMeshAgent.enabled = true;
        //
        //         bigEnemy.Die += OnKillBigEnemy;
        //         _counterBigEnemies++;
        //     }
        // }
        //
        // public void SpawnAlienEnemyTurret(List<Vector3> spawnPointPositions, Vector3 playerSpawnPoint)
        // {
        //     if (spawnPointPositions.Count == MinValue)
        //         return;
        //
        //     foreach (var enemyPosition in spawnPointPositions)
        //     {
        //         EnemyTurret enemyTurret = _gameInitSystem.CreateEnemyTurret(_gameInitSystem.Player, enemyPosition);
        //         enemyTurret.transform.LookAt(playerSpawnPoint);
        //
        //         var enemySpawnPosition = enemyPosition +
        //                                  (Vector3.one * Random.Range(-RandomPositionFactor, RandomPositionFactor));
        //         
        //         enemySpawnPosition.y = enemyPosition.y;
        //
        //         enemyTurret.transform.position = enemySpawnPosition;
        //     }
        // }

        private void OnKillSmallEnemy(Enemy.Enemy enemyActor)
        {
            _counterSmallEnemies--;
            // enemyActor.Die -= OnKillSmallEnemy;
        }

        private void OnKillBigEnemy(Enemy.Enemy enemyActor)
        {
            _counterBigEnemies--;
            // enemyActor.Die -= OnKillBigEnemy;
        }

        private void OnKillGunnerEnemy(Enemy.Enemy enemyActor)
        {
            _counterGunnerEnemies--;
            // enemyActor.Die -= OnKillGunnerEnemy;
        }
    }
}