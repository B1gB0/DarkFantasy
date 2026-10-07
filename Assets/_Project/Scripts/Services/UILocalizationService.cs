using System.Collections.Generic;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Game.Constant;
using _Project.Scripts.UI;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using UnityEngine;
using YG;

namespace _Project.Scripts.Services
{
    public class UILocalizationService : IUILocalizationService
    {
        private readonly Dictionary<UITextType, UILocalizationData> _uiLocalizationData = new();

        private IDataBaseService _dataBaseService;

        public bool IsInitiated { get; private set; }

        [Inject]
        public void Construct(IDataBaseService dataBaseService)
        {
            _dataBaseService = dataBaseService;
        }

        public UniTask Init()
        {
            if (IsInitiated)
                return UniTask.CompletedTask;

            foreach (var data in _dataBaseService.Content.UILocalizationData)
            {
                _uiLocalizationData.TryAdd(data.UITextType, data);
            }

            IsInitiated = true;

            return UniTask.CompletedTask;
        }

        public UILocalizationData GetLevelTextData(UITextType type)
        {
            return _uiLocalizationData[type];
        }
        
        public string GetLocalizedText(UITextType type)
        {
            if (!_uiLocalizationData.TryGetValue(type, out var data))
            {
                Debug.LogWarning($"[UILocalization] Missing: {type}");
                return type.ToString();
            }

            return YG2.lang switch
            {
                LocalizationCode.Ru => data.NameRu,
                LocalizationCode.En => data.NameEn,
                LocalizationCode.Tr => data.NameTr,
                _ => data.NameEn,
            };
        }
    }
}