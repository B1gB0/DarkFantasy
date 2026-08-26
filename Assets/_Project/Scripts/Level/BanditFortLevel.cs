using _Project.Scripts.Level.Triggers;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Level
{
    public class BanditFortLevel : Level
    {
        [SerializeField] private SpawnTrigger _spawnCyclicWaveTrigger;
        [SerializeField] private NextLevelTrigger _nextLevelTrigger;
        
        private void OnEnable()
        {
            IsInitiatedSpawners += SpawnStartWaves;
            _spawnCyclicWaveTrigger.OnSpawnEnemies += CreateFifthWave;
        }
        
        private void FixedUpdate()
        {
            if (_spawnCyclicWaveTrigger.IsEnemySpawned)
            {
                SpawnCyclicWave();
            }
        }

        private void OnDisable()
        {
            IsInitiatedSpawners -= SpawnStartWaves;
            _spawnCyclicWaveTrigger.OnSpawnEnemies -= CreateFifthWave;
        }

        private void OnDestroy()
        {
            EnemySpawner.OnPriestKilled -= OnBanditLeaderKilled;
            _nextLevelTrigger.OnGoToNextLevel -= HandleMissionTransition;
        }

        public override async UniTask OnStartLevel()
        {
            await base.OnStartLevel();
            
            EnemySpawner.OnBanditLeaderKilled += OnBanditLeaderKilled;
            
            _nextLevelTrigger.OnGoToNextLevel += HandleMissionTransition;
        }

        private void SpawnStartWaves()
        {
            CreateWaveOfEnemies(FirstWaveEnemy);
            CreateWaveOfEnemies(SecondWaveEnemy);
            CreateWaveOfEnemies(ThirdWaveEnemy);
        }

        private void SpawnCyclicWave()
        {
            CreateWaveOfEnemyByTimer(FourthWaveEnemy);
        }

        private void CreateFifthWave()
        {
            CreateWaveOfEnemies(FifthWaveNumber);
        }

        private void OnBanditLeaderKilled()
        {
            _nextLevelTrigger.Activate();
            _spawnCyclicWaveTrigger.OnOffEnemySpawn();
        }
        
        private void HandleMissionTransition()
        {
            ViewFactory.GameplayEntryPoint.GetVillageHubExitParameters();
            ViewFactory.UIScene.HandleGoToNextScene();
        }
    }
}