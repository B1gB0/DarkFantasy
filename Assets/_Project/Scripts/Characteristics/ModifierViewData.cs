namespace _Project.Scripts.Characteristics
{
    public readonly struct ModifierViewData
    {
        public readonly ModifierType Type;
        public readonly float RemainingTime;
        public readonly float TotalDuration;

        public ModifierViewData(ModifierType type, float remainingTime, float totalDuration)
        {
            Type = type;
            RemainingTime = remainingTime;
            TotalDuration = totalDuration;
        }

        public float Progress01 =>
            TotalDuration <= 0f ? 0f : RemainingTime / TotalDuration;
    }
}