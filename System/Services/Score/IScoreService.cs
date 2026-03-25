public interface IScoreService : IService
{
    int CurrentScore { get; }
    void AddScore(eBallType type);
    void AddCombo();
    void Initialize(int startScore);
    void Reset();
}
