using _Project.Scripts.Services;
using Reflex.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.View
{
    public class InventoryIsFullView : View
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Image _image;
        
        private ITweenAnimationService _tweenAnimationService;

        [Inject]
        private void Construct(ITweenAnimationService tweenAnimationService)
        {
            _tweenAnimationService = tweenAnimationService;
        }

        public override void Show()
        {
            gameObject.SetActive(true);
            transform.SetAsLastSibling();
            _tweenAnimationService.AnimateTemporaryAppearance(_text.transform);
            _tweenAnimationService.AnimateTemporaryAppearance(_image.transform);
        }
    }
}