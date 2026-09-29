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
        
        private const float EquipmentChance  = 0.15f;
        private const float ConsumableChance = 0.35f;
        
        private const int LowGold  = 10;
        private const int HighGold = 50;
        private const float HighGoldChance = 0.2f;   // 20% шанс 50, иначе 10

        private readonly List<ItemData> _equipment = new();
        private readonly List<ItemData> _consumables = new();

        private IDataBaseService _dataBaseService;
        private IInventoryService _inventoryService;

        public bool IsInitiated { get; private set; }

        [Inject]
        private void Construct(IDataBaseService dataBaseService, IInventoryService inventoryService)
        {
            _dataBaseService = dataBaseService;
            _inventoryService = inventoryService;
        }

        public UniTask Init()
        {
            if (IsInitiated) return UniTask.CompletedTask;

            foreach (var item in _dataBaseService.Content.ItemsData)
            {
                // В луте участвуют только те, что НЕ продаются в магазине
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

        public LootResult TryGetReward()
        {
            if (!IsInitiated)
                return LootResult.None();

            float roll = Random.value;

            // 1. Снаряга
            if (roll < EquipmentChance)
            {
                if (TryGetRandomEquipment(out var equipment))
                    return LootResult.Item(equipment.Type, LootType.Equipment);

                // Снаряга закончилась — падаем в расходник
                if (TryGetRandomConsumable(out var consumable))
                    return LootResult.Item(consumable.Type, LootType.Consumable);

                // И расходников нет — только золото
                return LootResult.Gold(RollGold());
            }

            // 2. Расходник
            if (roll < EquipmentChance + ConsumableChance)
            {
                if (TryGetRandomConsumable(out var consumable))
                    return LootResult.Item(consumable.Type, LootType.Consumable);

                return LootResult.Gold(RollGold());
            }

            // 3. Золото (по умолчанию)
            return LootResult.Gold(RollGold());
        }

        /// <summary>
        /// Случайная снаряга, которой ещё НЕТ у игрока.
        /// </summary>
        private bool TryGetRandomEquipment(out ItemData result)
        {
            result = null;
            if (_equipment.Count == MinValue) return false;

            // Фильтруем те, что уже есть в инвентаре
            var available = _equipment
                .Where(e => !_inventoryService.HasItem(e.Type))
                .ToList();

            if (available.Count == MinValue) return false;

            int idx = Random.Range(MinValue, available.Count);
            result = available[idx];
            return true;
        }

        /// <summary>
        /// Случайный расходник. Расходники могут повторяться.
        /// </summary>
        private bool TryGetRandomConsumable(out ItemData result)
        {
            result = null;
            if (_consumables.Count == MinValue) return false;

            int idx = Random.Range(MinValue, _consumables.Count);
            result = _consumables[idx];
            return true;
        }

        private static int RollGold()
        {
            return Random.value < HighGoldChance ? HighGold : LowGold;
        }
    }
}