using _Project.Scripts.Items;
using _Project.Scripts.Level.Triggers;
using Cysharp.Threading.Tasks;
using UnityEngine;
using YG;

namespace _Project.Scripts.Level
{
    public class BanditCampLevel : Level
    {
        [SerializeField] private SpawnTrigger _spawnLastWaveTrigger;
        [SerializeField] private NextLevelTrigger _nextLevelTrigger;
        [SerializeField] private LootChestTrigger _lootChestTrigger;
        [SerializeField] private LootChest _lootChest;

        private void OnEnable()
        {
            IsInitiatedSpawners += SpawnStartWaves;
            _spawnLastWaveTrigger.OnSpawnEnemies += SpawnLastWave;
        }

        private void OnDisable()
        {
            IsInitiatedSpawners -= SpawnStartWaves;
            _spawnLastWaveTrigger.OnSpawnEnemies -= SpawnLastWave;
        }

        private void OnDestroy()
        {
            EnemySpawner.OnAllEnemiesKilled -= _nextLevelTrigger.Activate;
            EnemySpawner.OnAllEnemiesKilled -= OnShowWaypointToNextLevel;
            _lootChest.OnChestOpened -= ViewFactory.UIScene.RewardView.Show;
            _lootChest.OnChestOpened -= OnChestOpenedHandler;
            _lootChest.OnInventoryIsFull -= ViewFactory.UIScene.InventoryIsFullView.Show;
            _lootChestTrigger.OnGotLoot -= _lootChest.OpenInstantly;
            _nextLevelTrigger.OnGoToNextLevel -= HandleMissionTransition;

            YG2.SaveProgress();
        }

        public override async UniTask OnStartLevel()
        {
            await base.OnStartLevel();

            EnemySpawner.OnAllEnemiesKilled += _nextLevelTrigger.Activate;
            EnemySpawner.OnAllEnemiesKilled += OnShowWaypointToNextLevel;

            _nextLevelTrigger.OnGoToNextLevel += HandleMissionTransition;
            
            _lootChest.OnChestOpened += ViewFactory.UIScene.RewardView.Show;
            _lootChest.OnInventoryIsFull += ViewFactory.UIScene.InventoryIsFullView.Show;
            _lootChest.OnChestOpened += OnChestOpenedHandler;
            _lootChestTrigger.OnGotLoot += _lootChest.OpenInstantly;
        }

        private void SpawnStartWaves()
        {
            CreateWaveOfEnemies(FirstWaveEnemy);
            CreateWaveOfEnemies(SecondWaveEnemy);
            CreateWaveOfEnemies(ThirdWaveEnemy);
        }

        private void SpawnLastWave()
        {
            CreateWaveOfEnemies(FourthWaveEnemy);
        }

        private void HandleMissionTransition()
        {
            ViewFactory.GameplayEntryPoint.GetGameplayExitParameters();
            ViewFactory.UIScene.HandleGoToNextScene();
        }

        private void OnShowWaypointToNextLevel()
        {
            NavMeshWaypointService.ShowWaypoint(_nextLevelTrigger.transform);
        }
        
        private void OnChestOpenedHandler(LootResult result)
        {
            _lootChestTrigger.MarkAsUsed();
        }
    }
}