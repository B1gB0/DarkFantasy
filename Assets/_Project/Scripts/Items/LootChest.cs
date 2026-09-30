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
        private CancellationTokenSource _cts;

        [Inject]
        private void Construct(ILootService lootService)
        {
            _lootService = lootService;
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

            OpenAsync(_cts.Token).Forget();
        }

        private async UniTaskVoid OpenAsync(CancellationToken token)
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


            OnChestOpened?.Invoke(_lootService.GetReward());
        }
    }
}