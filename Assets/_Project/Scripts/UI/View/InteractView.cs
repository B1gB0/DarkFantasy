using _Project.Scripts.Services;
using DG.Tweening;
using Reflex.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace _Project.Scripts.UI.View
{
    public class InteractView : View
    {
        [SerializeField] private Button _mobileButton;
        [SerializeField] private GameObject _desktopButton;
        [SerializeField] private Transform _showPoint;
        [SerializeField] private Transform _hidePoint;
        
        [Header("Texts")]
        [SerializeField] private TMP_Text _pickUpText;
        [SerializeField] private TMP_Text _standUpText;
        [SerializeField] private TMP_Text _sitDownText;
        [SerializeField] private TMP_Text _descriptionText;

        private ITweenAnimationService _tweenAnimationService;

        [Inject]
        private void Construct(ITweenAnimationService tweenAnimationService)
        {
            _tweenAnimationService = tweenAnimationService;
        }

        private void Start()
        {
            if (YG2.envir.isDesktop)
            {
                _mobileButton.gameObject.SetActive(false);
                _desktopButton.gameObject.SetActive(true);
            }
            else
            {
                _mobileButton.gameObject.SetActive(true);
                _desktopButton.gameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            transform.DOKill();
        }
        
        public void Show(InteractViewType type)
        {
            _pickUpText.gameObject.SetActive(false);
            _standUpText.gameObject.SetActive(false);
            _sitDownText.gameObject.SetActive(false);
            _descriptionText.gameObject.SetActive(false);
            
            switch (type)
            {
                case InteractViewType.PickUp:
                    _pickUpText.gameObject.SetActive(true);
                    break;
                case InteractViewType.SitDown:
                    _sitDownText.gameObject.SetActive(true);
                    _descriptionText.gameObject.SetActive(true);
                    break;
                case InteractViewType.StandUp:
                    _standUpText.gameObject.SetActive(true);
                    break;
            }
            
            _tweenAnimationService.AnimateMove(transform, _showPoint, _hidePoint);
        }

        public override void Show()
        {
            Show(InteractViewType.PickUp);
        }

        public override void Hide()
        {
            _tweenAnimationService.AnimateMove(transform, _showPoint, _hidePoint, true);
        }
    }
}