using System;
using _Project.Scripts.Game.Constant;
using _Project.Scripts.Level;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace _Project.Scripts.UI.View
{
    public class MissionView : View
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _name;
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
            SetData();
        }

        private void OnChooseMission()
        {
            OnMissionChose?.Invoke(_mission);
        }

        private void SetData()
        {
            _icon.sprite = _mission.Image;

            _name.text = YG2.lang switch
            {
                LocalizationCode.Ru => _mission.NameRu,
                LocalizationCode.En => _mission.NameEn,
                LocalizationCode.Tr => _mission.NameTr,
                _ => _name.text
            };
        }
    }
}