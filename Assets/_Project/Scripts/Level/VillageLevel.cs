using _Project.Scripts.Level.Triggers;
using _Project.Scripts.UI.Panel;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Level
{
    public class VillageLevel : Level
    {
        [SerializeField] private ShopTrigger _shopTrigger;
        [SerializeField] private MissionChoosingTrigger _missionChoosingTrigger;

        private ShopPanel _shopPanel;
        private MissionChoosingPanel _missionChoosingPanel;

        private void OnDestroy()
        {
            _shopTrigger.OnOpenShop -= _shopPanel.Show;
            _shopTrigger.OnOpenShop -= UIRootView.UIRootButtons.Deactivate;
            _shopTrigger.OnOpenShop -= HealthBar.Hide;
            _shopPanel.OnBackToSceneButtonPressed -= UIRootView.UIRootButtons.Activate;
            _shopPanel.OnBackToSceneButtonPressed -= _shopPanel.Hide;
            _shopPanel.OnBackToSceneButtonPressed -= HealthBar.Show;

            _missionChoosingTrigger.OnOpenMissionPanel -= _missionChoosingPanel.Show;
            _missionChoosingTrigger.OnOpenMissionPanel -= UIRootView.UIRootButtons.Deactivate;
            _missionChoosingTrigger.OnOpenMissionPanel -= HealthBar.Hide;
            _missionChoosingPanel.OnBackToSceneButtonPressed -= UIRootView.UIRootButtons.Activate;
            _missionChoosingPanel.OnBackToSceneButtonPressed -= _missionChoosingPanel.Hide;
            _missionChoosingPanel.OnBackToSceneButtonPressed -= HealthBar.Show;
        }

        public override async UniTask OnStartLevel()
        {
            await ShopService.Init();

            _shopPanel = await ViewFactory.CreateShopPanel();
            _missionChoosingPanel = await ViewFactory.CreateMissionChoosingPanel();

            _shopTrigger.OnOpenShop += _shopPanel.Show;
            _shopTrigger.OnOpenShop += UIRootView.UIRootButtons.Deactivate;
            _shopPanel.OnBackToSceneButtonPressed += UIRootView.UIRootButtons.Activate;
            _shopPanel.OnBackToSceneButtonPressed += _shopPanel.Hide;
            
            _missionChoosingTrigger.OnOpenMissionPanel += _missionChoosingPanel.Show;
            _missionChoosingTrigger.OnOpenMissionPanel += UIRootView.UIRootButtons.Deactivate;
            _missionChoosingPanel.OnBackToSceneButtonPressed += UIRootView.UIRootButtons.Activate;
            _missionChoosingPanel.OnBackToSceneButtonPressed += _missionChoosingPanel.Hide;

            await base.OnStartLevel();
            
            _shopTrigger.OnOpenShop += HealthBar.Hide;
            _shopPanel.OnBackToSceneButtonPressed += HealthBar.Show;
            _missionChoosingTrigger.OnOpenMissionPanel += HealthBar.Hide;
            _missionChoosingPanel.OnBackToSceneButtonPressed += HealthBar.Show;
        }
    }
}