using _Project.Scripts.DataBase.InitDataSO;
using _Project.Scripts.Game.Gameplay.Root.View;
using _Project.Scripts.Game.GameRoot;
using _Project.Scripts.Services;
using Cysharp.Threading.Tasks;
using R3;
using Reflex.Attributes;
using Reflex.Core;
using Reflex.Extensions;
using Reflex.Injectors;
using UnityEngine;

namespace _Project.Scripts.Game.Gameplay
{
    public class GameplayEntryPoint : MonoBehaviour
    {
        // [SerializeField] private CinemachineVirtualCamera _cinemachineVirtualCamera;
        [SerializeField] private UIGameplayRootBinder _sceneUIRootPrefab;
        [SerializeField] private DataFactory _dataFactory;
        [SerializeField] private LevelInitData _levelInitData;
        // [SerializeField] private ViewFactory _viewFactory;

        private Level.Level _level;
        private UIRootView _uiRoot;
        private UIGameplayRootBinder _uiScene;
        private Container _container;
        private GameplayExitParameters _exitParameters;

        private IEnemyService _enemyService;
        private IDataBaseService _dataBaseService;

        private SkeletonInitData _skeletonInitData;

        [Inject]
        private void Construct(IEnemyService enemyService, IDataBaseService dataBaseService)
        {
            _enemyService = enemyService;
            _dataBaseService = dataBaseService;
        }

        public async UniTask<Observable<GameplayExitParameters>> Run(
            UIRootView uiRoot,
            GameplayEnterParameters enterParameters = null)
        {
            _container = gameObject.scene.GetSceneContainer();

            _uiRoot = uiRoot;

            // await _particleEffectsService.Init();

            _uiScene = Instantiate(_sceneUIRootPrefab);

            // _viewFactory.GetUIRootAndUIScene(uiRoot, _uiScene, _container);


            uiRoot.AttachSceneUI(_uiScene.gameObject);
            
            var container = gameObject.scene.GetSceneContainer();
            GameObjectInjector.InjectRecursive(uiRoot.gameObject, container);

            _uiScene.GetUIStateMachine(uiRoot.UIStateMachine);

            // uiRoot.ExitPanel.OnExitToMainMenu += GetMainMenuExitParameters;
            //  uiRoot.ExitPanel.OnExitToMainMenu += _uiScene.HandleGoToNextSceneButtonClick;
            
            //Вот здесь можно писать код для механик и инициализации
            
            await InitData();
            
            await _dataBaseService.Init();
            await _enemyService.Init();
            
            _enemyService.GetData(_levelInitData, _skeletonInitData);
            _level = FindObjectOfType<Level.Level>();
            _level.GetServices(_enemyService, _levelInitData);




            var exitSceneSignalSubject = new Subject<Unit>();
            _uiScene.Bind(exitSceneSignalSubject);

            var exitToSceneSignal = exitSceneSignalSubject.Select(_ => _exitParameters);

            return exitToSceneSignal;
        }
        
        private async UniTask InitData()
        {
            _levelInitData = Instantiate(_levelInitData);
            _skeletonInitData = await _dataFactory.CreateSkeletonInitData();
        }
    }
}