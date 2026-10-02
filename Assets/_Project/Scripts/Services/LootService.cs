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
        
        private readonly HashSet<ItemType> _reservedItems = new();

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
                        switch (item.Rarity)
                        {
                            case ItemRarity.Common:   _commonEquipment.Add(item);
                                break;
                            case ItemRarity.Uncommon: _uncommonEquipment.Add(item);
                                break;
                            case ItemRarity.Rare:     _rareEquipment.Add(item);
                                break;
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

            if (!(roll < ConsumableChance) || !TryGetRandomConsumable(out var consumable)) return RollGold();

            _inventoryService.AddItem(consumable.Type);
            return LootResult.Item(consumable.Type, LootType.Consumable);
        }
        
        public void ReleaseReservation(ItemType type)
        {
            if (type == ItemType.None) return;
            _reservedItems.Remove(type);
        }
        
        public void ClearReservations()
        {
            _reservedItems.Clear();
        }
        
        private bool IsItemAvailable(ItemType type)
        {
            return !_inventoryService.HasItem(type) && !_reservedItems.Contains(type);
        }
        
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
            
            var available = pool.Where(e => IsItemAvailable(e.Type)).ToList();
            if (available.Count == 0) return false;

            int idx = Random.Range(0, available.Count);
            result = available[idx];
            
            _reservedItems.Add(result.Type);
            
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