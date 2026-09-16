using System;
using System.Collections.Generic;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Game.Constant;
using _Project.Scripts.Items;
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

        private ItemData _currentData;
        private IShopService _shopService;
        
        public event Action<ItemData, ShopItemView> OnButtonClicked;

        [Inject]
        public void Construct(IShopService shopService)
        {
            _shopService = shopService;
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
            if(_currentData.Price > gold)
                _price.color = Colors.GetColor(ColorName.RedCurrencyColor);
            else
                _price.color = Colors.GetColor(ColorName.DefaultWhiteTextColor);
        }

        public void Set(ItemData itemData)
        {
            _currentData = itemData;
            
            switch (_currentData.Type)
            {
                case ItemType.HealthPotion:
                    _iconItem.sprite = _shopService.GetItemSpriteByType(ItemType.HealthPotion);
                    break;
                case ItemType.SpeedPotion:
                    _iconItem.sprite = _shopService.GetItemSpriteByType(ItemType.SpeedPotion);;
                    break;
                case ItemType.Meat:
                    _iconItem.sprite = _shopService.GetItemSpriteByType(ItemType.Meat);;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

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
                LocalizationCode.Ru => "+" + _currentData.Value + "ОЗ",
                LocalizationCode.En => "+" + _currentData.Value + "HP",
                LocalizationCode.Tr => "+" + _currentData.Value + "CP",
                _ => _name.text
            };
        }

        private void OnButtonClick()
        {
            OnButtonClicked?.Invoke(_currentData, this);
        }
    }
}