using _Project.Scripts.Items;
using _Project.Scripts.Level;

namespace _Project.Scripts.Services
{
    public interface ILootService : IService
    {
        public LootResult GetEnemyReward(bool isBoss);
        public LootResult GetChestReward();
        public void ReleaseReservation(ItemType type, LevelDifficulty difficulty);
        public void ClearReservations();
    }
}