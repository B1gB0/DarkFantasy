using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Items;
using _Project.Scripts.Level;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using UnityEngine;

namespace _Project.Scripts.Services
{
    public class LootService : ILootService
    {
        private const int MinValue = 0;

        private const float EnemyEquipmentChance = 0.3f;
        private const float ConsumableChance = 0.35f;
        private const float PrimaryWeight = 0.7f;

        private const int LowGold = 10;
        private const int HighGold = 50;
        private const float HighGoldChance = 0.2f;

        private readonly List<ItemData> _commonEquipment = new();
        private readonly List<ItemData> _uncommonEquipment = new();
        private readonly List<ItemData> _rareEquipment = new();
        private readonly List<ItemData> _consumables = new();

        private readonly HashSet<(ItemType, LevelDifficulty)> _reserved = new();

        private IDataBaseService _dataBaseService;
        private IInventoryService _inventoryService;
        private IProgressionService _progressionService;

        public bool IsInitiated { get; private set; }

        [Inject]
        private void Construct(
            IDataBaseService dataBaseService,
            IInventoryService inventoryService,
            IProgressionService progressionService)
        {
            _dataBaseService = dataBaseService;
            _inventoryService = inventoryService;
            _progressionService = progressionService;
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
                        switch (item.Rarity)
                        {
                            case ItemRarity.Common:   _commonEquipment.Add(item); break;
                            case ItemRarity.Uncommon: _uncommonEquipment.Add(item); break;
                            case ItemRarity.Rare:     _rareEquipment.Add(item); break;
                        }
                        break;

                    case ItemKind.Consumable:
                        _consumables.Add(item);
                        break;
                }
            }

            IsInitiated = true;
            return UniTask.CompletedTask;
        }

        public LootResult GetEnemyReward(bool isBoss)
        {
            if (!IsInitiated) return LootResult.None();

            if (isBoss)
            {
                if (TryRollBossEquipment(out var bossItem))
                    return LootResult.Item(bossItem.Type, LootType.Equipment);

                return LootResult.None();
            }

            if (Random.value > EnemyEquipmentChance)
                return LootResult.None();

            if (TryRollCommonEquipment(out var commonItem))
                return LootResult.Item(commonItem.Type, LootType.Equipment);

            return LootResult.None();
        }

        public LootResult GetChestReward()
        {
            if (!IsInitiated) return LootResult.None();

            float roll = Random.value;

            if (roll < ConsumableChance && TryGetRandomConsumable(out var consumable))
                return LootResult.Item(consumable.Type, LootType.Consumable);

            return RollGold();
        }

        public void ReleaseReservation(ItemType type, LevelDifficulty difficulty)
        {
            _reserved.Remove((type, difficulty));
        }

        public void ClearReservations() => _reserved.Clear();

        private bool TryRollCommonEquipment(out ItemData result)
        {
            result = null;

            float roll = Random.value;
            ItemRarity target = roll < PrimaryWeight ? ItemRarity.Common : ItemRarity.Uncommon;

            if (TryRollFrom(GetPool(target), out result)) return true;

            ItemRarity fallback = target == ItemRarity.Common
                ? ItemRarity.Uncommon
                : ItemRarity.Common;

            return TryRollFrom(GetPool(fallback), out result);
        }

        private bool TryRollBossEquipment(out ItemData result)
        {
            result = null;

            float roll = Random.value;
            ItemRarity target = roll < PrimaryWeight ? ItemRarity.Uncommon : ItemRarity.Rare;

            if (TryRollFrom(GetPool(target), out result)) return true;

            ItemRarity fallback = target == ItemRarity.Uncommon
                ? ItemRarity.Rare
                : ItemRarity.Uncommon;

            return TryRollFrom(GetPool(fallback), out result);
        }

        private List<ItemData> GetPool(ItemRarity rarity) => rarity switch
        {
            ItemRarity.Common   => _commonEquipment,
            ItemRarity.Uncommon => _uncommonEquipment,
            ItemRarity.Rare     => _rareEquipment,
            _ => null,
        };

        private bool TryRollFrom(List<ItemData> pool, out ItemData result)
        {
            result = null;
            if (pool == null || pool.Count == 0) return false;

            var difficulty = _progressionService.CurrentDifficulty;

            var available = pool
                .Where(e => !_inventoryService.HasEquipment(e.Type, difficulty))
                .Where(e => !_reserved.Contains((e.Type, difficulty)))
                .ToList();

            if (available.Count == 0) return false;

            int idx = Random.Range(0, available.Count);
            result = available[idx];
            
            _reserved.Add((result.Type, difficulty));
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
            if (Random.value < HighGoldChance)
                return LootResult.Gold(HighGold, true);

            return LootResult.Gold(LowGold, false);
        }
    }
}