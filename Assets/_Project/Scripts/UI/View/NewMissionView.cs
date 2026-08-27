using System;
using _Project.Scripts.Level;
using _Project.Scripts.Services;
using Reflex.Attributes;
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
        private ITweenAnimationService _tweenAnimationService;

        public event Action<Mission> OnMissionChose;

        [Inject]
        private void Construct(ITweenAnimationService tweenAnimationService)
        {
            _tweenAnimationService = tweenAnimationService;
        }

        private void Start()
        {
            if (_hover != null)
            {
                _hover.gameObject.SetActive(true);
                _tweenAnimationService.AnimateFade(_hover.transform, true);
            }
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(OnChooseMission);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnChooseMission);
            
            if (_hover != null)
                _tweenAnimationService.AnimateFade(_hover.transform, true);
        }

        public void GetMission(Mission mission)
        {
            _mission = mission;
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_hover != null)
                _tweenAnimationService.AnimateFade(_hover.transform);
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            if (_hover != null)
                _tweenAnimationService.AnimateFade(_hover.transform, true);
        }

        private void OnChooseMission()
        {
            OnMissionChose?.Invoke(_mission);
        }
    }
}