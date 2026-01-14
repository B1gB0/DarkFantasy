using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace Project.Scripts.Services
{
    public class TweenAnimationService : ITweenAnimationService
    {
        public bool IsInitiated { get; private set; }

        public UniTask Init()
        {
            if (IsInitiated)
                return UniTask.CompletedTask;

            DOTween.Init(recycleAllByDefault: true, useSafeMode: true, logBehaviour: LogBehaviour.Default);

            IsInitiated = true;

            return UniTask.CompletedTask;
        }
    }
}