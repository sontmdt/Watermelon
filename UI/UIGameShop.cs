using UnityEngine;
using UnityEngine.UI;

public class UIGameShop : MenuBase
{
    [SerializeField] private Button btnExit;
    [SerializeField] private Button btnCoin;
    [SerializeField] private Button btnDiamond;
    [SerializeField] private GameObject shopCoin;
    [SerializeField] private GameObject shopDiamond;

    private AnimationUI animationUI;
    private AnimationUI AnimUI => animationUI ? animationUI : animationUI = GetComponent<AnimationUI>();

    private bool _initialized;

    public override void Setup()
    {
        if (!_initialized)
        {
            btnExit.AddAnimatedListener(OnClickExit);
            btnCoin.AddAnimatedListener(OnClickCoin);
            btnDiamond.AddAnimatedListener(OnClickDiamond);
            _initialized = true;
        }
    }

    public override void Show()
    {
        AnimUI.Show();
        OnClickCoin();
    }

    public override void Hide() => AnimUI.Hide();

    private void OnClickExit() => this.PostEvent(EventID.StateMainMenu);

    private void OnClickCoin()
    {
        shopDiamond.SetActive(false);
        shopCoin.SetActive(true);
        btnCoin.interactable = false;
        btnDiamond.interactable = true;
    }

    private void OnClickDiamond()
    {
        shopCoin.SetActive(false);
        shopDiamond.SetActive(true);
        btnDiamond.interactable = false;
        btnCoin.interactable = true;
    }
}
