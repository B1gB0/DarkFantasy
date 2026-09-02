using _Project.Scripts.Level.Triggers;
using Cysharp.Threading.Tasks;
using UnityEngine;
using YG;

namespace _Project.Scripts.Level
{
    public class TempleLevel : Level
    {
        [SerializeField] private SpawnTrigger _spawnCyclicWaveTrigger;
        [SerializeField] private NextLevelTrigger _nextLevelTrigger;
        [SerializeField] private GameObject[] _portals;
        
        private void OnEnable()
        {
            IsInitiatedSpawners += SpawnStartWaves;
            _spawnCyclicWaveTrigger.OnSpawnEnemies += ShowPortals;
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
            _spawnCyclicWaveTrigger.OnSpawnEnemies -= ShowPortals;
        }

        private void OnDestroy()
        {
            EnemySpawner.OnPriestKilled -= OnPriestKilled;
            _nextLevelTrigger.OnGoToNextLevel -= HandleMissionTransition;
        }

        public override async UniTask OnStartLevel()
        {
            await base.OnStartLevel();
            
            EnemySpawner.OnPriestKilled += OnPriestKilled;
            
            _nextLevelTrigger.OnGoToNextLevel += HandleMissionTransition;
        }

        private void SpawnStartWaves()
        {
            CreateWaveOfEnemies(FirstWaveEnemy);
            CreateWaveOfEnemies(SecondWaveEnemy);
            CreateWaveOfEnemies(ThirdWaveEnemy);
            CreateWaveOfEnemies(FifthWaveNumber);
        }

        private void SpawnCyclicWave()
        {
            CreateWaveOfEnemyByTimer(FourthWaveEnemy);
        }

        private void ShowPortals()
        {
            BossHealthBar.Show();
            SetBossNameLocalization();
            
            foreach (var portal in _portals)
            {
                portal.SetActive(true);
            }
        }

        private void OnPriestKilled()
        {
            _nextLevelTrigger.Activate();
            _spawnCyclicWaveTrigger.OnOffEnemySpawn();

            EnemyWaves[FirstWaveEnemy].KillEnemies();
            EnemyWaves[SecondWaveEnemy].KillEnemies();
            EnemyWaves[ThirdWaveEnemy].KillEnemies();
            EnemyWaves[FourthWaveEnemy].KillEnemies();
            
            foreach (var portal in _portals)
            {
                portal.SetActive(false);
            }
            
            BossHealthBar.Hide();
            
            YG2.SaveProgress();
        }
        
        private void HandleMissionTransition()
        {
            ViewFactory.GameplayEntryPoint.GetVillageHubExitParameters();
            ViewFactory.UIScene.HandleGoToNextScene();
        }
    }
}