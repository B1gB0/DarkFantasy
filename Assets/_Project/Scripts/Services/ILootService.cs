using _Project.Scripts.Items;

namespace _Project.Scripts.Services
{
    public interface ILootService : IService
    {
        public LootResult TryGetReward();
    }
}