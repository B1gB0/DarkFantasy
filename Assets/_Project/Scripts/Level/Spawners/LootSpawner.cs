using System.Collections.Generic;
using _Project.Scripts.Items;
using _Project.Scripts.Level.Triggers;
using _Project.Scripts.Services;
using _Project.Scripts.UI.View;
using Reflex.Attributes;
using UnityEngine;

namespace _Project.Scripts.Level.Spawners
{
    public class LootSpawner : MonoBehaviour
    {
        private const int DefaultCountObjectsInPool = 4;
        private const float SpawnOffsetY = 0.5f;
        private const string LootEnemyTriggers = nameof(LootEnemyTriggers);

        private readonly List<LootEnemyTrigger> _activeTriggers = new();

        [SerializeField] private LootEnemyTrigger _pickupPrefab;

        private EnemySpawner _enemySpawner;
        private ObjectPool<LootEnemyTrigger> _pool;
        private RewardView _rewardView;
        private PickUpView _pickUpView;
        private InventoryIsFullView _inventoryIsFullView;
        private LootEnemyTrigger _currentTarget;

        private IInventoryService _inventoryService;
        private ICurrencyService _currencyService;
        private IPlayerService _playerService;
        private ILootService _lootService;
        private IProgressionService  _progressionService;
        private IShopService _shopService;

        [Inject]
        private void Construct(
            IInventoryService inventoryService,
            ICurrencyService currencyService,
            IPlayerService playerService,
            ILootService lootService,
            IProgressionService progressionService,
            IShopService shopService)
        {
            _inventoryService = inventoryService;
            _currencyService = currencyService;
            _playerService = playerService;
            _lootService = lootService;
            _progressionService = progressionService;
            _shopService = shopService;
        }

        private void Start()
        {
            CreateLootTriggerPool();
        }

        private void Update()
        {
            if (_currentTarget == null) return;
            if (_playerService?.Player == null) return;

            if (_playerService.Player.InputController.IsPickUpPressed)
            {
                var target = _currentTarget;
                _currentTarget = null;

                _pickUpView?.Hide();
                target.Pickup();
            }
        }

        private void OnDestroy()
        {
            _lootService?.ClearReservations();
        }

        public void Register(LootEnemyTrigger trigger)
        {
            if (!_activeTriggers.Contains(trigger))
                _activeTriggers.Add(trigger);

            RecalculateNearest();
        }

        public void Unregister(LootEnemyTrigger trigger)
        {
            if (!_activeTriggers.Remove(trigger)) return;

            RecalculateNearest();
        }

        public void GetViews(RewardView rewardView, PickUpView pickUpView, InventoryIsFullView  inventoryIsFullView)
        {
            _rewardView = rewardView;
            _pickUpView = pickUpView;
            _inventoryIsFullView = inventoryIsFullView;
        }

        public void SpawnLoot(LootResult reward, Vector3 position)
        {
            if (reward.Type == LootType.None) return;

            Vector3 spawnPos = position + Vector3.up * SpawnOffsetY;
            var lootEnemyTrigger = _pool.GetFreeElement();
            lootEnemyTrigger.transform.position = spawnPos;
            lootEnemyTrigger.OnPickedUp -= _rewardView.Show;
            lootEnemyTrigger.OnInventoryIsFull -= _inventoryIsFullView.Show;
            
            lootEnemyTrigger.Setup(
                reward,
                this,
                _inventoryService,
                _currencyService,
                _lootService,
                _progressionService,
                _shopService);
            
            lootEnemyTrigger.OnPickedUp += _rewardView.Show;
            lootEnemyTrigger.OnInventoryIsFull += _inventoryIsFullView.Show;
        }

        private void RecalculateNearest()
        {
            var nearest = FindNearest();

            if (nearest == _currentTarget) return;

            _currentTarget = nearest;

            if (_currentTarget != null)
                _pickUpView?.Show();
            else
                _pickUpView?.Hide();
        }

        private LootEnemyTrigger FindNearest()
        {
            if (_activeTriggers.Count == 0) return null;
            if (_playerService?.Player == null) return null;

            Vector3 playerPos = _playerService.Player.transform.position;

            LootEnemyTrigger best = null;
            float bestSqr = float.MaxValue;

            for (int i = _activeTriggers.Count - 1; i >= 0; i--)
            {
                var t = _activeTriggers[i];

                if (t == null || !t.gameObject.activeInHierarchy)
                {
                    _activeTriggers.RemoveAt(i);
                    continue;
                }

                float sqr = (t.transform.position - playerPos).sqrMagnitude;
                if (sqr < bestSqr)
                {
                    bestSqr = sqr;
                    best = t;
                }
            }

            return best;
        }

        private void CreateLootTriggerPool()
        {
            if (_pool != null)
                return;

            _pool = new ObjectPool<LootEnemyTrigger>(
                _pickupPrefab,
                DefaultCountObjectsInPool,
                new GameObject(LootEnemyTriggers).transform)
            {
                AutoExpand = true,
            };
        }
    }
}