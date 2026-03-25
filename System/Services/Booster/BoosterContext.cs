public class BoosterContext : IBoosterContext
{
    public eActiveBooster ActiveBooster { get; private set; }
    public bool IsActive => ActiveBooster != eActiveBooster.None;
    public bool InCancelZone { get; set; }

    public void Activate(eActiveBooster type) => ActiveBooster = type;

    public void Deactivate()
    {
        ActiveBooster = eActiveBooster.None;
        InCancelZone = false;
    }
}
