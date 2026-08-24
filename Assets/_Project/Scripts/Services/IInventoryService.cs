using System;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Items;

namespace _Project.Scripts.Services
{
    public interface IInventoryService : IService
    {
        public void AddItem(ItemType itemType, int amount = 1);
        public bool EquipItem(ItemType itemType);
        public bool RemoveItem(ItemType itemType, int amount = 1);
        public bool HasItem(ItemType itemType);
        public int GetItemCount(ItemType itemType);
        public event Action<ItemData, int> OnEquippedItem;
        public event Action OnUnEquippedItem;
    }
}