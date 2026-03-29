using System.Collections.Generic;
using _Project.Scripts.Characteristics;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Services;
using _Project.Scripts.UI.View;
using DG.Tweening;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace _Project.Scripts.UI.Panel
{
    public class ShopPanel : View.View
    {
        [SerializeField] private List<AttributeView> _attributeViews;
        [SerializeField] private Button _backSceneButton;
        
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
            Deactivate();
            
            _healthAttributes = _shopService.GetAttributesByType(CharacteristicType.Health);
            _damageAttributes = _shopService.GetAttributesByType(CharacteristicType.Damage);
            _armorAttributes = _shopService.GetAttributesByType(CharacteristicType.Armor);
        }

        private void OnEnable()
        {
            _backSceneButton.onClick.AddListener(Hide);
            _attributeViews[0].OnButtonClicked += ApplyPurchase;
            _attributeViews[1].OnButtonClicked += ApplyPurchase;
            _attributeViews[2].OnButtonClicked += ApplyPurchase;
        }

        private void OnDisable()
        {
            _backSceneButton.onClick.RemoveListener(Hide);
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
            SetAttributeViews();
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

        private void ApplyPurchase(PlayerAttributeLevelData data, AttributeView attributeView)
        {
            List<PlayerAttributeLevelData> attributeList = data.Type switch
            {
                CharacteristicType.Health => _healthAttributes,
                CharacteristicType.Damage => _damageAttributes,
                CharacteristicType.Armor => _armorAttributes,
                _ => null
            };
            
            if (attributeList == null || attributeList.IndexOf(data) == attributeList.Count - 1)
                return;
            
            YG2.saves.PlayerCharacteristics.ApplyImprovement(data.Type, data.Value);
            
            PlayerAttributeLevelData newAttributeData = null;
            
            switch (data.Type)
            {
                case CharacteristicType.Health:
                    newAttributeData = _healthAttributes[YG2.saves.HealthAttributeNumber];
                    break;
                case CharacteristicType.Damage:
                    newAttributeData = _damageAttributes[YG2.saves.DamageAttributeNumber];
                    break;
                case CharacteristicType.Armor:
                    newAttributeData = _armorAttributes[YG2.saves.ArmorAttributeNumber];
                    break;
            }
            
            attributeView.Set(
                _shopService.GetLocalizationDataByType(data.Type),
                newAttributeData);
            
            YG2.SaveProgress();
        }
    }
}