using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "HouseUpgrade", menuName = "Upgrades/House Upgrade")]
public class ObjectUpgrade : UpgradeSystem
{
    public List<ObjectUpgradeStruct> upgradeLevels = new List<ObjectUpgradeStruct>();

    public override bool CanUpgrade
    {
        get
        {
            int level = GetCurrentLevel();
            if (level + 1 >= upgradeLevels.Count) return false;
            int cost = upgradeLevels[level + 1].cost;
            return GameManager.Instance.m_playerAssetController.GetAsset(eAssetType.Coin) >= cost;
        }
    }

    public override bool ReachedMaxLevel => GetCurrentLevel() >= upgradeLevels.Count - 1;

    public override void Upgrade()
    {
        int level = GetCurrentLevel();
        if (!CanUpgrade) return;

        int cost = upgradeLevels[level + 1].cost;
        GameManager.Instance.m_playerAssetController.UseAsset(eAssetType.Coin, cost);

        SetCurrentLevel(level + 1);
    }
}
