using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Game.Constant;
using _Project.Scripts.Level.Triggers;
using _Project.Scripts.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;
using YG;

namespace _Project.Scripts.Level
{
    public class CryptLevel : Level
    {
        [SerializeField] private SpawnTrigger _spawnCyclicWaveTrigger;
        [SerializeField] private NextLevelTrigger _nextLevelTrigger;
        [SerializeField] private GameObject[] _portals;
        
        private bool _isBossTriggered;

        private void OnEnable()
        {
            IsInitiatedSpawners += SpawnStartWaves;
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
            _spawnCyclicWaveTrigger.OnSpawnEnemies -= OnSpawnCyclicWave;
            OnBossHealthBarCreated -= TryShowBossUI;
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

        private void OnSpawnCyclicWave()
        {
            foreach (var portal in _portals)
            {
                portal.SetActive(true);
            }
            
            _isBossTriggered = true;
            TryShowBossUI();
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

            YG2.saves.IsBanditCampUnlock = true;
            YG2.SaveProgress();
        }
        
        private void HandleMissionTransition()
        {
            ViewFactory.GameplayEntryPoint.GetVillageHubExitParameters();
            ViewFactory.UIScene.HandleGoToNextScene();
        }
        
        private void TryShowBossUI()
        {
            if (!_isBossTriggered || BossHealthBar == null || Boss == null)
                return;
            
            BossHealthBar.Show();
            SetBossNameLocalization();
            
            OnBossHealthBarCreated -= TryShowBossUI;
        }
    }
}