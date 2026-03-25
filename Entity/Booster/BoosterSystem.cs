public class BoosterSystem
{
    private readonly IBooster _booster;

    public BoosterSystem(IBooster booster)
    {
        _booster = booster;
    }

    public virtual void Execute()
    {
        _booster.Effect();
    }
    public virtual void Cancel()
    {
        _booster.Cancel();
    }
}