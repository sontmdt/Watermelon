using DG.Tweening;
using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.UI;

public class Rewarded : MonoBehaviour, IAdsService
{
    [SerializeField] private Button btnShowAds;
    private RewardedAd rewardedAds;

#if UNITY_ANDROID
    private string adUnitId = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
    private string adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
    private string adUnitId = "unexpected_platform";
#endif

    private void Start()
    {
        ServiceLocator.Instance.RegisterService<IAdsService>(this);
        btnShowAds.onClick.AddListener(ShowRewardedAds);
        MobileAds.Initialize(_ => LoadAd());
    }

    private void OnDestroy()
    {
        if (rewardedAds != null)
        {
            rewardedAds.OnAdFullScreenContentClosed -= OnAdsClosed;
            rewardedAds.Destroy();
        }
    }

    public void LoadAd()
    {
        RewardedAd.Load(adUnitId, new AdRequest(), (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null)
            {
                Debug.LogError("Failed to load rewarded ad: " + error);
                return;
            }

            rewardedAds = ad;
            rewardedAds.OnAdFullScreenContentClosed += OnAdsClosed;
            btnShowAds.interactable = true;
        });
    }

    private void ShowRewardedAds()
    {
        if (rewardedAds != null && rewardedAds.CanShowAd())
        {
            if (GameManager.Instance.CurrentState == EventID.StateGamePopup) RewardedItem();
        }
    }

    private void UpdateCoolDownAds(float rate)
    {
        btnShowAds.interactable = rate == 0f;
    }

    private void OnAdsClosed()
    {
        var ctx = ServiceLocator.Instance.GetService<IBoosterContext>();

        if (ctx.ActiveBooster == eActiveBooster.ReviseAndDestroy)
        {
            DOVirtual.DelayedCall(0.7f, () =>
            {
                GameManager.Instance.m_levelController.m_playerController.ReviseAndDestroy();
            }).SetId("Gameplay");
        }
        else if (ctx.ActiveBooster == eActiveBooster.Shake)
        {
            GameManager.Instance.m_levelController.m_playerController.Shake();
        }

        EventManager.Instance.PostEvent(EventID.StateGameHanging);
    }

    private void RewardedItem()
    {
        rewardedAds.Show(reward =>
        {
            var ctx = ServiceLocator.Instance.GetService<IBoosterContext>();

            if (ctx.ActiveBooster == eActiveBooster.Upgrade)
            {
                GameManager.Instance.m_playerAssetController.AddAsset(eAssetType.UpgradeBooster, 1);
                GameManager.Instance.m_levelController.numbleUpgradeBoosterByAds -= 1;
            }
            if (ctx.ActiveBooster == eActiveBooster.Destroy)
            {
                GameManager.Instance.m_playerAssetController.AddAsset(eAssetType.DestroyBooster, 1);
                GameManager.Instance.m_levelController.numbleDestroyBoosterByAds -= 1;
            }
            if (ctx.ActiveBooster == eActiveBooster.SmallDestroy)
            {
                GameManager.Instance.m_playerAssetController.AddAsset(eAssetType.SmallDestroyBooster, 1);
                GameManager.Instance.m_levelController.numbleSmallDestroyBoosterByAds -= 1;
            }
            if (ctx.ActiveBooster == eActiveBooster.Shake)
            {
                GameManager.Instance.m_playerAssetController.AddAsset(eAssetType.ShakeBooster, 1);
                GameManager.Instance.m_levelController.numbleShakeBoosterByAds -= 1;
            }
            if (ctx.ActiveBooster == eActiveBooster.ReviseAndDestroy)
            {
                GameManager.Instance.m_levelController.numbleReviveByAds -= 1;
            }
        });
        btnShowAds.interactable = false;
    }
}
