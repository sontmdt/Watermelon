using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGameStarted : MenuBase
{
    [SerializeField] private Button btnPause;
    [SerializeField] private Button btnShakeBooster;
    [SerializeField] private Button btnDestroyBooster;
    [SerializeField] private Button btnUpgradeBooster;
    [SerializeField] private Button btnSmallDestroyBooster;

    [SerializeField] private Image nextBallImage;
    [SerializeField] private Image coolDownToShakeImage;
    [SerializeField] private TextMeshProUGUI currentScoreText;
    [SerializeField] private TextMeshProUGUI highestScoreText;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private TextMeshProUGUI coinText;

    [SerializeField] private TextMeshProUGUI numbleDestroyBoosterText;
    [SerializeField] private TextMeshProUGUI numbleUpgradeBoosterText;
    [SerializeField] private TextMeshProUGUI numbleSmallDestroyBoosterText;
    [SerializeField] private TextMeshProUGUI numbleShakeBoosterText;

    [SerializeField] private Image addDestroy;
    [SerializeField] private Image addUpgrade;
    [SerializeField] private Image addSmallDestroy;
    [SerializeField] private Image addShake;

    private IGameDataService GameData => ServiceLocator.Instance.GetService<IGameDataService>();
    private IBoosterContext _boosterCtx;
    private IBoosterContext BoosterCtx => _boosterCtx ??= ServiceLocator.Instance.GetService<IBoosterContext>();
    private Vector3 scoreOriginalScale;

    private void OnCoinChangedProxy(object o) => UpdateCoin((int)o);
    private void OnScoreChangedProxy(object o) => UpdateScore((int)o);
    private void OnComboChangedProxy(object o)
    { var (c, m) = ((int, float))o; UpdateCombo(c, m); }
    private void OnDroppedProxy(object o) => UpdateNextBall((eBallType)o);
    private void OnDestroyBoosterProxy(object o) => UpdateDestroyBooster((int)o);
    private void OnShakeBoosterProxy(object o) => UpdateShakeBooster((int)o);
    private void OnUpgradeBoosterProxy(object o) => UpdateUpgradeBooster((int)o);
    private void OnSmallDestroyBoosterProxy(object o) => UpdateSmallDestroyBooster((int)o);

    private void OnDestroy()
    {
        var em = EventManager.Instance;
        if (em == null) return;
        em.RemoveListener(EventID.OnCoinChanged, OnCoinChangedProxy);
        em.RemoveListener(EventID.OnScoreChanged, OnScoreChangedProxy);
        em.RemoveListener(EventID.OnComboChanged, OnComboChangedProxy);
        em.RemoveListener(EventID.OnDropped, OnDroppedProxy);
        em.RemoveListener(EventID.OnDestroyBoosterChanged, OnDestroyBoosterProxy);
        em.RemoveListener(EventID.OnShakeBoosterChanged, OnShakeBoosterProxy);
        em.RemoveListener(EventID.OnUpgradeBoosterChanged, OnUpgradeBoosterProxy);
        em.RemoveListener(EventID.OnSmallDestroyBoosterChanged, OnSmallDestroyBoosterProxy);

        btnPause.onClick.RemoveAllListeners();
        btnShakeBooster.onClick.RemoveAllListeners();
        btnDestroyBooster.onClick.RemoveAllListeners();
        btnUpgradeBooster.onClick.RemoveAllListeners();
        btnSmallDestroyBooster.onClick.RemoveAllListeners();
    }

    private bool _initialized;

    public override void Setup()
    {
        if (!_initialized)
        {
            btnPause.AddAnimatedListener(OnClickPause);
            btnShakeBooster.AddAnimatedListener(OnClickShakeItem);
            btnDestroyBooster.AddAnimatedListener(OnClickDestroyItem);
            btnUpgradeBooster.AddAnimatedListener(OnClickUpgradeItem);
            btnSmallDestroyBooster.AddAnimatedListener(OnClickSmallDestroyItem);
            scoreOriginalScale = currentScoreText.transform.localScale;
            _initialized = true;
        }

        coinText.SetText("{0}", GameManager.Instance.m_playerAssetController.GetAsset(eAssetType.Coin));
        UpdateNumbleBooster();

        var em = EventManager.Instance;
        em.RemoveListener(EventID.OnCoinChanged, OnCoinChangedProxy);
        em.RegisterListener(EventID.OnCoinChanged, OnCoinChangedProxy);
        em.RemoveListener(EventID.OnScoreChanged, OnScoreChangedProxy);
        em.RegisterListener(EventID.OnScoreChanged, OnScoreChangedProxy);
        em.RemoveListener(EventID.OnComboChanged, OnComboChangedProxy);
        em.RegisterListener(EventID.OnComboChanged, OnComboChangedProxy);
        em.RemoveListener(EventID.OnDropped, OnDroppedProxy);
        em.RegisterListener(EventID.OnDropped, OnDroppedProxy);
        em.RemoveListener(EventID.OnDestroyBoosterChanged, OnDestroyBoosterProxy);
        em.RegisterListener(EventID.OnDestroyBoosterChanged, OnDestroyBoosterProxy);
        em.RemoveListener(EventID.OnShakeBoosterChanged, OnShakeBoosterProxy);
        em.RegisterListener(EventID.OnShakeBoosterChanged, OnShakeBoosterProxy);
        em.RemoveListener(EventID.OnUpgradeBoosterChanged, OnUpgradeBoosterProxy);
        em.RegisterListener(EventID.OnUpgradeBoosterChanged, OnUpgradeBoosterProxy);
        em.RemoveListener(EventID.OnSmallDestroyBoosterChanged, OnSmallDestroyBoosterProxy);
        em.RegisterListener(EventID.OnSmallDestroyBoosterChanged, OnSmallDestroyBoosterProxy);
    }

    public override void Show()
    {
        var score = ServiceLocator.Instance.GetService<IScoreService>().CurrentScore;
        currentScoreText.SetText("{0}", score);
        highestScoreText.SetText("{0}", GameData.data.playerData.highestScore);
        comboText.SetText("");
        gameObject.SetActive(true);
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnClickPause()
    {
        this.PostEvent(EventID.StateGamePause);
    }

    private void OnClickDestroyItem()
    {
        if (Ball.ballMap.Count == 0) return;
        int numbleItem = GameManager.Instance.m_playerAssetController.GetAsset(eAssetType.DestroyBooster);
        if (numbleItem <= 0)
        {
            BoosterCtx.Activate(eActiveBooster.Destroy);
            this.PostEvent(EventID.StateGamePopup);
            return;
        }
        BoosterCtx.Activate(eActiveBooster.Destroy);
        EventManager.Instance.PostEvent(EventID.StateGameHanging);
    }

    private void OnClickUpgradeItem()
    {
        if (Ball.ballMap.Count == 0) return;
        int numbleItem = GameManager.Instance.m_playerAssetController.GetAsset(eAssetType.UpgradeBooster);
        if (numbleItem <= 0)
        {
            BoosterCtx.Activate(eActiveBooster.Upgrade);
            this.PostEvent(EventID.StateGamePopup);
            return;
        }
        BoosterCtx.Activate(eActiveBooster.Upgrade);
        EventManager.Instance.PostEvent(EventID.StateGameHanging);
    }

    private void OnClickSmallDestroyItem()
    {
        if (Ball.ballMap.Count == 0) return;
        int numbleItem = GameManager.Instance.m_playerAssetController.GetAsset(eAssetType.SmallDestroyBooster);
        if (numbleItem <= 0)
        {
            BoosterCtx.Activate(eActiveBooster.SmallDestroy);
            this.PostEvent(EventID.StateGamePopup);
            return;
        }
        BoosterCtx.Activate(eActiveBooster.SmallDestroy);
        EventManager.Instance.PostEvent(EventID.StateGameHanging);
    }

    private void OnClickShakeItem()
    {
        if (Ball.ballMap.Count == 0) return;
        int numbleItem = GameManager.Instance.m_playerAssetController.GetAsset(eAssetType.ShakeBooster);
        if (numbleItem <= 0)
        {
            BoosterCtx.Activate(eActiveBooster.Shake);
            this.PostEvent(EventID.StateGamePopup);
            return;
        }
        BoosterCtx.Activate(eActiveBooster.Shake);
        DOVirtual.DelayedCall(0f, () => EventManager.Instance.PostEvent(EventID.StateGameHanging)).SetId("Gameplay");
        GameManager.Instance.m_levelController.m_playerController.Shake();
    }

    [SerializeField] private Sprite[] nextBalls;
    private void UpdateNextBall(eBallType nextBallName)
    {
        nextBallImage.sprite = nextBalls[(int)nextBallName - 1];
        var t = nextBallImage.transform;
        t.DOKill();
        t.localScale = Vector3.zero;
        t.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutBack).SetUpdate(true);
    }

    private void UpdateScore(int score)
    {
        currentScoreText.SetText("{0}", score);

        if (score > GameData.data.playerData.highestScore)
        {
            GameData.data.playerData.highestScore = score;
            highestScoreText.SetText("{0}", score);
        }

        Transform t = currentScoreText.transform;
        t.DOKill();

        Sequence seq = DOTween.Sequence();
        seq.Append(t.DOScale(scoreOriginalScale * 1.2f, 0.1f).SetEase(Ease.OutQuad));
        seq.Append(t.DOScale(scoreOriginalScale, 0.1f).SetEase(Ease.InQuad));
    }

    private void UpdateCombo(int combo, float multiplier)
    {
        if (combo > 1)
        {
            comboText.SetText("COMBO x{0}", combo);
            comboText.transform.DOKill();
            comboText.transform.localScale = Vector3.one * 1.3f;
            comboText.transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
        }
        else comboText.SetText("");
    }

    private void UpdateCoin(int coin) => coinText.SetText("{0}", coin);

    private void UpdateDestroyBooster(int numble)
    {
        if (numble == 0) { addDestroy.gameObject.SetActive(true); return; }
        addDestroy.gameObject.SetActive(false);
        numbleDestroyBoosterText.SetText("{0}", numble);
    }

    private void UpdateUpgradeBooster(int numble)
    {
        if (numble == 0) { addUpgrade.gameObject.SetActive(true); return; }
        addUpgrade.gameObject.SetActive(false);
        numbleUpgradeBoosterText.SetText("{0}", numble);
    }

    private void UpdateSmallDestroyBooster(int numble)
    {
        if (numble == 0) { addSmallDestroy.gameObject.SetActive(true); return; }
        addSmallDestroy.gameObject.SetActive(false);
        numbleSmallDestroyBoosterText.SetText("{0}", numble);
    }

    private void UpdateShakeBooster(int numble)
    {
        if (numble == 0) { addShake.gameObject.SetActive(true); return; }
        addShake.gameObject.SetActive(false);
        numbleShakeBoosterText.SetText("{0}", numble);
    }

    private void UpdateNumbleBooster()
    {
        UpdateDestroyBooster(GameData.data.playerData.numbleDestroyBooster);
        UpdateUpgradeBooster(GameData.data.playerData.numbleUpgradeBooster);
        UpdateSmallDestroyBooster(GameData.data.playerData.numbleSmallDestroyBooster);
        UpdateShakeBooster(GameData.data.playerData.numbleShakeBooster);
    }
}
