using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Items;
using YG;

namespace _Project.Scripts.Services
{
    public class InventoryService : IInventoryService
    {
        private Dictionary<ItemType, int> _items = new ();
        private ItemType _equippedItemType;

        public bool IsInitiated { get; private set; }

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
                return false;

            _equippedItemType = itemType;
            Save();
            return true;
        }

        // Снять предмет с ячейки
        // public void UnequipItem()
        // {
        //     _equippedItemType = null;
        //     Save();
        // }

        // public string GetEquippedItemId()
        // {
        //     return _equippedItemType;
        // }

        // public ItemData GetEquippedItemData()
        // {
        //     if (string.IsNullOrEmpty(_equippedItemType))
        //         return null;
        //     // Получить ItemData из базы данных (через IDataBaseService)
        //     return _dataBaseService.Content.Items.FirstOrDefault(i => i.Id == _equippedItemType);
        // }

        private void Save()
        {
            YG2.saves.InventoryItems = _items;
            YG2.saves.EquippedItemType = _equippedItemType;
            YG2.SaveProgress();
        }
    }
}