using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Items
{
    [CreateAssetMenu(menuName = "Icons Config")]
    public class IconsConfig : ScriptableObject
    {
        [SerializeField] private List<IconEntry> _entries = new();

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