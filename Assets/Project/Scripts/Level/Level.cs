using System;
using System.Collections.Generic;
using Cinemachine;
using Cysharp.Threading.Tasks;
using Project.Scripts.Camera;
using Project.Scripts.DataBase.InitDataSO;
using Project.Scripts.Enemy;
using Project.Scripts.Game.GameRoot;
using Project.Scripts.Level.Spawners;
using Project.Scripts.Player;
using Project.Scripts.Services;
using Project.Scripts.UI;
using Project.Scripts.UI.Panel;
using Project.Scripts.UI.View;
using Reflex.Attributes;
using UnityEngine;

namespace Project.Scripts.Level
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

        [SerializeField] private float _spawnWaveOfEnemyDelay = 10f;
        [SerializeField] private List<EnemyWave> _enemyWaves;
        [SerializeField] private LootSpawner _lootSpawner;

        [SerializeField] private int _limitEnemies;
        [SerializeField] private LevelInitData _levelInitData;
        [SerializeField] private CameraTargetFollower _cameraTargetFollower;

        private ViewFactory _viewFactory;
        private UIRootView _uiRootView;

        private NavMeshWaypointService _navMeshWaypointService;
        private MissionService _missionService;
        private IProgressionService _progressionService;

        private EnemySpawner _enemySpawner;
        private BossHealthBar _bossHealthBar;
        private bool _isBossTriggered;

        private IEnemyService _enemyService;
        private IPlayerService _playerService;
        private IUILocalizationService _uiLocalizationService;
        private ParticleEffectsService _particleEffectsService;
        private AudioSoundsService _audioSoundsService;
        private IFloatingTextService _floatingTextService;
        private ILootService _lootService;
        private IDataBaseService _dataBaseService;

        private Enemy.Enemy _boss;
        private float _lastSpawnTime;
        private CinemachineFreeLook _cinemachineFreeLook;

        public event Action IsInitiatedSpawners;

        public event Action OnBossHealthBarCreated;

        public HealthBar HealthBar { get; private set; }
        public ModifiersPanel ModifiersPanel { get; private set; }

        protected ViewFactory ViewFactory => _viewFactory;
        protected UIRootView UIRootView => _uiRootView;
        protected List<EnemyWave> EnemyWaves => _enemyWaves;
        protected LootSpawner LootSpawner => _lootSpawner;
        protected NavMeshWaypointService NavMeshWaypointService => _navMeshWaypointService;
        protected MissionService MissionService => _missionService;
        protected IProgressionService ProgressionService => _progressionService;
        protected EnemySpawner EnemySpawner => _enemySpawner;
        protected BossHealthBar BossHealthBar => _bossHealthBar;

        [Inject]
        private void Construct(
            IEnemyService enemyService,
            IPlayerService playerService,
            ParticleEffectsService particleEffectsService,
            AudioSoundsService audioSoundsService,
            IUILocalizationService uiLocalizationService,
            NavMeshWaypointService navMeshWaypointService,
            IFloatingTextService floatingTextService,
            ILootService lootService,
            MissionService missionService,
            IProgressionService progressionService,
            IDataBaseService dataBaseService)
        {
            _enemyService = enemyService;
            _playerService = playerService;
            _particleEffectsService = particleEffectsService;
            _audioSoundsService = audioSoundsService;
            _uiLocalizationService = uiLocalizationService;
            _floatingTextService = floatingTextService;
            _lootService = lootService;
            _dataBaseService = dataBaseService;

            _navMeshWaypointService = navMeshWaypointService;
            _missionService = missionService;
            _progressionService = progressionService;
        }

        private void OnDestroy()
        {
            _enemySpawner.OnBossSpawned -= OnBossSpawned;
            _enemySpawner.OnRewardDropped -= _lootSpawner.SpawnLoot;
            _uiRootView.LocalizationLanguageSwitcher.OnLanguageChanged -= SetBossNameLocalization;
        }

        public void GetDependencies(
            CinemachineFreeLook cinemachineFreeLook,
            ViewFactory viewFactory,
            UIRootView uiRootView)
        {
            _cinemachineFreeLook = cinemachineFreeLook;

            _viewFactory = viewFactory;
            _uiRootView = uiRootView;
        }

        public virtual async UniTask OnStartLevel()
        {
            await CreatePlayer();

            InitSpawners(_enemyService);

            await _navMeshWaypointService.Init();
        }

        public void TryShowBossUI()
        {
            if (!_isBossTriggered || _bossHealthBar == null || _boss == null || _boss.IsDead)
                return;

            _bossHealthBar.Show();
            SetBossNameLocalization();

            OnBossHealthBarCreated -= TryShowBossUI;
        }

        public void TryHideBossUI()
        {
            if (!_isBossTriggered || _bossHealthBar == null || _boss == null)
                return;

            _bossHealthBar.Hide();
        }

        protected void SetIsBossTriggered(bool value)
        {
            _isBossTriggered = value;
        }

        protected void CreateWaveOfEnemyByTimer(int numberWaveEnemy)
        {
            if (_lastSpawnTime <= MinValue)
            {
                CreateWaveOfEnemies(numberWaveEnemy);

                foreach (var enemy in _enemyWaves[numberWaveEnemy].Enemies)
                {
                    enemy.ChangeFollowEnemyState(true);
                }

                _lastSpawnTime = _spawnWaveOfEnemyDelay;
            }

            _lastSpawnTime -= Time.fixedDeltaTime;
        }

        protected void CreateWaveOfEnemies(int numberWave)
        {
            if (_enemyWaves.Count == MinIndex)
                return;

            _enemySpawner.SpawnWave(_enemyWaves[numberWave]);
        }

        private async UniTask CreatePlayer()
        {
            var data = _playerService.GetPlayerDataByType(PlayerType.CommonHero);

            Player.Core.Player player = _playerService.CreatePlayerByPrefab(
                _dataBaseService.PlayerInitData.CommonHero,
                _levelInitData.PlayerSpawnPosition);

            var playerCharacteristics = _playerService.InitPlayerCharacteristics(data);

            player.Construct(
                playerCharacteristics,
                _particleEffectsService,
                _floatingTextService);

            player.Initialize();

            _cinemachineFreeLook.LookAt = _cameraTargetFollower.transform;
            _cinemachineFreeLook.Follow = _cameraTargetFollower.transform;

            _playerService.Player.PlayerCollisionHandler.GetEnemyWaves(_enemyWaves);

            _playerService.SpawnPlayer();

            HealthBar = await _viewFactory.GetOrCreateHealthBar(player.Health);
            HealthBar.Show();

            ModifiersPanel = await _viewFactory.CreateModifiersPanel();
            ModifiersPanel.Show();
        }

        private void SetBossNameLocalization()
        {
            if (_boss == null || _bossHealthBar == null) return;

            UITextType uiTextType = GetBossUITextType(_boss.Data.Type);
            string localizedName = GetLocalizedText(uiTextType);
            _bossHealthBar.SetName(localizedName);
        }

        private async void OnBossSpawned(Enemy.Enemy enemy)
        {
            _boss = enemy;
            await CreateBossHealthBar();
            OnBossHealthBarCreated?.Invoke();
            _uiRootView.LocalizationLanguageSwitcher.OnLanguageChanged += SetBossNameLocalization;
        }

        private UITextType GetBossUITextType(EnemyType enemyType) => enemyType switch
        {
            EnemyType.Priest => UITextType.PriestName,
            EnemyType.BanditLeader => UITextType.BanditLeaderName,
            EnemyType.DarkLord => UITextType.DarkLordName,
            _ => throw new ArgumentOutOfRangeException(nameof(enemyType), enemyType, "Unknown boss type"),
        };

        private string GetLocalizedText(UITextType uiTextType)
        {
            var text = _uiLocalizationService.GetLocalizedText(uiTextType);
            return text;
        }

        private void InitSpawners(IEnemyService enemyService)
        {
            InitEnemyWaves();

            _enemySpawner = new EnemySpawner(
                enemyService,
                _limitEnemies,
                _audioSoundsService,
                _particleEffectsService,
                _lootService);

            _lootSpawner.GetViews(
                _viewFactory.UIScene.RewardView,
                _viewFactory.UIScene.InteractView,
                _viewFactory.UIScene.InventoryIsFullView);

            _enemySpawner.OnBossSpawned += OnBossSpawned;
            _enemySpawner.OnRewardDropped += _lootSpawner.SpawnLoot;

            IsInitiatedSpawners?.Invoke();
        }

        private void InitEnemyWaves()
        {
            for (int i = MinIndex; i < _enemyWaves.Count; i++)
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
            _bossHealthBar = await _viewFactory.CreateBossHealthBar(_boss.Health);
        }
    }
}