using System;
using System.Collections.Generic;
using _Project.Scripts.DataBase.InitDataSO;
using _Project.Scripts.Game.GameRoot;
using _Project.Scripts.Level.Spawners;
using _Project.Scripts.Player;
using _Project.Scripts.Services;
using _Project.Scripts.UI.StateMachine;
using _Project.Scripts.UI.View;
using Cinemachine;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using UnityEngine;

namespace _Project.Scripts.Level
{
    public abstract class Level : MonoBehaviour
    {
        protected const float MinValue = 0f;

        protected const int FirstWaveEnemy = 0;
        protected const int SecondWaveEnemy = 1;
        protected const int ThirdWaveEnemy = 2;
        protected const int FourthWaveEnemy = 3;
        protected const int FifthWaveNumber = 4;

        [Header("EnemyWaves")]
        [SerializeField] protected float SpawnWaveOfEnemyDelay = 10f;

        [SerializeField] private List<EnemyWave> _enemyWaves;
        [SerializeField] private int _limitEnemies;

        protected ViewFactory ViewFactory;
        protected UIStateMachine UIStateMachine;
        protected UIRootView UIRootView;

        protected IShopService ShopService;
        protected IInventoryService InventoryService;

        protected float LastSpawnTime;

        protected EnemySpawner EnemySpawner;

        private IEnemyService _enemyService;
        private IPlayerService _playerService;
        private ParticleEffectsService _particleEffectsService;
        private AudioSoundsService _audioSoundsService;

        private LevelInitData _levelInitData;
        private PlayerInitData _playerInitData;
        private CinemachineFreeLook _cinemachineFreeLook;
        
        public event Action IsInitiatedSpawners;
        public event Action PlayerIsSpawned;
        public event Action OnGoToNextScene;

        public HealthBar HealthBar { get; private set; }
        public List<EnemyWave> EnemyWaves => _enemyWaves;

        [Inject]
        private void Construct(
            IEnemyService enemyService,
            IPlayerService playerService,
            ParticleEffectsService particleEffectsService,
            IShopService shopService,
            IInventoryService inventoryService,
            AudioSoundsService audioSoundsService)
        {
            _enemyService = enemyService;
            _playerService = playerService;
            _particleEffectsService = particleEffectsService;
            ShopService = shopService;
            InventoryService = inventoryService;
            _audioSoundsService = audioSoundsService;
        }

        public void GetDependencies(
            LevelInitData levelInitData,
            PlayerInitData playerInitData,
            CinemachineFreeLook cinemachineFreeLook,
            ViewFactory viewFactory,
            UIStateMachine uiStateMachine,
            UIRootView uiRootView
        )
        {
            _levelInitData = levelInitData;
            _playerInitData = playerInitData;
            _cinemachineFreeLook = cinemachineFreeLook;

            ViewFactory = viewFactory;
            UIStateMachine = uiStateMachine;
            UIRootView = uiRootView;
        }

        public virtual async UniTask OnStartLevel()
        {
            await CreatePlayer();

            InitSpawners(_enemyService);
        }

        protected async UniTask CreatePlayer()
        {
            var data = _playerService.GetPlayerDataByType(PlayerType.CommonHero);

            Player.Core.Player player = _playerService.CreatePlayerByPrefab(
                _playerInitData.CommonHero,
                _levelInitData.PlayerSpawnPosition);

            var playerCharacteristics = _playerService.InitPlayerCharacteristics(data);
            player.Construct(playerCharacteristics, _particleEffectsService);

            HealthBar = await ViewFactory.CreateHealthBar(player.Health);
            HealthBar.Show();

            var playerTransform = player.transform;

            _cinemachineFreeLook.LookAt = playerTransform;
            _cinemachineFreeLook.Follow = playerTransform;

            PlayerIsSpawned?.Invoke();

            _playerService.Player.PlayerCollisionHandler.GetEnemyWaves(_enemyWaves);
            
            _playerService.SpawnPlayer();
        }

        protected void CreateWaveOfEnemyByTimer(int numberWaveEnemy)
        {
            if (LastSpawnTime <= MinValue)
            {
                CreateWaveOfEnemies(numberWaveEnemy);

                foreach (var enemy in _enemyWaves[numberWaveEnemy].Enemies)
                {
                    enemy.ChangeFollowEnemyState(true);
                }

                LastSpawnTime = SpawnWaveOfEnemyDelay;
            }

            LastSpawnTime -= Time.fixedDeltaTime;
        }

        protected void CreateWaveOfEnemies(int numberWave)
        {
            if (_enemyWaves.Count == 0)
                return;

            EnemySpawner.SpawnWave(_enemyWaves[numberWave]);
        }

        protected void GoToNextScene()
        {
            OnGoToNextScene?.Invoke();
        }

        private void InitSpawners(IEnemyService enemyService)
        {
            InitEnemyWaves();

            EnemySpawner = new EnemySpawner(enemyService, _limitEnemies, _audioSoundsService, _particleEffectsService);

            IsInitiatedSpawners?.Invoke();
        }

        private void InitEnemyWaves()
        {
            for (int i = 0; i < _enemyWaves.Count; i++)
            {
                switch (i)
                {
                    case FirstWaveEnemy:
                        _enemyWaves[i].GetEnemyPositions(
                            _levelInitData.FirstWaveSpawnPoints,
                            _levelInitData.EnemyFirstPatrolPositions);
                        break;
                    case SecondWaveEnemy:
                        _enemyWaves[i].GetEnemyPositions(
                            _levelInitData.SecondWaveSpawnPoints,
                            _levelInitData.EnemySecondPatrolPositions);
                        break;
                    case ThirdWaveEnemy:
                        _enemyWaves[i].GetEnemyPositions(
                            _levelInitData.ThirdWaveSpawnPoints,
                            _levelInitData.EnemyThirdPatrolPositions);
                        break;
                    case FourthWaveEnemy:
                        _enemyWaves[i].GetEnemyPositions(
                            _levelInitData.FourthWaveSpawnPoints,
                            _levelInitData.EnemyFourthPatrolPositions);
                        break;
                    case FifthWaveNumber:
                        _enemyWaves[i].GetEnemyPositions(
                            _levelInitData.FifthWaveSpawnPoints,
                            _levelInitData.EnemyFifthPatrolPositions);
                        break;
                    default:
                        throw new Exception("There is not enough data for new waves");
                }
            }
        }
    }
}