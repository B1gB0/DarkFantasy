using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace _Project.Scripts.Services
{
    public class ResourceService : IResourceService
    {
        public async UniTask<T> LoadAsset<T>(string assetName)
        {
            var handle = Addressables.LoadAssetAsync<T>(assetName);
            await handle.Task;
            
            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"[Resource] Failed to load: {assetName}");
                return default;
            }
            
            return handle.Result;
        }

        public async UniTask<T> InstantiateAsync<T>(string assetName, Transform parent = null) where T : Component
        {
            var gameObject = await InstantiateAsync(assetName, parent);
            return gameObject != null ? gameObject.GetComponent<T>() : null;
        }
        
        private async UniTask<GameObject> InstantiateAsync(string assetName, Transform parent = null)
        {
            var handle = Addressables.InstantiateAsync(assetName, parent);
            await handle.Task;
            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"[Resource] Failed to instantiate: {assetName}");
                return null;
            }
            return handle.Result;
        }
    }
}