using System;
using System.Text;
using _Project.Scripts.Audio.Sounds;
using _Project.Scripts.Characteristics;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Game.Constant;
using _Project.Scripts.Items;
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
    public class DescriptionItemView : View
    {
        [SerializeField] private TMP_Text _description;
        [SerializeField] private TMP_Text _name;
        [SerializeField] private TMP_Text _rarity;
        [SerializeField] private Image _icon;
        [SerializeField] private Button _equipButton;
        [SerializeField] private Button _backButton;

        private AudioSoundsService _audioSoundsService;
        private IInventoryService _inventoryService;
        private ITweenAnimationService _tweenAnimationService;
        private IShopService _shopService;
        private IProgressionService _progressionService;
        private IUILocalizationService  _uiLocalizationService;

        private ItemData _currentTemplate;
        private Item _currentInstance;

        [Inject]
        private void Construct(
            AudioSoundsService audioSoundsService,
            IInventoryService inventoryService,
            ITweenAnimationService tweenAnimationService,
            IShopService shopService,
            IProgressionService progressionService,
            IUILocalizationService uiLocalizationService)
        {
            _audioSoundsService = audioSoundsService;
            _inventoryService = inventoryService;
            _tweenAnimationService = tweenAnimationService;
            _shopService = shopService;
            _progressionService = progressionService;
            _uiLocalizationService =  uiLocalizationService;
        }

        private void OnEnable()
        {
            _backButton.onClick.AddListener(Hide);
        }

        private void Start()
        {
            _equipButton.onClick.AddListener(OnEquippedButtonClicked);
        }

        private void OnDisable()
        {
            _backButton.onClick.RemoveListener(Hide);
        }

        private void OnDestroy()
        {
            _equipButton.onClick.RemoveListener(OnEquippedButtonClicked);
            transform.DOKill();
        }

        public override void Show() => _tweenAnimationService.AnimateScale(transform);
        public override void Hide() => _tweenAnimationService.AnimateScale(transform, true);

        public void SetConsumable(ItemData template)
        {
            _currentTemplate = template;
            _currentInstance = null;

            SetName(template);
            SetRarity(template);
            SetIcon(template);

            _description.text = GetLocalizedDescription(template);
        }

        public void SetEquipment(ItemData template, Item instance)
        {
            _currentTemplate = template;
            _currentInstance = instance;

            SetName(template);
            SetRarity(template);
            SetIcon(template);

            _description.text = BuildEquipmentStats(template, instance);
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

        private void SetIcon(ItemData data)
        {
            _icon.sprite = _shopService.GetItemSpriteByType(data.Type);
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

        private string BuildEquipmentStats(ItemData template, Item instance)
        {
            if (instance == null) return string.Empty;

            float mul = _progressionService.GetItemMultiplier(instance.Difficulty);

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

        private void OnEquippedButtonClicked()
        {
            if (_currentTemplate == null) return;

            _audioSoundsService.PlaySound(SoundsType.UIButtonClick).Forget();

            if (_currentTemplate.Kind == ItemKind.Consumable)
            {
                _inventoryService.EquipConsumableItem(_currentTemplate);
            }
            else if (_currentTemplate.Kind == ItemKind.Equipment)
            {
                if (_currentInstance == null) return;
                _inventoryService.EquipItem(_currentInstance.Id);
            }
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