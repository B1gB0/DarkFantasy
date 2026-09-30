namespace _Project.Scripts.Items
{
    public struct LootResult
    {
        public LootType Type { get; private set; }
        public ItemType ItemType { get; private set; }
        public int GoldValue { get; private set; }
        public bool IsHighChance { get; private set; }

        public static LootResult None() => new LootResult { Type = LootType.None };

        public static LootResult Gold(int amount, bool isHighChance) => new LootResult
        {
            Type = LootType.Gold,
            GoldValue = amount,
            IsHighChance =  isHighChance,
        };

        public static LootResult Item(ItemType type, LootType lootType) => new LootResult
        {
            Type = lootType,
            ItemType = type,
        };
    }
}