using _Project.Scripts.DataBase;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;

namespace _Project.Scripts.Services
{
    public class DataBaseService : IDataBaseService
    {
        private const string DataContainer = nameof(DataContainer);

        private IResourceService _resourceService;
        private SpreadsheetContainer _data;

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

            _data = await _resourceService.Load<SpreadsheetContainer>(DataContainer);

            IsInitiated = true;
        }
    }
}