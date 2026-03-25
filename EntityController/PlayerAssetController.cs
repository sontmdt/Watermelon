using UnityEngine;

public class PlayerAssetController : MonoBehaviour
{
    private IGameDataService GameData => ServiceLocator.Instance.GetService<IGameDataService>();
    public PlayerSaveData playerData => GameData.data.playerData;

    public int GetAsset(eAssetType type)
    {
        return type switch
        {
            eAssetType.Coin => playerData.coin,
            eAssetType.Diamond => playerData.diamond,
            eAssetType.DestroyBooster => playerData.numbleDestroyBooster,
            eAssetType.ShakeBooster => playerData.numbleShakeBooster,
            eAssetType.UpgradeBooster => playerData.numbleUpgradeBooster,
            eAssetType.SmallDestroyBooster => playerData.numbleSmallDestroyBooster,
            _ => 0
        };
    }

    private void SetAsset(eAssetType type, int value)
    {
        switch (type)
        {
            case eAssetType.Coin:
                playerData.coin = value;
                EventManager.Instance.PostEvent(EventID.OnCoinChanged, value);
                break;
            case eAssetType.Diamond:
                playerData.diamond = value;
                EventManager.Instance.PostEvent(EventID.OnDiamondChanged, value);
                break;
            case eAssetType.DestroyBooster:
                playerData.numbleDestroyBooster = value;
                EventManager.Instance.PostEvent(EventID.OnDestroyBoosterChanged, value);
                break;
            case eAssetType.ShakeBooster:
                playerData.numbleShakeBooster = value;
                EventManager.Instance.PostEvent(EventID.OnShakeBoosterChanged, value);
                break;
            case eAssetType.UpgradeBooster:
                playerData.numbleUpgradeBooster = value;
                EventManager.Instance.PostEvent(EventID.OnUpgradeBoosterChanged, value);
                break;
            case eAssetType.SmallDestroyBooster:
                playerData.numbleSmallDestroyBooster = value;
                EventManager.Instance.PostEvent(EventID.OnSmallDestroyBoosterChanged, value);
                break;
        }
        GameData.MarkDirty();
    }

    // Thêm hoặc sử dụng asset
    public void AddAsset(eAssetType type, int amount)
    {
        int current = GetAsset(type);
        SetAsset(type, current + amount);
    }

    public bool UseAsset(eAssetType type, int amount)
    {
        int current = GetAsset(type);
        if (current - amount < 0) return false;
        SetAsset(type, current - amount);
        return true;
    }
}
