using System;
using System.Collections.Generic;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Items;
using _Project.Scripts.Services;
using Reflex.Attributes;
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
        private IPlayerService _playerService;
        private IInventoryService _inventoryService;

        [Inject]
        private void Construct(IPlayerService playerService,  IInventoryService inventoryService)
        {
            _playerService = playerService;
            _inventoryService = inventoryService;
        }

        private void Start()
        {
            if (_itemData != null) return;
            
            _iconImage.gameObject.SetActive(false);
            _count.gameObject.SetActive(false);
        }

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
        
        public void ApplyItemEffect()
        {
            if (_itemData == null) return;
            var characteristics = _playerService.Player.PlayerCharacteristics;

            switch (_itemData.Type)
            {
                case ItemType.SpeedPotion:
                    characteristics.AddSpeedModifier(_itemData.Value, _itemData.Duration, _itemData.IsMultiplier);
                    break;
                case ItemType.HealthPotion:
                    _playerService.Player.Health.AddHealth(_itemData.Value);
                    break;
                case ItemType.Meat:
                    _playerService.Player.Health.AddHealthOverTime(
                        _itemData.Value,
                        _itemData.Duration)
                        .Forget();
                    break;
            }
            
            _inventoryService.RemoveItem(_itemData.Type);
            Set(_itemData, _inventoryService.GetItemCount(_itemData.Type));
        }
    }
}