using System.Collections.Generic;
using _Project.Scripts.Enemy;
using _Project.Scripts.Enemy.StateMachine.Behaviour.States;
using _Project.Scripts.Services;
using UnityEngine;

namespace _Project.Scripts.Level.Spawners
{
    public class EnemySpawner
    {
        private const int MinValue = 0;
        private const int CorrectCountFactor = 1;
        private const float RandomPositionFactor = 2f;
        private const float OffsetYPolygonEnemies = 0.5f;

        private readonly IEnemyService _enemyService;

        private int _counterSkeletonEnemies;
        private int _counterSkeletonHeavyArmorEnemies;
        private int _counterSkeletonRangerEnemies;

        public EnemySpawner(IEnemyService enemyService)
        {
            _enemyService = enemyService;
        }

        public void SpawnWave(EnemyWave wave)
        {
            List<Vector3> spawnPoints = wave.WaveSpawnPoints;
            List<Vector3> patrolPoints = wave.PatrolPoints;

            if (spawnPoints == null || spawnPoints.Count == 0)
                return;

            List<Vector3> availableSpawnPoints = new List<Vector3>(spawnPoints);

            int skeletonsToSpawn = wave.SkeletonEnemyCount;
            int heavyToSpawn = wave.SkeletonHeavyArmorCount;
            int rangersToSpawn = wave.SkeletonRangerCount;

            for (int i = 0; i < skeletonsToSpawn; i++)
            {
                if (availableSpawnPoints.Count == 0)
                    break;

                Vector3 candidatePoint = availableSpawnPoints[0];
                SpawnSkeletonEnemy(candidatePoint, patrolPoints);
                availableSpawnPoints.RemoveAt(0);
            }
            
            for (int i = 0; i < heavyToSpawn; i++)
            {
                if (availableSpawnPoints.Count == 0)
                    break;

                Vector3 candidatePoint = availableSpawnPoints[0];
                SpawnSkeletonHeavyArmorEnemy(candidatePoint, patrolPoints);
                availableSpawnPoints.RemoveAt(0);
            }
            
            for (int i = 0; i < rangersToSpawn; i++)
            {
                if (availableSpawnPoints.Count == 0)
                    break;

                Vector3 candidatePoint = availableSpawnPoints[0];
                SpawnSkeletonRangerEnemy(candidatePoint, patrolPoints);
                availableSpawnPoints.RemoveAt(0);
            }
        }

        public void SpawnSkeletonEnemy(Vector3 enemyPosition, List<Vector3> patrolPoints)
        {
            Skeleton skeleton = _enemyService.CreateSkeleton();

            skeleton.NavMeshAgent.enabled = false;

            var enemySpawnPosition = enemyPosition +
                                     (Vector3.one * Random.Range(-RandomPositionFactor, RandomPositionFactor));

            enemySpawnPosition.y = enemyPosition.y + OffsetYPolygonEnemies;

            skeleton.transform.position = enemySpawnPosition;

            skeleton.NavMeshAgent.enabled = true;

            // gunnerEnemy.Die += OnKillGunnerEnemy;
            _counterSkeletonEnemies++;

            skeleton.EnemyStateMachine.AddState(new PatrolState(patrolPoints));
            skeleton.EnemyStateMachine.InitializeAllStates();
            skeleton.EnemyStateMachine.SwitchState<PatrolState>();
        }

        public void SpawnSkeletonHeavyArmorEnemy(Vector3 enemyPosition, List<Vector3> patrolPoints)
        {
            SkeletonHeavyArmor skeleton = _enemyService.CreateSkeletonHeavyArmor();

            skeleton.NavMeshAgent.enabled = false;

            var enemySpawnPosition = enemyPosition +
                                     (Vector3.one * Random.Range(-RandomPositionFactor, RandomPositionFactor));

            enemySpawnPosition.y = enemyPosition.y + OffsetYPolygonEnemies;

            skeleton.transform.position = enemySpawnPosition;

            skeleton.NavMeshAgent.enabled = true;

            // gunnerEnemy.Die += OnKillGunnerEnemy;
            _counterSkeletonHeavyArmorEnemies++;

            skeleton.EnemyStateMachine.AddState(new PatrolState(patrolPoints));
            skeleton.EnemyStateMachine.InitializeAllStates();
            skeleton.EnemyStateMachine.SwitchState<PatrolState>();
        }

        public void SpawnSkeletonRangerEnemy(Vector3 enemyPosition, List<Vector3> patrolPoints)
        {
            SkeletonRanger skeleton = _enemyService.CreateSkeletonRanger();

            skeleton.NavMeshAgent.enabled = false;

            var enemySpawnPosition = enemyPosition +
                                     (Vector3.one * Random.Range(-RandomPositionFactor, RandomPositionFactor));

            enemySpawnPosition.y = enemyPosition.y + OffsetYPolygonEnemies;

            skeleton.transform.position = enemySpawnPosition;

            skeleton.NavMeshAgent.enabled = true;

            // gunnerEnemy.Die += OnKillGunnerEnemy;
            _counterSkeletonHeavyArmorEnemies++;

            skeleton.EnemyStateMachine.AddState(new PatrolState(patrolPoints));
            skeleton.EnemyStateMachine.InitializeAllStates();
            skeleton.EnemyStateMachine.SwitchState<PatrolState>();
        }


        private void OnKillSmallEnemy(Enemy.Enemy enemyActor)
        {
            // _counterSmallEnemies--;
            // enemyActor.Die -= OnKillSmallEnemy;
        }

        private void OnKillBigEnemy(Enemy.Enemy enemyActor)
        {
            _counterSkeletonHeavyArmorEnemies--;
            // enemyActor.Die -= OnKillBigEnemy;
        }

        private void OnKillGunnerEnemy(Enemy.Enemy enemyActor)
        {
            _counterSkeletonEnemies--;
            // enemyActor.Die -= OnKillGunnerEnemy;
        }
    }
}