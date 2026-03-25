using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeDatabase", menuName = "Databases/Upgrades/UpgradeDatabase")]
public class UpgradeDatabase : ScriptableObject
{
    public List<UpgradeSystem> upgradeEntityList = new List<UpgradeSystem>();

    private IGameDataService GameData => ServiceLocator.Instance.GetService<IGameDataService>();

    public void ResetAllUpgrades()
    {
        foreach (var upgrade in upgradeEntityList)
            GameData.data.objectUpgradeData.SetObjectLevel(upgrade.upgradeId, 0);

        GameData.Save();
        Debug.Log("[UpgradeDatabase] All upgrades reset!");
    }

    public void SyncFromSave()
    {
        foreach (var upgrade in upgradeEntityList)
        {
            int level = GameData.data.objectUpgradeData.GetObjectLevel(upgrade.upgradeId);
            Debug.Log($"[UpgradeDatabase] Synced {upgrade.upgradeId} → Level {level}");
        }
    }
}
