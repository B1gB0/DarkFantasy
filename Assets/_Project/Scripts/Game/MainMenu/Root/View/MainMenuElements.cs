using _Project.Scripts.Services;
using _Project.Scripts.UI;
using DG.Tweening;
using Reflex.Attributes;
using UnityEngine;

namespace _Project.Scripts.Game.MainMenu.Root.View
{
    public class MainMenuElements : UI.View.View
    {
        private ITweenAnimationService _tweenAnimationService;

        [Inject]
        public void Construct(ITweenAnimationService tweenAnimationService)
        {
            _tweenAnimationService = tweenAnimationService;
        }

        private void OnDestroy()
        {
            transform.DOKill();
        }

        public override void Show()
        {
            gameObject.SetActive(true);
            _tweenAnimationService.AnimateScale(transform);
        }

        public override void Hide()
        {
            _tweenAnimationService.AnimateScale(transform, true);
        }
    }
}