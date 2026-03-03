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

        public void SpawnSkeletonEnemy(List<Vector3> spawnPointPositions, int countEnemies, List<Vector3> patrolPoints)
        {
            if (spawnPointPositions.Count == MinValue)
                return;

            foreach (var enemyPosition in spawnPointPositions)
            {
                if (_counterSkeletonEnemies > countEnemies - CorrectCountFactor)
                    return;

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
                skeleton.EnemyStateMachine.SwitchState<PatrolState>();
            }
        }
        
        public void SpawnSkeletonHeavyArmorEnemy(
            List<Vector3> spawnPointPositions,
            int countEnemies,
            List<Vector3> patrolPoints)
        {
            if (spawnPointPositions.Count == MinValue)
                return;

            foreach (var enemyPosition in spawnPointPositions)
            {
                if (_counterSkeletonHeavyArmorEnemies > countEnemies - CorrectCountFactor)
                    return;

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
                skeleton.EnemyStateMachine.SwitchState<PatrolState>();
            }
        }
        
        public void SpawnSkeletonRangerEnemy(
            List<Vector3> spawnPointPositions,
            int countEnemies,
            List<Vector3> patrolPoints)
        {
            if (spawnPointPositions.Count == MinValue)
                return;

            foreach (var enemyPosition in spawnPointPositions)
            {
                if (_counterSkeletonRangerEnemies > countEnemies - CorrectCountFactor)
                    return;

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
                skeleton.EnemyStateMachine.SwitchState<PatrolState>();
            }
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