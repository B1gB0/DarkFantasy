using System.Collections.Generic;
using _Project.Scripts.Services;
using _Project.Scripts.UI.View;
using DG.Tweening;
using Reflex.Attributes;
using UnityEngine;

namespace _Project.Scripts.UI.Panel
{
    public class ShopPanel : View.View
    {
        [SerializeField] private List<AttributeView> _attributeViews;
        
        private ITweenAnimationService _tweenAnimationService;
        
        [Inject]
        private void Construct(ITweenAnimationService tweenAnimationService)
        {
            _tweenAnimationService = tweenAnimationService;
        }
        
        private void OnDestroy()
        {
            transform.DOKill();
        }
        
        public override void Show()
        {
            _tweenAnimationService.AnimateScale(transform);
        }

        public override void Hide()
        {
            _tweenAnimationService.AnimateScale(transform, true);
        }
    }
}