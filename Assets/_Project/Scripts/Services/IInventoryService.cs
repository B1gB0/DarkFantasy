using _Project.Scripts.Items;

namespace _Project.Scripts.Services
{
    public interface IInventoryService : IService
    {
        public void AddItem(ItemType itemType, int amount = 1);
        public bool RemoveItem(ItemType itemType, int amount = 1);
        public bool HasItem(ItemType itemType);
    }
}