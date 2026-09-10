using _Project.Scripts.Player.Input;
using _Project.Scripts.Services;
using Cinemachine;
using Reflex.Attributes;
using UnityEngine;

namespace _Project.Scripts.Camera
{
    public class CameraDragRotate : MonoBehaviour
    {
        private const float MinValue = 0f;

        private IPlayerService _playerService;

        private bool _isConfigured;
        
        [Inject]
        public void Construct(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        private void LateUpdate()
        {
            if (!TryGetDependencies(out var cam, out var input)) return;

            if (!_isConfigured)
            {
                ConfigureCamera(cam);
                _isConfigured = true;
            }

            if (input.IsCameraRotating)
            {
                var dir = input.CameraLookDirection;
                cam.m_XAxis.m_InputAxisValue = dir.x;
                cam.m_YAxis.m_InputAxisValue = -dir.y;
            }
            else
            {
                cam.m_XAxis.m_InputAxisValue = MinValue;
                cam.m_YAxis.m_InputAxisValue = MinValue;
            }
        }

        private bool TryGetDependencies(out CinemachineFreeLook cam, out InputController input)
        {
            cam = null;
            input = null;

            if (_playerService == null) return false;
            if (_playerService.FreeLookCamera == null) return false;
            if (_playerService.Player == null) return false;
            if (_playerService.Player.InputController == null) return false;

            cam = _playerService.FreeLookCamera;
            input = _playerService.Player.InputController;
            return true;
        }

        private void ConfigureCamera(CinemachineFreeLook cam)
        {
            cam.m_XAxis.m_InputAxisName = string.Empty;
            cam.m_YAxis.m_InputAxisName = string.Empty;
            cam.m_XAxis.m_InputAxisValue = MinValue;
            cam.m_YAxis.m_InputAxisValue = MinValue;
        }
    }
}