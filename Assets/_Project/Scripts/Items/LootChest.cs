using System;
using System.Threading;
using _Project.Scripts.Services;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using UnityEngine;

namespace _Project.Scripts.Items
{
    public class LootChest : MonoBehaviour
    {
        private const float Duration = 0.1f;
        
        private readonly int Open = Animator.StringToHash(nameof(Open));

        [SerializeField] private Animator _animator;

        private ILootService _lootService;
        private IInventoryService _inventoryService;
        private ICurrencyService _currencyService;
        private IShopService _shopService;

        private CancellationTokenSource _cts;

        public event Action OnInventoryIsFull;

        [Inject]
        private void Construct(
            ILootService lootService,
            IInventoryService inventoryService,
            ICurrencyService currencyService,
            IShopService shopService)
        {
            _lootService = lootService;
            _inventoryService = inventoryService;
            _currencyService = currencyService;
            _shopService = shopService;
        }

        public event Action<LootResult> OnChestOpened;

        private void Awake()
        {
            _cts = new CancellationTokenSource();
        }

        private void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }

        public void OpenInstantly()
        {
            if (_cts == null || _cts.IsCancellationRequested) return;

            var reward = _lootService.GetChestReward();

            if (!CanApplyReward(reward))
            {
                OnInventoryIsFull?.Invoke();
                return;
            }

            OpenAsync(reward, _cts.Token).Forget();
        }

        private bool CanApplyReward(LootResult reward)
        {
            switch (reward.Type)
            {
                case LootType.Gold:
                    return true;
                case LootType.Consumable:
                case LootType.Equipment:
                    var data = _shopService.GetItemDataByType(reward.ItemType);
                    return data != null && _inventoryService.CanFit(data);
                default:
                    return false;
            }
        }

        private async UniTaskVoid OpenAsync(LootResult reward, CancellationToken token)
        {
            _animator.CrossFade(Open, Duration);

            await UniTask.WaitUntil(() =>
            {
                var info = _animator.GetCurrentAnimatorStateInfo(0);
                return info.IsName("Open");
            }, cancellationToken: token);

            await UniTask.WaitUntil(() =>
            {
                var info = _animator.GetCurrentAnimatorStateInfo(0);
                return info.normalizedTime >= 1f;
            }, cancellationToken: token);

            ApplyReward(reward);

            OnChestOpened?.Invoke(reward);
        }

        private void ApplyReward(LootResult reward)
        {
            switch (reward.Type)
            {
                case LootType.Gold:
                    _currencyService.AddGold(reward.GoldValue);
                    break;
                case LootType.Consumable:
                    _inventoryService.AddItem(reward.ItemType);
                    break;
            }
        }
    }
}