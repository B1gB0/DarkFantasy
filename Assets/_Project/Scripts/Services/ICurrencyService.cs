using System;

namespace _Project.Scripts.Services
{
    public interface ICurrencyService : IService
    {
        public event Action<int> OnGoldValueChanged;

        public int Gold { get; }
        public int AccumulatedGold { get; }

        public void AddGold(int gold);
        public void SpendGold(int gold);
        public void ResetAccumulatedGold();
        public void SaveGold();
    }
}