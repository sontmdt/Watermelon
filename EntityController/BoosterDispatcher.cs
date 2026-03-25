using UnityEngine;

public class BoosterDispatcher : MonoBehaviour
{
    [SerializeField] private ShakeBooster shakeBooster;
    [SerializeField] private SmallDestroyBooster smallDestroyBooster;
    [SerializeField] private DestroyBooster destroyBooster;
    [SerializeField] private UpgradeBooster upgradeBooster;
    [SerializeField] private ReviseAndDestroyBooster reviseAndDestroyBooster;

    private BoosterSystem shakeBoosterSystem;
    private BoosterSystem smallDestroyBoosterSystem;
    private BoosterSystem destroyBoosterSystem;
    private BoosterSystem upgradeBoosterSystem;
    private BoosterSystem reviseAndDestroyBoosterSystem;

    private IBoosterContext _boosterCtx;

    public void Setup()
    {
        _boosterCtx = ServiceLocator.Instance.GetService<IBoosterContext>();
        if (shakeBooster != null) shakeBoosterSystem = new BoosterSystem(shakeBooster);
        if (smallDestroyBooster != null) smallDestroyBoosterSystem = new BoosterSystem(smallDestroyBooster);
        if (destroyBooster != null) destroyBoosterSystem = new BoosterSystem(destroyBooster);
        if (upgradeBooster != null) upgradeBoosterSystem = new BoosterSystem(upgradeBooster);
        if (reviseAndDestroyBooster != null) reviseAndDestroyBoosterSystem = new BoosterSystem(reviseAndDestroyBooster);
    }

    public void Execute(eActiveBooster type)
    {
        switch (type)
        {
            case eActiveBooster.Destroy:          
                destroyBoosterSystem?.Execute();          
                break;
            case eActiveBooster.SmallDestroy:     
                smallDestroyBoosterSystem?.Execute();     
                break;
            case eActiveBooster.Upgrade:          
                upgradeBoosterSystem?.Execute();          
                break;
            case eActiveBooster.Shake:            
                shakeBoosterSystem?.Execute();            
                break;
            case eActiveBooster.ReviseAndDestroy: 
                reviseAndDestroyBoosterSystem?.Execute(); 
                break;
        }
    }

    public void Cancel()
    {
        switch (_boosterCtx.ActiveBooster)
        {
            case eActiveBooster.Destroy:      
                destroyBoosterSystem?.Cancel();      
                break;
            case eActiveBooster.SmallDestroy: 
                smallDestroyBoosterSystem?.Cancel(); 
                break;
            case eActiveBooster.Upgrade:      
                upgradeBoosterSystem?.Cancel();      
                break;
        }
    }

    public void Shake() => shakeBoosterSystem?.Execute();
    public void SmallDestroy() => smallDestroyBoosterSystem?.Execute();
    public void Destroy() => destroyBoosterSystem?.Execute();
    public void Upgrade() => upgradeBoosterSystem?.Execute();
    public void ReviseAndDestroy() => reviseAndDestroyBoosterSystem?.Execute();
}
