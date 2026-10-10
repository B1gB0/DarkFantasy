using _Project.Scripts.Services;
using Reflex.Attributes;
using TMPro;
using UnityEngine;

namespace _Project.Scripts.UI.View
{
    public class InventoryIsFullView : View
    {
        [SerializeField] private TMP_Text _text;
        
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
        }
    }
}