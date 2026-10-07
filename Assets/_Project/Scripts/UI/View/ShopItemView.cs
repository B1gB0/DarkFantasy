using System;
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
        [SerializeField] private TMP_Text _value;
        [SerializeField] private TMP_Text _price;
        [SerializeField] private TMP_Text _rarity;
        [SerializeField] private TMP_Text _purchaisedText;

        private ItemData _currentData;
        private IShopService _shopService;
        private IInventoryService _inventoryService;
        private IUILocalizationService _uiLocalizationService;

        public event Action<ItemData, ShopItemView> OnButtonClicked;

        [Inject]
        public void Construct(
            IShopService shopService,
            IInventoryService inventoryService,
            IUILocalizationService uiLocalizationService)
        {
            _shopService = shopService;
            _inventoryService = inventoryService;
            _uiLocalizationService = uiLocalizationService;
        }

        private void OnEnable()
        {
            _buyButton.onClick.AddListener(OnButtonClick);
        }

        private void OnDisable()
        {
            _buyButton.onClick.RemoveListener(OnButtonClick);
        }

        public void SetCurrencyColor(int gold)
        {
            if (_currentData.Price > gold)
                _price.color = Colors.GetColor(ColorName.RedCurrencyColor);
            else
                _price.color = Colors.GetColor(ColorName.DefaultWhiteTextColor);
        }

        public void Set(ItemData itemData)
        {
            _currentData = itemData;

            if (_inventoryService.HasEquipment(itemData.Type, LevelDifficulty.Easy) 
                && itemData.Kind == ItemKind.Equipment)
            {
                OnPurchased();
            }
            else
            {
                _buyButton.gameObject.SetActive(true);
                _purchaisedText.gameObject.SetActive(false);
            }

            _iconItem.sprite = _shopService.GetItemSpriteByType(_currentData.Type);

            SetLocalization();

            _price.text = itemData.Price.ToString();
        }

        public void SetLocalization()
        {
            _name.text = YG2.lang switch
            {
                LocalizationCode.Ru => _currentData.NameRu,
                LocalizationCode.En => _currentData.NameEn,
                LocalizationCode.Tr => _currentData.NameTr,
                _ => _name.text
            };

            _value.text = YG2.lang switch
            {
                LocalizationCode.Ru => _currentData.DescriptionRu,
                LocalizationCode.En => _currentData.DescriptionEn,
                LocalizationCode.Tr => _currentData.DescriptionTr,
                _ => _value.text
            };

            if (_currentData.Rarity == ItemRarity.None)
            {
                _rarity.gameObject.SetActive(false);
                return;
            }

            _rarity.gameObject.SetActive(true);
            
            _rarity.text = _currentData.Rarity switch
            {
                ItemRarity.Common => _uiLocalizationService.GetLocalizedText(UITextType.Common),
                ItemRarity.Uncommon => _uiLocalizationService.GetLocalizedText(UITextType.Uncommon),
                ItemRarity.Rare => _uiLocalizationService.GetLocalizedText(UITextType.Rare),
                _ => throw new ArgumentOutOfRangeException()
            };

            _rarity.color = _currentData.Rarity switch
            {
                ItemRarity.Common => Colors.GetColor(ColorName.RarityCommon),
                ItemRarity.Uncommon => Colors.GetColor(ColorName.RarityUncommon),
                ItemRarity.Rare => Colors.GetColor(ColorName.RarityRare),
                _ => Colors.GetColor(ColorName.DefaultWhiteTextColor),
            };
        }

        private void OnButtonClick()
        {
            OnPurchased();
            OnButtonClicked?.Invoke(_currentData, this);
        }

        private void OnPurchased()
        {
            if (_currentData.Kind != ItemKind.Equipment) return;

            _buyButton.gameObject.SetActive(false);
            _purchaisedText.gameObject.SetActive(true);
        }
    }
}