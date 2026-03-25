public interface IGameDataService : IService
{
    GameSaveData data { get; }
    void Save();
    void MarkDirty();
    void SaveAllBallState();
    void LoadLevel(BallSpawner ballSpawner);
    void ResetLevel();
}
