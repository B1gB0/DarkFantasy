using System;
using _Project.Scripts.Level;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Project.Scripts.UI.View
{
    public class NewMissionView : View, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image _hover;
        [SerializeField] private Button _button;

        private Mission _mission;

        public event Action<Mission> OnMissionChose;

        private void Awake()
        {
            if (_hover != null)
                _hover.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(OnChooseMission);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnChooseMission);
            
            if (_hover != null)
                _hover.gameObject.SetActive(false);
        }

        public void GetMission(Mission mission)
        {
            _mission = mission;
        }

        private void ActivateHover()
        {
            if (_hover != null)
                _hover.gameObject.SetActive(true);
        }

        private void DeactivateHover()
        {
            if (_hover != null)
                _hover.gameObject.SetActive(false);
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            ActivateHover();
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            DeactivateHover();
        }

        private void OnChooseMission()
        {
            OnMissionChose?.Invoke(_mission);
        }
    }
}