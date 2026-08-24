using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Items;
using Reflex.Attributes;
using YG;

namespace _Project.Scripts.Services
{
    public class InventoryService : IInventoryService
    {
        private Dictionary<ItemType, int> _items = new();

        private ItemType _equippedItemType;
        private IShopService _shopService;

        public bool IsInitiated { get; private set; }

        public event Action<ItemData, int> OnEquippedItem;
        public event Action OnUnEquippedItem;

        [Inject]
        private void Construct(IShopService shopService)
        {
            _shopService = shopService;
        }

        public void Init()
        {
            if (IsInitiated) return;

            if (YG2.saves.InventoryItems != null)
            {
                _items = YG2.saves.InventoryItems.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }
            else
            {
                _items = new Dictionary<ItemType, int>();
                YG2.saves.InventoryItems = _items;
            }

            _equippedItemType = YG2.saves.EquippedItemType;

            IsInitiated = true;
        }

        public void AddItem(ItemType itemType, int amount = 1)
        {
            _items.TryAdd(itemType, 0);
            _items[itemType] += amount;

            Save();
        }

        public bool RemoveItem(ItemType itemType, int amount = 1)
        {
            if (!_items.ContainsKey(itemType) || _items[itemType] < amount)
                return false;

            _items[itemType] -= amount;
            if (_items[itemType] <= 0)
                _items.Remove(itemType);

            Save();
            return true;
        }

        public int GetItemCount(ItemType itemType)
        {
            return _items.GetValueOrDefault(itemType, 0);
        }

        public Dictionary<ItemType, int> GetAllItems()
        {
            return new Dictionary<ItemType, int>(_items);
        }

        public bool HasItem(ItemType itemType)
        {
            return _items.ContainsKey(itemType) && _items[itemType] > 0;
        }

        public bool EquipItem(ItemType itemType)
        {
            if (!HasItem(itemType))
            {
                UnequipItem();
                return false;
            }

            _equippedItemType = itemType;
            
            ItemData data = GetEquippedItemData();
            int count = GetItemCount(itemType);
            OnEquippedItem?.Invoke(data, count);

            Save();
            return true;
        }

        public void UnequipItem()
        {
            _equippedItemType = ItemType.None;
            OnUnEquippedItem?.Invoke();
            Save();
        }

        public ItemType GetEquippedItemType()
        {
            return _equippedItemType;
        }

        public ItemData GetEquippedItemData()
        {
            return _shopService.GetItemDataByType(_equippedItemType);
        }

        private void Save()
        {
            YG2.saves.InventoryItems = _items;
            YG2.saves.EquippedItemType = _equippedItemType;
            YG2.SaveProgress();
        }
    }
}