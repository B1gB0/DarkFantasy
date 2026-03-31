using _Project.Scripts.DataBase.InitDataSO;
using _Project.Scripts.Game.Gameplay.Root.View;
using _Project.Scripts.Game.GameRoot;
using _Project.Scripts.Services;
using _Project.Scripts.UI.View;
using Cinemachine;
using Cysharp.Threading.Tasks;
using R3;
using Reflex.Attributes;
using Reflex.Core;
using Reflex.Extensions;
using Reflex.Injectors;
using UnityEngine;
using YG;

namespace _Project.Scripts.Game.Gameplay
{
    public class GameplayEntryPoint : MonoBehaviour
    {
        [SerializeField] private CinemachineFreeLook _freeLookCamera;
        [SerializeField] private UIGameplayRootBinder _sceneUIRootPrefab;
        [SerializeField] private DataFactory _dataFactory;
        [SerializeField] private LevelInitData _levelInitData;
        [SerializeField] private ViewFactory _viewFactory;

        private Level.Level _level;
        private UIRootView _uiRoot;
        private UIGameplayRootBinder _uiScene;
        private Container _container;
        private GameplayExitParameters _exitParameters;

        private IEnemyService _enemyService;
        private IDataBaseService _dataBaseService;
        private IPlayerService _playerService;
        private IFloatingTextService _floatingTextService;
        private ParticleEffectsService _particleEffectsService;
        private AudioSoundsService _audioSoundsService;

        private EnemyInitData _enemyInitData;
        private PlayerInitData _playerInitData;

        [Inject]
        private void Construct(
            IEnemyService enemyService,
            IDataBaseService dataBaseService,
            IPlayerService playerService,
            ParticleEffectsService particleEffectsService,
            AudioSoundsService audioSoundsService,
            IFloatingTextService floatingTextService)
        {
            _enemyService = enemyService;
            _dataBaseService = dataBaseService;
            _playerService = playerService;
            _particleEffectsService = particleEffectsService;
            _audioSoundsService = audioSoundsService;
            _floatingTextService = floatingTextService;
        }

        public async UniTask<Observable<GameplayExitParameters>> Run(
            UIRootView uiRoot,
            GameplayEnterParameters enterParameters = null)
        {
            _container = gameObject.scene.GetSceneContainer();

            _uiRoot = uiRoot;

            await _particleEffectsService.Init();

            _uiScene = Instantiate(_sceneUIRootPrefab);

            _viewFactory.GetUIRootAndUIScene(uiRoot, _uiScene, _container);

            uiRoot.AttachSceneUI(_uiScene.gameObject);
            
            GameObjectInjector.InjectRecursive(uiRoot.gameObject, _container);

            _uiScene.GetUIStateMachine(uiRoot.UIStateMachine, _uiRoot.UIRootButtons);

            // uiRoot.ExitPanel.OnExitToMainMenu += GetMainMenuExitParameters;
            //  uiRoot.ExitPanel.OnExitToMainMenu += _uiScene.HandleGoToNextSceneButtonClick;

            //Вот здесь можно писать код для механик и инициализации

            await InitData();

            await _dataBaseService.Init();
            await _enemyService.Init();
            await _playerService.Init();
            await _audioSoundsService.Init();
            
            _playerService.GetSceneObjects(_container, _freeLookCamera);

            _level = FindObjectOfType<Level.Level>();
            GameObjectInjector.InjectObject(_level.gameObject, _container);

            _enemyService.GetData(_enemyInitData);

            _level.GetDependencies(
                _levelInitData,
                _playerInitData,
                _freeLookCamera,
                _viewFactory,
                _uiRoot.UIStateMachine,
                _uiRoot);

            FloatingTextView floatingTextView = await _viewFactory.CreateFloatingTextView();
            floatingTextView.Deactivate();
            
            _floatingTextService.Init(floatingTextView);

            await _level.OnStartLevel();

            var exitSceneSignalSubject = new Subject<Unit>();
            _uiScene.Bind(exitSceneSignalSubject);

            var exitToSceneSignal = exitSceneSignalSubject.Select(_ => _exitParameters);

            return exitToSceneSignal;
        }

        private async UniTask InitData()
        {
            _levelInitData = Instantiate(_levelInitData);
            _enemyInitData = await _dataFactory.CreateSkeletonInitData();
            _playerInitData = await _dataFactory.CreatePlayerInitData();
        }
    }
}