using _Project.Scripts.Level.Triggers;
using _Project.Scripts.Services;
using _Project.Scripts.UI.Panel;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using UnityEngine;

namespace _Project.Scripts.Level
{
    public class VillageLevel : Level
    {
        [SerializeField] private ShopTrigger _shopTrigger;
        
        private ShopPanel _shopPanel;
        
        // private IShopService _shopService;

        // [Inject]
        // private void Construct(IShopService shopService)
        // {
        //     _shopService = shopService;
        // }

        private void OnEnable()
        {
            
        }
        
        private void OnDisable()
        {
            
        }

        private void OnDestroy()
        {
            _shopTrigger.OnOpenShop -= _shopPanel.Show;
        }

        public override async UniTask OnStartLevel()
        {
            await ShopService.Init();
            
            _shopPanel = await ViewFactory.CreateShopPanel();
            
            _shopTrigger.OnOpenShop += _shopPanel.Show;
            
            await base.OnStartLevel();
        }
    }
}