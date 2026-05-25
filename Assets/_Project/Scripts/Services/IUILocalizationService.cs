using _Project.Scripts.DataBase.Data;
using _Project.Scripts.UI;

namespace _Project.Scripts.Services
{
    public interface IUILocalizationService : IService
    {
        public UILocalizationData GetLevelTextData(UITextType type);
    }
}