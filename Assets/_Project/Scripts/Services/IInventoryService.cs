using System;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Items;

namespace _Project.Scripts.Services
{
    public interface IInventoryService : IService
    {
        public void AddItem(ItemType itemType, int amount = 1);
        public void EquipConsumableItem(ItemType itemType);
        public void EquipItem(ItemType itemType);
        public void RemoveItem(ItemType itemType, int amount = 1);
        public bool HasItem(ItemType itemType);
        public int GetItemCount(ItemType itemType);
        public event Action<ItemType, int> OnEquippedConsumableItem;
        public event Action OnUnEquippedConsumableItem;
        public event Action OnEquippedItem;
        public event Action OnUnEquippedItem;
    }
}