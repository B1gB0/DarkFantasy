using System;
using System.Collections.Generic;
using _Project.Scripts.Services;
using DG.Tweening;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Panel
{
    public class SettingsPanel : View.View
    {
        private const string MusicVolume = nameof(MusicVolume);
        private const string EffectsVolume = nameof(EffectsVolume);
        private const string CameraSensitivity = nameof(CameraSensitivity);
        private const string MouseInverted = nameof(MouseInverted);

        private const float StartVolumeSlider = 0.8f;
        private const float StartSensitivitySlider = 1.5f;
        
        [SerializeField] private List<GameObject> _elements;

        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _backToSceneButton;

        [SerializeField] private AudioMixerGroup _mixer;

        [SerializeField] private float _minVolume = -80f;
        [SerializeField] private float _maxVolume;

        [SerializeField] private Slider _musicVolumeSlider;
        [SerializeField] private Slider _effectsVolumeSlider;
        [SerializeField] private Slider _cameraSensitivitySlider;
        [SerializeField] private Toggle _isMouseInverted;

        private ITweenAnimationService _tweenAnimationService;
        private IPlayerService _playerService;

        public event Action OnBackToSceneButtonPressed;

        [Inject]
        private void Construct(ITweenAnimationService tweenAnimationService, IPlayerService playerService)
        {
            _tweenAnimationService = tweenAnimationService;
            _playerService = playerService;
        }

        private void OnEnable()
        {
            _backToSceneButton.onClick.AddListener(MoveBackToScene);
            _settingsButton.gameObject.SetActive(false);

            _musicVolumeSlider.onValueChanged.AddListener(ChangeMusicVolume);
            _effectsVolumeSlider.onValueChanged.AddListener(ChangeEffectsVolume);
            _isMouseInverted.onValueChanged.AddListener(ChangeMouseInverted);
            _cameraSensitivitySlider.onValueChanged.AddListener(ChangeCameraSensitivity);
            
            Application.focusChanged += OnFocusChanged;
        }

        private void Start()
        {
            LoadValuesVolume();
            LoadSensitivityMouseVolume();
            LoadToggleMouseInverted();
            
            foreach (var element in _elements)
                element.SetActive(true);
            
            Deactivate();
        }

        private void OnDisable()
        {
            _backToSceneButton.onClick.RemoveListener(MoveBackToScene);
            _settingsButton.gameObject.SetActive(true);

            _musicVolumeSlider.onValueChanged.RemoveListener(ChangeMusicVolume);
            _effectsVolumeSlider.onValueChanged.RemoveListener(ChangeEffectsVolume);
            _isMouseInverted.onValueChanged.RemoveListener(ChangeMouseInverted);
            _cameraSensitivitySlider.onValueChanged.RemoveListener(ChangeCameraSensitivity);
            
            Application.focusChanged -= OnFocusChanged;
        }

        private void OnDestroy()
        {
            transform.DOKill();
        }

        public override void Show()
        {
            _tweenAnimationService.AnimateScale(transform);
        }

        public override void Hide()
        {
            _tweenAnimationService.AnimateScale(transform, true);

            if (_playerService.Player != null)
                _playerService.Player.InputController.UnlockPlayerMovement();
        }
        
        private void OnFocusChanged(bool hasFocus)
        {
            if (hasFocus)
            {
                ChangeMusicVolume(PlayerPrefs.GetFloat(MusicVolume, StartVolumeSlider));
                ChangeEffectsVolume(PlayerPrefs.GetFloat(EffectsVolume, StartVolumeSlider));
            }
        }

        private void LoadValuesVolume()
        {
            _musicVolumeSlider.value = PlayerPrefs.GetFloat(MusicVolume, StartVolumeSlider);
            _effectsVolumeSlider.value = PlayerPrefs.GetFloat(EffectsVolume, StartVolumeSlider);
        }

        private void LoadSensitivityMouseVolume()
        {
            _cameraSensitivitySlider.value = PlayerPrefs.GetFloat(CameraSensitivity, StartSensitivitySlider);
        }

        private void LoadToggleMouseInverted()
        {
            _isMouseInverted.isOn = PlayerPrefs.GetInt(MouseInverted, 0) == 1;
        }

        private void MoveBackToScene()
        {
            Hide();
            OnBackToSceneButtonPressed?.Invoke();
        }

        private void ChangeMusicVolume(float volume)
        {
            _mixer.audioMixer.SetFloat(MusicVolume, Mathf.Lerp(_minVolume, _maxVolume, volume));

            PlayerPrefs.SetFloat(MusicVolume, volume);
            PlayerPrefs.Save();
        }

        private void ChangeEffectsVolume(float volume)
        {
            _mixer.audioMixer.SetFloat(EffectsVolume, Mathf.Lerp(_minVolume, _maxVolume, volume));

            PlayerPrefs.SetFloat(EffectsVolume, volume);
            PlayerPrefs.Save();
        }
        
        private void ChangeCameraSensitivity(float sensitivity)
        {
            PlayerPrefs.SetFloat(CameraSensitivity, sensitivity);
            PlayerPrefs.Save();
        }

        private void ChangeMouseInverted(bool isOn)
        {
            PlayerPrefs.SetInt(MouseInverted, isOn ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}