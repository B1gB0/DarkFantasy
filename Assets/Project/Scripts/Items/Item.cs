using System;
using Newtonsoft.Json;
using Project.Scripts.Level;

namespace Project.Scripts.Items
{
    [Serializable]
    public class Item
    {
        [JsonProperty] private string _id;
        [JsonProperty] private ItemType _type;
        [JsonProperty] private LevelDifficulty _difficulty;

        [JsonIgnore] public string Id => _id;
        [JsonIgnore] public ItemType Type => _type;
        [JsonIgnore] public LevelDifficulty Difficulty => _difficulty;

        public static Item Create(ItemType type, LevelDifficulty difficulty)
        {
            var item = new Item();

            item.SetId(Guid.NewGuid().ToString());
            item.SetType(type);
            item.SetDifficulty(difficulty);

            return item;
        }

        private void SetId(string id)
        {
            _id = id;
        }

        private void SetType(ItemType type)
        {
            _type = type;
        }

        private void SetDifficulty(LevelDifficulty difficulty)
        {
            _difficulty = difficulty;
        }
    }
}