using _Project.Scripts.Level.Triggers;
using Cysharp.Threading.Tasks;
using UnityEngine;
using YG;

namespace _Project.Scripts.Level
{
    public class BanditFortLevel : Level
    {
        [SerializeField] private SpawnTrigger _spawnCyclicWaveTrigger;
        [SerializeField] private NextLevelTrigger _nextLevelTrigger;
        
        [SerializeField] private GameObject[] _portals;
        
        private void OnEnable()
        {
            IsInitiatedSpawners += SpawnStartWaves;
            _spawnCyclicWaveTrigger.OnSpawnEnemies += CreateFifthWave;
            _spawnCyclicWaveTrigger.OnSpawnEnemies += OnSpawnCyclicWave;
            OnBossHealthBarCreated += TryShowBossUI;
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
            _spawnCyclicWaveTrigger.OnSpawnEnemies -= OnSpawnCyclicWave;
            OnBossHealthBarCreated -= TryShowBossUI;
        }

        private void OnDestroy()
        {
            EnemySpawner.OnBanditLeaderKilled -= OnBanditLeaderKilled;
            EnemySpawner.OnBanditLeaderKilled -= OnShowWaypointToNextLevel;
            _nextLevelTrigger.OnGoToNextLevel -= HandleMissionTransition;
        }

        public override async UniTask OnStartLevel()
        {
            await base.OnStartLevel();
            
            EnemySpawner.OnBanditLeaderKilled += OnBanditLeaderKilled;
            EnemySpawner.OnBanditLeaderKilled += OnShowWaypointToNextLevel;
            
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
        
        private void OnSpawnCyclicWave()
        {
            IsBossTriggered = true;
            TryShowBossUI();
        }

        private void CreateFifthWave()
        {
            foreach (var portal in _portals)
            {
                portal.SetActive(true);
            }
            
            CreateWaveOfEnemies(FifthWaveNumber);
            
            foreach (var enemy in EnemyWaves[FifthWaveNumber].Enemies)
            {
                enemy.ChangeFollowEnemyState(true);
            }
        }

        private void OnBanditLeaderKilled()
        {
            _nextLevelTrigger.Activate();
            _spawnCyclicWaveTrigger.OnOffEnemySpawn();
            
            foreach (var portal in _portals)
            {
                portal.SetActive(false);
            }
            
            BossHealthBar.Hide();
            
            YG2.saves.IsCastleUnlock = true;
            YG2.SaveProgress();
        }
        
        private void HandleMissionTransition()
        {
            ViewFactory.GameplayEntryPoint.GetVillageHubExitParameters();
            ViewFactory.UIScene.HandleGoToNextScene();
        }
        
        private void OnShowWaypointToNextLevel()
        {
            NavMeshWaypointService.ShowWaypoint(_nextLevelTrigger.transform);
        }
    }
}