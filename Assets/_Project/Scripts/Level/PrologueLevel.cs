using _Project.Scripts.Level.Triggers;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Level
{
    public class PrologueLevel : Level
    {
        [SerializeField] private NextLevelTrigger _nextLevelTrigger;
        
        private void OnEnable()
        {
            IsInitiatedSpawners += SpawnStartWaves;
        }

        private void OnDisable()
        {
            IsInitiatedSpawners -= SpawnStartWaves;
        }

        private void OnDestroy()
        {
            EnemySpawner.OnAllEnemiesKilled -= _nextLevelTrigger.Activate;
            _nextLevelTrigger.OnGoToNextLevel -= HandleMissionTransition;
        }

        public override async UniTask OnStartLevel()
        {
            await base.OnStartLevel();
            
            EnemySpawner.OnAllEnemiesKilled += _nextLevelTrigger.Activate;
            _nextLevelTrigger.OnGoToNextLevel += HandleMissionTransition;
        }

        private void SpawnStartWaves()
        {
            CreateWaveOfEnemies(FirstWaveEnemy);
            CreateWaveOfEnemies(SecondWaveEnemy);
        }

        private void HandleMissionTransition()
        {
            ViewFactory.GameplayEntryPoint.GetVillageHubExitParameters();
            ViewFactory.UIScene.HandleGoToNextSceneButtonClick();
        }
    }
}