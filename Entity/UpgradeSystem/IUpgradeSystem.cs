public interface IUpgradeSystem
{
    bool CanUpgrade { get; }
    bool ReachedMaxLevel { get; }
    void Upgrade();
}
