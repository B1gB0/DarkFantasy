using System;
using _Project.Scripts.Level;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.View
{
    public class NewMissionView : View
    {
        [SerializeField] private Image _hover;
        [SerializeField] private Button _button;

        private Mission _mission;

        public event Action<Mission> OnMissionChose;

        private void OnEnable()
        {
            _button.onClick.AddListener(OnChooseMission);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnChooseMission);
        }

        public void GetMission(Mission mission)
        {
            _mission = mission;
        }

        private void ActivateHover()
        {
            _hover.gameObject.SetActive(true);
        }

        private void OnChooseMission()
        {
            OnMissionChose?.Invoke(_mission);
        }
    }
}