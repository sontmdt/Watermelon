using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGamePopUp : MenuBase
{
    [SerializeField] private Button btnAds;
    [SerializeField] private Button btnBuyBooster;
    [SerializeField] private Button btnExit;

    [SerializeField] private Image iconImg;
    [SerializeField] private TextMeshProUGUI detailText;
    [SerializeField] private TextMeshProUGUI diamondText;

    private IBoosterContext _boosterCtx;
    private IBoosterContext BoosterCtx => _boosterCtx ??= ServiceLocator.Instance.GetService<IBoosterContext>();

    private AnimationUI animationUI;
    private AnimationUI AnimUI => animationUI ? animationUI : animationUI = GetComponent<AnimationUI>();

    private bool _initialized;

    private void OnDiamondChangedProxy(object o) => UpdateDiamond((int)o);

    private void OnDestroy()
    {
        EventManager.Instance?.RemoveListener(EventID.OnDiamondChanged, OnDiamondChangedProxy);
    }

    public override void Setup()
    {
        if (!_initialized)
        {
            btnAds.AddAnimatedListener(OnClickAds);
            btnBuyBooster.AddAnimatedListener(OnClickBuy);
            btnExit.AddAnimatedListener(OnClickExit);
            _initialized = true;
        }

        var pac = GameManager.Instance.m_playerAssetController;
        diamondText.text = pac.GetAsset(eAssetType.Diamond).ToString();
        var em = EventManager.Instance;
        em.RemoveListener(EventID.OnDiamondChanged, OnDiamondChangedProxy);
        em.RegisterListener(EventID.OnDiamondChanged, OnDiamondChangedProxy);
    }

    public override void Show()
    {
        btnAds.gameObject.SetActive(true);
        var lc = GameManager.Instance.m_levelController;
        switch (BoosterCtx.ActiveBooster)
        {
            case eActiveBooster.Destroy:
                btnAds.interactable = lc.numbleDestroyBoosterByAds > 0;
                btnExit.gameObject.SetActive(true);
                break;
            case eActiveBooster.SmallDestroy:
                btnAds.interactable = lc.numbleSmallDestroyBoosterByAds > 0;
                btnExit.gameObject.SetActive(true);
                break;
            case eActiveBooster.Shake:
                btnAds.interactable = lc.numbleShakeBoosterByAds > 0;
                btnExit.gameObject.SetActive(true);
                break;
            case eActiveBooster.Upgrade:
                btnAds.interactable = lc.numbleUpgradeBoosterByAds > 0;
                btnExit.gameObject.SetActive(true);
                break;
            case eActiveBooster.ReviseAndDestroy:
                btnAds.interactable = lc.numbleReviveByAds > 0;
                btnExit.gameObject.SetActive(false);
                break;
        }
        UpdateUI();
        AnimUI.Show();
    }

    public override void Hide() => AnimUI.Hide();

    private void OnClickAds()
    {
        ServiceLocator.Instance.GetService<IAdsService>().LoadAd();
    }

    private void OnClickBuy()
    {
        bool canBuy = GameManager.Instance.m_playerAssetController.UseAsset(eAssetType.Diamond, 5);
        if (!canBuy) return;

        switch (BoosterCtx.ActiveBooster)
        {
            case eActiveBooster.Destroy:
                GameManager.Instance.m_playerAssetController.AddAsset(eAssetType.DestroyBooster, 1);
                EventManager.Instance.PostEvent(EventID.StateGameHanging);
                break;
            case eActiveBooster.Upgrade:
                DOVirtual.DelayedCall(0.1f, () =>
                {
                    GameManager.Instance.m_playerAssetController.AddAsset(eAssetType.UpgradeBooster, 1);
                    EventManager.Instance.PostEvent(EventID.StateGameHanging);
                }).SetId("Gameplay");
                break;
            case eActiveBooster.SmallDestroy:
                GameManager.Instance.m_playerAssetController.AddAsset(eAssetType.SmallDestroyBooster, 1);
                EventManager.Instance.PostEvent(EventID.StateGameHanging);
                break;
            case eActiveBooster.Shake:
                GameManager.Instance.m_playerAssetController.AddAsset(eAssetType.ShakeBooster, 1);
                DOVirtual.DelayedCall(0f, () =>
                {
                    EventManager.Instance.PostEvent(EventID.StateGameHanging);
                }).SetId("Gameplay");
                GameManager.Instance.m_levelController.m_playerController.Shake();
                break;
            case eActiveBooster.ReviseAndDestroy:
                EventManager.Instance.PostEvent(EventID.StateGameHanging);
                GameManager.Instance.m_levelController.m_playerController.ReviseAndDestroy();
                break;
        }
    }

    private void OnClickExit()
    {
        BoosterCtx.Deactivate();
        this.PostEvent(EventID.StateGameStarted);
    }

    [SerializeField] private Sprite[] boosterIcons;
    private void UpdateUI()
    {
        switch (BoosterCtx.ActiveBooster)
        {
            case eActiveBooster.Destroy:
                iconImg.sprite = boosterIcons[(int)eBoosterType.DestroyBooster - 1];
                detailText.SetText("Tap to remove a specific Fruits");
                break;
            case eActiveBooster.Upgrade:
                iconImg.sprite = boosterIcons[(int)eBoosterType.UpgradeBooster - 1];
                detailText.SetText("Tap to upgrade a specific Fruits");
                break;
            case eActiveBooster.SmallDestroy:
                iconImg.sprite = boosterIcons[(int)eBoosterType.SmallDestroyBooster - 1];
                detailText.SetText("Remove the smallest Fruits");
                break;
            case eActiveBooster.Shake:
                iconImg.sprite = boosterIcons[(int)eBoosterType.ShakeBooster - 1];
                detailText.SetText("Shake the Box to mix them up");
                break;
            case eActiveBooster.ReviseAndDestroy:
                iconImg.sprite = boosterIcons[(int)eBoosterType.ReviveAndDestroyBooster - 1];
                detailText.SetText("Revive and destroy");
                break;
        }
    }

    private void UpdateDiamond(int diamond) => diamondText.text = diamond.ToString();
}
