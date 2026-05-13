using _Project.Scripts.UI.StateMachine;
using _Project.Scripts.UI.StateMachine.States;
using _Project.Scripts.UI.View;
using R3;
using UnityEngine;

namespace _Project.Scripts.Game.Gameplay.Root.View
{
    public class UIGameplayRootBinder : MonoBehaviour
    {
        private Subject<Unit> _exitSceneSignalSubject;
        private UIStateMachine _uiStateMachine;
        
        [field: SerializeField] public GameplayElements UIScene { get; private set; }
        [field: SerializeField] public Transform ShowHealthPoint { get; private set; }
        [field: SerializeField] public Transform HideHealthPoint { get; private set; }
        [field: SerializeField] public Transform WeaponPoint { get; private set; }
        
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
        
        public void HandleGoToNextSceneButtonClick()
        {
            // AudioSoundsService.PlaySound(SoundsType.Button).Forget();
            _exitSceneSignalSubject?.OnNext(Unit.Default);
        }
    }
}