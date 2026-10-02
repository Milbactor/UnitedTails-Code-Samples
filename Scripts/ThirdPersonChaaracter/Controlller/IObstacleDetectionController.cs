namespace WhiteKNight
{
    public interface IObstacleDetectionController
    {
        bool IsFrontBlocked();
        bool IsLeftFree();
        bool IsRightFree();
    }


}
