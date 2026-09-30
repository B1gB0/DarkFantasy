using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Items;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using UnityEngine;

namespace _Project.Scripts.Services
{
    public class LootService : ILootService
    {
        private const int MinValue = 0;

        private const float EquipmentChance = 0.15f;
        private const float ConsumableChance = 0.35f;

        private const int LowGold = 10;
        private const int HighGold = 50;
        private const float HighGoldChance = 0.2f;

        private readonly List<ItemData> _equipment = new();
        private readonly List<ItemData> _consumables = new();

        private IDataBaseService _dataBaseService;
        private IInventoryService _inventoryService;
        private ICurrencyService _currencyService;

        public bool IsInitiated { get; private set; }

        [Inject]
        private void Construct(
            IDataBaseService dataBaseService,
            IInventoryService inventoryService,
            ICurrencyService currencyService)
        {
            _dataBaseService = dataBaseService;
            _inventoryService = inventoryService;
            _currencyService = currencyService;
        }

        public UniTask Init()
        {
            if (IsInitiated) return UniTask.CompletedTask;

            foreach (var item in _dataBaseService.Content.ItemsData)
            {
                if (item.IsSold && item.Kind == ItemKind.Equipment) continue;

                switch (item.Kind)
                {
                    case ItemKind.Equipment:
                        _equipment.Add(item);
                        break;
                    case ItemKind.Consumable:
                        _consumables.Add(item);
                        break;
                }
            }

            IsInitiated = true;
            return UniTask.CompletedTask;
        }

        public LootResult GetReward()
        {
            if (!IsInitiated) return LootResult.None();

            float roll = Random.value;

            if (roll < EquipmentChance)
            {
                if (TryGetRandomEquipment(out var equipment))
                {
                    _inventoryService.AddItem(equipment.Type);
                    return LootResult.Item(equipment.Type, LootType.Equipment);
                }

                if (TryGetRandomConsumable(out var consumable))
                {
                    _inventoryService.AddItem(consumable.Type);
                    return LootResult.Item(equipment.Type, LootType.Consumable);
                }

                return RollGold();
            }

            if (roll < EquipmentChance + ConsumableChance)
            {
                if (TryGetRandomConsumable(out var consumable))
                {
                    _inventoryService.AddItem(consumable.Type);
                    return LootResult.Item(consumable.Type, LootType.Consumable);
                }
            }

            return RollGold();
        }

        private bool TryGetRandomEquipment(out ItemData result)
        {
            result = null;
            if (_equipment.Count == MinValue) return false;

            var available = _equipment
                .Where(e => !_inventoryService.HasItem(e.Type))
                .ToList();

            if (available.Count == MinValue) return false;

            int idx = Random.Range(MinValue, available.Count);
            result = available[idx];
            return true;
        }

        private bool TryGetRandomConsumable(out ItemData result)
        {
            result = null;
            if (_consumables.Count == MinValue) return false;

            int idx = Random.Range(MinValue, _consumables.Count);
            result = _consumables[idx];
            return true;
        }

        private LootResult RollGold()
        {
            int gold;

            if (Random.value < HighGoldChance)
            {
                gold = HighGold;
                _currencyService.AddGold(gold);
                return LootResult.Gold(gold, true);
            }

            gold = LowGold;
            _currencyService.AddGold(gold);
            return LootResult.Gold(gold, false);
        }
    }
}