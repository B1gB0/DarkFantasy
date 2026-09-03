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
        [SerializeField] private Image _hoverImage;
        [SerializeField] private TMP_Text _count;
        [SerializeField] private Button _button;
        [SerializeField] private List<Sprite> _icons;

        private ItemData _itemData;
        
        public event Action<ItemType, InventoryItemView> OnSelectButtonPressed;

        private void Start()
        {
            _button.onClick.AddListener(OnSelectItem);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnSelectItem);
        }

        public void Set(ItemData itemData = null, int count = 0)
        {
            _itemData = itemData;
            _count.text = count.ToString();
            
            _hoverImage.gameObject.SetActive(false);
            _iconImage.gameObject.SetActive(true);
            _count.gameObject.SetActive(true);
            
            if (itemData == null)
            {
                _iconImage.gameObject.SetActive(false);
                _count.gameObject.SetActive(false);
                
                return;
            }
            
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
                    _iconImage.gameObject.SetActive(false);
                    _count.gameObject.SetActive(false);
                    break;
            }
        }

        public void ShowHover()
        {
            _hoverImage.gameObject.SetActive(true);
        }
        
        public void HideHover()
        {
            _hoverImage.gameObject.SetActive(false);
        }

        private void OnSelectItem()
        {
            if(_itemData != null)
                OnSelectButtonPressed?.Invoke(_itemData.Type, this);
        }
    }
}