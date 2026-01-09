using Cysharp.Threading.Tasks;
using Project.Scripts.Game.Gameplay.Root.View;
using Project.Scripts.Game.GameRoot;
using Project.Scripts.UI.View;
using R3;
using Reflex.Attributes;
using Reflex.Core;
using Reflex.Extensions;
using UnityEngine;

namespace Project.Scripts.Game.Gameplay
{
    public class GameplayEntryPoint : MonoBehaviour
    {
        // [SerializeField] private CinemachineVirtualCamera _cinemachineVirtualCamera;
        [SerializeField] private UIGameplayRootBinder _sceneUIRootPrefab;
        // [SerializeField] private ViewFactory _viewFactory;

        private UIRootView _uiRoot;
        private UIGameplayRootBinder _uiScene;
        private Container _container;
        private GameplayExitParameters _exitParameters;

        public async UniTask<Observable<GameplayExitParameters>> Run(
            UIRootView uiRoot,
            GameplayEnterParameters enterParameters)
        {
            _container = gameObject.scene.GetSceneContainer();

            _uiRoot = uiRoot;

            // await _particleEffectsService.Init();

            _uiScene = Instantiate(_sceneUIRootPrefab);

            // _viewFactory.GetUIRootAndUIScene(uiRoot, _uiScene, _container);


            uiRoot.AttachSceneUI(_uiScene.gameObject);

            _uiScene.GetUIStateMachine(uiRoot.UIStateMachine);


            // uiRoot.ExitPanel.OnExitToMainMenu += GetMainMenuExitParameters;
            //  uiRoot.ExitPanel.OnExitToMainMenu += _uiScene.HandleGoToNextSceneButtonClick;

            var exitSceneSignalSubject = new Subject<Unit>();
            _uiScene.Bind(exitSceneSignalSubject);

            var exitToSceneSignal = exitSceneSignalSubject.Select(_ => _exitParameters);

            return exitToSceneSignal;
        }
    }
}