using System;
using System.Collections.Generic;
using _Project.Scripts.Audio.Sounds;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Items;
using _Project.Scripts.Services;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.View
{
    public class EquippedConsumableItemView : View
    {
        private const int MinCount = 0;
        
        [SerializeField] private Image _iconImage;
        [SerializeField] private TMP_Text _count;

        private ItemData _itemData;
        private IPlayerService _playerService;
        private IInventoryService _inventoryService;
        private IShopService _shopService;
        private AudioSoundsService _audioService;

        [Inject]
        private void Construct(
            IPlayerService playerService,
            IInventoryService inventoryService,
            AudioSoundsService audioSoundsService,
            IShopService shopService)
        {
            _playerService = playerService;
            _inventoryService = inventoryService;
            _audioService = audioSoundsService;
            _shopService = shopService;
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

            _iconImage.sprite = _shopService.GetItemSpriteByType(_itemData.Type);
        }

        public void UnSet()
        {
            _iconImage.gameObject.SetActive(false);
            _count.gameObject.SetActive(false);
        }

        public void ApplyItemEffect()
        {
            if (_itemData == null) return;

            int currentCount = _inventoryService.GetItemCount(_itemData.Type);
            if (currentCount <= MinCount) return;

            var characteristics = _playerService.Player.PlayerCharacteristics;
            bool effectApplied = false;

            switch (_itemData.Type)
            {
                case ItemType.SpeedPotion:
                    effectApplied = characteristics.AddSpeedModifier(
                        _itemData.Value,
                        _itemData.Duration,
                        _itemData.IsMultiplier);
                    break;
                case ItemType.HealthPotion:
                    _playerService.Player.Health.AddHealth(_itemData.Value);
                    effectApplied = true;
                    break;
                case ItemType.Meat:
                    effectApplied = _playerService.Player.Health.TryStartHealingOverTime(
                        _itemData.Value,
                        _itemData.Duration);
                    break;
            }

            if (!effectApplied)
                return;

            _audioService.PlaySound(SoundsType.PotionSound).Forget();
            _inventoryService.RemoveItem(_itemData.Type);

            int newCount = _inventoryService.GetItemCount(_itemData.Type);
            if (newCount > MinCount)
            {
                Set(_itemData, newCount);
            }
            else
            {
                UnSet();
            }
        }
    }
}