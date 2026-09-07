using System;
using UnityEngine.EventSystems;

namespace _Project.Scripts.Services
{
    public interface IPauseService
    {
        public void OnStopGameWithoutMusic();
        public void OnStopGameWithMusic();
        public void OnPlayGame();
        public void GetEventSystem(EventSystem eventSystem);
        public void DisableEventSystem();
        public void EnableEventSystem();
    }
}