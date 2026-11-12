using _Project.Scripts.Player.Input;
using _Project.Scripts.Services;
using Cinemachine;
using Reflex.Attributes;
using UnityEngine;
using YG;

namespace _Project.Scripts.Camera
{
    public class CameraDragRotate : MonoBehaviour
    {
        private const string MouseInverted = nameof(MouseInverted);
        private const string CameraSensitivity = nameof(CameraSensitivity);
        
        private const float MinValue = 0f;
        private const float Sign = 1f;
        private const float DefaultSensitivity = 1.5f;
        private const float RotationClamp = 10f;
        private const float MouseMultiplier = 0.3f; 
        private const float JoystickMultiplier = 100f; 

        private IPlayerService _playerService;

        private bool _isConfigured;
        
        private bool _isInverted;
        private float _sensitivity = DefaultSensitivity;
        private bool _isMobile;
        
        [Inject]
        public void Construct(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        private void Start()
        {
            LoadSettings();
            
            _isMobile = YG2.envir.isMobile; 
        }

        private void LateUpdate()
        {
            if (!TryGetDependencies(out var cam, out var input)) return;
            
            if (Time.timeScale == MinValue) return;

            if (!_isConfigured)
            {
                ConfigureCamera(cam);
                _isConfigured = true;
            }

            if (input.IsCameraRotating)
            {
                var dir = input.CameraLookDirection;
                float sign = _isInverted ? -Sign : Sign;
                
                float rotationDelta = 0f;
                
                if (_isMobile)
                {
                    rotationDelta = dir.x * JoystickMultiplier * _sensitivity * sign * Time.deltaTime;
                }
                else
                {
                    rotationDelta = dir.x * MouseMultiplier * _sensitivity * sign;
                    rotationDelta = Mathf.Clamp(rotationDelta, -RotationClamp, RotationClamp);
                }
                
                cam.m_XAxis.Value += rotationDelta;
            }
            else
            {
                cam.m_XAxis.m_InputAxisValue = MinValue;
            }

            cam.m_YAxis.m_InputAxisValue = MinValue;
        }

        public void ChangeMouseSensitivity(float value)
        {
            _sensitivity = value;
        }

        public void ChangeMouseInversion(bool isOn)
        {
            _isInverted = isOn;
        }
        
        private void LoadSettings()
        {
            _isInverted = PlayerPrefs.GetInt(MouseInverted, (int)MinValue) == (int)Sign;
            _sensitivity = PlayerPrefs.GetFloat(CameraSensitivity, DefaultSensitivity);
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
            
            cam.m_YAxis.m_MaxSpeed = MinValue;
            cam.m_XAxis.m_MaxSpeed = MinValue;
            
            cam.m_XAxis.m_InputAxisValue = MinValue;
            cam.m_YAxis.m_InputAxisValue = MinValue;
        }
    }
}