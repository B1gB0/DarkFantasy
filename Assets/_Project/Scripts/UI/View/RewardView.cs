using _Project.Scripts.Audio.Sounds;
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
    public class RewardView : View
    {
        [SerializeField] private TMP_Text _description;
        [SerializeField] private TMP_Text _name;
        [SerializeField] private Image _icon;
        [SerializeField] private Button _exitButton;
        [SerializeField] private Sprite _lowGoldSprite;
        [SerializeField] private Sprite _highGoldSprite;
        
        private AudioSoundsService _audioSoundsService;
        private ITweenAnimationService _tweenAnimationService;
        private IShopService _shopService;
        private IPauseService _pauseService;

        [Inject]
        private void Construct(
            AudioSoundsService audioSoundsService,
            ITweenAnimationService tweenAnimationService,
            IShopService shopService,
            IPauseService pauseService)
        {
            _audioSoundsService = audioSoundsService;
            _tweenAnimationService = tweenAnimationService;
            _shopService =  shopService;
            _pauseService = pauseService;
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
                case LootType.Consumable:
                {
                    var data = _shopService.GetItemDataByType(result.ItemType);
                    _name.gameObject.SetActive(true);

                    _name.text = YG2.lang switch
                    {
                        LocalizationCode.Ru => data.NameRu,
                        LocalizationCode.En => data.NameEn,
                        LocalizationCode.Tr => data.NameTr,
                        _ => _name.text
                    };

                    _description.text = YG2.lang switch
                    {
                        LocalizationCode.Ru => data.DescriptionRu,
                        LocalizationCode.En => data.DescriptionEn,
                        LocalizationCode.Tr => data.DescriptionTr,
                        _ => _description.text
                    };
            
                    _icon.sprite = _shopService.GetItemSpriteByType(data.Type);
                    break;
                }
                case LootType.Gold:
                    _name.gameObject.SetActive(true);
                
                    _name.text = YG2.lang switch
                    {
                        LocalizationCode.Ru => "Золото",
                        LocalizationCode.En => "Gold",
                        LocalizationCode.Tr => "Altın",
                        _ => _name.text
                    };

                    _description.text = YG2.lang switch
                    {
                        LocalizationCode.Ru => "Получено " + $"{result.GoldValue} золотых",
                        LocalizationCode.En => "Received " + $"{result.GoldValue} gold",
                        LocalizationCode.Tr => "Aldı " + $"{result.GoldValue} altın",
                        _ => _description.text
                    };
            
                    _icon.sprite = result.IsHighChance ?  _highGoldSprite : _lowGoldSprite;
                    break;
            }
        }
    }
}