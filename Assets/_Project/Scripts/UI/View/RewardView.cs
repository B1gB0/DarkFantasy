using System;
using System.Text;
using _Project.Scripts.Audio.Sounds;
using _Project.Scripts.Characteristics;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Game.Constant;
using _Project.Scripts.Items;
using _Project.Scripts.Level;
using _Project.Scripts.Services;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Reflex.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace _Project.Scripts.UI.View
{
    public class RewardView : View
    {
        [SerializeField] private TMP_Text _description;
        [SerializeField] private TMP_Text _name;
        [SerializeField] private TMP_Text _rarity;
        [SerializeField] private Image _icon;
        [SerializeField] private Button _exitButton;
        [SerializeField] private Sprite _lowGoldSprite;
        [SerializeField] private Sprite _highGoldSprite;

        private AudioSoundsService _audioSoundsService;
        private ITweenAnimationService _tweenAnimationService;
        private IShopService _shopService;
        private IPauseService _pauseService;
        private IProgressionService _progressionService;
        private IUILocalizationService _uiLocalizationService;

        [Inject]
        private void Construct(
            AudioSoundsService audioSoundsService,
            ITweenAnimationService tweenAnimationService,
            IShopService shopService,
            IPauseService pauseService,
            IProgressionService progressionService,
            IUILocalizationService uiLocalizationService)
        {
            _audioSoundsService = audioSoundsService;
            _tweenAnimationService = tweenAnimationService;
            _shopService = shopService;
            _pauseService = pauseService;
            _progressionService = progressionService;
            _uiLocalizationService = uiLocalizationService;
        }

        private void OnEnable()
        {
            _exitButton.onClick.AddListener(Hide);
        }

        private void OnDisable()
        {
            _exitButton.onClick.RemoveListener(Hide);
        }

        private void OnDestroy()
        {
            transform.DOKill();
        }

        public void Show(LootResult result)
        {
            SetDescription(result);
            Show();
        }

        public override void Show()
        {
            _tweenAnimationService.AnimateScale(transform);
            _pauseService.OnStopGameWithoutMusic();
        }

        public override void Hide()
        {
            _audioSoundsService.PlaySound(SoundsType.UIButtonClick).Forget();
            _tweenAnimationService.AnimateScale(transform, true);
            _pauseService.OnPlayGame();
        }

        private void SetDescription(LootResult result)
        {
            switch (result.Type)
            {
                case LootType.Equipment:
                    SetEquipment(result);
                    break;
                case LootType.Consumable:
                    SetConsumable(result);
                    break;
                case LootType.Gold:
                    SetGold(result);
                    break;
            }
        }

        private void SetEquipment(LootResult result)
        {
            var data = _shopService.GetItemDataByType(result.ItemType);
            if (data == null) return;

            var difficulty = _progressionService.CurrentDifficulty;

            SetName(data);
            SetRarity(data);

            _icon.sprite = _shopService.GetItemSpriteByType(data.Type);

            _description.text = BuildEquipmentStats(data, difficulty);
        }

        private string BuildEquipmentStats(ItemData template, LevelDifficulty difficulty)
        {
            float mul = _progressionService.GetItemMultiplier(difficulty);

            var sb = new StringBuilder();

            AppendStat(sb, template.CharacteristicType, template.Value * mul);
            AppendStat(sb, template.CharacteristicType2, template.Value2 * mul);
            AppendStat(sb, template.CharacteristicType3, template.Value3 * mul);

            return sb.ToString();
        }

        private void AppendStat(StringBuilder sb, CharacteristicType type, float value)
        {
            if (type == CharacteristicType.None) return;
            if (value <= 0f) return;

            string localized = _shopService.GetLocalizedCharacteristicName(type);
            if (string.IsNullOrEmpty(localized)) return;

            sb.Append('+');
            sb.Append(value.ToString("0.#"));
            sb.Append(' ');
            sb.AppendLine(localized);
        }

        private void SetConsumable(LootResult result)
        {
            var data = _shopService.GetItemDataByType(result.ItemType);
            if (data == null) return;

            _rarity.gameObject.SetActive(false);

            SetName(data);
            _icon.sprite = _shopService.GetItemSpriteByType(data.Type);

            _description.text = GetLocalizedDescription(data);
        }

        private void SetGold(LootResult result)
        {
            _name.gameObject.SetActive(true);

            _name.text = YG2.lang switch
            {
                LocalizationCode.Ru => "Золото",
                LocalizationCode.En => "Gold",
                LocalizationCode.Tr => "Altın",
                _ => "Gold"
            };

            _description.text = YG2.lang switch
            {
                LocalizationCode.Ru => $"Получено {result.GoldValue} золотых",
                LocalizationCode.En => $"Received {result.GoldValue} gold",
                LocalizationCode.Tr => $"{result.GoldValue} altın alındı",
                _ => $"+{result.GoldValue}"
            };

            _icon.sprite = result.IsHighChance ? _highGoldSprite : _lowGoldSprite;
        }

        private void SetName(ItemData data)
        {
            _name.gameObject.SetActive(true);

            _name.text = YG2.lang switch
            {
                LocalizationCode.Ru => data.NameRu,
                LocalizationCode.En => data.NameEn,
                LocalizationCode.Tr => data.NameTr,
                _ => data.NameEn
            };
        }

        private string GetLocalizedDescription(ItemData data)
        {
            return YG2.lang switch
            {
                LocalizationCode.Ru => data.DescriptionRu,
                LocalizationCode.En => data.DescriptionEn,
                LocalizationCode.Tr => data.DescriptionTr,
                _ => data.DescriptionEn
            };
        }

        private void SetRarity(ItemData data)
        {
            _rarity.gameObject.SetActive(true);

            _rarity.text = data.Rarity switch
            {
                ItemRarity.Common => _uiLocalizationService.GetLocalizedText(UITextType.Common),
                ItemRarity.Uncommon => _uiLocalizationService.GetLocalizedText(UITextType.Uncommon),
                ItemRarity.Rare => _uiLocalizationService.GetLocalizedText(UITextType.Rare),
                _ => throw new ArgumentOutOfRangeException()
            };

            _rarity.color = data.Rarity switch
            {
                ItemRarity.Common => Colors.GetColor(ColorName.RarityCommon),
                ItemRarity.Uncommon => Colors.GetColor(ColorName.RarityUncommon),
                ItemRarity.Rare => Colors.GetColor(ColorName.RarityRare),
                _ => Colors.GetColor(ColorName.DefaultWhiteTextColor),
            };
        }
    }
}