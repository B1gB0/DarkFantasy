using System;
using System.Collections.Generic;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.DataBase.InitDataSO;
using _Project.Scripts.Enemy;
using _Project.Scripts.Game.Constant;
using _Project.Scripts.Game.GameRoot;
using _Project.Scripts.Level.Spawners;
using _Project.Scripts.Player;
using _Project.Scripts.Services;
using _Project.Scripts.UI;
using _Project.Scripts.UI.StateMachine;
using _Project.Scripts.UI.View;
using Cinemachine;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using UnityEngine;
using YG;

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

        [Header("EnemyWaves")] [SerializeField]
        protected float SpawnWaveOfEnemyDelay = 10f;

        [SerializeField] private List<EnemyWave> _enemyWaves;
        [SerializeField] private int _limitEnemies;

        protected ViewFactory ViewFactory;
        protected UIStateMachine UIStateMachine;
        protected UIRootView UIRootView;
        protected Enemy.Enemy Boss;

        protected IShopService ShopService;
        protected IInventoryService InventoryService;
        protected NavMeshWaypointService NavMeshWaypointService;

        protected float LastSpawnTime;

        protected EnemySpawner EnemySpawner;
        protected bool IsBossTriggered;

        private IEnemyService _enemyService;
        private IPlayerService _playerService;
        private IUILocalizationService _uiLocalizationService;
        private ParticleEffectsService _particleEffectsService;
        private AudioSoundsService _audioSoundsService;

        private LevelInitData _levelInitData;
        private PlayerInitData _playerInitData;
        private CinemachineFreeLook _cinemachineFreeLook;
        
        public event Action IsInitiatedSpawners;
        public event Action OnBossHealthBarCreated;
        public event Action PlayerIsSpawned;
        public event Action OnGoToNextScene;

        public HealthBar HealthBar { get; private set; }
        public BossHealthBar BossHealthBar { get; private set; }
        public List<EnemyWave> EnemyWaves => _enemyWaves;

        [Inject]
        private void Construct(
            IEnemyService enemyService,
            IPlayerService playerService,
            ParticleEffectsService particleEffectsService,
            IShopService shopService,
            IInventoryService inventoryService,
            AudioSoundsService audioSoundsService,
            IUILocalizationService uiLocalizationService,
            NavMeshWaypointService navMeshWaypointService)
        {
            _enemyService = enemyService;
            _playerService = playerService;
            _particleEffectsService = particleEffectsService;
            ShopService = shopService;
            InventoryService = inventoryService;
            _audioSoundsService = audioSoundsService;
            _uiLocalizationService = uiLocalizationService;
            NavMeshWaypointService = navMeshWaypointService;
        }

        private void OnDestroy()
        {
            EnemySpawner.OnBossSpawned -= OnBossSpawned;
            UIRootView.LocalizationLanguageSwitcher.OnLanguageChanged -= SetBossNameLocalization;
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
        
        public void TryShowBossUI()
        {
            if (!IsBossTriggered || BossHealthBar == null || Boss == null)
                return;
            
            BossHealthBar.Show();
            SetBossNameLocalization();
            
            OnBossHealthBarCreated -= TryShowBossUI;
        }
        
        public void TryHideBossUI()
        {
            if (!IsBossTriggered || BossHealthBar == null || Boss == null)
                return;
            
            BossHealthBar.Hide();
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
        
        protected void SetBossNameLocalization()
        {
            if (Boss == null || BossHealthBar == null) return;

            UITextType uiTextType = GetBossUITextType(Boss.Data.Type);
            string localizedName = GetLocalizedText(uiTextType);
            BossHealthBar.SetName(localizedName);
        }
        
        protected async void OnBossSpawned(Enemy.Enemy enemy)
        {
            Boss = enemy;
            await CreateBossHealthBar();
            OnBossHealthBarCreated?.Invoke();
            UIRootView.LocalizationLanguageSwitcher.OnLanguageChanged += SetBossNameLocalization;
        }

        private UITextType GetBossUITextType(EnemyType enemyType) => enemyType switch
        {
            EnemyType.Priest => UITextType.PriestName,
            EnemyType.BanditLeader => UITextType.BanditLeaderName,
            EnemyType.DarkLord => UITextType.DarkLordName,
            _ => throw new ArgumentOutOfRangeException(nameof(enemyType), enemyType, "Unknown boss type")
        };

        private string GetLocalizedText(UITextType uiTextType)
        {
            var data = _uiLocalizationService.GetLevelTextData(uiTextType);
            return YG2.lang switch
            {
                LocalizationCode.Ru => data.NameRu,
                LocalizationCode.En => data.NameEn,
                LocalizationCode.Tr => data.NameTr,
                _ => data.NameEn
            };
        }

        private void InitSpawners(IEnemyService enemyService)
        {
            InitEnemyWaves();

            EnemySpawner = new EnemySpawner(enemyService, _limitEnemies, _audioSoundsService, _particleEffectsService);
            
            EnemySpawner.OnBossSpawned += OnBossSpawned;
            
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

        private async UniTask CreateBossHealthBar()
        {
            BossHealthBar = await ViewFactory.CreateBossHealthBar(Boss.Health);
        }
    }
}