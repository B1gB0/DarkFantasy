using _Project.Scripts.UI.View;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using UnityEngine;

namespace _Project.Scripts.Services
{
    public class NavMeshWaypointService : MonoBehaviour
    {
        [SerializeField] private NavMeshWaypoint _waypoint;

        private IPlayerService _playerService;

        [Inject]
        private void Construct(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        public UniTask Init()
        {
            _waypoint = Instantiate(_waypoint);
            _waypoint.GetPlayer(_playerService.Player);

            return UniTask.CompletedTask;
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