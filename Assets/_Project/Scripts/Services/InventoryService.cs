using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Items;
using _Project.Scripts.Level;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using YG;

namespace _Project.Scripts.Services
{
    public class InventoryService : IInventoryService
    {
        private const int MaxSlots = 16;

        private Dictionary<ItemType, int> _consumables = new();
        private List<Item> _equipment = new();

        private Item _equippedWeapon;
        private Item _equippedArmor;
        private Item _equippedRing;
        private ItemType _equippedConsumableType;

        private IShopService _shopService;

        public bool IsInitiated { get; private set; }
        public int MaxSlotsCount => MaxSlots;

        public IReadOnlyList<Item> Equipment => _equipment;
        public Item EquippedWeapon => _equippedWeapon;
        public Item EquippedArmor => _equippedArmor;
        public Item EquippedRing => _equippedRing;

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

            _consumables = YG2.saves.Consumables != null
                ? YG2.saves.Consumables.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
                : new Dictionary<ItemType, int>();
            YG2.saves.Consumables = _consumables;

            _equipment = YG2.saves.Equipment ?? new List<Item>();
            YG2.saves.Equipment = _equipment;

            _equippedWeapon = GetEquipment(YG2.saves.EquippedWeaponId);
            _equippedArmor  = GetEquipment(YG2.saves.EquippedArmorId);
            _equippedRing   = GetEquipment(YG2.saves.EquippedRingId);
            _equippedConsumableType = YG2.saves.EquippedItemType;

            IsInitiated = true;
            return UniTask.CompletedTask;
        }

        public void AddEquipment(Item instance)
        {
            if (instance == null) return;
            _equipment.Add(instance);
            Save();
        }
        
        public void RemoveEquipment(string id)
        {
            if (string.IsNullOrEmpty(id)) return;

            bool wasEquipped =
                _equippedWeapon?.Id == id ||
                _equippedArmor?.Id == id ||
                _equippedRing?.Id == id;

            _equipment.RemoveAll(i => i.Id == id);

            if (_equippedWeapon?.Id == id) _equippedWeapon = null;
            if (_equippedArmor?.Id  == id) _equippedArmor  = null;
            if (_equippedRing?.Id   == id) _equippedRing   = null;

            Save();

            if (wasEquipped)
                OnUnEquippedItem?.Invoke();
        }

        public bool HasEquipment(ItemType type, LevelDifficulty difficulty)
        {
            foreach (var item in _equipment)
                if (item.Type == type && item.Difficulty == difficulty)
                    return true;

            return false;
        }

        public Item GetEquipment(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;

            foreach (var item in _equipment)
                if (item.Id == id) return item;

            return null;
        }

        public IReadOnlyList<Item> GetEquippedItems()
        {
            var result = new List<Item>(3);
            if (_equippedWeapon != null) result.Add(_equippedWeapon);
            if (_equippedArmor  != null) result.Add(_equippedArmor);
            if (_equippedRing   != null) result.Add(_equippedRing);
            return result;
        }

        public void EquipItem(string instanceId)
        {
            var instance = GetEquipment(instanceId);
            if (instance == null)
            {
                UnequipItemByInstanceId(instanceId);
                return;
            }

            var data = _shopService.GetItemDataByType(instance.Type);
            if (data == null || data.Kind != ItemKind.Equipment) return;

            switch (data.Slot)
            {
                case EquipmentType.Weapon: _equippedWeapon = instance; break;
                case EquipmentType.Armor:  _equippedArmor  = instance; break;
                case EquipmentType.Ring:   _equippedRing   = instance; break;
            }

            OnEquippedItem?.Invoke();
            Save();
        }

        public void UnequipItemByInstanceId(string instanceId)
        {
            if (_equippedWeapon?.Id == instanceId) _equippedWeapon = null;
            if (_equippedArmor?.Id  == instanceId) _equippedArmor  = null;
            if (_equippedRing?.Id   == instanceId) _equippedRing   = null;

            OnUnEquippedItem?.Invoke();
            Save();
        }

        public void AddItem(ItemType itemType, int amount = 1)
        {
            _consumables.TryAdd(itemType, 0);
            _consumables[itemType] += amount;
            Save();
        }
        
        public void RemoveItem(ItemType itemType, int amount = 1)
        {
            if (amount <= 0) return;
            if (!_consumables.TryGetValue(itemType, out var current)) return;

            int newCount = current - amount;

            if (newCount > 0)
                _consumables[itemType] = newCount;
            else
            {
                _consumables.Remove(itemType);

                if (_equippedConsumableType == itemType)
                    UnequipConsumableItem();
            }

            Save();
        }
        
        public void RemoveAllOfType(ItemType itemType)
        {
            if (!_consumables.Remove(itemType)) return;

            if (_equippedConsumableType == itemType)
                UnequipConsumableItem();

            Save();
        }

        public int GetItemCount(ItemType itemType)
            => _consumables.GetValueOrDefault(itemType, 0);

        public bool HasItem(ItemType itemType)
            => _consumables.TryGetValue(itemType, out var count) && count > 0;

        public void EquipConsumableItem(ItemData data)
        {
            if (data == null) return;
            if (data.Kind == ItemKind.Equipment) return;

            if (!HasItem(data.Type))
            {
                UnequipConsumableItem();
                return;
            }

            _equippedConsumableType = data.Type;
            ShowCurrentEquippedConsumableItem();
            Save();
        }

        public void ShowCurrentEquippedConsumableItem()
        {
            if (_equippedConsumableType == ItemType.None) return;

            var data = _shopService.GetItemDataByType(_equippedConsumableType);
            if (data == null)
            {
                UnequipConsumableItem();
                return;
            }

            int count = GetItemCount(_equippedConsumableType);
            if (count <= 0)
            {
                UnequipConsumableItem();
                return;
            }

            OnEquippedConsumableItem?.Invoke(_equippedConsumableType, count);
        }

        private void UnequipConsumableItem()
        {
            _equippedConsumableType = ItemType.None;
            OnUnEquippedConsumableItem?.Invoke();
            Save();
        }

        public bool IsFull()
            => GetTotalItemCount() >= MaxSlots;
        
        public bool CanFit(ItemData itemData)
        {
            if (itemData == null) return false;
            
            if (itemData.Kind == ItemKind.Consumable && HasItem(itemData.Type))
                return true;

            return !IsFull();
        }

        public int GetTotalItemCount()
        {
            int count = _equipment.Count;

            foreach (var kvp in _consumables)
                if (kvp.Value > 0) count++;

            return count;
        }

        private void Save()
        {
            YG2.saves.Consumables = _consumables;
            YG2.saves.Equipment = _equipment;
            YG2.saves.EquippedWeaponId = _equippedWeapon?.Id;
            YG2.saves.EquippedArmorId  = _equippedArmor?.Id;
            YG2.saves.EquippedRingId   = _equippedRing?.Id;
            YG2.saves.EquippedItemType = _equippedConsumableType;
            YG2.SaveProgress();
        }
    }
}