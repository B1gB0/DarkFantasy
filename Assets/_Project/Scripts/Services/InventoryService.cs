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

        private IShopService _shopService;
        public int MaxSlotsCount => MaxSlots;

        public bool IsInitiated { get; private set; }

        public IReadOnlyList<Item> Equipment => _equipment;
        public Item EquippedWeapon { get; private set; }

        public Item EquippedArmor { get; private set; }

        public Item EquippedRing { get; private set; }

        public ItemType EquippedConsumableType { get; private set; }

        public event Action<ItemType, int> OnEquippedConsumableItem;
        public event Action OnUnEquippedConsumableItem;
        public event Action OnEquippedItem;
        public event Action OnUnEquippedItem;

        public UniTask Init()
        {
            if (IsInitiated) return UniTask.CompletedTask;

            _consumables = YG2.saves.Consumables != null
                ? YG2.saves.Consumables.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
                : new Dictionary<ItemType, int>();
            YG2.saves.Consumables = _consumables;

            _equipment = YG2.saves.Equipment ?? new List<Item>();
            YG2.saves.Equipment = _equipment;

            EquippedWeapon = GetEquipment(YG2.saves.EquippedWeaponId);
            EquippedArmor = GetEquipment(YG2.saves.EquippedArmorId);
            EquippedRing = GetEquipment(YG2.saves.EquippedRingId);
            EquippedConsumableType = YG2.saves.EquippedItemType;

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

            var wasEquipped =
                EquippedWeapon?.Id == id ||
                EquippedArmor?.Id == id ||
                EquippedRing?.Id == id;

            _equipment.RemoveAll(i => i.Id == id);

            if (EquippedWeapon?.Id == id) EquippedWeapon = null;
            if (EquippedArmor?.Id == id) EquippedArmor = null;
            if (EquippedRing?.Id == id) EquippedRing = null;

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

        public IReadOnlyList<Item> GetEquippedItems()
        {
            var result = new List<Item>(3);
            if (EquippedWeapon != null) result.Add(EquippedWeapon);
            if (EquippedArmor != null) result.Add(EquippedArmor);
            if (EquippedRing != null) result.Add(EquippedRing);
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
                case EquipmentType.Weapon: EquippedWeapon = instance; break;
                case EquipmentType.Armor: EquippedArmor = instance; break;
                case EquipmentType.Ring: EquippedRing = instance; break;
            }

            OnEquippedItem?.Invoke();
            Save();
        }

        public void UnequipItemByInstanceId(string instanceId)
        {
            if (EquippedWeapon?.Id == instanceId) EquippedWeapon = null;
            if (EquippedArmor?.Id == instanceId) EquippedArmor = null;
            if (EquippedRing?.Id == instanceId) EquippedRing = null;

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

            var newCount = current - amount;

            if (newCount > 0)
            {
                _consumables[itemType] = newCount;
            }
            else
            {
                _consumables.Remove(itemType);

                if (EquippedConsumableType == itemType)
                    UnequipConsumableItem();
            }

            Save();
        }

        public void RemoveAllOfType(ItemType itemType)
        {
            if (!_consumables.Remove(itemType)) return;

            if (EquippedConsumableType == itemType)
                UnequipConsumableItem();

            Save();
        }

        public int GetItemCount(ItemType itemType)
        {
            return _consumables.GetValueOrDefault(itemType, 0);
        }

        public bool HasItem(ItemType itemType)
        {
            return _consumables.TryGetValue(itemType, out var count) && count > 0;
        }

        public void EquipConsumableItem(ItemData data)
        {
            if (data == null) return;
            if (data.Kind == ItemKind.Equipment) return;

            if (!HasItem(data.Type))
            {
                UnequipConsumableItem();
                return;
            }

            EquippedConsumableType = data.Type;
            ShowCurrentEquippedConsumableItem();
            Save();
        }

        public void ShowCurrentEquippedConsumableItem()
        {
            if (EquippedConsumableType == ItemType.None) return;

            var data = _shopService.GetItemDataByType(EquippedConsumableType);
            if (data == null)
            {
                UnequipConsumableItem();
                return;
            }

            var count = GetItemCount(EquippedConsumableType);
            if (count <= 0)
            {
                UnequipConsumableItem();
                return;
            }

            OnEquippedConsumableItem?.Invoke(EquippedConsumableType, count);
        }

        public void UnequipConsumableItem()
        {
            EquippedConsumableType = ItemType.None;
            OnUnEquippedConsumableItem?.Invoke();
            Save();
        }

        public bool IsFull()
        {
            return GetTotalItemCount() >= MaxSlots;
        }

        public bool CanFit(ItemData itemData)
        {
            if (itemData == null) return false;

            if (itemData.Kind == ItemKind.Consumable && HasItem(itemData.Type))
                return true;

            return !IsFull();
        }

        [Inject]
        private void Construct(IShopService shopService)
        {
            _shopService = shopService;
        }

        private int GetTotalItemCount()
        {
            var count = _equipment.Count;

            foreach (var kvp in _consumables)
                if (kvp.Value > 0)
                    count++;

            return count;
        }

        private Item GetEquipment(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;

            foreach (var item in _equipment)
                if (item.Id == id)
                    return item;

            return null;
        }

        private void Save()
        {
            YG2.saves.Consumables = _consumables;
            YG2.saves.Equipment = _equipment;
            YG2.saves.EquippedWeaponId = EquippedWeapon?.Id;
            YG2.saves.EquippedArmorId = EquippedArmor?.Id;
            YG2.saves.EquippedRingId = EquippedRing?.Id;
            YG2.saves.EquippedItemType = EquippedConsumableType;
            YG2.SaveProgress();
        }
    }
}