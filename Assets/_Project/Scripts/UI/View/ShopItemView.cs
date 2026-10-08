using System;
using System.Collections.Generic;
using _Project.Scripts.Characteristics;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Game.Constant;
using _Project.Scripts.Items;
using _Project.Scripts.Level;
using _Project.Scripts.Services;
using Reflex.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace _Project.Scripts.UI.View
{
    public class ShopItemView : View
    {
        [SerializeField] private Button _buyButton;
        [SerializeField] private Image _iconItem;

        [SerializeField] private TMP_Text _name;
        [SerializeField] private TMP_Text _description;
        [SerializeField] private TMP_Text _price;
        [SerializeField] private TMP_Text _rarity;
        [SerializeField] private TMP_Text _purchaisedText;

        private ItemData _currentData;
        private IShopService _shopService;
        private IInventoryService _inventoryService;
        private IUILocalizationService _uiLocalizationService;
        private IProgressionService _progression;

        public event Action<ItemData, ShopItemView> OnButtonClicked;

        [Inject]
        public void Construct(
            IShopService shopService,
            IInventoryService inventoryService,
            IUILocalizationService uiLocalizationService,
            IProgressionService progression)
        {
            _shopService = shopService;
            _inventoryService = inventoryService;
            _uiLocalizationService = uiLocalizationService;
            _progression = progression;
        }

        private void OnEnable() => _buyButton.onClick.AddListener(OnButtonClick);
        private void OnDisable() => _buyButton.onClick.RemoveListener(OnButtonClick);

        public void SetCurrencyColor(int gold)
        {
            if (_currentData == null) return;

            _price.color = _currentData.Price > gold
                ? Colors.GetColor(ColorName.RedCurrencyColor)
                : Colors.GetColor(ColorName.DefaultWhiteTextColor);
        }

        public void Set(ItemData itemData)
        {
            if (itemData == null) return;

            _currentData = itemData;

            bool owned = itemData.Kind == ItemKind.Equipment
                && _inventoryService.HasEquipment(itemData.Type, _progression.CurrentDifficulty);

            _buyButton.gameObject.SetActive(!owned);
            _purchaisedText.gameObject.SetActive(owned);

            _iconItem.sprite = _shopService.GetItemSpriteByType(itemData.Type);

            SetLocalization();

            _price.text = itemData.Price.ToString();
        }

        public void SetLocalization()
        {
            if (_currentData == null) return;
            
            _name.text = YG2.lang switch
            {
                LocalizationCode.Ru => _currentData.NameRu,
                LocalizationCode.En => _currentData.NameEn,
                LocalizationCode.Tr => _currentData.NameTr,
                _ => _currentData.NameEn,
            };
            
            _description.text = _currentData.Kind == ItemKind.Equipment
                ? BuildEquipmentStats(_currentData, _progression.CurrentDifficulty)
                : GetLocalizedDescription(_currentData);
            
            if (_currentData.Rarity == ItemRarity.None)
            {
                _rarity.gameObject.SetActive(false);
                return;
            }

            SetRarity(_currentData);

            _rarity.gameObject.SetActive(true);
        }
        
        private void SetRarity(ItemData data)
        {
            _rarity.gameObject.SetActive(true);

            _rarity.text = data.Rarity switch
            {
                ItemRarity.Common => _uiLocalizationService.GetLocalizedText(UITextType.Common),
                ItemRarity.Uncommon => _uiLocalizationService.GetLocalizedText(UITextType.Uncommon),
                ItemRarity.Rare => _uiLocalizationService.GetLocalizedText(UITextType.Rare),
                _ => throw new ArgumentOutOfRangeException()
            };

            _rarity.color = data.Rarity switch
            {
                ItemRarity.Common => Colors.GetColor(ColorName.RarityCommon),
                ItemRarity.Uncommon => Colors.GetColor(ColorName.RarityUncommon),
                ItemRarity.Rare => Colors.GetColor(ColorName.RarityRare),
                _ => Colors.GetColor(ColorName.DefaultWhiteTextColor),
            };
        }

        private string GetLocalizedDescription(ItemData data)
        {
            return YG2.lang switch
            {
                LocalizationCode.Ru => data.DescriptionRu,
                LocalizationCode.En => data.DescriptionEn,
                LocalizationCode.Tr => data.DescriptionTr,
                _ => data.DescriptionEn,
            };
        }

        private string BuildEquipmentStats(ItemData template, LevelDifficulty difficulty)
        {
            float mul = _progression.GetItemMultiplier(difficulty);

            var parts = new List<string>(3);

            AddStatPart(parts, template.CharacteristicType, template.Value * mul);
            AddStatPart(parts, template.CharacteristicType2, template.Value2 * mul);
            AddStatPart(parts, template.CharacteristicType3, template.Value3 * mul);

            return string.Join(", ", parts);
        }
        
        private void AddStatPart(List<string> parts, CharacteristicType type, float value)
        {
            if (type == CharacteristicType.None) return;
            if (value <= 0f) return;

            string localized = _shopService.GetLocalizedCharacteristicName(type);
            if (string.IsNullOrEmpty(localized)) return;

            parts.Add($"+{value:0.#} {localized}");
        }

        private void OnButtonClick()
        {
            OnButtonClicked?.Invoke(_currentData, this);
        }
    }
}