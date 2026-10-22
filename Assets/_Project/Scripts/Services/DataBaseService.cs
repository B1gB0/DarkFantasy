using _Project.Scripts.DataBase;
using _Project.Scripts.DataBase.InitDataSO;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;

namespace _Project.Scripts.Services
{
    public class DataBaseService : IDataBaseService
    {
        private const string DataContainer = nameof(DataContainer);
        private const string PlayerInitDataPath = "PlayerInitData";
        private const string EnemyInitDataPath = "EnemyInitData";

        private IResourceService _resourceService;
        private SpreadsheetContainer _data;
        
        public PlayerInitData PlayerInitData { get; private set; }
        public EnemyInitData EnemyInitData { get; private set; }
        public bool IsInitiated { get; private set; }
        public SpreadsheetContent Content => _data.Content;

        [Inject]
        private void Construct(IResourceService resourceService)
        {
            _resourceService = resourceService;
        }

        public async UniTask Init()
        {
            if (IsInitiated)
                return;

            _data = await _resourceService.LoadAsset<SpreadsheetContainer>(DataContainer);
            EnemyInitData = await _resourceService.LoadAsset<EnemyInitData>(EnemyInitDataPath);
            PlayerInitData = await _resourceService.LoadAsset<PlayerInitData>(PlayerInitDataPath);

            IsInitiated = true;
        }
    }
}