using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Services
{
    public interface IResourceService
    {
        UniTask<T> Load<T>(string assetName);
    }
}