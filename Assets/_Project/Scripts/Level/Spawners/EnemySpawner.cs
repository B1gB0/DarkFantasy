using System;
using System.Collections.Generic;
using _Project.Scripts.Enemy;
using _Project.Scripts.Enemy.StateMachine.Behaviour.States;
using _Project.Scripts.Services;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Level.Spawners
{
    public class EnemySpawner
    {
        private const int MinValue = 0;
        private const int CorrectCountFactor = 1;
        private const float RandomPositionFactor = 1f;
        private const float OffsetYPolygonEnemies = 0.5f;
        private const float HatChance = 0.5f;

        private readonly IEnemyService _enemyService;
        private readonly AudioSoundsService _audioSoundsService;
        private readonly ParticleEffectsService _particleEffectsService;

        private int _enemyCounter;
        private int _limitEnemies;

        public event Action OnPriestKilled;
        public event Action OnBanditLeaderKilled;
        public event Action OnDarkLordKilled;
        public event Action OnAllEnemiesKilled;

        public EnemySpawner(
            IEnemyService enemyService,
            int limitEnemies,
            AudioSoundsService audioSoundsService,
            ParticleEffectsService particleEffectsService)
        {
            _enemyService = enemyService;
            _limitEnemies = limitEnemies;
            _audioSoundsService = audioSoundsService;
            _particleEffectsService = particleEffectsService;
        }

        public void SpawnWave(EnemyWave wave)
        {
            if (_enemyCounter > _limitEnemies - CorrectCountFactor)
                return;

            List<Vector3> spawnPoints = wave.WaveSpawnPoints;
            List<Vector3> patrolPoints = wave.PatrolPoints;

            if (spawnPoints == null || spawnPoints.Count == MinValue)
                return;

            List<Vector3> availableSpawnPoints = new List<Vector3>(spawnPoints);

            int skeletonsToSpawn = wave.SkeletonEnemyCount;
            int heavyToSpawn = wave.SkeletonHeavyArmorCount;
            int rangersToSpawn = wave.SkeletonRangerCount;
            int priestToSpawn = wave.PriestCount;
            int banditsToSpawn = wave.BanditCount;
            int banditsRangersToSpawn = wave.BanditRangerCount;

            for (int i = 0; i < skeletonsToSpawn; i++)
            {
                if (availableSpawnPoints.Count == MinValue)
                    break;

                Vector3 candidatePoint = availableSpawnPoints[MinValue];
                Skeleton enemy = SpawnSkeletonEnemy(candidatePoint, patrolPoints);
                availableSpawnPoints.RemoveAt(MinValue);
                wave.AddEnemy(enemy);
                _enemyCounter++;
            }

            for (int i = 0; i < heavyToSpawn; i++)
            {
                if (availableSpawnPoints.Count == MinValue)
                    break;

                Vector3 candidatePoint = availableSpawnPoints[MinValue];
                SkeletonHeavyArmor enemy = SpawnSkeletonHeavyArmorEnemy(candidatePoint, patrolPoints);
                availableSpawnPoints.RemoveAt(MinValue);
                wave.AddEnemy(enemy);
                _enemyCounter++;
            }

            for (int i = 0; i < rangersToSpawn; i++)
            {
                if (availableSpawnPoints.Count == MinValue)
                    break;

                Vector3 candidatePoint = availableSpawnPoints[MinValue];
                SkeletonRanger enemy = SpawnSkeletonRangerEnemy(candidatePoint, patrolPoints);
                availableSpawnPoints.RemoveAt(MinValue);
                wave.AddEnemy(enemy);
                _enemyCounter++;
            }

            for (int i = 0; i < priestToSpawn; i++)
            {
                if (availableSpawnPoints.Count == MinValue)
                    break;

                Vector3 candidatePoint = availableSpawnPoints[MinValue];
                Priest enemy = SpawnPriest(candidatePoint, patrolPoints);
                availableSpawnPoints.RemoveAt(MinValue);
                wave.AddEnemy(enemy);
                _enemyCounter++;
            }
            
            for (int i = 0; i < banditsToSpawn; i++)
            {
                if (availableSpawnPoints.Count == MinValue)
                    break;

                Vector3 candidatePoint = availableSpawnPoints[MinValue];
                Bandit enemy = SpawnBanditEnemy(candidatePoint, patrolPoints);
                enemy.Hat.gameObject.SetActive(Random.value < HatChance);
                availableSpawnPoints.RemoveAt(MinValue);
                wave.AddEnemy(enemy);
                _enemyCounter++;
            }
            
            for (int i = 0; i < banditsRangersToSpawn; i++)
            {
                if (availableSpawnPoints.Count == MinValue)
                    break;

                Vector3 candidatePoint = availableSpawnPoints[MinValue];
                BanditRanger enemy = SpawnBanditRangerEnemy(candidatePoint, patrolPoints);
                enemy.Hat.gameObject.SetActive(Random.value < HatChance);
                availableSpawnPoints.RemoveAt(MinValue);
                wave.AddEnemy(enemy);
                _enemyCounter++;
            }
        }

        private Skeleton SpawnSkeletonEnemy(Vector3 enemyPosition, List<Vector3> patrolPoints)
        {
            Skeleton skeleton = _enemyService.CreateSkeleton();

            skeleton.NavMeshAgent.enabled = false;

            var enemySpawnPosition = enemyPosition +
                                     (Vector3.one * Random.Range(-RandomPositionFactor, RandomPositionFactor));

            enemySpawnPosition.y = enemyPosition.y + OffsetYPolygonEnemies;

            skeleton.transform.position = enemySpawnPosition;
            
            skeleton.Rigidbody.isKinematic = true;
            skeleton.Collider.enabled = true;

            skeleton.NavMeshAgent.enabled = true;
            skeleton.NavMeshAgent.isStopped = false;
            skeleton.NavMeshAgent.ResetPath();

            skeleton.Die += OnKillSkeleton;

            skeleton.EnemyStateMachine.AddState(new PatrolState(patrolPoints));
            skeleton.EnemyStateMachine.GetServices(_audioSoundsService, _particleEffectsService);
            skeleton.EnemyStateMachine.InitializeAllStates();
            skeleton.EnemyStateMachine.SwitchState<PatrolState>();

            return skeleton;
        }

        private SkeletonHeavyArmor SpawnSkeletonHeavyArmorEnemy(Vector3 enemyPosition, List<Vector3> patrolPoints)
        {
            SkeletonHeavyArmor skeleton = _enemyService.CreateSkeletonHeavyArmor();

            skeleton.NavMeshAgent.enabled = false;

            var enemySpawnPosition = enemyPosition +
                                     (Vector3.one * Random.Range(-RandomPositionFactor, RandomPositionFactor));

            enemySpawnPosition.y = enemyPosition.y + OffsetYPolygonEnemies;

            skeleton.transform.position = enemySpawnPosition;
            
            skeleton.Rigidbody.isKinematic = true;
            skeleton.Collider.enabled = true;

            skeleton.NavMeshAgent.enabled = true;
            skeleton.NavMeshAgent.isStopped = false;
            skeleton.NavMeshAgent.ResetPath();

            skeleton.Die += OnKillSkeletonHeavyArmor;

            skeleton.EnemyStateMachine.AddState(new PatrolState(patrolPoints));
            skeleton.EnemyStateMachine.GetServices(_audioSoundsService, _particleEffectsService);
            skeleton.EnemyStateMachine.InitializeAllStates();
            skeleton.EnemyStateMachine.SwitchState<PatrolState>();

            return skeleton;
        }

        private SkeletonRanger SpawnSkeletonRangerEnemy(Vector3 enemyPosition, List<Vector3> patrolPoints)
        {
            SkeletonRanger skeleton = _enemyService.CreateSkeletonRanger();

            skeleton.NavMeshAgent.enabled = false;

            var enemySpawnPosition = enemyPosition +
                                     (Vector3.one * Random.Range(-RandomPositionFactor, RandomPositionFactor));

            enemySpawnPosition.y = enemyPosition.y + OffsetYPolygonEnemies;

            skeleton.transform.position = enemySpawnPosition;
            
            skeleton.Rigidbody.isKinematic = true;
            skeleton.Collider.enabled = true;

            skeleton.NavMeshAgent.enabled = true;
            skeleton.NavMeshAgent.isStopped = false;
            skeleton.NavMeshAgent.ResetPath();

            skeleton.Die += OnKillSkeletonRanger;

            skeleton.EnemyStateMachine.AddState(new PatrolState(patrolPoints));
            skeleton.EnemyStateMachine.GetServices(_audioSoundsService, _particleEffectsService);
            skeleton.EnemyStateMachine.InitializeAllStates();
            skeleton.EnemyStateMachine.SwitchState<PatrolState>();

            return skeleton;
        }

        private Priest SpawnPriest(Vector3 enemyPosition, List<Vector3> patrolPoints)
        {
            Priest priest = _enemyService.CreatePriest();

            priest.NavMeshAgent.enabled = false;

            var enemySpawnPosition = enemyPosition +
                                     (Vector3.one * Random.Range(-RandomPositionFactor, RandomPositionFactor));

            enemySpawnPosition.y = enemyPosition.y + OffsetYPolygonEnemies;

            priest.transform.position = enemySpawnPosition;
            
            priest.Rigidbody.isKinematic = true;
            priest.Collider.enabled = true;

            priest.NavMeshAgent.enabled = true;
            priest.NavMeshAgent.isStopped = false;
            priest.NavMeshAgent.ResetPath();

            priest.Die += OnKillPriest;

            priest.EnemyStateMachine.AddState(new PatrolState(patrolPoints));
            priest.EnemyStateMachine.GetServices(_audioSoundsService, _particleEffectsService);
            priest.EnemyStateMachine.InitializeAllStates();
            priest.EnemyStateMachine.SwitchState<PatrolState>();

            return priest;
        }
        
        private Bandit SpawnBanditEnemy(Vector3 enemyPosition, List<Vector3> patrolPoints)
        {
            Bandit bandit = _enemyService.CreateBandit();

            bandit.NavMeshAgent.enabled = false;

            var enemySpawnPosition = enemyPosition +
                                     (Vector3.one * Random.Range(-RandomPositionFactor, RandomPositionFactor));

            enemySpawnPosition.y = enemyPosition.y + OffsetYPolygonEnemies;

            bandit.transform.position = enemySpawnPosition;
            
            bandit.Rigidbody.isKinematic = true;
            bandit.Collider.enabled = true;

            bandit.NavMeshAgent.enabled = true;
            bandit.NavMeshAgent.isStopped = false;
            bandit.NavMeshAgent.ResetPath();

            bandit.Die += OnKillBandit;

            bandit.EnemyStateMachine.AddState(new PatrolState(patrolPoints));
            bandit.EnemyStateMachine.GetServices(_audioSoundsService, _particleEffectsService);
            bandit.EnemyStateMachine.InitializeAllStates();
            bandit.EnemyStateMachine.SwitchState<PatrolState>();

            return bandit;
        }
        
        private BanditRanger SpawnBanditRangerEnemy(Vector3 enemyPosition, List<Vector3> patrolPoints)
        {
            BanditRanger bandit = _enemyService.CreateBanditRanger();

            bandit.NavMeshAgent.enabled = false;

            var enemySpawnPosition = enemyPosition +
                                     (Vector3.one * Random.Range(-RandomPositionFactor, RandomPositionFactor));

            enemySpawnPosition.y = enemyPosition.y + OffsetYPolygonEnemies;

            bandit.transform.position = enemySpawnPosition;
            
            bandit.Rigidbody.isKinematic = true;
            bandit.Collider.enabled = true;

            bandit.NavMeshAgent.enabled = true;
            bandit.NavMeshAgent.isStopped = false;
            bandit.NavMeshAgent.ResetPath();

            bandit.Die += OnKillBandit;

            bandit.EnemyStateMachine.AddState(new PatrolState(patrolPoints));
            bandit.EnemyStateMachine.GetServices(_audioSoundsService, _particleEffectsService);
            bandit.EnemyStateMachine.InitializeAllStates();
            bandit.EnemyStateMachine.SwitchState<PatrolState>();

            return bandit;
        }
        
        private BanditLeader SpawnBanditLeader(Vector3 enemyPosition, List<Vector3> patrolPoints)
        {
            BanditLeader banditLeader = _enemyService.CreateBanditLeader();

            banditLeader.NavMeshAgent.enabled = false;

            var enemySpawnPosition = enemyPosition +
                                     (Vector3.one * Random.Range(-RandomPositionFactor, RandomPositionFactor));

            enemySpawnPosition.y = enemyPosition.y + OffsetYPolygonEnemies;

            banditLeader.transform.position = enemySpawnPosition;
            
            banditLeader.Rigidbody.isKinematic = true;
            banditLeader.Collider.enabled = true;

            banditLeader.NavMeshAgent.enabled = true;
            banditLeader.NavMeshAgent.isStopped = false;
            banditLeader.NavMeshAgent.ResetPath();

            banditLeader.Die += OnKillBanditLeader;

            banditLeader.EnemyStateMachine.AddState(new PatrolState(patrolPoints));
            banditLeader.EnemyStateMachine.GetServices(_audioSoundsService, _particleEffectsService);
            banditLeader.EnemyStateMachine.InitializeAllStates();
            banditLeader.EnemyStateMachine.SwitchState<PatrolState>();

            return banditLeader;
        }

        private void OnKillSkeleton(Enemy.Enemy enemy)
        {
            enemy.Die -= OnKillSkeleton;
            _enemyCounter--;

            CheckEnemiesCount();
        }

        private void OnKillSkeletonHeavyArmor(Enemy.Enemy enemy)
        {
            enemy.Die -= OnKillSkeletonHeavyArmor;
            _enemyCounter--;

            CheckEnemiesCount();
        }

        private void OnKillSkeletonRanger(Enemy.Enemy enemy)
        {
            enemy.Die -= OnKillSkeletonRanger;
            _enemyCounter--;

            CheckEnemiesCount();
        }

        private void OnKillPriest(Enemy.Enemy enemy)
        {
            enemy.Die -= OnKillPriest;
            OnPriestKilled?.Invoke();
            _enemyCounter--;

            CheckEnemiesCount();
        }
        
        private void OnKillBandit(Enemy.Enemy enemy)
        {
            enemy.Die -= OnKillBandit;
            _enemyCounter--;

            CheckEnemiesCount();
        }
        
        private void OnKillBanditLeader(Enemy.Enemy enemy)
        {
            enemy.Die -= OnKillPriest;
            OnBanditLeaderKilled?.Invoke();
            _enemyCounter--;

            CheckEnemiesCount();
        }

        private void CheckEnemiesCount()
        {
            if (_enemyCounter == MinValue)
                OnAllEnemiesKilled?.Invoke();
        }
    }
}