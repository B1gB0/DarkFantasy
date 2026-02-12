using _Project.Scripts.Services;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using UnityEngine;

namespace _Project.Scripts.DataBase.InitDataSO
{
    public class DataFactory : MonoBehaviour
    {
        private const string PlayerInitData = "PlayerInitData";
        private const string SkeletonInitData = "SkeletonInitData";

        private IResourceService _resourceService;

        [Inject]
        private void Construct(IResourceService resourceService)
        {
            _resourceService = resourceService;
        }

        public async UniTask<SkeletonInitData> CreateSkeletonInitData()
        {
            var skeletonData = await _resourceService.Load<SkeletonInitData>(SkeletonInitData);
            return skeletonData;
        }
        
        public async UniTask<PlayerInitData> CreatePlayerInitData()
        {
            var playerData = await _resourceService.Load<PlayerInitData>(PlayerInitData);
            return playerData;
        }
    }
}