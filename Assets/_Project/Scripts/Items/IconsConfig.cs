using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Items
{
    [CreateAssetMenu(menuName = "Icons Config")]
    public class IconsConfig : ScriptableObject
    {
        [Serializable]
        public struct Entry
        {
            public ItemType ItemType;
            public Sprite Icon;
        }

        [SerializeField] private List<Entry> _entries = new();

        private Dictionary<ItemType, Sprite> _map;

        public Sprite Get(ItemType type)
        {
            if (_map == null)
            {
                _map = new Dictionary<ItemType, Sprite>();
                foreach (var e in _entries)
                    _map[e.ItemType] = e.Icon;
            }

            return _map[type];
        }
    }
}