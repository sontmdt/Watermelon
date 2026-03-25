using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGameHanging : MenuBase
{
    [SerializeField] private Button btnUpgradeBoosterCancel;
    [SerializeField] private CancelZone cancelZone;
    [SerializeField] private TextMeshProUGUI manual;

    private bool _initialized;

    public override void Setup()
    {
        if (!_initialized)
        {
            btnUpgradeBoosterCancel.AddAnimatedListener(OnClickUpgradeCancel);
            this.RegisterListener(EventID.OnBall11Spawned, _ => HideForEffect());
            _initialized = true;
        }
    }

    public override void Show()
    {
        Manual();
        gameObject.SetActive(true);
        GameManager.Instance.m_levelController.m_playerController.player.SetActive(false);
        var ctx = ServiceLocator.Instance.GetService<IBoosterContext>();
        btnUpgradeBoosterCancel.gameObject.SetActive(ctx.ActiveBooster == eActiveBooster.Upgrade);
        cancelZone.gameObject.SetActive(
            ctx.ActiveBooster == eActiveBooster.Destroy || ctx.ActiveBooster == eActiveBooster.SmallDestroy);
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
        GameManager.Instance.m_levelController.m_playerController.player.SetActive(true);
    }

    public void HideForEffect()
    {
        btnUpgradeBoosterCancel.gameObject.SetActive(false);
        cancelZone.gameObject.SetActive(false);
        manual.gameObject.SetActive(false);
    }

    private void OnClickUpgradeCancel()
    {
        var ctx = ServiceLocator.Instance.GetService<IBoosterContext>();
        if (ctx.ActiveBooster == eActiveBooster.Upgrade)
            GameManager.Instance.m_levelController.m_playerController.boosterDispatcher.Cancel();
    }

    private void Manual()
    {
        var ctx = ServiceLocator.Instance.GetService<IBoosterContext>();
        switch (ctx.ActiveBooster)
        {
            case eActiveBooster.Destroy:      
                manual.SetText("Hold and move to the fruit to be destroyed"); 
                break;
            case eActiveBooster.Upgrade:      
                manual.SetText("Select the fruit you want to upgrade");       
                break;
            case eActiveBooster.SmallDestroy: 
                manual.SetText("Small Destroy");                              
                break;
            default:                          
                manual.SetText("");                                            
                break;
        }
    }
}
