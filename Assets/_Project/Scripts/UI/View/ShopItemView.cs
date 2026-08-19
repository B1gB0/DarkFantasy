using System;
using System.Collections.Generic;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Game.Constant;
using _Project.Scripts.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace _Project.Scripts.UI.View
{
    public class ShopItemView : View
    {
        [SerializeField] private Button _attributeButton;
        [SerializeField] private List<Sprite> _icons;
        [SerializeField] private Image _iconAttribute;
        
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _value;
        [SerializeField] private TMP_Text _price;

        private ItemData _currentData;
        
        public event Action<ItemData, ShopItemView> OnButtonClicked;
        
        private void OnEnable()
        {
            _attributeButton.onClick.AddListener(OnButtonClick);
        }

        private void OnDisable()
        {
            _attributeButton.onClick.RemoveListener(OnButtonClick);
        }

        public void SetCurrencyColor(int gold)
        {
            if(_currentData.Price > gold)
                _price.color = Colors.GetColor(ColorName.RedCurrencyColor);
            else
                _price.color = Colors.GetColor(ColorName.BlackUIColor);
        }

        public void Set(ItemData itemData)
        {
            _currentData = itemData;
            
            switch (_currentData.Type)
            {
                case ItemType.HealthPotion:
                    _iconAttribute.sprite = _icons[0];
                    SetLocalization();
                    break;
                case ItemType.SpeedPotion:
                    _iconAttribute.sprite = _icons[1];
                    SetLocalization();
                    break;
                case ItemType.Meat:
                    _iconAttribute.sprite = _icons[2];
                    SetLocalization();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _value.text = itemData.Value.ToString();
            _price.text = itemData.Price.ToString();
        }

        private void SetLocalization()
        {
            _title.text = YG2.lang switch
            {
                LocalizationCode.Ru => _currentData.NameRu,
                LocalizationCode.En => _currentData.NameEn,
                LocalizationCode.Tr => _currentData.NameTr,
                _ => _title.text
            };
        }

        private void OnButtonClick()
        {
            OnButtonClicked?.Invoke(_currentData, this);
        }
    }
}