using System;
using System.Collections.Generic;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Items;
using _Project.Scripts.Level;

namespace _Project.Scripts.Services
{
    public interface IInventoryService : IService
    {
        public void ShowCurrentEquippedConsumableItem();
        public void AddItem(ItemType itemType, int amount = 1);
        public void AddEquipment(Item instance);
        public void EquipConsumableItem(ItemData data);
        public IReadOnlyList<Item> GetEquippedItems();
        public void RemoveItem(ItemType itemType, int amount = 1);
        public void RemoveEquipment(string id);
        public void RemoveAllOfType(ItemType itemType);
        public bool HasItem(ItemType itemType);
        public bool CanFit(ItemData itemData);
        public bool HasEquipment(ItemType type, LevelDifficulty difficulty);
        public int GetItemCount(ItemType itemType);
        public void EquipItem(string instanceId);
        public bool IsFull();
        public event Action<ItemType, int> OnEquippedConsumableItem;
        public event Action OnUnEquippedConsumableItem;
        public event Action OnEquippedItem;
        public event Action OnUnEquippedItem;
        public IReadOnlyList<Item> Equipment { get; }
        public Item EquippedWeapon { get; }
        public Item EquippedArmor { get; }
        public Item EquippedRing { get; }
    }
}