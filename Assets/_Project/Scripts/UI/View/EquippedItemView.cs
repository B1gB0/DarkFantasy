using System;
using System.Collections.Generic;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.View
{
    public class EquippedItemView : View
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private TMP_Text _count;
        [SerializeField] private List<Sprite> _icons;
        
        private ItemData _itemData;

        public void Set(ItemData itemData, int count)
        {
            _iconImage.gameObject.SetActive(true);
            _count.gameObject.SetActive(true);
            
            _itemData = itemData;
            _count.text = count.ToString();
            
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
        }
        
        public void UnSet()
        {
            _iconImage.gameObject.SetActive(false);
            _count.gameObject.SetActive(false);
        }
    }
}