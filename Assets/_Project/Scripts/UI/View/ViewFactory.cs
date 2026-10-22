using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.DataBase.Data;
using _Project.Scripts.Game.Gameplay;
using _Project.Scripts.Game.Gameplay.Root.View;
using _Project.Scripts.Game.GameRoot;
using _Project.Scripts.Services;
using _Project.Scripts.UI.Panel;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using Reflex.Core;
using Reflex.Injectors;
using UnityEngine;

namespace _Project.Scripts.UI.View
{
    public class ViewFactory : MonoBehaviour
    {
#if CHEATS
        private const string CheatPanelPath = "CheatPanel";
#endif
        private const string HealthBarPath = "HealthBar";
        private const string BossHealthBarPath = "BossHealthBar";
        private const string TextViewPath = "TextView";
        private const string ShopAttributePanelPath = "ShopAttributePanel";
        private const string ShopItemsPanelPath = "ShopItemsPanel";
        private const string MissionChoosingPanelPath = "MissionChoosingPanel";
        private const string EndGamePanelPath = "EndGamePanel";
        private const string ShopItemViewPath = "ShopItemView";
        private const string InventoryPanelPath = "InventoryPanel";
        private const string ModifierPanelPath = "ModifierPanel";
        private const string InventoryRigPath = "InventoryRig";

        private readonly List<ShopItemView> _shopItemViews = new();

        private IResourceService _resourceService;
        private ICurrencyService _currencyService;
        private IShopService _shopService;

        private UIRootView _uiRoot;
        private Container _container;

        private ShopAttributePanel _shopAttributePanel;
        private ShopItemsPanel _shopItemsPanel;
        private MissionChoosingPanel _missionChoosingPanel;
        private EndGamePanel _endGamePanel;
        private ModifiersPanel _modifiersPanel;
        private InventoryPanel _inventoryPanel;
        private InventoryRig _inventoryRig;

        private List<ItemData> _itemsData = new();

        public UIGameplayRootBinder UIScene { get; private set; }
        public GameplayEntryPoint GameplayEntryPoint { get; private set; }

        [Inject]
        public void Construct(
            IResourceService resourceService,
            ICurrencyService currencyService,
            IShopService shopService)
        {
            _resourceService = resourceService;
            _currencyService = currencyService;
            _shopService = shopService;
        }

        private void OnDestroy()
        {
            if (_shopAttributePanel != null)
                _uiRoot.LocalizationLanguageSwitcher.OnLanguageChanged -= _shopAttributePanel.OnChangeLanguage;

            if (_shopItemsPanel != null)
            {
                _uiRoot.LocalizationLanguageSwitcher.OnLanguageChanged -= _shopItemsPanel.OnChangeLanguage;
                _shopItemsPanel.OnInventoryIsFull -= UIScene.InventoryIsFullView.Show;
            }

            foreach (var shopItemView in _shopItemViews.Where(shopItemView => shopItemView != null))
            {
                _currencyService.OnGoldValueChanged -= shopItemView.SetCurrencyColor;
            }
        }

        public void GetEntities(
            UIRootView uiRoot,
            UIGameplayRootBinder uiScene,
            Container container,
            GameplayEntryPoint gameplayEntryPoint)
        {
            _uiRoot = uiRoot;
            UIScene = uiScene;
            _container = container;

            GameplayEntryPoint = gameplayEntryPoint;

            GameObjectInjector.InjectRecursive(UIScene.gameObject, _container);
        }

        public async UniTask<BossHealthBar> CreateBossHealthBar(Health health)
        {
            BossHealthBar healthBar = await _resourceService.InstantiateAsync<BossHealthBar>(
                BossHealthBarPath,
                _uiRoot.UISceneContainer.TemporaryContainer);
            
            GameObjectInjector.InjectSingle(healthBar.gameObject, _container);
            healthBar.Construct(health);
            healthBar.GetPoints(UIScene.ShowBossHealthPoint, UIScene.HideBossHealthPoint, UIScene.WeaponPoint);
            healthBar.gameObject.SetActive(false);

            return healthBar;
        }

        public async UniTask<FloatingTextView> CreateFloatingTextView()
        {
            FloatingTextView textView =
                await _resourceService.InstantiateAsync<FloatingTextView>(TextViewPath);
            
            return textView;
        }

        public async UniTask<ShopAttributePanel> CreateShopAttributePanel()
        {
            _shopAttributePanel = await _resourceService.InstantiateAsync<ShopAttributePanel>(
                ShopAttributePanelPath,
                _uiRoot.UISceneContainer.TemporaryContainer);
            
            GameObjectInjector.InjectRecursive(_shopAttributePanel.gameObject, _container);

            _uiRoot.LocalizationLanguageSwitcher.OnLanguageChanged += _shopAttributePanel.OnChangeLanguage;

            return _shopAttributePanel;
        }

        public async UniTask<ShopItemsPanel> CreateShopItemsPanel()
        {
            _shopItemsPanel = await _resourceService.InstantiateAsync<ShopItemsPanel>(
                ShopItemsPanelPath,
                _uiRoot.UISceneContainer.TemporaryContainer);
            
            _shopItemsPanel.gameObject.SetActive(false);
            
            GameObjectInjector.InjectRecursive(_shopItemsPanel.gameObject, _container);

            _uiRoot.LocalizationLanguageSwitcher.OnLanguageChanged += _shopItemsPanel.OnChangeLanguage;

            _shopItemsPanel.OnInventoryIsFull += UIScene.InventoryIsFullView.Show;

            _itemsData = _shopService.GetItemsData();

            foreach (var itemData in _itemsData.Where(itemData => itemData.IsSold))
            {
                ShopItemView itemView = await CreateShopItemView(_shopItemsPanel.ItemContent);
                itemView.Set(itemData);
                GameObjectInjector.InjectRecursive(itemView.gameObject, _container);
            }

            _shopItemsPanel.GetItemViews(_shopItemViews);

            return _shopItemsPanel;
        }

        public async UniTask<MissionChoosingPanel> CreateMissionChoosingPanel()
        {
            _missionChoosingPanel =
                await _resourceService.InstantiateAsync<MissionChoosingPanel>(
                    MissionChoosingPanelPath,
                    _uiRoot.UISceneContainer.TemporaryContainer);
            
            GameObjectInjector.InjectRecursive(_missionChoosingPanel.gameObject, _container);

            return _missionChoosingPanel;
        }
        
        public async UniTask<ModifiersPanel> CreateModifiersPanel()
        {
            _modifiersPanel = await _resourceService.InstantiateAsync<ModifiersPanel>(
                ModifierPanelPath, 
                _uiRoot.UISceneContainer.TemporaryContainer);
            
            GameObjectInjector.InjectObject(_modifiersPanel.gameObject, _container);
            _modifiersPanel.GetPoints(UIScene.ShowModifiersPanel, UIScene.HideModifiersPanel);

            return _modifiersPanel;
        }
        
        public async UniTask<HealthBar> GetOrCreateHealthBar(Health health)
        {
            var cached = _uiRoot.UISceneContainer.Get<HealthBar>(HealthBarPath);
            
            if (cached != null)
            {
                cached.Construct(health);
                cached.GetPoints(UIScene.ShowHealthPoint, UIScene.HideHealthPoint, UIScene.WeaponPoint);
                return cached;
            }
            
            HealthBar healthBar = await _resourceService.InstantiateAsync<HealthBar>(
                HealthBarPath,
                _uiRoot.UISceneContainer.PersistentContainer);
            
            GameObjectInjector.InjectSingle(healthBar.gameObject, _container);
            healthBar.Construct(health);
            healthBar.GetPoints(UIScene.ShowHealthPoint, UIScene.HideHealthPoint, UIScene.WeaponPoint);
            
            _uiRoot.UISceneContainer.Register(HealthBarPath, healthBar);

            return healthBar;
        }

        public async UniTask<EndGamePanel> GetOrCreateEndGamePanel()
        {
            var cached = _uiRoot.UISceneContainer.Get<EndGamePanel>(EndGamePanelPath);
            if (cached != null) return cached;

            var panel = await _resourceService.InstantiateAsync<EndGamePanel>(
                EndGamePanelPath, 
                _uiRoot.UISceneContainer.PersistentContainer);

            if (panel != null)
            {
                GameObjectInjector.InjectRecursive(panel.gameObject, _container);
                panel.gameObject.SetActive(false);

                _uiRoot.UISceneContainer.Register(EndGamePanelPath, panel);
            }

            return panel;
        }

        public async UniTask<InventoryPanel> GetOrCreateInventoryPanel()
        {
            var cached = _uiRoot.UISceneContainer.Get<InventoryPanel>(InventoryPanelPath);
            if (cached != null) return cached;

            var panel = await _resourceService.InstantiateAsync<InventoryPanel>(
                InventoryPanelPath,
                _uiRoot.UISceneContainer.PersistentContainer);

            if (panel != null)
            {
                GameObjectInjector.InjectRecursive(panel.gameObject, _container);
                panel.gameObject.SetActive(false);

                _uiRoot.UISceneContainer.Register(InventoryPanelPath, panel);
            }

            return panel;
        }

        public async UniTask<InventoryRig> GetOrCreateInventoryRig()
        {
            var cached = _uiRoot.UISceneContainer.Get<InventoryRig>(InventoryRigPath);
            if (cached != null) return cached;

            var rig = await _resourceService.InstantiateAsync<InventoryRig>(InventoryRigPath);

            if (rig == null) return null;

            DontDestroyOnLoad(rig.gameObject);
            rig.transform.localScale = Vector3.one;

            _uiRoot.UISceneContainer.Register(InventoryRigPath, rig);
            return rig;
        }

#if CHEATS
        public async UniTask<CheatPanel> CreateCheatPanel()
        {
            CheatPanel cheatPanel = await _resourceService.InstantiateAsync<CheatPanel>(
                CheatPanelPath,
                UIScene.transform);
            
            GameObjectInjector.InjectObject(cheatPanel.gameObject, _container);
            return cheatPanel;
        }
#endif

        private async UniTask<ShopItemView> CreateShopItemView(Transform content)
        {
            ShopItemView shopItemView =
                await _resourceService.InstantiateAsync<ShopItemView>(ShopItemViewPath, content);
            
            GameObjectInjector.InjectRecursive(shopItemView.gameObject, _container);

            _currencyService.OnGoldValueChanged += shopItemView.SetCurrencyColor;

            _shopItemViews.Add(shopItemView);

            return shopItemView;
        }
    }
}