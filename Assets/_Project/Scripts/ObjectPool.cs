using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Scripts
{
    public class ObjectPool<T>
        where T : MonoBehaviour
    {
        private readonly Transform _container;
        private readonly T _prefab;

        private List<T> _pool;

        public ObjectPool(T prefab, int count, Transform container)
        {
            _prefab = prefab;
            _container = container;
            CrateObjectPool(count);
        }

        public bool AutoExpand { get; set; }

        public T GetFreeElement()
        {
            if (HasFreeElement(out var element))
                return element;

            if (AutoExpand)
                return CreateObject(true);

            throw new Exception($"There is no free elements in pool of type {typeof(T)}");
        }
        
        public void Return(T element)
        {
            if (element == null) return;
            element.gameObject.SetActive(false);
        }

        private bool HasFreeElement(out T element)
        {
            for (int i = _pool.Count - 1; i >= 0; i--)
            {
                var obj = _pool[i];
                if (obj != null && !obj.gameObject.activeInHierarchy)
                {
                    obj.gameObject.SetActive(true);
                    element = obj;
                    return true;
                }
            }

            element = null;
            return false;
        }

        private void CrateObjectPool(int count)
        {
            _pool = new List<T>();

            for (int i = 0; i < count; i++)
            {
                CreateObject();
            }
        }

        private T CreateObject(bool isActiveByDefault = false)
        {
            var createdObject = Object.Instantiate(_prefab, _container);
            createdObject.gameObject.SetActive(isActiveByDefault);

            _pool.Add(createdObject);

            return createdObject;
        }
    }
}