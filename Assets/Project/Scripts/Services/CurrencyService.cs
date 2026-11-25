using System;
using Cysharp.Threading.Tasks;
using YG;

namespace Project.Scripts.Services
{
    public class CurrencyService : ICurrencyService
    {
        private const int MinValue = 0;

        public event Action<int> OnGoldValueChanged;

        public int Gold { get; private set; }
        public int AccumulatedGold { get; private set; }
        public bool IsInitiated { get; private set; }

        public UniTask Init()
        {
            if (IsInitiated)
                return UniTask.CompletedTask;

            Gold = YG2.saves.Gold;
            OnGoldValueChanged?.Invoke(Gold);

            IsInitiated = true;

            return UniTask.CompletedTask;
        }

        public void AddGold(int gold)
        {
            Gold += gold;
            AccumulatedGold += gold;
            YG2.saves.Gold = Gold;
            OnGoldValueChanged?.Invoke(Gold);
        }

        public void SpendGold(int gold)
        {
            Gold -= gold;
            YG2.saves.Gold = Gold;
            OnGoldValueChanged?.Invoke(Gold);
        }

        public void ResetAccumulatedGold()
        {
            AccumulatedGold = MinValue;
        }

        public void SaveGold()
        {
            YG2.saves.Gold = Gold;
            YG2.SaveProgress();
        }
    }
}