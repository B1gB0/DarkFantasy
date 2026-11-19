using Project.Scripts.Services;
using Reflex.Attributes;
using UnityEngine;

namespace Project.Scripts.Camera
{
    public class CameraTargetFollower : MonoBehaviour
    {
        private IPlayerService _playerService;

        [Inject]
        private void Construct(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        private void LateUpdate()
        {
            if (_playerService.Player == null) return;

            transform.position = _playerService.Player.transform.position;
            transform.rotation = Quaternion.identity;
        }
    }
}