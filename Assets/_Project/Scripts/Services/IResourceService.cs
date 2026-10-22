using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Services
{
    public interface IResourceService
    {
        public UniTask<T> LoadAsset<T>(string assetName);
        public UniTask<T> InstantiateAsync<T>(string assetName, Transform parent = null) where T : Component;
    }
}