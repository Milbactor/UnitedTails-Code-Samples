namespace WhiteKNight
{
    public readonly struct AIApproachContext
    {
        public bool HasTarget { get; }

        public float DistanceToTarget { get; }
        public float HeightDifferenceToTarget { get; }


        public AIApproachContext(
            bool hasTarget,
            float distanceToTarget,
            float heightDifferenceToTarget)
        {
            HasTarget = hasTarget;

            DistanceToTarget = distanceToTarget;
            HeightDifferenceToTarget = heightDifferenceToTarget;
        }
    }
}