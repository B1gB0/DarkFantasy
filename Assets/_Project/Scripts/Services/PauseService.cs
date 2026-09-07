using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Project.Scripts.Services
{
    public class PauseService : IPauseService
    {
        private const float StopTime = 0f;
        private const float PlayTime = 1f;

        private EventSystem _eventSystem;

        public void OnStopGameWithoutMusic()
        {
            AudioListener.pause = false;
            Time.timeScale = StopTime;
        }

        public void OnStopGameWithMusic()
        {
            AudioListener.pause = true;
            Time.timeScale = StopTime;
        }

        public void OnPlayGame()
        {
            AudioListener.pause = false;
            Time.timeScale = PlayTime;
        }

        public void GetEventSystem(EventSystem eventSystem)
        {
            _eventSystem = eventSystem;
        }

        public void DisableEventSystem()
        {
            _eventSystem.enabled = false;
        }

        public void EnableEventSystem()
        {
            _eventSystem.enabled = true;
        }
    }
}