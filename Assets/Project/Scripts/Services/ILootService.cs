using Project.Scripts.Items;
using Project.Scripts.Level;

namespace Project.Scripts.Services
{
    public interface ILootService : IService
    {
        public LootResult GetEnemyReward(bool isBoss);

        public LootResult GetChestReward();

        public void ReleaseReservation(ItemType type, LevelDifficulty difficulty);

        public void ClearReservations();
    }
}