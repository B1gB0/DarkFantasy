using _Project.Scripts.Level.Triggers;
using _Project.Scripts.UI.Panel;
using _Project.Scripts.UI.StateMachine.States;
using _Project.Scripts.UI.View;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Level
{
    public class VillageLevel : Level
    {
        [SerializeField] private ShopTrigger _shopAttributePanelTrigger;
        [SerializeField] private ShopTrigger _shopItemsPanelTrigger;
        [SerializeField] private MissionChoosingTrigger _missionChoosingTrigger;

        private ShopAttributePanel _shopAttributePanel;
        private ShopItemsPanel _shopItemsPanel;
        private MissionChoosingPanel _missionChoosingPanel;

        private void OnDestroy()
        {
            _shopAttributePanelTrigger.OnOpenShop -= _shopAttributePanel.Show;
            _shopAttributePanelTrigger.OnOpenShop -= UIRootView.UIRootButtons.Deactivate;
            _shopAttributePanelTrigger.OnOpenShop -= HealthBar.Hide;
            _shopAttributePanel.OnBackToSceneButtonPressed -= UIRootView.UIRootButtons.Activate;
            _shopAttributePanel.OnBackToSceneButtonPressed -= _shopAttributePanel.Hide;
            _shopAttributePanel.OnBackToSceneButtonPressed -= HealthBar.Show;
            
            _shopItemsPanelTrigger.OnOpenShop -= _shopItemsPanel.Show;
            _shopItemsPanelTrigger.OnOpenShop -= UIRootView.UIRootButtons.Deactivate;
            _shopItemsPanelTrigger.OnOpenShop -= HealthBar.Hide;
            _shopItemsPanel.OnBackToSceneButtonPressed -= UIRootView.UIRootButtons.Activate;
            _shopItemsPanel.OnBackToSceneButtonPressed -= _shopItemsPanel.Hide;
            _shopItemsPanel.OnBackToSceneButtonPressed -= HealthBar.Show;

            _missionChoosingTrigger.OnOpenMissionPanel -= _missionChoosingPanel.Show;
            _missionChoosingTrigger.OnOpenMissionPanel -= UIRootView.UIRootButtons.Deactivate;
            _missionChoosingTrigger.OnOpenMissionPanel -= HealthBar.Hide;
            _missionChoosingPanel.OnBackToSceneButtonPressed -= UIRootView.UIRootButtons.Activate;
            _missionChoosingPanel.OnBackToSceneButtonPressed -= _missionChoosingPanel.Hide;
            _missionChoosingPanel.OnBackToSceneButtonPressed -= HealthBar.Show;
            _missionChoosingPanel.OnGoToMission -= HandleMissionTransition;
        }

        public override async UniTask OnStartLevel()
        {
            await ShopService.Init();
            await InventoryService.Init();

            _shopAttributePanel = await ViewFactory.CreateShopAttributePanel();
            _shopItemsPanel = await ViewFactory.CreateShopItemsPanel();
            _missionChoosingPanel = await ViewFactory.CreateMissionChoosingPanel();

            _shopAttributePanelTrigger.OnOpenShop += _shopAttributePanel.Show;
            _shopAttributePanelTrigger.OnOpenShop += UIRootView.UIRootButtons.Deactivate;
            _shopAttributePanel.OnBackToSceneButtonPressed += UIRootView.UIRootButtons.Activate;
            _shopAttributePanel.OnBackToSceneButtonPressed += _shopAttributePanel.Hide;
            
            _shopItemsPanelTrigger.OnOpenShop += _shopItemsPanel.Show;
            _shopItemsPanelTrigger.OnOpenShop += UIRootView.UIRootButtons.Deactivate;
            _shopItemsPanel.OnBackToSceneButtonPressed += UIRootView.UIRootButtons.Activate;
            _shopItemsPanel.OnBackToSceneButtonPressed += _shopItemsPanel.Hide;
            
            _missionChoosingTrigger.OnOpenMissionPanel += _missionChoosingPanel.Show;
            _missionChoosingTrigger.OnOpenMissionPanel += UIRootView.UIRootButtons.Deactivate;
            _missionChoosingPanel.OnBackToSceneButtonPressed += UIRootView.UIRootButtons.Activate;
            _missionChoosingPanel.OnBackToSceneButtonPressed += _missionChoosingPanel.Hide;
            _missionChoosingPanel.OnGoToMission += HandleMissionTransition;

            await base.OnStartLevel();

            _shopAttributePanelTrigger.OnOpenShop += HealthBar.Hide;
            _shopAttributePanel.OnBackToSceneButtonPressed += HealthBar.Show;
            
            _shopItemsPanelTrigger.OnOpenShop += HealthBar.Hide;
            _shopItemsPanel.OnBackToSceneButtonPressed += HealthBar.Show;
            
            _missionChoosingTrigger.OnOpenMissionPanel += HealthBar.Hide;
            _missionChoosingPanel.OnBackToSceneButtonPressed += HealthBar.Show;
        }

        private void HandleMissionTransition()
        {
            ViewFactory.GameplayEntryPoint.GetGameplayExitParameters();
            ViewFactory.UIScene.HandleGoToNextScene();
        }
    }
}