namespace _Project.Scripts.Items
{
    public struct LootResult
    {
        private LootType _type;
        private ItemType _item;
        private int _gold;

        public static LootResult None() => new LootResult { _type = LootType.None };

        public static LootResult Gold(int amount) => new LootResult
        {
            _type = LootType.Gold,
            _gold = amount,
        };

        public static LootResult Item(ItemType type, LootType lootType) => new LootResult
        {
            _type = lootType,
            _item = type,
        };
    }
}