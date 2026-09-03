using _Project.Scripts.UI.View;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using UnityEngine;

namespace _Project.Scripts.Services
{
    public class NavMeshWaypointService : MonoBehaviour, IService
    {
        private const string NavMeshWaypointPath = "WaypointLine";

        private IPlayerService _playerService;
        private IResourceService _resourceService;

        private NavMeshWaypoint _waypoint;

        public bool IsInitiated { get; private set; }

        [Inject]
        private void Construct(IPlayerService playerService, IResourceService resourceService)
        {
            _playerService = playerService;
            _resourceService = resourceService;
        }

        public async UniTask Init()
        {
            if(_waypoint != null)
                _waypoint.GetPlayer(_playerService.Player);
            
            if (IsInitiated) return;

            var waypointTemplate = await _resourceService.Load<GameObject>(NavMeshWaypointPath);
            waypointTemplate = Instantiate(waypointTemplate);
            
            _waypoint = waypointTemplate.GetComponent<NavMeshWaypoint>();
            _waypoint.GetPlayer(_playerService.Player);

            DontDestroyOnLoad(_waypoint.gameObject);

            IsInitiated = true;
        }

        public void ShowWaypoint(Transform target)
        {
            _waypoint.SetTarget(target);
            _waypoint.SetActive(true);
        }

        public void HideWaypoint()
        {
            _waypoint.SetActive(false);
        }
    }
}