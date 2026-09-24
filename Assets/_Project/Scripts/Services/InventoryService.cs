using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Items;
using Cysharp.Threading.Tasks;
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

        public event Action<ItemType, int> OnEquippedConsumableItem;
        public event Action OnUnEquippedConsumableItem;
        public event Action OnEquippedItem;
        public event Action OnUnEquippedItem;

        [Inject]
        private void Construct(IShopService shopService)
        {
            _shopService = shopService;
        }

        public UniTask Init()
        {
            if (IsInitiated) return UniTask.CompletedTask;

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

            return UniTask.CompletedTask;
        }

        public void AddItem(ItemType itemType, int amount = 1)
        {
            _items.TryAdd(itemType, 0);
            _items[itemType] += amount;

            Save();
        }

        public void ShowCurrentEquippedConsumableItem()
        {
            var data = _shopService.GetItemDataByType(_equippedItemType);
            int count = GetItemCount(data.Type);

            if (count < 0)
            {
                UnequipConsumableItem();
                return;
            }
            
            OnEquippedConsumableItem?.Invoke(_equippedItemType, count);
        }

        public void RemoveItem(ItemType itemType, int amount = 1)
        {
            if (!_items.ContainsKey(itemType) || _items[itemType] < amount)
                return;

            _items[itemType] -= amount;
            if (_items[itemType] <= 0)
                _items.Remove(itemType);

            Save();
        }

        public int GetItemCount(ItemType itemType)
        {
            return _items.GetValueOrDefault(itemType, 0);
        }

        public bool HasItem(ItemType itemType)
        {
            return _items.ContainsKey(itemType) && _items[itemType] > 0;
        }

        public void EquipConsumableItem(ItemData data)
        {
            if (data.Kind == ItemKind.Equipment)
                return;

            if (!HasItem(data.Type))
            {
                UnequipConsumableItem();
                return;
            }

            if (data.Kind == ItemKind.Equipment)
                return;

            _equippedItemType = data.Type;

            ShowCurrentEquippedConsumableItem();

            Save();
        }

        public void EquipItem(ItemType itemType)
        {
            if (!HasItem(itemType))
            {
                UnequipItem(itemType);
                return;
            }

            ItemData data = _shopService.GetItemDataByType(itemType);

            if (data.Kind == ItemKind.Consumable)
                return;

            switch (data.Slot)
            {
                case EquipmentType.Weapon:
                    YG2.saves.EquipedWeaponType = itemType;
                    break;
                case EquipmentType.Armor:
                    YG2.saves.EquipedArmorType = itemType;
                    break;
                case EquipmentType.Ring:
                    YG2.saves.EquipedRingType = itemType;
                    break;
            }

            OnEquippedItem?.Invoke();
        }

        private void UnequipItem(ItemType itemType)
        {
            ItemData data = _shopService.GetItemDataByType(itemType);

            switch (data.Slot)
            {
                case EquipmentType.Weapon:
                    YG2.saves.EquipedWeaponType = ItemType.None;
                    break;
                case EquipmentType.Armor:
                    YG2.saves.EquipedArmorType = ItemType.None;
                    break;
                case EquipmentType.Ring:
                    YG2.saves.EquipedRingType = ItemType.None;
                    break;
            }

            OnUnEquippedItem?.Invoke();
        }

        private void UnequipConsumableItem()
        {
            _equippedItemType = ItemType.None;
            OnUnEquippedConsumableItem?.Invoke();
            Save();
        }

        private void Save()
        {
            YG2.saves.InventoryItems = _items;
            YG2.saves.EquippedItemType = _equippedItemType;
            YG2.SaveProgress();
        }
    }
}