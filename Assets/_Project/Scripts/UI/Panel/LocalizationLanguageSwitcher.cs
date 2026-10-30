using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace _Project.Scripts.UI.Panel
{
    public class LocalizationLanguageSwitcher : MonoBehaviour
    {
        private const string LanguagePrefsKey = "SelectedLanguage";
        
        private const int MinIndex = 0;
        private const int IndexFactor = 1;
        
        private readonly List<string> _languages = new () { "en", "ru", "tr" };

        [SerializeField] private Button _priviousButton;
        [SerializeField] private Button _nextButton;

        private int _currentIndex;

        public event Action OnLanguageChanged;

        private void OnEnable()
        {
            _priviousButton.onClick.AddListener(SetPreviousLanguage);
            _nextButton.onClick.AddListener(SetNextLanguage);
        }
        
        private void Awake()
        {
            string savedLang = PlayerPrefs.GetString(LanguagePrefsKey, string.Empty);

            YG2.SwitchLanguage(!string.IsNullOrEmpty(savedLang) ? savedLang : YG2.lang);

            string currentLang = YG2.lang.ToLower();
            _currentIndex = _languages.IndexOf(currentLang);
            
            if (_currentIndex < MinIndex) 
            {
                _currentIndex = MinIndex;
                YG2.SwitchLanguage(_languages[_currentIndex]);
            }
            
            OnLanguageChanged?.Invoke();
        }

        private void OnDisable()
        {
            _priviousButton.onClick.RemoveListener(SetPreviousLanguage);
            _nextButton.onClick.RemoveListener(SetNextLanguage);
        }

        private void SetNextLanguage()
        {
            if (_currentIndex == _languages.Count - IndexFactor)
                _currentIndex = MinIndex;
            else
                _currentIndex++;

            SetLanguage(_currentIndex);
        }

        private void SetPreviousLanguage()
        {
            if (_currentIndex == MinIndex)
                _currentIndex = _languages.Count - IndexFactor;
            else
                _currentIndex--;

            SetLanguage(_currentIndex);
        }

        private void SetLanguage(int index)
        {
            string langCode = _languages[index];
            
            YG2.SwitchLanguage(langCode);
            
            PlayerPrefs.SetString(LanguagePrefsKey, langCode);
            PlayerPrefs.Save();
            
            OnLanguageChanged?.Invoke();
        }
    }
}