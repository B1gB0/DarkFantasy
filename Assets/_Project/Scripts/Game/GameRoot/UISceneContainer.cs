using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Game.GameRoot
{
    public class UISceneContainer : MonoBehaviour
    {
        private readonly Dictionary<string, MonoBehaviour> _cache = new();
        
        [SerializeField] private Transform _persistentContainer;
        [SerializeField] private Transform _temporaryContainer;

        public Transform PersistentContainer => _persistentContainer;
        public Transform TemporaryContainer => _temporaryContainer;

        public void ClearTemporaryUI()
        {
            for (int i = _temporaryContainer.childCount - 1; i >= 0; i--)
            {
                Destroy(_temporaryContainer.GetChild(i).gameObject);
            }
        }
        
        public T Get<T>(string key) where T : MonoBehaviour
        {
            if (_cache.TryGetValue(key, out var cached))
            {
                if (cached != null) return cached as T;
                _cache.Remove(key);
            }
            return null;
        }

        public void Register<T>(string key, T instance) where T : MonoBehaviour
        {
            _cache[key] = instance;
        }
    }
}