using System;
using System.Collections.Generic;
using _Project.Scripts.DataBase.InitDataSO;
using _Project.Scripts.Enemy;
using _Project.Scripts.Game.Constant;
using _Project.Scripts.Game.GameRoot;
using _Project.Scripts.Level.Spawners;
using _Project.Scripts.Player;
using _Project.Scripts.Services;
using _Project.Scripts.UI;
using _Project.Scripts.UI.Panel;
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
        protected const int FirstWaveEnemy = 0;
        protected const int SecondWaveEnemy = 1;
        protected const int ThirdWaveEnemy = 2;
        protected const int FourthWaveEnemy = 3;
        protected const int FifthWaveNumber = 4;

        private const float MinValue = 0f;
        private const int MinIndex = 0;

        [Header("EnemyWaves")] 
        [SerializeField] protected float SpawnWaveOfEnemyDelay = 10f;
        [SerializeField] protected List<EnemyWave> EnemyWaves;
        [SerializeField] protected LootSpawner LootSpawner;
        [SerializeField] private int _limitEnemies;

        protected ViewFactory ViewFactory;
        protected UIStateMachine UIStateMachine;
        protected UIRootView UIRootView;

        protected NavMeshWaypointService NavMeshWaypointService;

        protected EnemySpawner EnemySpawner;
        protected BossHealthBar BossHealthBar;
        protected bool IsBossTriggered;

        private IEnemyService _enemyService;
        private IPlayerService _playerService;
        private IUILocalizationService _uiLocalizationService;
        private ParticleEffectsService _particleEffectsService;
        private AudioSoundsService _audioSoundsService;
        private IFloatingTextService _floatingTextService;
        private ILootService _lootService;

        private Enemy.Enemy _boss;
        private float _lastSpawnTime;
        private LevelInitData _levelInitData;
        private PlayerInitData _playerInitData;
        private CinemachineFreeLook _cinemachineFreeLook;

        public event Action IsInitiatedSpawners;
        public event Action OnBossHealthBarCreated;
        public event Action PlayerIsSpawned;
        public event Action OnGoToNextScene;

        public HealthBar HealthBar { get; private set; }
        public ModifiersPanel ModifiersPanel { get; private set; }

        [Inject]
        private void Construct(
            IEnemyService enemyService,
            IPlayerService playerService,
            ParticleEffectsService particleEffectsService,
            AudioSoundsService audioSoundsService,
            IUILocalizationService uiLocalizationService,
            NavMeshWaypointService navMeshWaypointService,
            IFloatingTextService floatingTextService,
            ILootService lootService)
        {
            _enemyService = enemyService;
            _playerService = playerService;
            _particleEffectsService = particleEffectsService;
            _audioSoundsService = audioSoundsService;
            _uiLocalizationService = uiLocalizationService;
            NavMeshWaypointService = navMeshWaypointService;
            _floatingTextService = floatingTextService;
            _lootService = lootService;
        }

        private void OnDestroy()
        {
            EnemySpawner.OnBossSpawned -= OnBossSpawned;
            EnemySpawner.OnRewardDropped -= LootSpawner.SpawnLoot;
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

            await NavMeshWaypointService.Init();
        }

        public void TryShowBossUI()
        {
            if (!IsBossTriggered || BossHealthBar == null || _boss == null)
                return;

            BossHealthBar.Show();
            SetBossNameLocalization();

            OnBossHealthBarCreated -= TryShowBossUI;
        }

        public void TryHideBossUI()
        {
            if (!IsBossTriggered || BossHealthBar == null || _boss == null)
                return;

            BossHealthBar.Hide();
        }

        protected void CreateWaveOfEnemyByTimer(int numberWaveEnemy)
        {
            if (_lastSpawnTime <= MinValue)
            {
                CreateWaveOfEnemies(numberWaveEnemy);

                foreach (var enemy in EnemyWaves[numberWaveEnemy].Enemies)
                {
                    enemy.ChangeFollowEnemyState(true);
                }

                _lastSpawnTime = SpawnWaveOfEnemyDelay;
            }

            _lastSpawnTime -= Time.fixedDeltaTime;
        }

        protected void CreateWaveOfEnemies(int numberWave)
        {
            if (EnemyWaves.Count == MinIndex)
                return;

            EnemySpawner.SpawnWave(EnemyWaves[numberWave]);
        }

        protected void GoToNextScene()
        {
            OnGoToNextScene?.Invoke();
        }

        private async UniTask CreatePlayer()
        {
            var data = _playerService.GetPlayerDataByType(PlayerType.CommonHero);

            Player.Core.Player player = _playerService.CreatePlayerByPrefab(
                _playerInitData.CommonHero,
                _levelInitData.PlayerSpawnPosition);

            var playerCharacteristics = _playerService.InitPlayerCharacteristics(data);
            player.Construct(playerCharacteristics, _particleEffectsService, _floatingTextService);

            HealthBar = await ViewFactory.CreateHealthBar(player.Health);
            HealthBar.Show();

            ModifiersPanel = await ViewFactory.CreateModifiersPanel();
            ModifiersPanel.Show();

            var playerTransform = player.transform;

            _cinemachineFreeLook.LookAt = playerTransform;
            _cinemachineFreeLook.Follow = playerTransform;

            PlayerIsSpawned?.Invoke();

            _playerService.Player.PlayerCollisionHandler.GetEnemyWaves(EnemyWaves);

            _playerService.SpawnPlayer();
        }

        private void SetBossNameLocalization()
        {
            if (_boss == null || BossHealthBar == null) return;

            UITextType uiTextType = GetBossUITextType(_boss.Data.Type);
            string localizedName = GetLocalizedText(uiTextType);
            BossHealthBar.SetName(localizedName);
        }

        private async void OnBossSpawned(Enemy.Enemy enemy)
        {
            _boss = enemy;
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
            var text = _uiLocalizationService.GetLocalizedText(uiTextType);
            return text;
        }

        private void InitSpawners(IEnemyService enemyService)
        {
            InitEnemyWaves();

            EnemySpawner = new EnemySpawner(
                enemyService,
                _limitEnemies,
                _audioSoundsService,
                _particleEffectsService,
                _lootService);
            
            LootSpawner.GetViews(
                ViewFactory.UIScene.RewardView,
                ViewFactory.UIScene.PickUpView,
                ViewFactory.UIScene.InventoryIsFullView);
            
            EnemySpawner.OnBossSpawned += OnBossSpawned;
            EnemySpawner.OnRewardDropped += LootSpawner.SpawnLoot;

            IsInitiatedSpawners?.Invoke();
        }

        private void InitEnemyWaves()
        {
            for (int i = MinIndex; i < EnemyWaves.Count; i++)
            {
                switch (i)
                {
                    case FirstWaveEnemy:
                        EnemyWaves[i].GetEnemyPositions(
                            _levelInitData.FirstWaveSpawnPoints,
                            _levelInitData.EnemyFirstPatrolPositions);
                        break;
                    case SecondWaveEnemy:
                        EnemyWaves[i].GetEnemyPositions(
                            _levelInitData.SecondWaveSpawnPoints,
                            _levelInitData.EnemySecondPatrolPositions);
                        break;
                    case ThirdWaveEnemy:
                        EnemyWaves[i].GetEnemyPositions(
                            _levelInitData.ThirdWaveSpawnPoints,
                            _levelInitData.EnemyThirdPatrolPositions);
                        break;
                    case FourthWaveEnemy:
                        EnemyWaves[i].GetEnemyPositions(
                            _levelInitData.FourthWaveSpawnPoints,
                            _levelInitData.EnemyFourthPatrolPositions);
                        break;
                    case FifthWaveNumber:
                        EnemyWaves[i].GetEnemyPositions(
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
            BossHealthBar = await ViewFactory.CreateBossHealthBar(_boss.Health);
        }
    }
}