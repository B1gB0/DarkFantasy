using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Project.Scripts.Services
{
    public class PauseService : IPauseService
    {
        private EventSystem _eventSystem;

        public event Action OnGameStarted;
        public event Action OnGamePaused;
        
        private bool _isGamePausedByUser;

        public void OnStopGameWithoutMusic()
        {
            if (_isGamePausedByUser) return;
            _isGamePausedByUser = true;
            Time.timeScale = 0f;
            OnGamePaused?.Invoke();
        }

        public void OnStopGameWithMusic()
        {
            if (_isGamePausedByUser) return;
            _isGamePausedByUser = true;
            Time.timeScale = 0f;
            AudioListener.pause = true;
            OnGamePaused?.Invoke();
        }

        public void OnPlayGame()
        {
            if (!_isGamePausedByUser) return;
            _isGamePausedByUser = false;
            Time.timeScale = 1f;
            AudioListener.pause = false;
            OnGameStarted?.Invoke();
        }
        
        public void HandleSdkPause()
        {
            AudioListener.pause = true;
            _eventSystem.enabled = false;
            OnGamePaused?.Invoke();
        }

        public void HandleSdkResume()
        {
            AudioListener.pause = false;
            _eventSystem.enabled = true;
            OnGameStarted?.Invoke();
        }

        public void GetEventSystem(EventSystem eventSystem)
        {
            _eventSystem = eventSystem;
        }

        public void DisableEventSystem() => _eventSystem.enabled = false;
        public void EnableEventSystem() => _eventSystem.enabled = true;
    }
}