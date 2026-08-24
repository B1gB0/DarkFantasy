using System;
using System.Threading;
using _Project.Scripts.Services;
using _Project.Scripts.UI.StateMachine;
using _Project.Scripts.UI.StateMachine.States;
using _Project.Scripts.UI.View;
using Cysharp.Threading.Tasks;
using R3;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace _Project.Scripts.Game.Gameplay.Root.View
{
    public class UIGameplayRootBinder : MonoBehaviour
    {
        private const int DelayToShowTutorial = 5;

        private Subject<Unit> _exitSceneSignalSubject;
        private UIStateMachine _uiStateMachine;
        private CancellationTokenSource _tutorialCancellationToken;

        private ITweenAnimationService _tweenAnimationService;

        [field: SerializeField] public GameplayElements UIScene { get; private set; }
        [field: SerializeField] public Transform PointerPoint { get; private set; }
        [field: SerializeField] public Transform ShowKeyboardTutorialPoint { get; private set; }
        [field: SerializeField] public Transform HideKeyboardTutorialPoint { get; private set; }
        [field: SerializeField] public Transform ShowHealthPoint { get; private set; }
        [field: SerializeField] public Transform HideHealthPoint { get; private set; }
        [field: SerializeField] public Transform ShowBossHealthPoint { get; private set; }
        [field: SerializeField] public Transform HideBossHealthPoint { get; private set; }
        [field: SerializeField] public Transform WeaponPoint { get; private set; }
        [field: SerializeField] public Joystick Joystick { get; private set; }
        [field: SerializeField] public GameObject JoystickIcon { get; private set; }
        [field: SerializeField] public TutorialPointer TutorialPointer { get; private set; }
        [field: SerializeField] public KeyboardTutorialView KeyboardTutorialView { get; private set; }
        [field: SerializeField] public GameObject MouseTutorialView { get; private set; }
        [field: SerializeField] public GameObject SpaceTutorialView { get; private set; }
        [field: SerializeField] public Button AttackButton { get; private set; }
        [field: SerializeField] public Button RollButton { get; private set; }
        [field: SerializeField] public Button InventoryButton { get; private set; }
#if UNITY_EDITOR
        [field: SerializeField] public Button CheatButton { get; private set; }
#endif
        [field: SerializeField] public Button EquippedItemButton { get; private set; }
        [field: SerializeField] public EquippedItemView EquippedItemView { get; private set; }

        [Inject]
        public void Construct(ITweenAnimationService tweenAnimationService)
        {
            _tweenAnimationService = tweenAnimationService;
        }

        public void GetUIStateMachine(UIStateMachine uiStateMachine, UIRootButtons uiRootButtons)
        {
            _uiStateMachine = uiStateMachine;
            _uiStateMachine.RemoveState<GameplayState>();
            _uiStateMachine.AddState(new GameplayState(UIScene, uiRootButtons));
        }

        public void Bind(Subject<Unit> exitSceneSignalSubject)
        {
            _exitSceneSignalSubject = exitSceneSignalSubject;
        }

        public void HandleGoToNextScene()
        {
            // AudioSoundsService.PlaySound(SoundsType.Button).Forget();
            _exitSceneSignalSubject?.OnNext(Unit.Default);
        }

        public void ResetCountdownTutorialPointer()
        {
            if (YG2.envir.isDesktop)
            {
                _tweenAnimationService.AnimateMove(
                    KeyboardTutorialView.transform,
                    ShowKeyboardTutorialPoint,
                    HideKeyboardTutorialPoint,
                    true);
            }
            else
            {
                JoystickIcon.SetActive(false);
                TutorialPointer.Deactivate();
            }

            CountdownToShowStoryButtonFoot().Forget();
        }

        public void HandlePCTutorialButtons()
        {
            if (YG2.envir.isDesktop)
            {
                MouseTutorialView.SetActive(true);
                SpaceTutorialView.SetActive(true);
            }
            else
            {
                MouseTutorialView.SetActive(false);
                SpaceTutorialView.SetActive(false);
            }
        }

        private void ShowTutorialPointer()
        {
            if (TutorialPointer == null)
                return;

            JoystickIcon.gameObject.SetActive(true);
            TutorialPointer.Activate();
            TutorialPointer.transform.position = PointerPoint.transform.position;
            _tweenAnimationService.AnimatePointer(TutorialPointer.transform, PointerPoint);
        }

        private void ShowTutorialKeyboardView()
        {
            if (KeyboardTutorialView == null)
                return;

            KeyboardTutorialView.Activate();

            _tweenAnimationService.AnimateMove(
                KeyboardTutorialView.transform,
                ShowKeyboardTutorialPoint,
                HideKeyboardTutorialPoint);
        }

        private async UniTaskVoid CountdownToShowStoryButtonFoot()
        {
            _tutorialCancellationToken?.Cancel();
            _tutorialCancellationToken?.Dispose();
            _tutorialCancellationToken = new CancellationTokenSource();

            try
            {
                var completedTask = await UniTask.WhenAny(
                    UniTask.Delay(TimeSpan.FromSeconds(DelayToShowTutorial),
                        DelayType.DeltaTime,
                        cancellationToken: _tutorialCancellationToken.Token));

                if (completedTask == 0)
                {
                    if (YG2.envir.isDesktop)
                        ShowTutorialKeyboardView();
                    else
                        ShowTutorialPointer();
                }
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}