using UnityEngine;

public abstract class UpgradeSystem : ScriptableObject, IUpgradeSystem
{
    [Header("Upgrade Info")]
    public string upgradeId;
    public string upgradeName;

    public abstract bool CanUpgrade { get; }
    public abstract bool ReachedMaxLevel { get; }
    public abstract void Upgrade();

    private IGameDataService GameData => ServiceLocator.Instance.GetService<IGameDataService>();

    public int GetCurrentLevel()
    {
        return GameData.data.objectUpgradeData.GetObjectLevel(upgradeId);
    }

    protected void SetCurrentLevel(int level)
    {
        GameData.data.objectUpgradeData.SetObjectLevel(upgradeId, level);
        GameData.Save();
    }
}
