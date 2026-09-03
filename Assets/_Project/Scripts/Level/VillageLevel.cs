using _Project.Scripts.Level.Triggers;
using _Project.Scripts.UI.Panel;
using Cysharp.Threading.Tasks;
using UnityEngine;
using YG;

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
            _shopAttributePanelTrigger.OnOpenShop -= OnShowAttributePanel;
            _shopAttributePanelTrigger.OnOpenShop -= UIRootView.UIRootButtons.Deactivate;
            _shopAttributePanelTrigger.OnOpenShop -= HealthBar.Hide;
            _shopAttributePanel.OnBackToSceneButtonPressed -= UIRootView.UIRootButtons.Activate;
            _shopAttributePanel.OnBackToSceneButtonPressed -= _shopAttributePanel.Hide;
            _shopAttributePanel.OnBackToSceneButtonPressed -= HealthBar.Show;
            _shopAttributePanel.OnBackToSceneButtonPressed -= OnShowInventoryShop;

            _shopItemsPanelTrigger.OnOpenShop -= _shopItemsPanel.Show;
            _shopItemsPanelTrigger.OnOpenShop -= UIRootView.UIRootButtons.Deactivate;
            _shopItemsPanelTrigger.OnOpenShop -= HealthBar.Hide;
            _shopItemsPanel.OnBackToSceneButtonPressed -= UIRootView.UIRootButtons.Activate;
            _shopItemsPanel.OnBackToSceneButtonPressed -= _shopItemsPanel.Hide;
            _shopItemsPanel.OnBackToSceneButtonPressed -= HealthBar.Show;
            _shopItemsPanel.OnBackToSceneButtonPressed -= OnShowMissionChoosingPanel;

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
            _shopAttributePanelTrigger.OnOpenShop += OnShowAttributePanel;
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
            _shopAttributePanel.OnBackToSceneButtonPressed += OnShowInventoryShop;

            _shopItemsPanelTrigger.OnOpenShop += HealthBar.Hide;
            _shopItemsPanel.OnBackToSceneButtonPressed += HealthBar.Show;
            _shopItemsPanel.OnBackToSceneButtonPressed += OnShowMissionChoosingPanel;

            _missionChoosingTrigger.OnOpenMissionPanel += HealthBar.Hide;
            _missionChoosingPanel.OnBackToSceneButtonPressed += HealthBar.Show;

            if (!YG2.saves.IsAttributeShopVisited)
            {
                NavMeshWaypointService.ShowWaypoint(_shopAttributePanelTrigger.transform);
                _shopItemsPanelTrigger.Deactivate();
                _missionChoosingTrigger.Deactivate();
                return;
            }

            if (!YG2.saves.IsInventoryShopVisited)
            {
                NavMeshWaypointService.ShowWaypoint(_shopItemsPanelTrigger.transform);
                _missionChoosingTrigger.Deactivate();
                return;
            }
            
            if (!YG2.saves.IsMissionPanelVisited)
            {
                NavMeshWaypointService.ShowWaypoint(_missionChoosingTrigger.transform);
            }
        }

        private void HandleMissionTransition()
        {
            if (!YG2.saves.IsMissionPanelVisited)
                YG2.saves.IsMissionPanelVisited = true;
            
            ViewFactory.GameplayEntryPoint.GetGameplayExitParameters();
            ViewFactory.UIScene.HandleGoToNextScene();
        }

        private void OnShowAttributePanel()
        {
            if (YG2.saves.IsAttributeShopVisited) return;

            YG2.saves.IsAttributeShopVisited = true;
        }

        private void OnShowInventoryShop()
        {
            if (YG2.saves.IsInventoryShopVisited) return;
            
            NavMeshWaypointService.ShowWaypoint(_shopItemsPanelTrigger.transform);
            _shopItemsPanelTrigger.Activate();

            YG2.saves.IsInventoryShopVisited = true;
        }

        private void OnShowMissionChoosingPanel()
        {
            if (YG2.saves.IsMissionPanelVisited) return;

            _missionChoosingTrigger.Activate();
            NavMeshWaypointService.ShowWaypoint(_missionChoosingTrigger.transform);
        }
    }
}