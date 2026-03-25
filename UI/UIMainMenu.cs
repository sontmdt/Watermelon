using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMainMenu : MenuBase
{
    [SerializeField] private Button btnSetting;
    [SerializeField] private Button btnPlay;
    [SerializeField] private Button btnExit;
    [SerializeField] private Button btnLogin;
    [SerializeField] private Button btnShop;
    [SerializeField] private Button btnUpgrade;
    public Text levelTxt;

    [SerializeField] private GameObject gameSetting;
    [SerializeField] private GameObject mainMenu;

    [SerializeField] private Slider musicVolume;
    [SerializeField] private Slider effectVolume;

    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI diamondText;

    private void OnCoinChangedProxy(object o)    => UpdateCoin((int)o);
    private void OnDiamondChangedProxy(object o) => UpdateDiamond((int)o);

    private void OnDestroy()
    {
        btnSetting.onClick.RemoveAllListeners();
        btnExit.onClick.RemoveAllListeners();
        btnPlay.onClick.RemoveAllListeners();
        btnLogin.onClick.RemoveAllListeners();
        btnUpgrade.onClick.RemoveAllListeners();
        btnShop.onClick.RemoveAllListeners();
        var em = EventManager.Instance;
        if (em == null) return;
        em.RemoveListener(EventID.OnCoinChanged, OnCoinChangedProxy);
        em.RemoveListener(EventID.OnDiamondChanged, OnDiamondChangedProxy);
    }

    private void OnEnable()
    {
        var audio = ServiceLocator.Instance.GetService<IAudioService>();
        if (audio == null) return;
        musicVolume.value = audio.GetMusicVolume();
        effectVolume.value = audio.GetSoundVolume();
        musicVolume.onValueChanged.AddListener(audio.SetMusicVolume);
        effectVolume.onValueChanged.AddListener(audio.SetSoundVolume);
    }

    private void OnDisable()
    {
        var audio = ServiceLocator.Instance.GetService<IAudioService>();
        if (audio == null) return;
        musicVolume.onValueChanged.RemoveListener(audio.SetMusicVolume);
        effectVolume.onValueChanged.RemoveListener(audio.SetSoundVolume);
    }

    private bool _initialized;

    public override void Setup()
    {
        if (!_initialized)
        {
            btnSetting.AddAnimatedListener(OnClickSetting);
            btnExit.AddAnimatedListener(OnClickExit);
            btnPlay.AddAnimatedListener(OnClickPlay);
            btnLogin.AddAnimatedListener(OnClickLogin);
            btnUpgrade.AddAnimatedListener(OnClickUpgrade);
            btnShop.AddAnimatedListener(OnClickShop);
            _initialized = true;
        }

        var pac = GameManager.Instance.m_playerAssetController;
        coinText.text = pac.GetAsset(eAssetType.Coin).ToString();
        diamondText.text = pac.GetAsset(eAssetType.Diamond).ToString();
        var em = EventManager.Instance;
        em.RemoveListener(EventID.OnCoinChanged, OnCoinChangedProxy);
        em.RegisterListener(EventID.OnCoinChanged, OnCoinChangedProxy);
        em.RemoveListener(EventID.OnDiamondChanged, OnDiamondChangedProxy);
        em.RegisterListener(EventID.OnDiamondChanged, OnDiamondChangedProxy);
    }

    public override void Show()
    {
        gameObject.SetActive(true);
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnClickSetting()
    {
        mainMenu.SetActive(false);
        gameSetting.SetActive(true);
    }

    private void OnClickExit()
    {
        mainMenu.SetActive(true);
        gameSetting.SetActive(false);
    }

    public Login login;
    private void OnClickLogin() => login.SignIn();

    private void OnClickPlay()
    {
        EventManager.Instance.PostEvent(EventID.StateGameSetup);
        gameObject.SetActive(false);
        GameManager.Instance.m_mapController.ResetCamera();
        Loader.Load(eScene.InGame);
    }

    private void OnClickUpgrade()
    {
        UpgradeSystem entity = GameManager.Instance.upgradeDatabase.upgradeEntityList[0];
        entity.Upgrade();
        int level = ServiceLocator.Instance.GetService<IGameDataService>().data.objectUpgradeData.GetObjectLevel(entity.upgradeId);
        levelTxt.text = $"Level: {level}";
    }

    private void OnClickShop()
    {
        this.PostEvent(EventID.StateGameShop);
    }

    private void UpdateCoin(int coin)    => coinText.text = coin.ToString();
    private void UpdateDiamond(int dia)  => diamondText.text = dia.ToString();
}
