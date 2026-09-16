using System.Collections.Generic;
using _Project.Scripts.Characteristics;
using _Project.Scripts.Game.Constant;
using _Project.Scripts.Items;
using _Project.Scripts.Services;
using _Project.Scripts.UI.View;
using Reflex.Attributes;
using UnityEngine;

namespace _Project.Scripts.UI.Panel
{
    public class ModifiersPanel : View.View
    {
        private readonly List<ModifierView> _active = new();
        private readonly List<ModifierViewData> _buffer = new();

        [SerializeField] private ModifierView _viewPrefab;
        [SerializeField] private Transform _container;
        [SerializeField] private int _initialPoolSize = 4;
        [SerializeField] private float _refreshInterval = 0.1f;
        [SerializeField] private Transform _showPoint;
        [SerializeField] private Transform _hidePoint;

        private ObjectPool<ModifierView> _pool;
        private IPlayerService _playerService;
        private IShopService _shopService;
        private ITweenAnimationService _tweenAnimationService;
        private float _timer;

        [Inject]
        public void Construct(
            IPlayerService playerService,
            IShopService shopService,
            ITweenAnimationService tweenAnimationService)
        {
            _playerService = playerService;
            _shopService = shopService;
            _tweenAnimationService = tweenAnimationService;
        }

        private void Awake()
        {
            _pool = new ObjectPool<ModifierView>(_viewPrefab, _initialPoolSize, _container)
            {
                AutoExpand = true
            };
        }

        private void Update()
        {
            _timer -= Time.unscaledDeltaTime;
            if (_timer > 0f) return;
            _timer = _refreshInterval;

            SyncViews();
        }
        
        public override void Show()
        {
            _tweenAnimationService.AnimateMove(transform, _showPoint, _hidePoint);
        }

        public override void Hide()
        {
            _tweenAnimationService.AnimateMove(transform, _showPoint, _hidePoint, true);
        }
        
        public void GetPoints(Transform showPoint, Transform hidePoint)
        {
            _showPoint = showPoint;
            _hidePoint = hidePoint;
        }

        private void SyncViews()
        {
            if (!_playerService.IsInitiated || !_shopService.IsInitiated) return;

            var data = CollectActiveModifiers();

            while (_active.Count < data.Count)
                _active.Add(_pool.GetFreeElement());

            while (_active.Count > data.Count)
            {
                var view = _active[^1];
                _active.RemoveAt(_active.Count - 1);
                _pool.Return(view);
            }

            for (int i = 0; i < data.Count; i++)
            {
                var viewData = data[i];
                
                switch (viewData.Type)
                {
                    case ModifierType.Health:
                        _active[i].SetIcon(_shopService.GetItemSpriteByType(ItemType.Meat));
                        _active[i].SetColor(Colors.GetColor(ColorName.HealthColorBar));
                        break;
                    case ModifierType.Speed:
                        _active[i].SetIcon(_shopService.GetItemSpriteByType(ItemType.SpeedPotion));
                        _active[i].SetColor(Colors.GetColor(ColorName.SpeedColorBar));
                        break;
                }
                
                _active[i].SetProgress(viewData.RemainingTime, viewData.TotalDuration);
            }
        }

        private List<ModifierViewData> CollectActiveModifiers()
        {
            _buffer.Clear();

            var heal = _playerService.Player.PlayerCharacteristics.HealingModifier;
            if (heal != null && heal.Timer.IsActive)
            {
                _buffer.Add(new ModifierViewData(
                    ModifierType.Health,
                    heal.Timer.RemainingTime,
                    heal.Timer.TotalDuration));
            }

            var speedModifiers = _playerService.Player.PlayerCharacteristics.SpeedModifiers;
            foreach (var speedModifier in speedModifiers)
            {
                if (!speedModifier.Timer.IsActive)
                    continue;
                
                _buffer.Add(new ModifierViewData(
                    ModifierType.Speed,
                    speedModifier.Timer.RemainingTime,
                    speedModifier.Timer.TotalDuration));
            }

            return _buffer;
        }
    }
}