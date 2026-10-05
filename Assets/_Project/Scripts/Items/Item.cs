using System;
using _Project.Scripts.Level;

namespace _Project.Scripts.Items
{
    [Serializable]
    public class Item
    {
        public string Id;
        public ItemType Type;
        public LevelDifficulty Difficulty;

        public static Item Create(ItemType type, LevelDifficulty difficulty)
        {
            return new Item
            {
                Id = Guid.NewGuid().ToString(),
                Type = type,
                Difficulty = difficulty,
            };
        }
    }
}