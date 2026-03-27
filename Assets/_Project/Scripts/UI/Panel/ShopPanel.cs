using System.Collections.Generic;
using _Project.Scripts.Characteristics;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Services;
using _Project.Scripts.UI.View;
using DG.Tweening;
using Reflex.Attributes;
using UnityEngine;
using YG;

namespace _Project.Scripts.UI.Panel
{
    public class ShopPanel : View.View
    {
        [SerializeField] private List<AttributeView> _attributeViews;
        
        private ITweenAnimationService _tweenAnimationService;
        private IShopService _shopService;
        private ICurrencyService _currencyService;

        private List<PlayerAttributeLevelData> _healthAttributes;
        private List<PlayerAttributeLevelData> _damageAttributes;
        private List<PlayerAttributeLevelData> _armorAttributes;

        [Inject]
        private void Construct(
            ITweenAnimationService tweenAnimationService,
            IShopService shopService,
            ICurrencyService currencyService)
        {
            _tweenAnimationService = tweenAnimationService;
            _shopService = shopService;
            _currencyService = currencyService;
        }

        private void Start()
        {
            _healthAttributes = _shopService.GetAttributesByType(CharacteristicType.Health);
            _damageAttributes = _shopService.GetAttributesByType(CharacteristicType.Damage);
            _armorAttributes = _shopService.GetAttributesByType(CharacteristicType.Armor);
            
            SetAttributeViews();
        }

        private void OnEnable()
        {
            _attributeViews[0].OnButtonClicked += ApplyPurchase;
            _attributeViews[1].OnButtonClicked += ApplyPurchase;
            _attributeViews[2].OnButtonClicked += ApplyPurchase;
        }

        private void OnDisable()
        {
            _attributeViews[0].OnButtonClicked -= ApplyPurchase;
            _attributeViews[1].OnButtonClicked -= ApplyPurchase;
            _attributeViews[2].OnButtonClicked -= ApplyPurchase;
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

        public void OnChangeLanguage()
        {
            SetAttributeViews();
        }

        private void SetAttributeViews()
        {
            _attributeViews[0].Set(
                _shopService.GetLocalizationDataByType(CharacteristicType.Health),
                _healthAttributes[YG2.saves.HealthAttributeNumber]);
            
            _attributeViews[1].Set(
                _shopService.GetLocalizationDataByType(CharacteristicType.Damage),
                _damageAttributes[YG2.saves.DamageAttributeNumber]);
            
            _attributeViews[2].Set(
                _shopService.GetLocalizationDataByType(CharacteristicType.Armor),
                _armorAttributes[YG2.saves.ArmorAttributeNumber]);
        }

        private void ApplyPurchase(PlayerAttributeLevelData data)
        {
            
            YG2.saves.PlayerCharacteristics.ApplyImprovement(data.Type, data.Value);
        }
    }
}