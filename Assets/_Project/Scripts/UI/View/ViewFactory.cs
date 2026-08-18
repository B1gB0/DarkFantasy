using _Project.Scripts.Experience;
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
#if UNITY_EDITOR
        private const string CheatPanelPath = "CheatPanel";
#endif
        private const string HealthBarPath = "HealthBar";
        private const string BossHealthBarPath = "BossHealthBar";
        private const string TextViewPath = "TextView";
        private const string ShopAttributePanelPath = "ShopAttributePanel";
        private const string ShopItemsPanelPath = "ShopItemsPanel";
        private const string MissionChoosingPanelPath = "MissionChoosingPanel";
        private const string EndGamePanelPath = "EndGamePanel";

        private IResourceService _resourceService;
        private IPlayerService _playerService;

        private UIRootView _uiRoot;
        private Container _container;
        
        private ShopAttributePanel _shopAttributePanel;
        private ShopItemsPanel _shopItemsPanel;
        private MissionChoosingPanel _missionChoosingPanel;
        private EndGamePanel _endGamePanel;
        
        public UIGameplayRootBinder UIScene { get; private set; }
        public GameplayEntryPoint GameplayEntryPoint { get; private set; }
        
        [Inject]
        public void Construct(IResourceService resourceService, IPlayerService playerService)
        {
            _resourceService = resourceService;
            _playerService = playerService;
        }

        private void OnDestroy()
        {
            if (_shopAttributePanel != null)
                _uiRoot.LocalizationLanguageSwitcher.OnLanguageChanged -= _shopAttributePanel.OnChangeLanguage;
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

        public async UniTask<HealthBar> CreateHealthBar(Health health)
        {
            var healthBarTemplate = await _resourceService.Load<GameObject>(HealthBarPath);
            healthBarTemplate = Instantiate(healthBarTemplate);

            HealthBar healthBar = healthBarTemplate.GetComponent<HealthBar>();
            GameObjectInjector.InjectSingle(healthBar.gameObject, _container);
            healthBar.Construct(health);
            healthBar.transform.SetParent(UIScene.transform, false);
            healthBar.GetPoints(UIScene.ShowHealthPoint, UIScene.HideHealthPoint, UIScene.WeaponPoint);

            return healthBar;
        }
        
        public async UniTask<BossHealthBar> CreateBossHealthBar(Health health)
        {
            var healthBarTemplate = await _resourceService.Load<GameObject>(BossHealthBarPath);
            healthBarTemplate = Instantiate(healthBarTemplate);

            BossHealthBar healthBar = healthBarTemplate.GetComponent<BossHealthBar>();
            GameObjectInjector.InjectSingle(healthBar.gameObject, _container);
            healthBar.Construct(health);
            healthBar.transform.SetParent(UIScene.transform, false);
            healthBar.GetPoints(UIScene.ShowBossHealthPoint, UIScene.HideBossHealthPoint, UIScene.WeaponPoint);

            return healthBar;
        }
        
        public async UniTask<FloatingTextView> CreateFloatingTextView()
        {
            var textViewTemplate = await _resourceService.Load<GameObject>(TextViewPath);
            textViewTemplate = Instantiate(textViewTemplate);
        
            FloatingTextView textView = textViewTemplate.GetComponent<FloatingTextView>();
            return textView;
        }

        public async UniTask<ShopAttributePanel> CreateShopAttributePanel()
        {
            var shopPanelTemplate = await _resourceService.Load<GameObject>(ShopAttributePanelPath);
            shopPanelTemplate = Instantiate(shopPanelTemplate);

            _shopAttributePanel = shopPanelTemplate.GetComponent<ShopAttributePanel>();
            GameObjectInjector.InjectRecursive(_shopAttributePanel.gameObject, _container);
            _shopAttributePanel.transform.SetParent(UIScene.transform, false);
            
            _uiRoot.LocalizationLanguageSwitcher.OnLanguageChanged += _shopAttributePanel.OnChangeLanguage;

            return _shopAttributePanel;
        }
        
        public async UniTask<ShopItemsPanel> CreateShopItemsPanel()
        {
            var shopPanelTemplate = await _resourceService.Load<GameObject>(ShopItemsPanelPath);
            shopPanelTemplate = Instantiate(shopPanelTemplate);

            _shopAttributePanel = shopPanelTemplate.GetComponent<ShopAttributePanel>();
            GameObjectInjector.InjectRecursive(_shopAttributePanel.gameObject, _container);
            _shopAttributePanel.transform.SetParent(UIScene.transform, false);
            
            _uiRoot.LocalizationLanguageSwitcher.OnLanguageChanged += _shopAttributePanel.OnChangeLanguage;

            return _shopItemsPanel;
        }
        
        public async UniTask<MissionChoosingPanel> CreateMissionChoosingPanel()
        {
            var missionPanelTemplate = await _resourceService.Load<GameObject>(MissionChoosingPanelPath);
            missionPanelTemplate = Instantiate(missionPanelTemplate);

            _missionChoosingPanel = missionPanelTemplate.GetComponent<MissionChoosingPanel>();
            GameObjectInjector.InjectRecursive(_missionChoosingPanel.gameObject, _container);
            _missionChoosingPanel.transform.SetParent(UIScene.transform, false);

            return _missionChoosingPanel;
        }
        
        public async UniTask<EndGamePanel> CreateEndGamePanel()
        {
            var endGamePanelTemplate = await _resourceService.Load<GameObject>(EndGamePanelPath);
            endGamePanelTemplate = Instantiate(endGamePanelTemplate);

            _endGamePanel = endGamePanelTemplate.GetComponent<EndGamePanel>();
            GameObjectInjector.InjectObject(_endGamePanel.gameObject, _container);
            _endGamePanel.transform.SetParent(UIScene.transform, false);
            _endGamePanel.gameObject.SetActive(false);

            return _endGamePanel;
        }

#if UNITY_EDITOR
        public async UniTask<CheatPanel> CreateCheatPanel(ExperiencePoints experiencePoints)
        {
            var cheatPanelTemplate = await _resourceService.Load<GameObject>(CheatPanelPath);
            cheatPanelTemplate = Instantiate(cheatPanelTemplate);

            CheatPanel cheatPanel = cheatPanelTemplate.GetComponent<CheatPanel>();
            GameObjectInjector.InjectObject(cheatPanel.gameObject, _container);
            cheatPanel.GetServices(experiencePoints);
            cheatPanel.transform.SetParent(UIScene.transform);
            return cheatPanel;
        }
#endif
    }
}