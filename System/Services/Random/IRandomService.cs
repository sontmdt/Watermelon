public interface IRandomService : IService
{
    int countConsecutiveSpawn { get; set; }
    eBallType RandomNextBall();
    void UpdateConsecutiveSpawn(eBallType type);
}
