using _Project.Scripts.Level.Triggers;
using _Project.Scripts.UI.Panel;
using UnityEngine;

namespace _Project.Scripts.Level
{
    public class VillageLevel : Level
    {
        [SerializeField] private ShopTrigger _shopTrigger;
        
        private ShopPanel _shopPanel;
        
        private void OnEnable()
        {
            IsInitiatedSpawners += OnCreateShop;
            _shopTrigger.OnOpenShop += _shopPanel.Show;
        }
        
        private void OnDisable()
        {
            IsInitiatedSpawners -= OnCreateShop;
            _shopTrigger.OnOpenShop -= _shopPanel.Show;
        }

        private async void OnCreateShop()
        {
            _shopPanel = await ViewFactory.CreateShopPanel();
        }
    }
}