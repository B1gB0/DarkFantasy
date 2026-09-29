using System.Threading;
using _Project.Scripts.Level.Triggers;
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
        [SerializeField] private LootTrigger _lootTrigger;
        
        private ILootService _lootService;
        private IPauseService _pauseService;

        [Inject]
        private void Construct(ILootService lootService, IPauseService pauseService)
        {
            _lootService = lootService;
            _pauseService = pauseService;
        }
        
        private async UniTaskVoid OpenAsync(CancellationToken token)
        {
            _animator.CrossFade(Open, Duration);

            // Сначала дожидаемся, что аниматор ВООБЩЕ вошёл в состояние Open/Opened
            await UniTask.WaitUntil(() =>
            {
                var info = _animator.GetCurrentAnimatorStateInfo(0);
                return info.IsName("Open");
            }, cancellationToken: token);

            // Теперь ждём, пока анимация доиграет до конца
            await UniTask.WaitUntil(() =>
            {
                var info = _animator.GetCurrentAnimatorStateInfo(0);
                return info.normalizedTime >= 1f;
            }, cancellationToken: token);

            // var reward = _lootService.TryGetReward();
            // if (reward.Type != LootType.None)
            //     GiveReward(reward);
        }
    }
}