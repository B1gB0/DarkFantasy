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

        public event Action<LootResult> OnPickedUp;

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
            ICurrencyService currencyService)
        {
            _reward = reward;
            _lootSpawner = lootSpawner;
            _inventoryService = inventoryService;
            _currencyService = currencyService;
        }
        
        public void Pickup()
        {
            _lootSpawner.Unregister(this);

            ApplyReward();
            Deactivate();
            OnPickedUp?.Invoke(_reward);
        }

        private void ApplyReward()
        {
            switch (_reward.Type)
            {
                case LootType.Gold:
                    _currencyService.AddGold(_reward.GoldValue);
                    break;
                case LootType.Consumable:
                case LootType.Equipment:
                    _inventoryService.AddItem(_reward.ItemType);
                    break;
            }
        }
    }
}