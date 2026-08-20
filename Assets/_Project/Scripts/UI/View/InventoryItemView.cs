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
    public class InventoryItemView : View
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private TMP_Text _name;
        [SerializeField] private TMP_Text _count;
        [SerializeField] private Button _button;
        [SerializeField] private List<Sprite> _icons;

        private ItemData _itemData;

        public void Set(ItemData itemData, int count, Action<ItemData> onUseCallback)
        {
            _itemData = itemData;
            _count.text = count.ToString();
            _button.onClick.AddListener(() => onUseCallback?.Invoke(_itemData));
            
            switch (_itemData.Type)
            {
                case ItemType.HealthPotion:
                    _iconImage.sprite = _icons[0];
                    break;
                case ItemType.SpeedPotion:
                    _iconImage.sprite = _icons[1];
                    break;
                case ItemType.Meat:
                    _iconImage.sprite = _icons[2];
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            SetLocalization();
        }

        private void SetLocalization()
        {
            _name.text = YG2.lang switch
            {
                LocalizationCode.Ru => _itemData.NameRu,
                LocalizationCode.En => _itemData.NameEn,
                LocalizationCode.Tr => _itemData.NameTr,
                _ => _name.text
            };
        }
    }
}