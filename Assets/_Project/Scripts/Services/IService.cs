using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Services
{
    public interface IService
    {
        public bool IsInitiated { get; }

        public UniTask Init()
        {
            return UniTask.CompletedTask;
        }
    }
}