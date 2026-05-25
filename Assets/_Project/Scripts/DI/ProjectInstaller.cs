using System.Collections.Generic;
using _Project.Scripts.Experience;
using _Project.Scripts.Game.GameRoot;
using _Project.Scripts.Services;
using Reflex.Core;
using Reflex.Injectors;
using Unity.VisualScripting;
using UnityEngine;

namespace _Project.Scripts.DI
{
    public class ProjectInstaller : MonoBehaviour, IInstaller
    {
        private readonly List<object> _monoServices = new();
        private readonly List<GameObject> _monoServiceObjects = new();

        [SerializeField] private AudioSoundsService _audioSoundsServicePrefab;
        [SerializeField] private PlayerService _playerServicePrefab;
        [SerializeField] private ParticleEffectsService _particleEffectsService;
        [SerializeField] private UIRootView _uiRootViewPrefab;
        [SerializeField] private GameEntryPoint _gameEntryPointPrefab;
        [SerializeField] private MissionService _missionServicePrefab;
        
        private void OnDestroy()
        {
            foreach (var obj in _monoServiceObjects)
            {
                if (obj != null) Destroy(obj);
            }
        }

        public void InstallBindings(ContainerBuilder builder)
        {
            RegisterCoreServices(builder);
            CreateMonoServices();
            RegisterCreatedServices(builder);
            RegisterContainerDependentServices(builder);
        }

        private void RegisterCoreServices(ContainerBuilder builder)
        {
            builder.AddSingleton(typeof(ResourceService), typeof(IResourceService));
            builder.AddSingleton(typeof(DataBaseService), typeof(IDataBaseService));
            builder.AddSingleton(typeof(ExperiencePoints), typeof(IExperiencePoints));
            builder.AddSingleton(typeof(EnemyService), typeof(IEnemyService));
            builder.AddSingleton(typeof(PauseService), typeof(IPauseService));
            builder.AddSingleton(typeof(FloatingTextService), typeof(IFloatingTextService));
            builder.AddSingleton(typeof(ShopService), typeof(IShopService));
            builder.AddSingleton(typeof(CurrencyService), typeof(ICurrencyService));
            builder.AddSingleton(typeof(TweenAnimationService), typeof(ITweenAnimationService));
            builder.AddSingleton(typeof(UILocalizationService), typeof(IUILocalizationService));
        }

        private void CreateMonoServices()
        {
            CreateService(_playerServicePrefab);
            CreateService(_audioSoundsServicePrefab);
            CreateService(_particleEffectsService);
            CreateService(_uiRootViewPrefab);
            CreateService(_gameEntryPointPrefab);
            CreateService(_missionServicePrefab);
        }

        private void CreateService<T>(T prefab)
            where T : MonoBehaviour
        {
            var instance = Instantiate(prefab);
            _monoServices.Add(instance);
            _monoServiceObjects.Add(instance.gameObject);
            DontDestroyOnLoad(instance);
        }

        private void RegisterCreatedServices(ContainerBuilder builder)
        {
            foreach (var service in _monoServices)
            {
                builder.AddSingleton(service);

                var serviceType = service.GetType();
                var interfaces = serviceType.GetInterfaces();

                foreach (var interfaceType in interfaces)
                {
                    builder.AddSingleton(serviceType, interfaceType);
                }
            }
        }

        private void RegisterContainerDependentServices(ContainerBuilder builder)
        {
            builder.OnContainerBuilt += container =>
            {
                foreach (var service in _monoServiceObjects)
                {
                    GameObjectInjector.InjectObject(service, container);
                }

                foreach (var service in _monoServices)
                {
                    if (service is IInitializable initializable)
                    {
                        initializable.Initialize();
                    }
                }
            };
        }
    }
}