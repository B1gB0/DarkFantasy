using System;
using _Project.Scripts.Items;
using _Project.Scripts.Level.Spawners;
using _Project.Scripts.Services;
using UnityEngine;

namespace _Project.Scripts.Level.Triggers
{
    public class LootEnemyTrigger : Trigger
    {
        private LootResult _reward;
        private LootSpawner _lootSpawner;

        private IInventoryService _inventoryService;
        private ICurrencyService _currencyService;
        private ILootService _lootService;
        private IProgressionService _progressionService;
        private IShopService _shopService;

        public event Action<LootResult> OnPickedUp;
        public event Action OnInventoryIsFull;

        private void OnTriggerEnter(Collider other)
        {
            if (_lootSpawner == null) return;
            if (!other.TryGetComponent(out Player.Core.Player _)) return;
            _lootSpawner.Register(this);
        }

        private void OnTriggerExit(Collider other)
        {
            if (_lootSpawner == null) return;
            if (!other.TryGetComponent(out Player.Core.Player _)) return;
            _lootSpawner.Unregister(this);
        }

        public void Setup(
            LootResult reward,
            LootSpawner lootSpawner,
            IInventoryService inventoryService,
            ICurrencyService currencyService,
            ILootService lootService,
            IProgressionService progressionService,
            IShopService shopService)
        {
            _reward = reward;
            _lootSpawner = lootSpawner;
            _inventoryService = inventoryService;
            _currencyService = currencyService;
            _lootService = lootService;
            _progressionService = progressionService;
            _shopService =  shopService;
        }

        public void Pickup()
        {
            if (!CanPickupReward())
            {
                OnInventoryIsFull?.Invoke();
                return;
            }
            
            _lootSpawner.Unregister(this);

            var difficulty = _progressionService.CurrentDifficulty;

            switch (_reward.Type)
            {
                case LootType.Equipment:
                    var instance = Item.Create(_reward.ItemType, difficulty);
                    _inventoryService.AddEquipment(instance);
                    _lootService.ReleaseReservation(_reward.ItemType, difficulty);
                    break;
                case LootType.Consumable:
                    _inventoryService.AddItem(_reward.ItemType);
                    break;
                case LootType.Gold:
                    _currencyService.AddGold(_reward.GoldValue);
                    break;
            }

            Deactivate();
            OnPickedUp?.Invoke(_reward);
        }
        
        private bool CanPickupReward()
        {
            if (_reward.Type == LootType.Gold) return true;

            var data = _shopService.GetItemDataByType(_reward.ItemType);
            if (data == null) return false;

            return _inventoryService.CanFit(data);
        }
    }
}