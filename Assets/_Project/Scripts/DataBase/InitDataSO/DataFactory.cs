using _Project.Scripts.Services;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using UnityEngine;

namespace _Project.Scripts.DataBase.InitDataSO
{
    public class DataFactory : MonoBehaviour
    {
        private const string PlayerInitData = "PlayerInitData";
        private const string EnemyInitData = "EnemyInitData";

        private IResourceService _resourceService;

        [Inject]
        private void Construct(IResourceService resourceService)
        {
            _resourceService = resourceService;
        }

        public async UniTask<EnemyInitData> CreateSkeletonInitData()
        {
            var skeletonData = await _resourceService.Load<EnemyInitData>(EnemyInitData);
            return skeletonData;
        }
        
        public async UniTask<PlayerInitData> CreatePlayerInitData()
        {
            var playerData = await _resourceService.Load<PlayerInitData>(PlayerInitData);
            return playerData;
        }
    }
}